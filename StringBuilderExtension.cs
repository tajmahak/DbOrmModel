using System.Text;

namespace DbOrmModel
{
    public static class StringBuilderExtension
    {
        public static void Line(this StringBuilder str, int level, string text, params object[] values)
        {
            string padding = string.Empty;
            if (level > 0)
                padding = new string(' ', level * 4);
            if (values.Length == 0)
                str.AppendLine(padding + text);
            else str.AppendLine(padding + string.Format(text, values));
        }
        public static void LineProperty(this StringBuilder str, int level, string text, string getText, string setText)
        {
            str.Line(level, text);
            str.Line(level, "{");

            str.Line(level + 1, "get");
            str.Line(level + 1, "{");
            str.Line(level + 2, getText);
            str.Line(level + 1, "}");

            if (setText != null)
            {
                str.Line(level + 1, "set");
                str.Line(level + 1, "{");
                str.Line(level + 2, setText);
                str.Line(level + 1, "}");
            }
            str.Line(level, "}");
        }
        public static void LineComment(this StringBuilder str, int level, string text)
        {
            str.Line(level, "/// <summary>");
            str.Line(level, "/// " + text);
            str.Line(level, "/// </summary>");
        }
    }
}
