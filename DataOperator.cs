using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    public class DataOperator
    {
        private MySqlConnection conn;

        public DataOperator()
        {
        }

        public void LogUserPerformenceToDb(TimeSpan playTime, int fieldSize, int mineCount)
        {
            conn = Connect(ReadSetting("dbConnString"));
            MySqlCommand cmd = InitializeSqlCommand(conn,
                AssemblyUserPerformenceCommandString(playTime, fieldSize, mineCount));
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        private MySqlCommand InitializeSqlCommand(MySqlConnection conn, string command)
        {
            return new MySqlCommand(command, conn);
        }

        private string AssemblyUserPerformenceCommandString(TimeSpan playTime, int fieldSize,
            int mineCount)
        {
            return
                $"INSERT INTO {ReadSetting("dbName")}.users_performances (userName, playTime, fieldSize, mineCount) VALUES ('{ReadSetting("userName")}', '{playTime}', {fieldSize}, {mineCount});";
        }

        private MySqlConnection Connect(string connString)
        {
            MySqlConnection conn;
            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = connString;
                conn.Open();
                return conn;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        private string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

            return null;
        }
    }
}

/*
CREATE TABLE sql11455972.users_performances (
                            userName varchar(64),
                            playTime TIME,
                            fieldSize int,
                            mineCount int
);

INSERT INTO sql11455972.users_performances (userName, playTime, fieldSize, mineCount) VALUES ('testovaciPepa', '03:14:07', 25, 4); 

select * from sql11455972.users_performances;

DROP TABLE sql11455972.users_performances;
*/