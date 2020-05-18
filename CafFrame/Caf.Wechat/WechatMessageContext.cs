using Senparc.NeuChar;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.NeuralSystems;
using System;

namespace Caf.Wechat
{
    public class WechatMessageContext<TRequest, TResponse> : IMessageContext<TRequest, TResponse>
        where TRequest : IRequestMessageBase
        where TResponse : IResponseMessageBase
    {
        public WechatMessageContext()
        {
            RequestMessages = new MessageContainer<TRequest>(MaxRecordCount);
            ResponseMessages = new MessageContainer<TResponse>(MaxRecordCount);
            LastActiveTime = DateTime.Now;
        }
        /// <summary>
        /// 匹配到的关键字
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// 图文消息类型的MediaId
        /// </summary>
        public string NewsMediaId { get; set; }
        public string UserName { get; set; }
        public DateTime LastActiveTime { get; set; }
        public MessageContainer<TRequest> RequestMessages { get; set; }
        public MessageContainer<TResponse> ResponseMessages { get; set; }

        private int _maxRecordCount;
        public int MaxRecordCount
        {
            get { return _maxRecordCount; }
            set
            {
                RequestMessages.MaxRecordCount = value;
                ResponseMessages.MaxRecordCount = value;
                _maxRecordCount = value;
            }
        }
        public object StorageData { get; set; }
        public double? ExpireMinutes { get; set; }
        public AppStoreState AppStoreState { get; set; }
        DateTimeOffset? IMessageContext<TRequest, TResponse>.LastActiveTime { get; set; }
        public DateTimeOffset? ThisActiveTime { get; set; }
        public AppDataItem CurrentAppDataItem { get; set; }

        public event EventHandler<WeixinContextRemovedEventArgs<TRequest, TResponse>> MessageContextRemoved;

        public void OnRemoved()
        {
            if (MessageContextRemoved != null)
            {
                var onRemovedArg = new WeixinContextRemovedEventArgs<TRequest, TResponse>(this);
                MessageContextRemoved(this, onRemovedArg);
            }
        }
    }

}
