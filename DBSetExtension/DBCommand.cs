using System.Collections.Generic;

namespace DBSetExtension
{
    public class DBCommand
    {
        public DBCommandTypeEnum CommandType { get; private set; }
        public DBTable Table { get; private set; }
        public bool IsView { get; private set; }
        internal List<object[]> Stack { get; private set; }

        public DBCommand(DBTable table)
        {
            if (table == null)
                throw DBSetException.ArgumentNull(() => table);
            if (table.Name == null)
                throw DBSetException.ProcessView();

            Table = table;
            CommandType = DBCommandTypeEnum.Select;
            Stack = new List<object[]>();
        }

        #region Построители SQL

        public DBCommand Update()
        {
            CommandType = DBCommandTypeEnum.Update;
            return this;
        }
        public DBCommand Set(string columnName, object value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Update) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "UpdateSet", columnName, value });
            return this;
        }

        public DBCommand Delete()
        {
            CommandType = DBCommandTypeEnum.Delete;
            return this;
        }

        public DBCommand Select(params string[] columns)
        {
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "Select", columns });
            return this;
        }
        public DBCommand SelectAs(string alias, string columnName)
        {
            if (string.IsNullOrEmpty(alias)) throw DBSetException.ArgumentNull(() => alias);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "SelectAs", alias, columnName });
            return this;
        }
        public DBCommand SelectSum(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "SelectSum", columns });
            return this;
        }
        public DBCommand SelectSumAs(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "SelectSumAs", columns });
            return this;
        }
        public DBCommand SelectCount(params string[] columns)
        {
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "SelectCount", columns });
            return this;
        }

        public DBCommand Distinct()
        {
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "Distinct" });
            return this;
        }
        public DBCommand First(int count)
        {
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "First", count });
            return this;
        }
        public DBCommand First()
        {
            First(1);
            return this;
        }
        public DBCommand Skip(int count)
        {
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "Skip", count });
            return this;
        }

        public DBCommand InnerJoin(string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "InnerJoin", joinColumnName, columnName });
            return this;
        }
        public DBCommand LeftOuterJoin(string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "LeftOuterJoin", joinColumnName, columnName });
            return this;
        }
        public DBCommand RightOuterJoin(string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "RightOuterJoin", joinColumnName, columnName });
            return this;
        }
        public DBCommand FullOuterJoin(string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "FullOuterJoin", joinColumnName, columnName });
            return this;
        }

        public DBCommand InnerJoinAs(string alias, string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(alias)) throw DBSetException.ArgumentNull(() => alias);
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "InnerJoinAs", alias, joinColumnName, columnName });
            return this;
        }
        public DBCommand LeftOuterJoinAs(string alias, string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(alias)) throw DBSetException.ArgumentNull(() => alias);
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "LeftOuterJoinAs", alias, joinColumnName, columnName });
            return this;
        }
        public DBCommand RightOuterJoinAs(string alias, string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(alias)) throw DBSetException.ArgumentNull(() => alias);
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "RightOuterJoinAs", alias, joinColumnName, columnName });
            return this;
        }
        public DBCommand FullOuterJoinAs(string alias, string joinColumnName, string columnName)
        {
            if (string.IsNullOrEmpty(alias)) throw DBSetException.ArgumentNull(() => alias);
            if (string.IsNullOrEmpty(joinColumnName)) throw DBSetException.ArgumentNull(() => joinColumnName);
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "FullOuterJoinAs", alias, joinColumnName, columnName });
            return this;
        }

        public DBCommand Where(string column, object value)
        {
            Where(column, "=", value);
            return this;
        }
        public DBCommand Where(string column1, object value1, string column2, object value2)
        {
            Where(column1, "=", value1);
            Where(column2, "=", value2);
            return this;
        }
        public DBCommand Where(string column1, object value1, string column2, object value2, string column3, object value3)
        {
            Where(column1, "=", value1);
            Where(column2, "=", value2);
            Where(column3, "=", value3);
            return this;
        }
        public DBCommand Where(string columnName, string equalOperator, object value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (string.IsNullOrEmpty(equalOperator)) throw DBSetException.ArgumentNull(() => equalOperator);

            Stack.Add(new object[] { "Where", columnName, equalOperator, value });
            return this;
        }
        public DBCommand WhereBetween(string columnName, object value1, object value2)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);

            Stack.Add(new object[] { "WhereBetween", columnName, value1, value2 });
            return this;
        }
        public DBCommand WhereUpper(string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (string.IsNullOrEmpty(value)) throw DBSetException.ArgumentNull(() => value);

            Stack.Add(new object[] { "WhereUpper", columnName, value });
            return this;
        }
        public DBCommand WhereContaining(string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (string.IsNullOrEmpty(value)) throw DBSetException.ArgumentNull(() => value);

            Stack.Add(new object[] { "WhereContaining", columnName, value });
            return this;
        }
        public DBCommand WhereContainingUpper(string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (string.IsNullOrEmpty(value)) throw DBSetException.ArgumentNull(() => value);

            Stack.Add(new object[] { "WhereContainingUpper", columnName, value });
            return this;
        }
        public DBCommand WhereLike(string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (string.IsNullOrEmpty(value)) throw DBSetException.ArgumentNull(() => value);

            Stack.Add(new object[] { "WhereLike", columnName, value });
            return this;
        }
        public DBCommand WhereLikeUpper(string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (string.IsNullOrEmpty(value)) throw DBSetException.ArgumentNull(() => value);

            Stack.Add(new object[] { "WhereLikeUpper", columnName, value });
            return this;
        }
        public DBCommand WhereIn(string columnName, DBCommand cmd)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (cmd == null) throw DBSetException.ArgumentNull(() => cmd);

            Stack.Add(new object[] { "WhereIn_command", columnName, cmd });
            return this;
        }
        public DBCommand WhereIn(string columnName, params object[] values)
        {
            if (string.IsNullOrEmpty(columnName)) throw DBSetException.ArgumentNull(() => columnName);
            if (values == null || values.Length == 0) throw DBSetException.ArgumentNull(() => values);

            Stack.Add(new object[] { "WhereIn_values", columnName, values });
            return this;
        }
        public DBCommand Or()
        {
            Stack.Add(new object[] { "Or" });
            return this;
        }

        public DBCommand OrderBy(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "OrderBy", columns });
            return this;
        }
        public DBCommand OrderByDesc(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "OrderByDesc", columns });
            return this;
        }
        public DBCommand OrderByUpper(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "OrderByUpper", columns });
            return this;
        }
        public DBCommand OrderByUpperDesc(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            Stack.Add(new object[] { "OrderByUpperDesc", columns });
            return this;
        }

        public DBCommand GroupBy(params string[] columns)
        {
            if (columns.Length == 0) throw DBSetException.ArgumentNull(() => columns);
            if (CommandType != DBCommandTypeEnum.Select) throw DBSetException.UnsupportedCommandContext();

            IsView = true;
            Stack.Add(new object[] { "GroupBy", columns });
            return this;
        }

        #endregion

        public enum DBCommandTypeEnum
        {
            Select,
            Update,
            Delete,
        }
    }
}