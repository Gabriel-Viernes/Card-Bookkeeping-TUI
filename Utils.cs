using MySql.Data;
using MySql.Data.MySqlClient;
using static Utils.LogUtils;
using CardBookkeepingTUI;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Utils {
    class SqlOperations {

        public static void DatabaseCheck(string connectionString) {
            using (SqliteConnection connection = new SqliteConnection(connectionString)) {

                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"
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
                
                using (var commandReader = command.ExecuteReader()) {
                    while(commandReader.Read()) {
                        Console.WriteLine("Checking Tables...");
                    }
                }

                connection.Close();
            }
                
        }

        public static bool CheckForExistingCard(string connectionString, string input) {
            using(var connection = new SqliteConnection(connectionString)) {

                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = $"SELECT name FROM cards WHERE name = '{input}';";
                using (var commandReader = command.ExecuteReader()) {
                    while (commandReader.Read()) {
                        string test = commandReader.GetString(0);
                        if(test == $"{input}") {
                            connection.Close();
                            return true;
                        }
                    }
                }

                connection.Close();
                return false;

            }
        }

        public static CardDataSkeleton FindExistingCard(string connectionString, string input) {

            using(var connection = new SqliteConnection(connectionString)) {

                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM cards WHERE name = '{input}';";
                CardDataSkeleton data = new CardDataSkeleton();
                using (var commandReader = command.ExecuteReader()) {
                    while (commandReader.Read()) {
                        data.id = commandReader.GetInt32(1);
                        data.name = commandReader.GetString(2);
                        data.type = commandReader.GetString(3);
                        data.frameType = commandReader.GetString(4);
                        data.desc = commandReader.GetString(5);
                        data.atk = commandReader.GetInt32(6);
                        data.def = commandReader.GetInt32(7);
                        data.level = commandReader.GetInt32(8);
                        data.race = commandReader.GetString(9);
                        data.attribute = commandReader.GetString(10);
                        data.copies = commandReader.GetInt32(11);
                    }
                }
                return data;
            }
        }

        public static void InsertCard(string connectionString,CardDataSkeleton input) {

            using(var connection = new SqliteConnection(connectionString)) {
            
                connection.Open();
                SqliteCommand command = connection.CreateCommand();

                command.CommandText = @"
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
                        command.Parameters.Add(pair.Key, pair.Value);
                        command.Parameters[pair.Key].Value = values[count].GetValue(input, null);
                    } else {
                        command.Parameters.Add(pair.Key, pair.Value);
                        switch(pair.Value) {
                            case SqliteType.Integer:
                                command.Parameters[pair.Key].Value = -1;
                                break;
                            case SqliteType.Text:
                                command.Parameters[pair.Key].Value = "No value";
                                break;
                        }
                    }
                    count++;
                }
                using (var commandReader = command.ExecuteReader()) {
                    Log($"{command.Parameters}", true);
                }
            }


           
        }

        public static void UpdateExistingCard(string connectionString, string cardName, int newCopies) {
            using(SqliteConnection connection = new SqliteConnection(connectionString)) {

                connection.Open();
                SqliteCommand command = connection.CreateCommand();

                command.CommandText = $"UPDATE cards set copies = {newCopies} WHERE name = '{cardName}';";
                using(var commandReader = command.ExecuteReader()) {
                    Console.WriteLine("Updating cards...");
                }

                connection.Close();
            }

        }

        public static void DeleteExistingCard(string connectionString, string cardName) {

            using(SqliteConnection connection = new SqliteConnection(connectionString)) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM cards WHERE name = '{cardName}';";
                using(var commandReader = command.ExecuteReader()) {
                    Console.WriteLine("Deleting cards...");
                }
                connection.Close();
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


