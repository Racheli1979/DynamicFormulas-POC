using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NCalc;

namespace FormulaEngine
{
    public static class DotNetFormulaEngine
    {
        public static DataTable Evaluate(DataTable dfData, DataTable dfFormulas)
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("data_id", typeof(int));
            dtResults.Columns.Add("targil_id", typeof(int));
            dtResults.Columns.Add("method", typeof(string));
            dtResults.Columns.Add("result", typeof(object));
            dtResults.Columns.Add("execution_time", typeof(double));

            Regex caseWhen = new Regex(@"CASE\s+WHEN\s+(.*?)\s+THEN\s+(.*?)\s+ELSE\s+(.*?)\s+END", RegexOptions.IgnoreCase);

            string ConvertCaseWhen(string expr) => SqlToNCalcConverter.ConvertCaseWhen(expr, caseWhen);

            string ConvertSqlToNCalc(string expr) => SqlToNCalcConverter.Convert(expr, caseWhen);

            Parallel.ForEach(dfFormulas.AsEnumerable(), formulaRow =>
            {
                int formulaId = (int)formulaRow["id"];
                string formula = ConvertSqlToNCalc(formulaRow["targil"].ToString());
                string tnai = formulaRow["tnai"] == DBNull.Value ? null : formulaRow["tnai"].ToString();

                string finalFormula;
                if (string.IsNullOrWhiteSpace(tnai))
                {
                    finalFormula = formula;
                }
                else if (Regex.IsMatch(formula, @"iif\s*\("))
                {
                    finalFormula = formula;
                }
                else
                {
                    finalFormula = $"iif({ConvertSqlToNCalc(tnai)}, {formula}, null)";
                }

                foreach (DataRow dataRow in dfData.Rows)
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    Expression e = new Expression(finalFormula);

                    // העברת כל השדות מה-DataRow ל-Expression
                    foreach (DataColumn col in dfData.Columns)
                        e.Parameters[col.ColumnName] = dataRow[col] == DBNull.Value ? 0 : dataRow[col];

                    // רישום כל הפונקציות
                    NCalcFunctions.Register(e, dataRow, dfData.Columns);

                    object resultObj;
                    try
                    {
                        resultObj = e.Evaluate() ?? DBNull.Value;
                    }
                    catch
                    {
                        resultObj = DBNull.Value;
                    }

                    sw.Stop();
                    double total_time = sw.Elapsed.TotalSeconds;

                    lock (dtResults)
                    {
                        DataRow r = dtResults.NewRow();
                        r["data_id"] = dataRow["id"];
                        r["targil_id"] = formulaId;
                        r["method"] = ".Net";
                        r["result"] = resultObj;
                        r["execution_time"] = total_time;
                        dtResults.Rows.Add(r);
                    }
                }
            });

            return dtResults.AsEnumerable()
                            .OrderBy(r => r.Field<int>("data_id"))
                            .ThenBy(r => r.Field<int>("targil_id"))
                            .CopyToDataTable();
        }
    }
}
