using MyLibrary.DataBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbOrmModel
{
    internal class OrmModelProjectMetaData
    {
        private const string headers = "#Название\t" +
            "#Пользовательское имя [/ Имя таблицы во множественном числе]\t" +
            "#Комментарий\t" +
            "#Тип данных\t" +
            "#Внешний ключ";
        private const int headersCount = 5;
        private readonly Dictionary<string, MetaItem> dict = new Dictionary<string, MetaItem>();

        public void Clear()
        {
            dict.Clear();
        }

        public string[] UploadInfo(DBProvider provider)
        {
            List<string> content = new List<string>();

            foreach (DBTable table in provider.Tables)
            {
                string key = table.Name;
                if (!dict.TryGetValue(key, out MetaItem item))
                {
                    item = new MetaItem();
                }
                content.Add(new string('\t', headersCount - 1));
                content.Add(CreateContentLine(key, item));

                foreach (DBColumn column in table.Columns)
                {
                    key = table.Name + "." + column.Name;
                    if (!dict.TryGetValue(key, out item))
                    {
                        item = new MetaItem();
                    }
                    content.Add(CreateContentLine(key, item));
                }
            }
            content[0] = headers;

            return content.ToArray();
        }

        public void LoadInfo(DBProvider provider, string[] content)
        {
            Clear();
            foreach (DBTable table in provider.Tables)
            {
                dict.Add(table.Name, new MetaItem());
                foreach (DBColumn column in table.Columns)
                {
                    dict.Add(table.Name + "." + column.Name, new MetaItem());
                }
            }

            if (content != null)
            {
                foreach (string line in content)
                {
                    if (line.Length == 0 || line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] split = line.Split('\t');
                    string key = split[0];
                    if (dict.ContainsKey(key))
                    {
                        MetaItem item = dict[key];

                        if (split.Length > 1)
                        {
                            item.UserName = split[1];
                            string[] splitUserName = item.UserName.Split(new string[] { "/" }, StringSplitOptions.None);
                            if (splitUserName.Length > 1)
                            {
                                item.UserName = splitUserName[0].Trim();
                                item.UserNameForList = splitUserName[1].Trim();
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

        public string GetCustomName(string name)
        {
            dict.TryGetValue(name, out MetaItem value);
            return value?.UserName;
        }

        public string GetCustomNameForList(string name)
        {
            dict.TryGetValue(name, out MetaItem value);
            return value?.UserNameForList;
        }

        public string GetComment(string name)
        {
            dict.TryGetValue(name, out MetaItem value);
            return value?.Comment;
        }

        public string GetDataType(string name)
        {
            dict.TryGetValue(name, out MetaItem value);
            return value?.DataType;
        }

        public string GetForeignKeyInfo(string name)
        {
            dict.TryGetValue(name, out MetaItem value);
            return value?.ForeignKey;
        }


        private string CreateContentLine(string key, MetaItem item)
        {
            string userName = item.UserName;
            if (item.UserNameForList.Length > 0)
            {
                userName += " / " + item.UserNameForList;
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
                    string[] split = key.Split('.');
                    string tableName = split[0];
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
            StringBuilder str = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                c = i == 0 ? char.ToUpper(c) : char.ToLower(c);
                str.Append(c);
            }
            return str.ToString();
        }


        private class MetaItem
        {
            public string UserName { get; set; } = string.Empty;
            public string UserNameForList { get; set; } = string.Empty;
            public string Comment { get; set; } = string.Empty;
            public string DataType { get; set; } = string.Empty;
            public string ForeignKey { get; set; } = string.Empty;
        }
    }
}
