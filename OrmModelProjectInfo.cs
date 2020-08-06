using System;
using System.Reflection.Emit;

namespace DbOrmModel
{
    [Serializable]
    public class OrmModelProjectInfo
    {
        public string DBPath { get; set; }
        public string NameSpace { get; set; }
        public string FileDirectory { get; set; }
    }
}
