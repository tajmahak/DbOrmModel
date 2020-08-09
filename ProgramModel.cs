using DbOrmModel.Properties;
using MyLibrary.DataBase;
using MyLibrary.DataBase.Firebird;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;

namespace DbOrmModel
{
    internal class ProgramModel
    {
        public IList<string> RecentList => recentList.AsReadOnly();

        private readonly List<string> recentList = new List<string>();

        public const string OrmProjectExtension = ".ormproj";

        public ProgramModel()
        {
            LoadRecentList();
        }


        public OrmModelProject CreateProject(string path)
        {
            string fileExtension = Path.GetExtension(path);
            if (string.Equals(fileExtension, OrmProjectExtension, StringComparison.OrdinalIgnoreCase))
            {
                // Загрузка файла проекта
                string projectFileContent = File.ReadAllText(path, Encoding.UTF8);
                OrmModelProjectInfo project = OrmModelProjectInfo.FromContent(projectFileContent);
                AddToRecent(path);
                return CreateProject(project);
            }
            else
            {
                // Загрузка БД
                OrmModelProjectInfo project = OrmModelProjectInfo.FromDBPath(path);
                AddToRecent(path);
                return CreateProject(project);
            }
        }

        public OrmModelProject CreateProject(OrmModelProjectInfo projectInfo)
        {
            return new OrmModelProject(this, projectInfo);
        }

        public DbConnection OpenDataBaseConnection(string path)
        {
            DbConnection dbConnection;
            if (Settings.Default.UseEmbeddedServer == 0)
            {
                dbConnection = FireBirdProviderFactory.CreateDefaultConnection(path, "127.0.0.1", "SYSDBA", "masterkey");
            }
            else
            {
                dbConnection = FireBirdProviderFactory.CreateEmbeddedConnection(path, "SYSDBA", "masterkey", "fbclient\\fbembed.dll");
            }
            try
            {
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка подключения к БД: " + ex.Message, ex);
            }
            return dbConnection;
        }

        public DBProvider CreateDBProvider(DbConnection dbConnection)
        {
            FireBirdProvider provider = new FireBirdProvider();
            provider.Initialize(dbConnection);
            return provider;
        }

        public void CrearRecentList()
        {
            recentList.Clear();
            SaveRecentList();
        }


        private string GetRecentFilePath()
        {
            return Settings.Default.RecentFilePath;
        }

        private int GetRecentCount()
        {
            return Settings.Default.RecentCount;
        }

        private void LoadRecentList()
        {
            recentList.Clear();

            string recentFilePath = GetRecentFilePath();
            if (File.Exists(recentFilePath))
            {
                string[] lines = File.ReadAllLines(recentFilePath, Encoding.UTF8);
                foreach (string line in lines)
                {
                    recentList.Add(line);
                }
            }
            FilterRecentList();
        }

        private void AddToRecent(string recentFilePath)
        {
            FilterRecentList();

            recentList.Insert(0, recentFilePath);
            for (int i = 1; i < recentList.Count; i++)
            {
                // удаление дубликатов
                if (string.Equals(recentList[i], recentFilePath))
                {
                    recentList.RemoveAt(i);
                    i--;
                }
            }

            SaveRecentList();
        }

        private bool FilterRecentList()
        {
            bool filtered = false;
            for (int i = 0; i < recentList.Count; i++)
            {
                if (!File.Exists(recentList[i]))
                {
                    recentList.RemoveAt(i);
                    i--;
                    filtered = true;
                }
            }

            int recentCount = GetRecentCount();
            while (recentList.Count > recentCount)
            {
                recentList.RemoveAt(recentCount - 1);
                filtered = true;
            }

            return filtered;
        }

        private void SaveRecentList()
        {
            string recentFilePath = GetRecentFilePath();
            File.Delete(recentFilePath);
            File.WriteAllLines(recentFilePath, recentList.ToArray(), Encoding.UTF8);
        }
    }
}
