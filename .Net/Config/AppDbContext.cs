using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using FormulaCalculatorEF.Data;

namespace FormulaCalculatorEF.Config
{
    public class AppDbContext : DbContext
    {
        public DbSet<DataRow> DataRow { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Env.Load();
            string server = Env.GetString("DB_SERVER");
            string database = Env.GetString("DB_DATABASE");

            string connStr = $"Server={server};Database={database};Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tableData = Env.GetString("DB_TABLE_DATA");
            var tableFormulas = Env.GetString("DB_TABLE_FORMULAS");
            var tableResults = Env.GetString("DB_TABLE_RESULTS");

            modelBuilder.Entity<DataRow>().ToTable(tableData);
            modelBuilder.Entity<Formula>().ToTable(tableFormulas);
            modelBuilder.Entity<Result>().ToTable(tableResults);
        }
    }
}
