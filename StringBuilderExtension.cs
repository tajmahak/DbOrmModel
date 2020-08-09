using System.Text;

namespace DbOrmModel
{
    public static class StringBuilderExtension
    {
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
