using DbOrmModel.Properties;
using FirebirdSql.Data.FirebirdClient;
using MyLibrary.DataBase;
using MyLibrary.DataBase.Firebird;
using System;
using System.Data.Common;
using System.IO;
using System.Text;

namespace DbOrmModel
{
    internal class ProgramModel
    {

        public OrmModelProject CreateProject(string dbPath)
        {
            OrmModelProjectInfo projectInfo = new OrmModelProjectInfo()
            {
                DBPath = dbPath,
            };
            return CreateProject(projectInfo);
        }

        public OrmModelProject CreateProject(OrmModelProjectInfo projectInfo)
        {
            return new OrmModelProject(this, projectInfo);
        }

        public DbConnection OpenDataBaseConnection(string path)
        {
            FbConnectionStringBuilder conBuilder = new FbConnectionStringBuilder
            {
                Dialect = 3,
                UserID = "SYSDBA",
                Password = "masterkey",
                Charset = "WIN1251",
                Database = path
            };

            if (Settings.Default.UseEmbeddedServer == 0)
            {
                conBuilder.ServerType = FbServerType.Default;
                conBuilder.DataSource = "127.0.0.1";
            }
            else
            {
                conBuilder.ServerType = FbServerType.Embedded;
                conBuilder.ClientLibrary = Path.GetFullPath("fbclient\\fbembed.dll");
            }

            FbConnection connection = new FbConnection(conBuilder.ToString());
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка подключения к БД: " + ex.Message, ex);
            }
            return connection;
        }

        public DBProvider CreateDBProvider(DbConnection dbConnection)
        {
            FireBirdProvider provider = new FireBirdProvider();
            provider.Initialize(dbConnection);
            return provider;
        }



    }
}
