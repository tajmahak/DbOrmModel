using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DBSetExtension
{
    public abstract class DBModelBase
    {
        public bool IsInitialized { get; protected internal set; }
        public DBTable[] Tables { get; protected internal set; }
        protected internal Dictionary<DBTable, string> DefaultSelectCommandsDict { get; private set; }
        protected internal Dictionary<DBTable, string> DefaultInsertCommandsDict { get; private set; }
        protected internal Dictionary<DBTable, string> DefaultUpdateCommandsDict { get; private set; }
        protected internal Dictionary<DBTable, string> DefaultDeleteCommandsDict { get; private set; }
        protected Dictionary<string, DBTable> TablesDict { get; private set; }
        protected Dictionary<string, DBColumn> ColumnsDict { get; private set; }

        public DBModelBase()
        {
            DefaultSelectCommandsDict = new Dictionary<DBTable, string>();
            DefaultInsertCommandsDict = new Dictionary<DBTable, string>();
            DefaultUpdateCommandsDict = new Dictionary<DBTable, string>();
            DefaultDeleteCommandsDict = new Dictionary<DBTable, string>();
            TablesDict = new Dictionary<string, DBTable>();
            ColumnsDict = new Dictionary<string, DBColumn>();
        }

        public abstract void Initialize(DbConnection connection);
        public abstract void AddParameter(DbCommand command, string name, object value);
        public abstract object ExecuteInsertCommand(DbCommand command);
        public abstract DbCommand BuildCommand(DbConnection connection, DBCommand command);

        public DBSet CreateDBSet(DbConnection connection)
        {
            return new DBSet(this, connection);
        }
        public DBTable GetTable(string tableName)
        {
            DBTable table;
            if (!TablesDict.TryGetValue(tableName, out table))
                throw DBSetException.UnknownTable(tableName);
            return table;
        }
        public DBColumn GetColumn(string columnName)
        {
            DBColumn column;
            if (!ColumnsDict.TryGetValue(columnName, out column))
                throw DBSetException.UnknownColumn(null, columnName);
            return ColumnsDict[columnName];
        }

        internal T PackRow<T>(object value)
        {
            if (typeof(T) == typeof(DBRow))
                return (T)value;
            return (T)Activator.CreateInstance(typeof(T), value);
        }
        internal DBRow UnpackRow(object value)
        {
            if (value == null)
                return null;
            if (value is IOrmTable)
                return (value as IOrmTable).Row;
            return (DBRow)value;
        }
    }
}
