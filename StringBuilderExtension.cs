using System.Text;

namespace DbOrmModel
{
    public static class StringBuilderExtension
    {
        public static void Add(this StringBuilder str, object content)
        {
            str.Append(content);
        }
        public static void AddLine(this StringBuilder str)
        {
            str.AppendLine();
        }
        public static void AddLine(this StringBuilder str, int level, object content)
        {
            string padding = string.Empty;
            if (level > 0)
            {
                padding = new string(' ', level * 4);
            }
            str.AppendLine(padding + content);
        }

        public static void AddProperty(this StringBuilder str, int level, string propertyName, string getText, string setText)
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
        public static void AddComment(this StringBuilder str, int level, string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                str.AddLine(level, "/// <summary>");
                str.AddLine(level, "/// " + comment);
                str.AddLine(level, "/// </summary>");
            }
        }

        public static bool RemoveLastLine(this StringBuilder str)
        {
            if (str.Length >= 2)
            {
                if (str[str.Length - 1] == '\n' && str[str.Length - 2] == '\r')
                {
                    str.Remove(str.Length - 2, 2);
                    return true;
                }
            }
            return false;
        }
    }
}
