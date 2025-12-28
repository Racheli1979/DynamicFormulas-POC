using System.Data;
using System.Data.Odbc;

namespace Data
{
    public static class DbLoader
    {
        public static DataTable LoadTable(string tableName)
        {
            using var conn = new OdbcConnection(Config.AppConfig.ConnectionString);
            conn.Open();

            using var cmd = new OdbcCommand($"SELECT * FROM {tableName}", conn);
            using var da = new OdbcDataAdapter(cmd);

            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }
    }
}
