using MySql.Data;
using MySql.Data.MySqlClient;
using YugiohLocalDatabase;
namespace Utils {
    class SqlOperations {
        public static void DatabaseCheck(MySqlCommand masterCommand) {
                masterCommand.CommandText = @"
                    CREATE DATABASE IF NOT EXISTS yugioh;
                ";
                using (MySqlDataReader masterCommandReader = masterCommand.ExecuteReader()) {
                    while(masterCommandReader.Read()){
                        Console.WriteLine("Checking for database...");
                    }
                }
                masterCommand.CommandText = @"
                    USE yugioh;
                    CREATE TABLE IF NOT EXISTS cards (
                        ID int NOT NULL AUTO_INCREMENT,
                        card_id int NOT NULL,
                        name varchar(255),
                        type varchar(255),
                        frametype varchar(255),
                        description text(65535),
                        atk int,
                        def int,
                        level int,
                        race varchar(255),
                        attribute varchar(255),
                        copies int,
                        PRIMARY KEY (ID)
                );";
                using (MySqlDataReader masterCommandReader = masterCommand.ExecuteReader()) {
                    while(masterCommandReader.Read()) {
                        Console.WriteLine("Checking Tables...");
                    }
                }
        }
        public static bool CheckForExistingCard(MySqlCommand masterCommand, string input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = @"SELECT name FROM cards WHERE name = @name;";
            Console.WriteLine("Checking for existing card...");
            masterCommand.Parameters.AddWithValue("@name", input);
            using (MySqlDataReader masterCommandReader = masterCommand.ExecuteReader()) {
                while (masterCommandReader.Read()) {
                    if(masterCommandReader.GetString("name") == $"{input}") {
                        return true;
                    }
                }
            }
            return false;
        }
        public static CardDataSkeleton FindExistingCard(MySqlCommand masterCommand, string input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = @"SELECT * FROM cards WHERE name = @name;";
            Console.WriteLine("Checking for existing card...");
            masterCommand.Parameters.AddWithValue("@name", input);
            CardDataSkeleton data = new CardDataSkeleton();
            using (MySqlDataReader masterCommandReader = masterCommand.ExecuteReader()) {
                while (masterCommandReader.Read()) {
                    data.id = masterCommandReader.GetInt32("card_id");
                    data.name = masterCommandReader.GetString("name");
                    data.type = masterCommandReader.GetString("type");
                    data.frameType = masterCommandReader.GetString("frametype");
                    data.desc = masterCommandReader.GetString("description");
                    data.atk = masterCommandReader.GetInt32("atk");
                    data.def = masterCommandReader.GetInt32("def");
                    data.level = masterCommandReader.GetInt32("level");
                    data.race = masterCommandReader.GetString("race");
                    data.attribute = masterCommandReader.GetString("attribute");
                    data.copies = masterCommandReader.GetInt32("copies");
                }
            }
            Console.WriteLine(data);
            return data;
        }
        public static void InsertCard(MySqlCommand masterCommand,CardDataSkeleton input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = @"
                INSERT INTO cards (card_id, name, type, frameType, description, atk, def, level, race, attribute, copies)
                VALUES (@card_id, @name, @type, @frameType, @description, @atk, @def, @level, @race, @attribute, @copies);";
            masterCommand.Parameters.AddWithValue("@card_id", input.id);
            masterCommand.Parameters.AddWithValue("@name", input.name);
            masterCommand.Parameters.AddWithValue("@type", input.type);
            masterCommand.Parameters.AddWithValue("@frameType", input.frameType);
            masterCommand.Parameters.AddWithValue("@description", input.desc);
            masterCommand.Parameters.AddWithValue("@atk", input.atk);
            masterCommand.Parameters.AddWithValue("@def", input.def);
            masterCommand.Parameters.AddWithValue("@level", input.level);
            masterCommand.Parameters.AddWithValue("@race", input.race);
            masterCommand.Parameters.AddWithValue("@attribute", input.attribute);
            masterCommand.Parameters.AddWithValue("@copies", input.copies);
            using (MySqlDataReader masterCommandReader = masterCommand.ExecuteReader()) {
                Console.WriteLine("Inserting cards...");
            }
           
        }
    }
}

//MySqlCommand myCommand = new MySqlCommand();
//                myCommand.Connection = myConnection;
//                myCommand.CommandText = @"
//                    CREATE TABLE card (
//                        ID int NOT NULL AUTO_INCREMENT,
//                        card_id int NOT NULL,
//                        type varchar(255),
//                        frametype varchar(255),
//                        description varchar(255),
//                        atk int,
//                        def int,
//                        level int,
//                        race varchar(255),
//                        attribute varchar(255),
//                        copies int,
//                        PRIMARY KEY (ID)
//                );";
//                MySqlDataReader rdr = myCommand.ExecuteReader();
//                while(rdr.Read()) {
//                    Console.WriteLine(rdr[0]);
//                }
//                rdr.Close();

