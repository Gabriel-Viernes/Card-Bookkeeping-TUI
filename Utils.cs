using MySql.Data;
using MySql.Data.MySqlClient;
using CardBookkeepingTUI;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Utils {
    class SqlOperations {
        public static void DatabaseCheck(SqliteCommand masterCommand) {
                //masterCommand.CommandText = @"
                //    CREATE DATABASE IF NOT EXISTS yugioh;
                //";
                //using (var masterCommandReader = masterCommand.ExecuteReader()) {
                //    while(masterCommandReader.Read()){
                //        Console.WriteLine("Checking for database...");
                //    }
                //}
                masterCommand.CommandText = @"
                    CREATE TABLE IF NOT EXISTS cards (
                        ID INTEGER PRIMARY KEY,
                        card_id INTEGER NOT NULL,
                        name TEXT,
                        type TEXT,
                        frametype TEXT,
                        description TEXT,
                        atk INTEGER,
                        def INTEGER,
                        level INTEGER,
                        race TEXT,
                        attribute TEXT,
                        copies INTEGER
                );";
                using (var masterCommandReader = masterCommand.ExecuteReader()) {
                    while(masterCommandReader.Read()) {
                        Console.WriteLine("Checking Tables...");
                    }
                }
        }
        public static bool CheckForExistingCard(SqliteCommand masterCommand, string input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = $"SELECT name FROM cards WHERE name = '{input}';";
            using (var masterCommandReader = masterCommand.ExecuteReader()) {
                while (masterCommandReader.Read()) {
                    string test = masterCommandReader.GetString(0);
                    if(test == $"{input}") {
                        return true;
                    }
                }
            }
            return false;
        }
        public static CardDataSkeleton FindExistingCard(SqliteCommand masterCommand, string input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = $"SELECT * FROM cards WHERE name = '{input}';";
            CardDataSkeleton data = new CardDataSkeleton();
            using (var masterCommandReader = masterCommand.ExecuteReader()) {
                while (masterCommandReader.Read()) {
                    data.id = masterCommandReader.GetInt32(1);
                    data.name = masterCommandReader.GetString(2);
                    data.type = masterCommandReader.GetString(3);
                    data.frameType = masterCommandReader.GetString(4);
                    data.desc = masterCommandReader.GetString(5);
                    data.atk = masterCommandReader.GetInt32(6);
                    data.def = masterCommandReader.GetInt32(7);
                    data.level = masterCommandReader.GetInt32(8);
                    data.race = masterCommandReader.GetString(9);
                    data.attribute = masterCommandReader.GetString(10);
                    data.copies = masterCommandReader.GetInt32(11);
                }
            }
            return data;
        }
        public static void InsertCard(SqliteCommand masterCommand,CardDataSkeleton input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = @"
                INSERT INTO cards (card_id, name, type, frameType, description, atk, def, level, race, attribute, copies)
                VALUES (@card_id, @name, @type, @frameType, @description, @atk, @def, @level, @race, @attribute, @copies);";
            Dictionary<string, SqliteType> schema = new Dictionary<string, SqliteType>() {
                {"@card_id", SqliteType.Integer},
                {"@name", SqliteType.Text},
                {"@type", SqliteType.Text},
                {"@frameType", SqliteType.Text},
                {"@description", SqliteType.Text},
                {"@atk", SqliteType.Integer},
                {"@def", SqliteType.Integer},
                {"@level", SqliteType.Integer},
                {"@race", SqliteType.Text},
                {"@attribute", SqliteType.Text},
                {"@copies", SqliteType.Integer}
            };
            PropertyInfo[] values = typeof(CardDataSkeleton).GetProperties();
            int count = 0;
            foreach(KeyValuePair<string, SqliteType> pair in schema) {
                if(values[count].GetValue(input,null) != null) {
                    masterCommand.Parameters.Add(pair.Key, pair.Value);
                    masterCommand.Parameters[pair.Key].Value = values[count].GetValue(input, null);
                } else {
                    masterCommand.Parameters.Add(pair.Key, pair.Value);
                    switch(pair.Value) {
                        case SqliteType.Integer:
                            masterCommand.Parameters[pair.Key].Value = -1;
                            break;
                        case SqliteType.Text:
                            masterCommand.Parameters[pair.Key].Value = "No value";
                            break;
                    }
                }
                count++;
            }
            using (var masterCommandReader = masterCommand.ExecuteReader()) {
                Console.WriteLine("Inserting cards...");
            }
           
        }
        public static void UpdateExistingCard(SqliteCommand masterCommand, string cardName, int newCopies) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = $"UPDATE cards set copies = {newCopies} WHERE name = '{cardName}';";
            using(var masterCommandReader = masterCommand.ExecuteReader()) {
                Console.WriteLine("Updating cards...");
            }
        }
        public static void DeleteExistingCard(SqliteCommand masterCommand, string cardName) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = $"DELETE FROM cards WHERE name = '{cardName}';";
            using(var masterCommandReader = masterCommand.ExecuteReader()) {
                Console.WriteLine("Deleting cards...");
            }
        }
        
    }
    class StringUtils {
        public static string MakeLengthUniform(int? input) {
            string output = input.ToString();
            if(output.Length < 8) {
                string empty = new string (' ', 8 - output.Length);
                output = empty+output;
            }
            return output;
        }
    }

    class LogUtils {

        public static void Log(string input, bool allowed) {
            if(allowed == false) {
                return;
            }
            
            using (StreamWriter writer = File.AppendText("./logs/logs.txt")) {
                writer.WriteLine($"{DateTime.Now}: {input}");
            }
        }
    }
}


