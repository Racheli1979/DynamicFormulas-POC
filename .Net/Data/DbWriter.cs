using System.Data;
using System.Data.Odbc;

namespace Data
{
    public static class DbWriter
    {
        public static void BulkInsert(string tableName, DataTable dt)
        {
            using var conn = new OdbcConnection(Config.AppConfig.ConnectionString);
            conn.Open();

            foreach (DataRow row in dt.Rows)
            {
                var cmd = new OdbcCommand(
                    $"INSERT INTO {tableName} (data_id, targil_id, method, result, execution_time) VALUES (?, ?, ?, ?, ?)",
                    conn
                );

                cmd.Parameters.AddWithValue("@data_id", row["data_id"]);
                cmd.Parameters.AddWithValue("@targil_id", row["targil_id"]);
                cmd.Parameters.AddWithValue("@method", row["method"]);
                cmd.Parameters.AddWithValue("@result", row["result"]);
                cmd.Parameters.AddWithValue("@execution_time", row["execution_time"]);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
