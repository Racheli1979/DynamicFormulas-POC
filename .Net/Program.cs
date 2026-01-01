using System;
using System.Linq;
using FormulaCalculatorEF.Config;
using FormulaCalculatorEF.Data;
using FormulaCalculatorEF.FormulaEngine;

namespace FormulaCalculatorEF
{
    class Program
    {
        static void Main()
        {
            using var context = new AppDbContext();

            var dfData = DbLoader.LoadData(context);
            var dfFormulas = DbLoader.LoadFormulas(context);

            var dtResults = FormulaCalculator.CalculateResults(dfData, dfFormulas);

            DbWriter.SaveResults(context, dtResults);

            Console.WriteLine("Saved successfully!");
        }
    }
}
