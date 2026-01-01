using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using NCalc;
using FormulaCalculatorEF.Data;

namespace FormulaCalculatorEF.FormulaEngine
{
    public static class FormulaCalculator
    {
        public static ConcurrentBag<Result> CalculateResults(
            System.Collections.Generic.List<DataRow> dfData,
            System.Collections.Generic.List<Formula> dfFormulas)
        {
            var dtResults = new ConcurrentBag<Result>();

            Parallel.ForEach(dfFormulas, formulaRow =>
            {
                int formulaId = formulaRow.id;
                string formula = SqlToNCalcConverter.ConvertSqlToNCalc(formulaRow.targil);
                string tnai = formulaRow.tnai;

                string finalFormula = string.IsNullOrWhiteSpace(tnai) ? formula
                    : (System.Text.RegularExpressions.Regex.IsMatch(formula, @"iif\s*\(") ? formula
                    : $"iif({SqlToNCalcConverter.ConvertSqlToNCalc(tnai)}, {formula}, null)");

                foreach (var dataRow in dfData)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    Expression e = new Expression(finalFormula);
                    foreach (var prop in dataRow.GetType().GetProperties())
                        e.Parameters[prop.Name] = prop.GetValue(dataRow);

                    NCalcFunctions.Register(e, dataRow);

                    double? resultValue;
                    try
                    {
                        var eval = e.Evaluate();
                        resultValue = eval == null ? (double?)null : Convert.ToDouble(eval);
                    }
                    catch
                    {
                        resultValue = null;
                    }
                    sw.Stop();

                    dtResults.Add(new Result
                    {
                        data_id = dataRow.id,
                        targil_id = formulaId,
                        method = ".Net",
                        result = resultValue,
                        execution_time = sw.Elapsed.TotalSeconds
                    });
                }
            });

            return dtResults;
        }
    }
}
