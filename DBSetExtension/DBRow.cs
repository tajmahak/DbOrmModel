using System;
using System.Data;

namespace DBSetExtension
{
    public class DBRow
    {
        public DBTable Table { get; private set; }
        internal DataRowState State;
        internal object[] Values;

        internal DBRow(DBTable table)
        {
            Table = table;
            Values = new object[table.Columns.Length];
            State = DataRowState.Detached;
        }

        #region Работа с данными

        public object this[int index]
        {
            get
            {
                return Values[index];
            }
            set
            {
                SetValueInternal(value, Table.Columns[index], index, false);
            }
        }
        public object this[string columnName]
        {
            get
            {
                var index = Table.GetIndex(columnName);
                return this[index];
            }
            set
            {
                var index = Table.GetIndex(columnName);
                var column = Table.Columns[index];
                SetValueInternal(value, column, index, false);
            }
        }

        public void SetNotNull()
        {
            for (int i = 0; i < Table.Columns.Length; i++)
                SetNotNullInternal(i);
        }
        public void SetNotNull(string columnName, object value)
        {
            var index = Table.GetIndex(columnName);
            var column = Table.Columns[index];
            SetValueInternal(value, column, index, true);
            SetNotNullInternal(index);
        }

        public T Get<T>(int index)
        {
            return ConvertValue<T>(Values[index]);
        }
        public T Get<T>(string columnName)
        {
            var index = Table.GetIndex(columnName);
            return ConvertValue<T>(Values[index]);
        }

        public string GetString(int columnIndex, bool allowNull)
        {
            var value = Get<string>(columnIndex);
            if (!allowNull && value == null)
                return string.Empty;
            return value;
        }
        public string GetString(string columnName, bool allowNull)
        {
            var index = Table.GetIndex(columnName);
            return GetString(index, allowNull);
        }

        public string GetString(int columnIndex, string format)
        {
            var value = this[columnIndex];
            if (!(value is IFormattable))
                throw DBSetException.StringFormat();
            return ((IFormattable)value).ToString(format, null);
        }
        public string GetString(string columnName, string format)
        {
            var index = Table.GetIndex(columnName);
            return GetString(index, format);
        }

        public bool IsNull(int index)
        {
            return (Values[index] is DBNull);
        }
        public bool IsNull(string columnName)
        {
            var index = Table.GetIndex(columnName);
            return IsNull(index);
        }

        #endregion

        public void Delete()
        {
            State = DataRowState.Deleted;
        }

        internal void InitializeValues()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                var column = Table.Columns[i];
                Values[i] = (column.IsPrimary) ? Guid.NewGuid() : column.DefaultValue;
            }
        }
        internal static T ConvertValue<T>(object value)
        {
            var type = typeof(T);

            if (value.GetType() == type)
                return (T)value;
            if (value is DBNull)
                return default(T);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);
            else if (type.BaseType == typeof(Enum))
                type = Enum.GetUnderlyingType(type);

            return (T)Convert.ChangeType(value, type);
        }

        private void SetValueInternal(object value, DBColumn column, int index, bool allowNull)
        {
            if (Table.Name == null)
                throw DBSetException.ProcessRow();

            value = value ?? DBNull.Value;

            if (value is DBNull)
            {
                if (!column.AllowDBNull && !(Values[index] is DBNull))
                {
                    if (!allowNull)
                        throw new ArgumentNullException(column.Name);
                }
            }
            else if (value is Guid)
            {
                if (column.IsPrimary)
                    throw DBSetException.GenerateSetID(column);
            }
            else
            {
                try
                {
                    value = Convert.ChangeType(value, column.DataType);
                }
                catch (Exception ex)
                {
                    throw DBSetException.DataConvert(column, value, ex);
                }

                if (value is string && ((string)value).Length > column.MaxTextLength)
                    throw DBSetException.StringOverflow(column);
            }

            if (State == DataRowState.Unchanged)
            {
                #region Проверка значения на разницу с предыдущим значением

                object prevValue = Values[index];
                bool isChanged = true;
                if (value.GetType() == prevValue.GetType() && value is IComparable)
                    isChanged = !object.Equals(value, prevValue);
                else if (value is byte[] && prevValue is byte[])
                    isChanged = !EqualsBlob((byte[])value, (byte[])prevValue);

                if (isChanged)
                    State = DataRowState.Modified;

                #endregion
            }
            Values[index] = value;
        }
        private void SetNotNullInternal(int index)
        {
            var column = Table.Columns[index];
            var value = Values[index];
            if ((value is DBNull) && !column.AllowDBNull)
            {
                var type = column.DataType;
                if (type == typeof(string))
                    value = string.Empty;
                else if (type.BaseType == typeof(Array))
                    value = Activator.CreateInstance(type, 0);
                else value = Activator.CreateInstance(type);

                Values[index] = value;
            }
        }
        private static bool EqualsBlob(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;

            int length = a.Length;
            for (int i = 0; i < length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }
    }
}
