using Caf.Core;
using Caf.Core.DependencyInjection;
using Caf.UnitOfWork.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Caf.UnitOfWork.Imp
{
    public class UowHelper : IUowHelper, ISingleton
    {
        private readonly AsyncLocal<IUnitOfWork> currentUow;
        public UowHelper()
        {
            currentUow = new AsyncLocal<IUnitOfWork>();
        }

        public bool IsExistUow => currentUow.Value!=null;

        public IUnitOfWork Create()
        {
            var uow = new UnitOfWork();
            currentUow.Value = uow;
            return uow;
        }
        public bool IsExistConnectionKey(string key)
        {
            if (!IsExistUow)
            {
                throw new CafException("当前不存在Uow");
            }
            return currentUow.Value.ConnectionDic.ContainsKey(key);
        }
        public IUOWConnection GetOrAddUOWConnection(string key,IUOWConnection uOWConnection)
        {
            if(!IsExistUow)
            {
                throw new CafException("当前不存在Uow");
            }
            var dic = currentUow.Value.ConnectionDic;
            if (dic.ContainsKey(key))
                return dic[key];
            currentUow.Value.ConnectionDic.Add(key,uOWConnection);
            return uOWConnection;
        }
    }
}
