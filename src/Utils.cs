using static Utils.LogUtils;
using Dialogs;
using Microsoft.Data.Sqlite;

namespace Utils {
    class SqlOperations {

        public static void DatabaseCheck(string connectionString) {
            using (SqliteConnection connection = new SqliteConnection(connectionString)) {

                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS cards (
                        ID INTEGER PRIMARY KEY,
                        name TEXT,
                        copies INTEGER,
                        type TEXT,
                        frametype TEXT,
                        description TEXT,
                        atk INTEGER,
                        def INTEGER,
                        level INTEGER,
                        race TEXT,
                        attribute TEXT,
                        card_id INTEGER NOT NULL
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

                connection.Dispose();
                return false;

            }
        }

        public static List<List<string>> FindExistingCard(string connectionString, string input) {
            
            List<List<string>> data = new List<List<string>>();
            using(var connection = new SqliteConnection(connectionString)) {

                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM cards WHERE name LIKE '%{input}%';";
                using (var commandReader = command.ExecuteReader()) {
                    while (commandReader.Read()) {
                        List<string> temp = new List<string>();
                        temp.Add(commandReader.GetString(1));
                        temp.Add(commandReader.GetString(2));
                        temp.Add(commandReader.GetString(3));
                        temp.Add(commandReader.GetString(4));
                        temp.Add(commandReader.GetString(5));
                        temp.Add(commandReader.GetString(6));
                        temp.Add(commandReader.GetString(7));
                        temp.Add(commandReader.GetString(8));
                        temp.Add(commandReader.GetString(9));
                        temp.Add(commandReader.GetString(10));
                        temp.Add(commandReader.GetString(11));
                        data.Add(temp);
                    }
                }

            }
            return data;
        }

        public static void InsertCard(string connectionString,List<string> input) {

            using(var connection = new SqliteConnection(connectionString)) {
            
                connection.Open();
                SqliteCommand command = connection.CreateCommand();

                command.CommandText = $@"
                    INSERT INTO cards (name, copies, type, frameType, description, atk, def, level, race, attribute, card_id)
                    VALUES ('{input[0]}', '{input[1]}', '{input[2]}', '{input[3]}', '{input[4]}', '{input[5]}', '{input[6]}', '{input[7]}', '{input[8]}', '{input[9]}', '{input[10]}');";

                try {
                    using (var commandReader = command.ExecuteReader()) {
                    }
                } catch(Exception e) {
                    Log($"{e}");

                }
                command.Dispose();
                connection.Dispose();
            }
            return;
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

        //SCREW pascaL casE
        public static string tententen(string input, int width) {
            input = input.Substring(0, width-3);
            return $"{input}...";
        }

        public static List<string> ConvertCardDataSkeletonToList(CardDataSkeleton input) {
            List<string> converted = new List<string>();
            converted.Add(input.name.ToLower());
            converted.Add(input.copies.ToString());
            converted.Add(input.type);
            converted.Add(input.frameType);
            converted.Add(input.desc);
            if(input.atk != null) {
                converted.Add(input.atk.ToString());
                converted.Add(input.def.ToString());
                converted.Add(input.level.ToString());
            } else {
                converted.Add("n/a");
                converted.Add("n/a");
                converted.Add("n/a");
           }
            converted.Add(input.race);
            converted.Add(input.attribute);
            converted.Add(input.id.ToString());
            
            for(int i = 0; i < converted.Count; i++) {
                if(converted[i] == null) {
                    converted[i] = "n/a";
                }
            }
            return converted;
        }

    }

    class LogUtils {

        public static void Log(string input) {
            using (StreamWriter writer = File.AppendText($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config/cbt/logs/logs.txt")) {
                writer.WriteLine($"{DateTime.Now}: {input}");
            }
        }

        public static void Log(string input, bool allowed) {
            if(allowed == false) {
                return;
            }
            
            using (StreamWriter writer = File.AppendText($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config/cbt/logs/logs.txt")) {
                writer.WriteLine($"{DateTime.Now}: {input}");
            }

        }
    }
}

                //I have to define this fresh hell because of differing and possibly null datatypes

                //Dictionary<string, SqliteType> schema = new Dictionary<string, SqliteType>() {
                //    {"@card_id", SqliteType.Integer},
                //    {"@name", SqliteType.Text},
                //    {"@type", SqliteType.Text},
                //    {"@frameType", SqliteType.Text},
                //    {"@description", SqliteType.Text},
                //    {"@atk", SqliteType.Integer},
                //    {"@def", SqliteType.Integer},
                //    {"@level", SqliteType.Integer},
                //    {"@race", SqliteType.Text},
                //    {"@attribute", SqliteType.Text},
                //    {"@copies", SqliteType.Integer}
                //};

                ////this terribleness adds all of the properties of CardDataSkeleton to an array to be referenced later
                //PropertyInfo[] values = typeof(CardDataSkeleton).GetProperties();

                //int count = 0;
                //foreach(KeyValuePair<string, SqliteType> pair in schema) {
                //    //if a value matching values[count] does not equal null, then execute
                //    if(values[count].GetValue(input,null) != null) {
                //        command.Parameters.Add(pair.Key, pair.Value);
                //        command.Parameters[pair.Key].Value = values[count].GetValue(input, null);
                //    } else {
                //        command.Parameters.Add(pair.Key, pair.Value);
                //        switch(pair.Value) {
                //            case SqliteType.Integer:
                //                command.Parameters[pair.Key].Value = -1;
                //                break;
                //            case SqliteType.Text:
                //                command.Parameters[pair.Key].Value = "No value";
                //                break;
                //        }
                //    }
                //    count++;
                //}
                //string logString = "";
                //for(int i = 0; i < values.Length; i++) {
                //    logString += $"|{values[i].GetValue(input, null)}";
                //}

