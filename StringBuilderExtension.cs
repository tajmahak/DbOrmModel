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
            if (setText == null)
            {
                str.Line(level, text + " => " + getText);
            }
            else
            {
                str.Line(level, text);
                str.Line(level, "{");
                str.Line(level + 1, "get => " + getText);
                str.Line(level + 1, "set => " + setText);
                str.Line(level, "}");
            }
        }
        public static void LineComment(this StringBuilder str, int level, string text)
        {
            str.Line(level, "/// <summary>");
            str.Line(level, "/// " + text);
            str.Line(level, "/// </summary>");
        }
    }
}
