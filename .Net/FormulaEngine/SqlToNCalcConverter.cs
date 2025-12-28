using System.Text.RegularExpressions;

namespace FormulaEngine
{
    public static class SqlToNCalcConverter
    {
        public static string Convert(string expr, Regex caseWhen)
        {
            if (expr == null) return "";

            expr = expr.Trim();
            expr = ConvertCaseWhen(expr, caseWhen);

            expr = Regex.Replace(expr, @"\bSQRT\b", "sqrt", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"\bLOG\b", "log", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"\bABS\b", "abs", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"\bPOWER\b", "pow", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"(\w+)\s*\^\s*(\w+)", "pow($1, $2)");

            // המרה של "=" פשוט בין משתנים ל־isequal
            expr = Regex.Replace(expr, @"(?<![<>!=])\b(\w+)\s*=\s*(\w+)\b", "isequal($1, $2)");

            return expr;
        }

        public static string ConvertCaseWhen(string expr, Regex caseWhen)
        {
            var m = caseWhen.Match(expr);
            if (!m.Success) return expr;

            string cond = m.Groups[1].Value;
            string t = m.Groups[2].Value;
            string f = m.Groups[3].Value;

            cond = cond.Replace("<>", "!=");
            cond = Regex.Replace(cond, @"(?<![<>!=])\b(\w+)\s*=\s*(\w+)\b", "isequal($1, $2)");

            return $"iif({cond}, {t}, {f})";
        }
    }
}
