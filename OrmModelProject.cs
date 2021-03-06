﻿using MyLibrary;
using MyLibrary.DataBase;
using System;
using System.IO;
using System.Text;

namespace DbOrmModel
{
    internal class OrmModelProject
    {
        public OrmModelProject(ProgramModel programModel, OrmModelProjectInfo projectInfo)
        {
            this.programModel = programModel;
            ProjectInfo = projectInfo;

            if (string.IsNullOrEmpty(projectInfo.DBPath))
            {
                throw new Exception("Не указан путь к БД");
            }

            meta = new OrmModelProjectMetaData();

            LoadDBProvider();

            UpdateMetaData();
        }

        public OrmModelProjectInfo ProjectInfo { get; private set; }
        public DBTableCollection Tables => dbProvider.Tables;
        public bool UseCustomNames { get; set; } = true;
        public bool UseComments { get; set; } = true;

        private readonly ProgramModel programModel;
        private DBProvider dbProvider;
        private readonly OrmModelProjectMetaData meta;


        public string GetMainDBNamespace(int level)
        {
            StringBuilder str = new StringBuilder();

            str.AddLine(level, "using MyLibrary.DataBase;");
            str.AddLine(level, "using System;");
            str.AddLine(level, "using System.Data.Common;");
            str.AddLine();

            str.AddLine(level, "namespace " + GetNamespaceName());
            str.AddLine(level, "{");
            str.AppendLine(GetMainDBClass(level + 1));
            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }

        public string GetTableItemNamespace(int level, DBTable table)
        {
            StringBuilder str = new StringBuilder();

            str.AddLine(level, "using System;");
            str.AddLine();

            str.AddLine(level, "namespace " + GetNamespaceName());
            str.AddLine(level, "{");
            str.AppendLine(GetTableItemClass(level + 1, table, false));
            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }

        public string GetTableItemsNamespace(int level)
        {
            StringBuilder str = new StringBuilder();

            str.AddLine(level, "using System;");
            str.AddLine();

            str.AddLine(level, "namespace " + GetNamespaceName());
            str.AddLine(level, "{");

            foreach (DBTable table in Tables)
            {
                str.AppendLine(GetTableItemClass(level + 1, table, true));
                str.AddLine();
            }
            str.RemoveLastLine();

            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }

        public void UpdateMetaData()
        {
            string mfPath = GetMetafilePath();
            string[] content = null;
            if (File.Exists(mfPath))
            {
                content = File.ReadAllLines(mfPath, Encoding.UTF8);
            }
            else
            {
                content = meta.UploadInfo(dbProvider);
            }
            meta.LoadInfo(dbProvider, content);
        }

        public string[] UploadMetaData()
        {
            UpdateMetaData();
            return meta.UploadInfo(dbProvider);
        }

        public string GetMetafilePath()
        {
            return ProjectInfo.DBPath + ".meta.txt";
        }


        private void LoadDBProvider()
        {
            string dbPath = Path.GetFullPath(ProjectInfo.DBPath);
            dbProvider = programModel.InitializeDBProvider(dbPath);
        }

        private string GetNamespaceName()
        {
            string namespaceName = string.IsNullOrEmpty(ProjectInfo.NameSpace) ? "UnknownNamespace" : ProjectInfo.NameSpace;
            return namespaceName;
        }

        private string GetMainDBClass(int level)
        {
            StringBuilder str = new StringBuilder();

            str.AddLine(level, "internal class DB : DBContext");
            str.AddLine(level, "{");

            foreach (DBTable table in Tables)
            {
                string originalTableName = GetOriginalTableName(table);
                string customTableName = GetCustomTableName(table);
                string customTableListName = GetCustomTableListName(table);

                str.AddLine(level + 1, "#region " + customTableName);
                str.AddLine();

                AddComment(level + 1, str, table);
                str.AddLine(level + 1, $"public const string {customTableName}TableName = \"{originalTableName}\";");
                str.AddLine();

                AddComment(level + 1, str, table);
                str.AddLine(level + 1, $"public DBQuery<{customTableName}Row> {customTableListName} => Select<{customTableName}Row>();");
                str.AddLine();

                str.AppendLine(GetStaticTableClass(level + 1, table));

                str.AppendLine(GetTableRowClass(level + 1, table));

                str.AddLine();
                str.AddLine(level + 1, "#endregion");
                str.AddLine();
            }

            str.AddLine(level + 1, @"public DB(DBProvider provider, DbConnection connection) : base(provider, connection)");
            str.AddLine(level + 1, "{");
            str.AddLine(level + 1, "}");

            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }

        private string GetStaticTableClass(int level, DBTable table)
        {
            StringBuilder str = new StringBuilder();

            string originalTableName = GetOriginalTableName(table);
            string customTableName = GetCustomTableName(table);

            str.AddLine(level, $"public static class {customTableName}");
            str.AddLine(level, "{");

            foreach (DBColumn column in table.Columns)
            {
                string originalColumnName = GetOriginalColumnName(column);
                string customColumnName = GetCustomColumnName(column);

                AddComment(level + 1, str, column, true);
                str.AddLine(level + 1, $"public const string {customColumnName} = \"{originalTableName}.{originalColumnName}\";");
                str.AddLine();
            }

            str.RemoveLastLine();
            str.AddLine(level, "}");

            return str.ToString();
        }

        private string GetTableRowClass(int level, DBTable table)
        {
            StringBuilder str = new StringBuilder();

            string originalTableName = GetOriginalTableName(table);
            string customTableName = GetCustomTableName(table);

            AddComment(level, str, table);
            str.AddLine(level, $"[DBOrmTable({customTableName}TableName)]");
            str.AddLine(level, $"public class {customTableName}Row : DBOrmRow<{customTableName}Row>");
            str.AddLine(level, "{");

            foreach (DBColumn column in table.Columns)
            {
                string originalColumnName = GetOriginalColumnName(column);
                string customColumnName = GetCustomColumnName(column);
                string columnTypeName = GetColumnTypeName(column);
                string foreignKey = meta.GetForeignKeyInfo(originalTableName + "." + originalColumnName);

                string notNullAttribute = "";
                string primaryKeyAttribute = "";
                string foreignKeyAttribute = "";

                if (column.NotNull)
                {
                    notNullAttribute = ", NotNull: true";
                }

                if (column.IsPrimary)
                {
                    primaryKeyAttribute = ", PrimaryKey: true";
                }

                if (!string.IsNullOrEmpty(foreignKey))
                {
                    string[] split = foreignKey.Split('.');
                    string foreignCustomTableName = GetCustomTableName(split[0]);
                    string foreignCustomColumnName = GetCustomTableName(foreignKey);
                    foreignKeyAttribute = $", ForeignKey: {foreignCustomTableName}.{foreignCustomColumnName}";
                }

                str.AddLine(level + 1, $"#region {customColumnName}");
                str.AddLine();

                AddComment(level + 1, str, column, false);
                str.AddLine(level + 1, $"[DBOrmColumn({customTableName}.{customColumnName}{notNullAttribute}{primaryKeyAttribute}{foreignKeyAttribute})]");
                AddProperty(level + 1, str, $"public {columnTypeName} {customColumnName}",
                    $"Row.GetValue<{columnTypeName}>({customTableName}.{customColumnName});",
                    $"Row[{customTableName}.{customColumnName}] = value;");

                str.AddLine();
                AddComment(level + 1, str, column, true);
                str.AddLine(level + 1, $"public void Set{customColumnName}(object value)");
                str.AddLine(level + 1, "{");
                str.AddLine(level + 2, $"Row[{customTableName}.{customColumnName}] = value;");
                str.AddLine(level + 1, "}");

                str.AddLine();
                str.AddLine(level + 1, "#endregion");
                str.AddLine();
            }

            str.AddLine(level + 1, $"public {customTableName}Row(DBRow row) : base(row) {{ }}");

            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }

        private string GetTableItemClass(int level, DBTable table, bool addRegion)
        {
            StringBuilder str = new StringBuilder();

            string customTableName = GetCustomTableName(table);

            if (addRegion)
            {
                str.AddLine(level, $"#region {customTableName}");
                str.AddLine();
            }

            AddComment(level, str, table);
            str.AddLine(level, $"public class {customTableName}Item");
            str.AddLine(level, "{");

            str.AddLine(level + 1, $"internal {customTableName}Item(DB.{customTableName}Row row)");
            str.AddLine(level + 1, "{");
            str.AddLine(level + 2, "Row = row;");
            str.AddLine(level + 1, "}");
            str.AddLine();

            str.AddLine(level + 1, $"internal DB.{customTableName}Row Row {{ get; private set; }}");
            str.AddLine();

            foreach (DBColumn column in table.Columns)
            {
                string customColumnName = GetCustomColumnName(column);
                string columnTypeName = GetColumnTypeName(column);

                AddComment(level + 1, str, column, false);
                AddProperty(level + 1, str, $"public {columnTypeName} {customColumnName}",
                    $"Row.{customColumnName};", null);
                str.AddLine();
            }
            str.AddLine();
            foreach (DBColumn column in table.Columns)
            {
                string customColumnName = GetCustomColumnName(column);
                string columnTypeName = GetColumnTypeName(column);

                AddComment(level + 1, str, column, false);
                str.AddLine(level + 1, $"public bool Set{customColumnName}({columnTypeName} value)");
                str.AddLine(level + 1, "{");
                str.AddLine(level + 2, $"Row.Set{customColumnName}(value);");
                str.AddLine(level + 2, "return true;");
                str.AddLine(level + 1, "}");
                str.AddLine();
            }

            str.RemoveLastLine();
            str.AddLine(level, "}");

            if (addRegion)
            {
                str.AddLine();
                str.AddLine(level, "#endregion");
            }

            str.RemoveLastLine();
            return str.ToString();
        }


        private string GetOriginalTableName(DBTable table)
        {
            return table.Name;
        }

        private string GetOriginalColumnName(DBColumn column)
        {
            return column.Name;
        }

        private string GetCustomTableName(string originalTableName)
        {
            if (UseCustomNames)
            {
                string customName = meta.GetCustomName(originalTableName);
                return string.IsNullOrEmpty(customName) ? originalTableName : customName;
            }
            else
            {
                return originalTableName;
            }
        }

        private string GetCustomColumnName(string originalTableName, string originalColumnName)
        {
            if (UseCustomNames)
            {
                string customName = meta.GetCustomName(originalTableName + "." + originalColumnName);
                return string.IsNullOrEmpty(customName) ? originalColumnName : customName;
            }
            else
            {
                return originalColumnName;
            }
        }

        private string GetCustomTableName(DBTable table)
        {
            string originalTableName = GetOriginalTableName(table);
            return GetCustomTableName(originalTableName);
        }

        private string GetCustomColumnName(DBColumn column)
        {
            string originalTableName = GetOriginalTableName(column.Table);
            string originalColumnName = GetOriginalColumnName(column);
            return GetCustomColumnName(originalTableName, originalColumnName);
        }

        private string GetCustomTableListName(DBTable table)
        {
            string customTableListName = meta.GetCustomNameForList(table.Name);
            if (string.IsNullOrEmpty(customTableListName))
            {
                string customName = GetCustomTableName(table);
                if (string.IsNullOrEmpty(customName))
                {
                    return GetOriginalTableName(table) + "s";
                }
                else
                {
                    return customName + "s";
                }
            }
            return customTableListName;
        }


        private void AddComment(int level, StringBuilder str, string comment)
        {
            if (UseComments && !string.IsNullOrEmpty(comment))
            {
                str.AddLine(level, "/// <summary>");
                str.AddLine(level, "/// " + comment);
                str.AddLine(level, "/// </summary>");
            }
        }

        private void AddComment(int level, StringBuilder str, DBTable table)
        {
            string tableComment = meta.GetComment(table.Name);
            AddComment(level, str, tableComment);
        }

        private void AddComment(int level, StringBuilder str, DBColumn column, bool insertTypeName)
        {
            if (UseComments)
            {
                string columnName = column.Table.Name + "." + column.Name;
                string comment = meta.GetComment(columnName);
                comment = Data.GetNotNullValue(comment);
                if (insertTypeName)
                {
                    string typeComment = $"[{GetCommentObjectType(column)}]";
                    if (comment.Length > 0)
                    {
                        comment += " " + typeComment;
                    }
                    else
                    {
                        comment += typeComment;
                    }
                }
                AddComment(level, str, comment);
            }
        }

        private static void AddProperty(int level, StringBuilder str, string propertyName, string getText, string setText)
        {
            if (setText == null)
            {
                str.AddLine(level, propertyName + " => " + getText);
            }
            else
            {
                str.AddLine(level, propertyName);
                str.AddLine(level, "{");
                str.AddLine(level + 1, "get => " + getText);
                str.AddLine(level + 1, "set => " + setText);
                str.AddLine(level, "}");
            }
        }

        private string GetCommentObjectType(DBColumn column)
        {
            string typeName = GetColumnTypeName(column);
            if (!column.NotNull && column.DataType.IsClass)
            {
                typeName += "?";
            }
            return typeName;
        }

        private string GetColumnTypeName(DBColumn column)
        {
            string dataType = meta.GetDataType(column.Table.Name + "." + column.Name);
            if (string.IsNullOrEmpty(dataType))
            {
                dataType = GetTypeName(column.DataType);
            }
            if (!column.NotNull && !column.DataType.IsClass)
            {
                dataType += "?";
            }
            return dataType;
        }

        private string GetTypeName(Type type)
        {
            if (type.IsArray)
            {
                Type arrayType = type.GetElementType();
                return GetTypeName(arrayType) + "[]";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }
            if (type == typeof(byte))
            {
                return "byte";
            }
            if (type == typeof(char))
            {
                return "char";
            }
            if (type == typeof(decimal))
            {
                return "decimal";
            }
            if (type == typeof(double))
            {
                return "double";
            }
            if (type == typeof(float))
            {
                return "float";
            }
            if (type == typeof(int))
            {
                return "int";
            }
            if (type == typeof(long))
            {
                return "long";
            }
            if (type == typeof(sbyte))
            {
                return "sbyte";
            }
            if (type == typeof(short))
            {
                return "short";
            }
            if (type == typeof(string))
            {
                return "string";
            }
            if (type == typeof(uint))
            {
                return "uint";
            }
            if (type == typeof(ulong))
            {
                return "ulong";
            }
            if (type == typeof(ushort))
            {
                return "ushort";
            }

            return type.Name;
        }
    }
}
