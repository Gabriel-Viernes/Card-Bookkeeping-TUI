using MySql.Data;
using MySql.Data.MySqlClient;
using YugiohLocalDatabase;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Collections.Generic;
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
            masterCommand.CommandText = @"SELECT name FROM cards WHERE name = @name;";
            Console.WriteLine("Checking for existing card...");
            masterCommand.Parameters.Add("@name", SqliteType.Text);
            masterCommand.Parameters["@name"].Value = input;
            using (var masterCommandReader = masterCommand.ExecuteReader()) {
                while (masterCommandReader.Read()) {
                    Console.WriteLine(masterCommandReader.GetString(2));
                    if(masterCommandReader.GetString(0) == $"{input}") {
                        return true;
                    }
                }
            }
            return false;
        }
        public static CardDataSkeleton FindExistingCard(SqliteCommand masterCommand, string input) {
            masterCommand.Parameters.Clear();
            masterCommand.CommandText = @"SELECT * FROM cards WHERE name = @name;";
            Console.WriteLine("Checking for existing card...");
            masterCommand.Parameters.Add("@name", SqliteType.Text);
            masterCommand.Parameters["@name"].Value = input;
            CardDataSkeleton data = new CardDataSkeleton();
            using (var masterCommandReader = masterCommand.ExecuteReader()) {

                while (masterCommandReader.Read()) {
                    Console.WriteLine(masterCommandReader.GetString(2));
                    //data.id = masterCommandReader.GetInt32(0);
                    //data.name = masterCommandReader.GetString(1);
                    //data.type = masterCommandReader.GetString(2);
                    //data.frameType = masterCommandReader.GetString(3);
                    //data.desc = masterCommandReader.GetString(4);
                    //data.atk = masterCommandReader.GetInt32(5);
                    //data.def = masterCommandReader.GetInt32(6);
                    //data.level = masterCommandReader.GetInt32(7);
                    //data.race = masterCommandReader.GetString(8);
                    //data.attribute = masterCommandReader.GetString(9);
                    //data.copies = masterCommandReader.GetInt32(10);
                }
            }
            Console.WriteLine(data.id);
            Console.WriteLine(data.name);
            Console.WriteLine(data.copies);
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
                //Console.WriteLine($"{pair.Key}||{pair.Value}");
                //Console.WriteLine(values[count].GetValue(input,null));

                if(values[count].GetValue(input,null) != null) {
                    masterCommand.Parameters.Add(pair.Key, pair.Value);
                    masterCommand.Parameters[pair.Key].Value = values[count].GetValue(input, null);
                    Console.WriteLine("Value added");
                } else {
                    Console.WriteLine("Null value detected");
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
    }
}

//SqliteCommand myCommand = new MySqlCommand();
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


            //masterCommand.Parameters.Add("@card_id", SqliteType.Integer);
            //masterCommand.Parameters["@card_id"].Value = input.id;
            //masterCommand.Parameters.Add("@name", SqliteType.Text);
            //masterCommand.Parameters["@name"].Value = input.name;
            //masterCommand.Parameters.Add("@type", SqliteType.Text);
            //masterCommand.Parameters["@type"].Value = input.type;
            //masterCommand.Parameters.Add("@frameType", SqliteType.Text);
            //masterCommand.Parameters["@frameType"].Value = input.frameType;
            //masterCommand.Parameters.Add("@description", SqliteType.Text);
            //masterCommand.Parameters["@description"].Value = input.desc;
            //masterCommand.Parameters.Add("@atk", SqliteType.Integer);
            //masterCommand.Parameters["@atk"].Value = input.atk;
            //masterCommand.Parameters.Add("@def", SqliteType.Integer);
            //masterCommand.Parameters["@def"].Value = input.def;
            //masterCommand.Parameters.Add("@level", SqliteType.Integer);
            //masterCommand.Parameters["@level"].Value = input.level;
            //masterCommand.Parameters.Add("@race", SqliteType.Text);
            //masterCommand.Parameters["@race"].Value = input.race;
            //masterCommand.Parameters.Add("@attribute", SqliteType.Text);
            //masterCommand.Parameters["@attribute"].Value = input.attribute;
            //masterCommand.Parameters.Add("@copies", SqliteType.Integer);
            //masterCommand.Parameters["@copies"].Value = input.copies;

