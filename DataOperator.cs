using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using Google.Protobuf.Reflection;
using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
    public class DataOperator
    {
        
        private TimeSpan playTime;
        private int fieldSize;
        private int mineCount;
        public void SaveResultData(TimeSpan playTime, int fieldSize, int mineCount)
        {
            this.playTime = playTime;
            this.fieldSize = fieldSize;
            this.mineCount = mineCount;
            Thread saving = new Thread(SavingDataToWebStorage);
            saving.Start();
            
        }

        private void SavingDataToWebStorage()
        {
            WebClient webClient = new WebClient();

            webClient.QueryString.Add("player", LoadSaveData()[0]);
            webClient.QueryString.Add("playtime", Convert.ToString(playTime));
            webClient.QueryString.Add("fieldSize", Convert.ToString(fieldSize));
            webClient.QueryString.Add("mineCount", Convert.ToString(mineCount));
            try
            {
                webClient.DownloadString(ReadAppSetting("webside"));
            }
            catch (Exception e)
            {
                // ignored
            }
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

        public string[] LoadSaveData()
        {
            load:
            if (File.Exists(ReadAppSetting("savePath")))
            {
                string[] splitedDataFromFile;
                LoadExistingFile(out _, out splitedDataFromFile);
                return splitedDataFromFile;
            }

            InitializeSaveFile();
            goto load;
        }

        public void WriteSetting(string[] dataToSave)
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
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    streamWriter.Write(outputData + new string(' ', dataFromFileInLine.Length));
                    streamWriter.Close();
                }
            }
        }

        private static string MargeSaveWithExistingData(string[] dataToSave, string[] splitedDataFromFile,
            string outputData)
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
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    dataFromFileInLine = streamReader.ReadToEnd();
                    splitedDataFromFile = dataFromFileInLine.Split(';');
                    streamReader.Close();
                }
            }
        }

        private void InitializeSaveFile()
        {
            using (StreamWriter streamWriter =
                new StreamWriter(File.Open(ReadAppSetting("savePath"), FileMode.OpenOrCreate)))
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