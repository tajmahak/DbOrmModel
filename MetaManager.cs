using MyLibrary.Data;
using MyLibrary.DataBase;
using System.Collections.Generic;
using System.Text;

namespace DbOrmModel
{
    internal class MetaManager
    {
        public bool UseUserNames { get; set; }
        public bool UseComments { get; set; }

        public void Clear()
        {
            _dict.Clear();
        }
        public string[] UploadInfo(DBProvider provider)
        {
            var content = new List<string>();

            foreach (var table in provider.Tables)
            {
                var key = table.Name;
                if (!_dict.TryGetValue(key, out var item))
                {
                    item = new MetaItem();
                }
                content.Add(new string('\t', _headersCount - 1));
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
            content[0] = _headers;

            return content.ToArray();
        }
        public void LoadInfo(DBProvider provider, string[] content)
        {
            Clear();
            foreach (var table in provider.Tables)
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
                    {
                        continue;
                    }

                    var split = line.Split('\t');
                    var key = split[0];
                    if (_dict.ContainsKey(key))
                    {
                        var item = _dict[key];

                        if (split.Length > 1)
                        {
                            item.UserName = split[1];
                            var splitUserName = Format.Split(item.UserName, "/");
                            if (splitUserName.Length > 1)
                            {
                                item.UserName = splitUserName[0].Trim();
                                item.UserName_TableList = splitUserName[1].Trim();
                            }
                        }

                        if (split.Length > 2)
                        {
                            item.Comment = split[2];
                        }

                        if (split.Length > 3)
                        {
                            item.DataType = split[3];
                        }

                        if (split.Length > 4)
                        {
                            item.ForeignKey = split[4];
                        }
                    }
                }
            }
        }

        public bool ContainsUserName(string tableName)
        {
            if (UseUserNames && _dict.ContainsKey(tableName))
            {
                return _dict[tableName].UserName != string.Empty;
            }
            return false;
        }
        public bool ContainsUserName_TableList(string tableName)
        {
            if (UseUserNames && _dict.ContainsKey(tableName))
            {
                return _dict[tableName].UserName_TableList != string.Empty;
            }
            return false;
        }
        public bool ContainsComment(string tableName)
        {
            if (UseComments && _dict.ContainsKey(tableName))
            {
                return _dict[tableName].Comment != string.Empty;
            }
            return false;
        }
        public bool ContainsDataType(string tableName)
        {
            if (_dict.ContainsKey(tableName))
            {
                return _dict[tableName].DataType != string.Empty;
            }
            return false;
        }
        public bool ContainsForeignKey(string tableName)
        {
            if (_dict.ContainsKey(tableName))
            {
                return _dict[tableName].ForeignKey != string.Empty;
            }
            return false;
        }

        public string GetUserName(string tableName)
        {
            return _dict[tableName].UserName;
        }
        public string GetUserName_TableList(string tableName)
        {
            return _dict[tableName].UserName_TableList;
        }
        public string GetComment(string tableName)
        {
            return _dict[tableName].Comment;
        }
        public string GetDataType(string tableName)
        {
            return _dict[tableName].DataType;
        }
        public string GetForeignKeyInfo(string tableName)
        {
            return _dict[tableName].ForeignKey;
        }

        private string CreateContentLine(string key, MetaItem item)
        {
            var userName = item.UserName;
            if (item.UserName_TableList.Length > 0)
            {
                userName += " / " + item.UserName_TableList;
            }

            if (userName.Length == 0)
            {
                if (!key.Contains("."))
                {
                    // Таблица
                    userName = PrepareUserName(key);
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
                    userName = PrepareUserName(userName);
                }
            }

            return $"{key}\t{userName}\t{item.Comment}\t{item.DataType}\t{item.ForeignKey}";
        }
        private string PrepareUserName(string value)
        {
            var str = new StringBuilder(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                c = i == 0 ? char.ToUpper(c) : char.ToLower(c);
                str.Append(c);
            }
            return str.ToString();
        }

        private const string _headers = "#Название" + "\t" + "#Пользовательское имя [/ Имя таблицы во множественном числе]" + "\t" + "#Комментарий" + "\t" + "#Тип данных" + "\t" + "#Внешний ключ";
        private const int _headersCount = 5;

        private readonly Dictionary<string, MetaItem> _dict = new Dictionary<string, MetaItem>();
        private class MetaItem
        {
            public string UserName = string.Empty;
            public string UserName_TableList = string.Empty;
            public string Comment = string.Empty;
            public string DataType = string.Empty;
            public string ForeignKey = string.Empty;
        }
    }
}
