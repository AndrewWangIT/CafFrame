using AspectCore.DynamicProxy;
using AspectCore.Injector;
using Caf.UnitOfWork.Interface;
using System.Threading.Tasks;

namespace Caf.UnitOfWork
{
    public class UowInterceptor : IInterceptor
    {
        public bool AllowMultiple { get; set; }

        public bool Inherited { get; set; }
        public int Order { get; set; }

        public async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var _uowHelper = context.ServiceProvider.Resolve<IUowHelper>();
            if (_uowHelper.IsExistUow)
            {
                await next(context);
            }
            else
            {
                using (var uow = _uowHelper.Create())
                {
                    await next(context);
                    uow.Commit();
                }
            }               
        }
    }
}