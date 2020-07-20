using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Caf.Domain.IntegrationEvent
{
    internal class ConsumerRegister : IConsumerRegister
    {
        private readonly ILogger _logger;
        private readonly IntegrationEventBusOptions _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConsumerClientFactory _consumerClientFactory;
        private readonly IConsumerErrHandler _consumerErrHandler;
        private readonly IIntegrationEventBus _integrationEventBus;
        private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);

        private CancellationTokenSource _cts;
        private BrokerAddress _serverAddress;
        private Task _compositeTask;
        private bool _disposed;
        private static bool _isHealthy = true;

        public ConsumerRegister(ILogger<ConsumerRegister> logger, IServiceScopeFactory serviceScopeFactory,
            IConsumerClientFactory consumerClientFactory, IIntegrationEventBus integrationEventBus,
            IConsumerErrHandler consumerErrHandler, IOptions<IntegrationEventBusOptions> options)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _consumerClientFactory = consumerClientFactory;
            _integrationEventBus = integrationEventBus;
            _options = options.Value;
            _consumerErrHandler = consumerErrHandler;
            _cts = new CancellationTokenSource();
        }

        public bool IsHealthy()
        {
            return _isHealthy;
        }

        public void Start()
        {
            //初始化
            registerhandlers();
            var groupingMatches = _integrationEventBus.GetGroups();

            foreach (var matchGroup in groupingMatches)
            {
                for (int i = 0; i < _options.ConsumerThreadCount; i++)
                {
                    DoWork(matchGroup);
                }
            }
            _compositeTask = Task.CompletedTask;
        }

        private void DoWork(KeyValuePair<string,List<string>> matchGroup)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _logger.LogInformation($"启调新work线程，线程Id:{Thread.CurrentThread.ManagedThreadId.ToString()}");
                    using (var client = _consumerClientFactory.Create(matchGroup.Key))
                    {
                        _serverAddress = client.BrokerAddress;

                        RegisterMessageProcessor(client);

                        client.Subscribe(matchGroup.Value);

                        client.Listening(_pollingDelay, _cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    //ignore
                }
                catch (BrokerConnectionException e)
                {
                    //_isHealthy = false;
                    _logger.LogError(e,  "BrokerConnectionException:"+ e.Message);

                    DoWork(matchGroup);
                    _logger.LogInformation("准备销毁当前线程");
                    try { System.Threading.Thread.CurrentThread.Abort(); }
                    catch
                    {
                        _logger.LogInformation("当前线程已销毁");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "OtherException:"+ e.Message);
                    DoWork(matchGroup);
                    _logger.LogInformation("准备销毁当前线程");
                    try { System.Threading.Thread.CurrentThread.Abort(); }
                    catch
                    {
                        _logger.LogInformation("当前线程已销毁");
                    }
                }
            }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        public void ReStart(bool force = false)
        {
            if (!IsHealthy() || force)
            {
                Pulse();

                _cts = new CancellationTokenSource();
                _isHealthy = true;

                Start();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            try
            {
                Pulse();

                _compositeTask?.Wait(TimeSpan.FromSeconds(2));
            }
            catch (AggregateException ex)
            {
                var innerEx = ex.InnerExceptions[0];
                if (!(innerEx is OperationCanceledException))
                {
                    _logger.LogError(innerEx, "");
                }
            }
        }

        public void Pulse()
        {
            _cts?.Cancel();
        }

        private void RegisterMessageProcessor(IConsumerClient client)
        {
            client.OnMessageReceived += (sender, transportMessage) =>
            {
                var handlerDescs = new List<IntegrationHandlerDesc>();
                try
                {                    
                    var group = transportMessage.GetGroup();
                    var topic = transportMessage.GetTopic();
                    handlerDescs = _integrationEventBus.GetIntegrationHandlerDescs(topic);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        foreach (var handlerDesc in handlerDescs.Where(o => o.GroupId == group))
                        {
                            var handlerobj = scope.ServiceProvider.GetRequiredService(handlerDesc.HandlerType);
                            var eventdata = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(transportMessage.Body), handlerDesc.IntegrationEventType);
                            var handlerTask = handlerDesc.HandlerMethod.Invoke(handlerobj, new object[] { eventdata, transportMessage, _cts.Token }) as Task;
                            handlerTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An exception occurred when process received message. Message:'{0}'.", transportMessage);
                    _consumerErrHandler.HandleConsumerErr(new ConsumerMessageContext { ex = e, integrationHandlerDescs = handlerDescs, message = transportMessage }).ConfigureAwait(false).GetAwaiter().GetResult(); 
                    //client.Reject(sender);
                }
                finally
                {
                    try
                    {
                        client.Commit(sender);
                    }
                    catch (Exception e)
                    {                       
                        _logger.LogError(e, "An exception occurred when Commit offset. Message:'{0}'.", transportMessage);
                        client.Reject(sender);
                        //var groupingMatches = _integrationEventBus.GetGroups();
                        //var group = groupingMatches.Where(o => o.Key == client.CurrentGroupId).FirstOrDefault();
                        //_logger.LogInformation("准备销毁KafkaClient连接");
                        //client.Dispose();
                        //_logger.LogInformation("KafkaClient连接已销户");

                        //DoWork(group);
                        //_logger.LogInformation("准备销毁当前线程");
                        //try { System.Threading.Thread.CurrentThread.Abort(); }
                        //catch
                        //{
                        //    _logger.LogInformation("当前线程已销毁");
                        //}


                    }
                    
                }
            };

            client.OnLog += WriteLog;
        }

        private void WriteLog(object sender, LogMessageEventArgs logmsg)
        {
            switch (logmsg.LogType)
            {

                case MqLogType.ConsumeError:
                    _logger.LogError("Kafka client consume error. --> " + logmsg.Reason);
                    break;
                case MqLogType.ServerConnError:
                    _isHealthy = false;
                    _logger.LogCritical("Kafka server connection error. --> " + logmsg.Reason);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void registerhandlers()
        {
            using (var scop = _serviceScopeFactory.CreateScope())
            {
                foreach (var item in scop.ServiceProvider.GetServices(typeof(IIntegrationEventHandler)))
                {
                    var handleType = item.GetType();
                    var eventType = handleType.BaseType.GetGenericArguments().First();
                    _integrationEventBus.Subscribe(eventType, handleType);
                }
            }
        }
    }
}
