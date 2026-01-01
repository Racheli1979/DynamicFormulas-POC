using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FormulaCalculatorEF.Config;

namespace FormulaCalculatorEF.Data
{
    public static class DbLoader
    {
        public static List<DataRow> LoadData(AppDbContext context)
        {
            return context.DataRow.AsNoTracking().ToList();
        }

        public static List<Formula> LoadFormulas(AppDbContext context)
        {
            return context.Formulas.AsNoTracking().ToList();
        }
    }
}
