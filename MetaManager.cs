using MyLibrary.DataBase;
using System.Collections.Generic;
using System.Text;

namespace DbOrmModel
{
    class MetaManager
    {
        public bool UseUserNames { get; set; }
        public bool UseComments { get; set; }
        public bool UseDebugInfo { get; set; }

        public void Clear()
        {
            _dict.Clear();
        }
        public string[] UploadInfo(DBModelBase model)
        {
            var content = new List<string>();

            MetaItem item;
            foreach (var table in model.Tables)
            {
                var key = table.Name;
                if (!_dict.TryGetValue(key, out item))
                {
                    item = new MetaItem();
                }
                content.Add(string.Empty);
                content.Add(CreateContentLine(key, item));

                foreach (var column in table.Columns)
                {
                    key = table.Name + "." + column.Name;
                    if (!_dict.TryGetValue(key, out item))
                    {
                        item = new MetaItem();
                    }
                    content.Add(CreateContentLine(key, item));
                }
            }
            content[0] = CreateHeaderLine();

            return content.ToArray();
        }
        public void LoadInfo(DBModelBase model, string[] content)
        {
            Clear();
            foreach (var table in model.Tables)
            {
                _dict.Add(table.Name, new MetaItem());
                foreach (var column in table.Columns)
                {
                    _dict.Add(table.Name + "." + column.Name, new MetaItem());
                }
            }

            if (content != null)
            {
                foreach (var line in content)
                {
                    if (line.Length == 0 || line.StartsWith("#"))
                        continue;

                    var split = line.Split('\t');
                    string key = split[0];
                    if (_dict.ContainsKey(key))
                    {
                        var item = _dict[key];

                        if (split.Length > 1)
                            item.UserName = split[1];

                        if (split.Length > 2)
                            item.Comment = split[2];

                        if (split.Length > 3)
                            item.DebugInfo = split[3];
                    }
                }
            }
        }

        public bool ContainsUserName(string tableName)
        {
            if (UseUserNames && _dict.ContainsKey(tableName))
            {
                return (_dict[tableName].UserName != string.Empty);
            }
            return false;
        }
        public bool ContainsComment(string tableName)
        {
            if (UseComments && _dict.ContainsKey(tableName))
            {
                return (_dict[tableName].Comment != string.Empty);
            }
            return false;
        }
        public bool ContainsDebugInfo(string tableName)
        {
            if (UseDebugInfo && _dict.ContainsKey(tableName))
            {
                return (_dict[tableName].DebugInfo != string.Empty);
            }
            return false;
        }

        public string GetUserName(string tableName)
        {
            return _dict[tableName].UserName;
        }
        public string GetComment(string tableName)
        {
            return _dict[tableName].Comment;
        }
        public string GetDebugInfo(string tableName)
        {
            return _dict[tableName].DebugInfo;
        }


        private string CreateHeaderLine()
        {
            return "#Название" + "\t" + "#Пользовательское имя" + "\t" + "#Комментарий" + "\t" + "#Отладочная информация";
        }
        private string CreateContentLine(string key, MetaItem item)
        {
            var userName = item.UserName;
            if (userName.Length == 0)
            {
                if (!key.Contains("."))
                {
                    // Таблица
                    userName = PrepareFieldName(key);
                }
                else
                {
                    // Столбец
                    var split = key.Split('.');
                    var tableName = split[0];
                    userName = split[1];
                    if (userName.StartsWith(tableName + "_"))
                    {
                        userName = userName.Remove(0, tableName.Length + 1);
                    }
                    userName = PrepareFieldName(userName);
                }
            }

            return key + "\t" + userName + "\t" + item.Comment + "\t" + item.DebugInfo;
        }
        private string PrepareFieldName(string value)
        {
            var str = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (i == 0)
                {
                    c = char.ToUpper(c);
                }
                else
                {
                    c = char.ToLower(c);
                }
                str.Append(c);
            }
            return str.ToString();
        }

        private Dictionary<string, MetaItem> _dict = new Dictionary<string, MetaItem>();
        private class MetaItem
        {
            public string UserName = string.Empty;
            public string Comment = string.Empty;
            public string DebugInfo = string.Empty;
        }
    }
}
