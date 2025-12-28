using DotNetEnv;

namespace Config
{
    public static class AppConfig
    {
        public static string ConnectionString { get; private set; }
        public static string TableData { get; private set; }
        public static string TableFormulas { get; private set; }
        public static string TableResults { get; private set; }

        public static void Load()
        {
            Env.Load();

            var server = Env.GetString("DB_SERVER");
            var database = Env.GetString("DB_DATABASE");
            var driver = Env.GetString("DB_DRIVER");

            ConnectionString = $"Driver={{{driver}}};Server={server};Database={database};Trusted_Connection=yes;";

            TableData = Env.GetString("DB_TABLE_DATA");
            TableFormulas = Env.GetString("DB_TABLE_FORMULAS");
            TableResults = Env.GetString("DB_TABLE_RESULTS");
        }
    }
}
