using FormulaCalculatorEF.Config;
using System.Collections.Generic;
using System.Linq;

namespace FormulaCalculatorEF.Data
{
    public static class DbWriter
    {
        public static void SaveResults(AppDbContext context, IEnumerable<Result> results)
        {
            context.Results.AddRange(
                results
                    .OrderBy(r => r.data_id)
                    .ThenBy(r => r.targil_id)
            );
            context.SaveChanges();
        }
    }
}
