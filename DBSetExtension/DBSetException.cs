using System;
using System.Linq.Expressions;

namespace DBSetExtension
{
    internal static class DBSetException
    {
        #region NameOf

        private static String NameOf<T, TT>(this Expression<Func<T, TT>> accessor)
        {
            return NameOf(accessor.Body);
        }
        private static String NameOf<T>(this Expression<Func<T>> accessor)
        {
            return NameOf(accessor.Body);
        }
        private static String NameOf<T, TT>(this T obj, Expression<Func<T, TT>> propertyAccessor)
        {
            return NameOf(propertyAccessor.Body);
        }
        private static String NameOf(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = expression as MemberExpression;
                if (memberExpression == null)
                    return null;
                return memberExpression.Member.Name;
            }
            return null;
        }

        #endregion
        public static Exception ArgumentNull<T>(Expression<Func<T>> accessor)
        {
            throw new ArgumentNullException(NameOf(accessor));
        }
        public static Exception UnknownTable(string tableName)
        {
            return new Exception(string.Format("Неизвестная таблица \"{0}\"", tableName));
        }
        public static Exception UnknownColumn(DBTable table, string columnName)
        {
            string text;
            if (table != null)
                text = string.Format("Таблица \"{0}\" - неизвестный столбец \"{1}\"", table.Name, columnName);
            else text = string.Format("Неизвестный столбец \"{0}\"", table.Name, columnName);
            return new Exception(text);
        }
        public static Exception DataConvert(DBColumn column, object value, Exception innerException)
        {
            return new Exception(string.Format("{1}: приведение из \"{2}\" в \"{3}\" невозможно",
                column.Name,
                column.DataType.Name,
                value.GetType().Name),
                innerException);
        }
        public static Exception SqlExecute()
        {
            return new Exception("SQL-команда не может быть выполнена в текущем контексте");
        }
        public static Exception ProcessRow()
        {
            return new Exception("Обработка строки невозможна");
        }
        public static Exception StringFormat()
        {
            return new Exception("Невозможно привести значение к форматированной строке");
        }
        public static Exception ProcessView()
        {
            return new Exception("Обработка представления невозможна");
        }
        public static Exception DbSave(DBRow row, Exception ex)
        {
            if (row == null)
                return ex;
            throw new Exception(string.Format("Ошибка сохранения БД. \"{0}\" - {1}", row.Table.Name, ex.Message), ex);
        }
        public static Exception DbSaveWrongRelations()
        {
            throw new Exception("Неверные связи между строками");
        }
        public static Exception StringOverflow(DBColumn column)
        {
            return new Exception(string.Format("\"{0}\": длина строки превышает допустимую длину", column.Name));
        }
        public static Exception GenerateSetID(DBColumn column)
        {
            return new Exception("Невозможно изменить значение первичного ключа");
        }
        public static Exception InadequateUpdateCommand()
        {
            return new Exception("Update-команда не содержит ни одного 'Set'");
        }
        public static Exception UnsupportedCommandContext()
        {
            throw new Exception("Недопустимая операция в текущем контексте команды");
        }
        public static Exception NotFindRow()
        {
            throw new Exception("Не найдено ни одной строки");
        }
        public static Exception RowDelete()
        {
            return new Exception("Невозможно удалить строку, т.к. нет привязки к DBSet");
        }
        public static Exception ParameterValuePairException()
        {
            return new Exception("Неверно заданы параметры запроса");
        }
    }
}
