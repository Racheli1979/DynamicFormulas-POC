using System.Text.RegularExpressions;

namespace FormulaCalculatorEF.FormulaEngine
{
    public static class SqlToNCalcConverter
    {
        private static readonly Regex caseWhen = new Regex(
            @"CASE\s+WHEN\s+(.*?)\s+THEN\s+(.*?)\s+ELSE\s+(.*?)\s+END",
            RegexOptions.IgnoreCase
        );

        public static string ConvertCaseWhen(string expr)
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

        public static string ConvertSqlToNCalc(string expr)
        {
            if (string.IsNullOrWhiteSpace(expr)) return "";
            expr = expr.Trim();
            expr = ConvertCaseWhen(expr);
            expr = Regex.Replace(expr, @"\bSQRT\b", "sqrt", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"\bLOG\b", "log", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"\bABS\b", "abs", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"\bPOWER\b", "pow", RegexOptions.IgnoreCase);
            expr = Regex.Replace(expr, @"(\w+)\s*\^\s*(\w+)", "pow($1, $2)");
            expr = Regex.Replace(expr, @"(?<![<>!=])\b(\w+)\s*=\s*(\w+)\b", "isequal($1, $2)");
            return expr;
        }
    }
}
