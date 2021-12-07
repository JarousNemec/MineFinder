using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    public class DataOperator
    {
        private MySqlConnection conn;

        public DataOperator()
        {
            conn = Connect(ReadAppSetting("dbConnString"));
            if (conn != null)
            {
                MySqlCommand cmd = InitializeSqlCommand(conn, "select * from sql11455972.users_performances;");
                var x = cmd.ExecuteReader();
                conn.Close();
            }
        }

        public void LogUserPerformenceToDb(TimeSpan playTime, int fieldSize, int mineCount)
        {
            conn = Connect(ReadAppSetting("dbConnString"));
            if (conn != null)
            {
                MySqlCommand cmd = InitializeSqlCommand(conn,
                    AssemblyUserPerformenceCommandString(playTime, fieldSize, mineCount));
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        private MySqlCommand InitializeSqlCommand(MySqlConnection conn, string command)
        {
            return new MySqlCommand(command, conn);
        }

        private string AssemblyUserPerformenceCommandString(TimeSpan playTime, int fieldSize,
            int mineCount)
        {
            return
                $"INSERT INTO {ReadAppSetting("dbName")}.users_performances (userName, playTime, fieldSize, mineCount) VALUES ('{ReadAppSetting("userName")}', '{playTime}', {fieldSize}, {mineCount});";
        }

        private MySqlConnection Connect(string connString)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = connString;
                connection.Open();
                return connection;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public string ReadAppSetting(string key)
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

        public void SetSetting(string[] dataToSave)
        {
            try
            {
                if (!File.Exists(ReadAppSetting("savePath")))
                {
                    InitializeSaveFile();
                }
                string[] splitedDataFromFile;
                string dataFromFileInLine;
                LoadExistingFile(out dataFromFileInLine, out splitedDataFromFile);
                SaveMargedData(dataToSave, splitedDataFromFile, dataFromFileInLine);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void SaveMargedData(string[] dataToSave, string[] splitedDataFromFile, string dataFromFileInLine)
        {
            using (Stream stream = File.Open(ReadAppSetting("savePath"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                string outputData = "";
                outputData = MargeSaveWithExistingData(dataToSave, splitedDataFromFile, outputData);
                StreamWriter streamWriter = new StreamWriter(stream);
                streamWriter.Write(outputData + new string(' ', dataFromFileInLine.Length));
                streamWriter.Close();
            }
        }

        private static string MargeSaveWithExistingData(string[] dataToSave, string[] splitedDataFromFile, string outputData)
        {
            for (int i = 0; i < dataToSave.Length; i++)
            {
                if (splitedDataFromFile[i] != dataToSave[i] && dataToSave[i] == "")
                {
                    outputData += splitedDataFromFile[i];
                }
                else
                {
                    outputData += dataToSave[i];
                }
                outputData += ";";
            }
            return outputData;
        }

        private void LoadExistingFile(out string dataFromFileInLine, out string[] splitedDataFromFile)
        {
            using (Stream stream = File.Open(ReadAppSetting("savePath"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamReader streamReader = new StreamReader(stream);
                dataFromFileInLine = streamReader.ReadToEnd();
                splitedDataFromFile = dataFromFileInLine.Split(';');
                streamReader.Close();
            }
        }

        private void InitializeSaveFile()
        {
            using (StreamWriter streamWriter = new StreamWriter(File.Open(ReadAppSetting("savePath"), FileMode.OpenOrCreate)))
            {
                streamWriter.Write("testovaciPepa;5");
                streamWriter.Close();
            }
        }
    }
}

/*
 
------------------------------- db commands -------------------------------
 
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