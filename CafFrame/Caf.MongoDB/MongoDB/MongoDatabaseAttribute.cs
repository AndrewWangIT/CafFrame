using System;
using System.Collections.Generic;
using System.Text;

namespace Caf.MongoDB.MongoDB
{
    public class MongoDatabaseAttribute
    {
        public string DatabaseName { get; set; }

        public MongoDatabaseAttribute()
        {

        }

        public MongoDatabaseAttribute(string databaseName)
        {
            DatabaseName = databaseName;
        }
    }
}
