using Caf.AppSetting.DbContextService;
using Caf.AppSetting.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Caf.AppSetting
{
    public class AppSettingsConfigure
    {
        private readonly CafAppsettingDbContext _dbContext;
        public AppSettingsConfigure(CafAppsettingDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        //初始化
        public void Init() 
        {
            //执行table_appsettings.sql, 生成table
            //var existCount = _dbContext.Database.ExecuteSqlCommand(
            //    "select * from sysobjects where id = object_id(@tablename) and OBJECTPROPERTY(id, N'IsUserTable') = 1",
            //    new[]{ new SqlParameter("tablename", Keys.AppSettingByCafs)});

            var exist = TableExists(Keys.AppSettingByCafs);
            if (!exist)
            {
                dynamic type = typeof(AppSettingsConfigure);
                string currentDirectory = Path.GetDirectoryName(type.Assembly.Location);
                //C:\InstallingSoftware\SourceCodes\CapResources\CafFrame\CafFrame\Caf.AppSetting\Tables\table_appsettingbycafs.sql
                string sql = File.ReadAllTextAsync($"{currentDirectory}/Tables/table_appsettingbycafs.sql").ConfigureAwait(false).GetAwaiter().GetResult();
                //var result = _dbContext.AppSettingByCafs.FromSql(sql);
                _dbContext.Database.ExecuteSqlCommand(sql);
            }
        }

        public bool TableExists(string tableName)
        {
            var connection = _dbContext.Database.GetDbConnection();

            if (connection.State.Equals(ConnectionState.Closed))
                connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"select * from sysobjects where id = object_id(@tablename) and OBJECTPROPERTY(id, N'IsUserTable') = 1";

                var tableNameParam = command.CreateParameter();
                tableNameParam.ParameterName = "@tablename";
                tableNameParam.Value = tableName;
                command.Parameters.Add(tableNameParam);

                return command.ExecuteScalar() != null;
            }
        }
    }
}
