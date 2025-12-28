using System;
using System.Data;
using DotNetEnv;
using Data;
using FormulaEngine;

class Program
{
    static void Main(string[] args)
    {
        DotNetEnv.Env.Load();

        string connStr = Config.DbConfig.GetConnectionString();

        DataTable dfData = DbLoader.LoadTable(connStr, "tmp_data");
        DataTable dfFormulas = DbLoader.LoadTable(connStr, "tmp_targil");

        DataTable dtResults = FormulaCalculator.Calculate(dfData, dfFormulas);

        DbLoader.BulkInsert(connStr, "tmp_results", dtResults);
        Console.WriteLine("Calculation completed and results inserted.");
    }
}
