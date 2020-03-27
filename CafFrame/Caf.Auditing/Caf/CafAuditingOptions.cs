using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace Caf.Auditing
{
    public class CafAuditingOptions
    {
        public bool HideErrors { get; set; }

        public bool IsEnabled { get; set; }

        public List<AttributeLogMapping> AttributeLogMappings { get; } = new List<AttributeLogMapping>();

        public void AddLog<TAttribute,TLogInfo>()
        {
            this.AttributeLogMappings.Add(new AttributeLogMapping(typeof(TAttribute), typeof(TLogInfo)));
        }
        public CafAuditingOptions()
        {
            IsEnabled = true;
            HideErrors = true;

            //EntityHistorySelectors = new EntityHistorySelectorList();
        }
    }
    public class AttributeLogMapping
    {
        public AttributeLogMapping(Type attributeType,Type logType)
        {
            AttributeType = attributeType;
            LogType = logType;
        }
        public Type AttributeType { get; set; }

        public Type LogType { get; set; }
    }
}
