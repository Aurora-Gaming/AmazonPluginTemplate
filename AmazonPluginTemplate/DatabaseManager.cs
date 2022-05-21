using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TShockAPI;
using TShockAPI.DB;

namespace PluginTemplate
{
    internal static class DatabaseManager
    {
        /// <summary>
        /// Generate the connection string based on the information in TShock config settings.
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                if (!TShock.Config.Settings.StorageType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
                    throw new NotImplementedException("This plugin only supports MYSQL databases.");

                // localhost:3306
                var hostTextRaw = TShock.Config.Settings.MySqlHost;
                bool hasPortNumber = hostTextRaw.Contains(":");
                string hostName = hostTextRaw.Split(':')[0];
                string hostPort = hasPortNumber ? hostTextRaw.Split(':')[1] : "3306";
                string databaseName = TShock.Config.Settings.MySqlDbName;
                string databaseUsername = TShock.Config.Settings.MySqlUsername;
                string databasePassword = TShock.Config.Settings.MySqlPassword;

                return $"Server={hostName}; Port={hostPort}; Database={databaseName}; Uid={databaseUsername}; Pwd={databasePassword}";
            }
        }

        public static void Connect()
        {
            MySqlConnection conn = new MySqlConnection(ConnectionString);

            SqlTableCreator tableCreator = new SqlTableCreator(conn, new MysqlQueryCreator());

            tableCreator.EnsureTableStructure(new SqlTable("amazon_usage",
                new SqlColumn("ID", MySqlDbType.Int32, 6) { AutoIncrement = true, Primary = true, Unique = true },
                    new SqlColumn("user_id", MySqlDbType.Int32, 8),
                    new SqlColumn("cmd_count", MySqlDbType.Int32, 8)
                ));
        }

        public static int IncreaseHelpCmdCount(int userId)
        {
            var conn = new MySqlConnection(ConnectionString);

            // Get current count
            int cmdCount = 0;

            var result = conn.QueryFirstOrDefault("SELECT * FROM amazon_usage WHERE user_id = @userId;", new { userId });
            if (result != null)
                cmdCount = (int)result.cmd_count;
            else
                conn.Execute("INSERT INTO amazon_usage (user_id, cmd_count) VALUES (@userId, @cmdCount);",new { cmdCount = 1, userId });

            // Increase count
            cmdCount++;
            conn.Execute("UPDATE amazon_usage SET cmd_count = @cmdCount WHERE user_id = @userId;", new { cmdCount, userId });

            return cmdCount;
        }
    }
}
