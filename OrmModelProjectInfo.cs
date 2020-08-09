using MyLibrary;
using System;

namespace DbOrmModel
{
    [Serializable]
    public class OrmModelProjectInfo
    {
        public string DBPath { get; set; }
        public string NameSpace { get; set; }
        public string FileDirectory { get; set; }

        public static OrmModelProjectInfo FromDBPath(string dbPath)
        {
            OrmModelProjectInfo info = new OrmModelProjectInfo()
            {
                DBPath = dbPath,
            };
            return info;
        }

        public static OrmModelProjectInfo FromContent(string projectData)
        {
            return Serializer.DeserializeFromText<OrmModelProjectInfo>(projectData);
        }
    }
}
