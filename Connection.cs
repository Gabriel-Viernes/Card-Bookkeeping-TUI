using MySql.Data;
using MySql.Data.MySqlClient;
namespace Connection {
    public class OpenConnection {
        public static void Open() {
            MySqlConnection myConnection;
            string connectionString = "server=localhost;userid=notsito;password=minifigure1-is-king";
            try {
                myConnection = new MySqlConnection(connectionString);
                Console.WriteLine("Connecting to MySQL server...");
                myConnection.Open();
                Console.WriteLine("Success!");
                MySqlCommand databaseCheck = new MySqlCommand();
                databaseCheck.Connection = myConnection;
                Console.WriteLine("Checking for database...");
                databaseCheck.CommandText = @"
                    CREATE DATABASE IF NOT EXISTS yugioh;
                ";
                MySqlDataReader databaseCheckReader = databaseCheck.ExecuteReader();
                databaseCheckReader.Close();
                Console.WriteLine("Checking Tables...");
                databaseCheck.CommandText = @"
                    USE yugioh;
                    CREATE TABLE IF NOT EXISTS cards (
                        ID int NOT NULL AUTO_INCREMENT,
                        card_id int NOT NULL,
                        type varchar(255),
                        frametype varchar(255),
                        description varchar(255),
                        atk int,
                        def int,
                        level int,
                        race varchar(255),
                        attribute varchar(255),
                        copies int,
                        PRIMARY KEY (ID)
                );";

                MySqlDataReader tableCheckReader = databaseCheck.ExecuteReader();
                databaseCheckReader.Close();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
    class SqlOperations {

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

