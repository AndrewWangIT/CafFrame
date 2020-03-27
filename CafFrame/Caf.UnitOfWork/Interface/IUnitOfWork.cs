using Caf.UnitOfWork.Imp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.UnitOfWork.Interface
{
    public interface IUnitOfWork:IDisposable
    {
        ConnectionDic ConnectionDic { get; }
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();
        /// <summary>
        /// 保存
        /// </summary>
        void SaveChanges();
        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();
    }
}
