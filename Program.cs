// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using Utils;
using static Utils.LogUtils;
using Microsoft.VisualBasic;
using CardBookkeepingTUI;
using Microsoft.Data.Sqlite;
using Formatting;

class Entry {
    static void Main(string[] args) {


        Console.WriteLine("Testing break");
        Console.ReadKey();
        Console.Clear();

        bool log = true;
        string currentDb = "yugioh.db";

        try {
            Directory.CreateDirectory("./logs");
        } catch(Exception e) {
            Console.WriteLine(e);
        }

        try {
            Directory.CreateDirectory("./config");
            Directory.CreateDirectory("./db");
        } catch(Exception e) {
            Log($"{e}", log);
        }




        SqliteConnection connection = new SqliteConnection($"Data Source=./db/{currentDb}");
        Log("Connecting to data server...", log);

        try {
            connection.Open();
        } catch(Exception e) {
            Console.WriteLine(e);
        }

        Log("Success!", log);
        Log("Initializing Master Command", log);

        var masterCommand = connection.CreateCommand();
        SqlOperations.DatabaseCheck(masterCommand);
        using HttpClient client = new();

        bool quit = false;
        List<string> mainMenuOptions = new List<string>() {"Add Card", "Find Card", "Change Card", "Delete Card", "Exit"};
        Menu mainMenu = new Menu(mainMenuOptions);

        while (quit == false) {
            Console.CursorVisible = false;
            Console.Clear();
            mainMenu.Display();

            switch(Console.ReadKey().Key) {
                case System.ConsoleKey.UpArrow:
                case System.ConsoleKey.K:
                    if(mainMenu.index > 0) {
                        mainMenu.index--;
                    }
                    Console.Clear();
                    break;

                case System.ConsoleKey.DownArrow:
                case System.ConsoleKey.J:
                    if(mainMenu.index < mainMenu.menuItems.Count-1) {
                        mainMenu.index++;
                    }
                    Console.Clear();
                    break;

                case System.ConsoleKey.Enter:
                    switch(mainMenu.index) {
                        case 0:
                            Console.Clear();
                            Menu.AddCardMenu(client, masterCommand);
                            break;

                        case 1:
                            Console.Clear();
                            Menu.FindCardMenu(masterCommand);
                            break;

                        case 2:
                            Console.Clear();
                            Menu.UpdateCardMenu(masterCommand);
                            break;

                        case 3:
                            Console.Clear();
                            Menu.DeleteCardMenu(masterCommand);
                            break;

                        case 4:
                            Console.Clear();
                            Console.WriteLine("Goodbye!");
                            quit = true;
                            Console.CursorVisible = true;
                            break;
                    }
                    break;
                default:
                    Console.Clear();
                    break;

            }
        }
    }
}

namespace CardBookkeepingTUI {

    public class Menu {

        public int index;
        public List<string> menuItems;

        public Menu(List<string> items) {
            index = 0;
            menuItems = items;
        }

        public void Display() {

            List<string> splash = new List<string>() {"", "Welcome to the Card Bookkeeping TUI!", ""};
            List<string> displayed = new List<string>();
            for(int i = 0; i < menuItems.Count; i++) {
                displayed.Add(menuItems[i]);
            }

            Textbox.Print(Screen.Center(Textbox.Generate(splash, 50, 5, 1)));

            for(int i = 0; i < displayed.Count; i++) {
                if(i == index) {
                    displayed[i] = $"[[{displayed[i]}]]";
                }
            }

            Textbox.Print(Screen.Center(Textbox.Generate(displayed, 20, 7, 1)));

            //for(int i = 0; i < menuItems.Count; i++) {
            //    if(i == index) {
            //        Console.WriteLine($"[[{menuItems[i]}]]");
            //    } else {
            //        Console.WriteLine($"{menuItems[i]}");
            //    }
            //}
            return;
        }       

        async public static void AddCardMenu(HttpClient client, SqliteCommand masterCommand) {

            List<string> message = new List<string>() {"", "Please enter the name of the card you would like to add", ""};
            
            Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));

            try {
                await HttpOperations.GetCard(masterCommand, client);
            } catch (Exception e) {
                Console.WriteLine("An error occurred");
                Console.WriteLine(e);
                AddCardMenu(client, masterCommand);
            }
        }

        async public static void FindCardMenu(SqliteCommand masterCommand) {

            List<string> message = new List<string>() {"", "What card are you looking for?", ""};
            Textbox.Print(Screen.Center(Textbox.Generate(message, 50, 5, 1)));
            string input = Textbox.PrintInputBox(30);

            CardDataSkeleton data = new CardDataSkeleton();

            try {
                data = SqlOperations.FindExistingCard(masterCommand, input);
            } catch (Exception e) {
                Console.WriteLine(e);
                return;
            }

            Console.Clear();
            message.Clear();
            message.Add("");
            message.Add($"You have {data.copies} copies of {data.name}");
            switch(data.type) {
                case "Spell Card":
                    message.Add($"Type: {data.type}");
                    message.Add($"{data.desc}");
                    break;
                case "Trap Card":
                    message.Add($"Type: {data.type}");
                    message.Add($"{data.desc}");
                    break;
                default:
                    message.Add($"Type: {data.type}");
                    message.Add($"|ATK:    |DEF:    |LEVEL:  |");
                    message.Add($"|{StringUtils.MakeLengthUniform(data.atk)}|{StringUtils.MakeLengthUniform(data.def)}|{StringUtils.MakeLengthUniform(data.level)}|");
                    
                    break;
            }
            
            message.Add($"Press any key to continue...");
            message.Add("");
            Textbox.Print(Screen.Center(Textbox.Generate(message, 40, (message.Count + 2),  1)));
            Console.ReadKey();
        }

        async public static void UpdateCardMenu(SqliteCommand masterCommand) {

            List<string> message = new List<string>() {"", "What card would you like to update?", "", ""};
            Textbox.Print(Screen.Center(Textbox.Generate( message,50, 6, 1)));
            string cardName = Textbox.PrintInputBox(30);

            Console.Clear();
            message[1] = "How many copies do you have now?";
            Textbox.Print(Screen.Center(Textbox.Generate(message, 50, 6, 1)));
            string newCopies = Textbox.PrintInputBox(30);

            int newCopiesConverted = 0;
            bool validInt = false;
            while(validInt == false) {
                if(Information.IsNumeric(newCopies) == true) {
                    newCopiesConverted = Convert.ToInt32(newCopies);
                    break;
                } else {
                    Console.Clear();
                    message[2] = "Invalid characters detected, please only enter numbers";
                    Textbox.Print(Screen.Center(Textbox.Generate(message, 60, 6, 1)));
                    newCopies = Textbox.PrintInputBox(30);
                }
            }

            try {
                SqlOperations.UpdateExistingCard(masterCommand, cardName, newCopiesConverted);
            } catch(Exception e) {
                Console.WriteLine(e);
            }

            Console.Clear();
            message[2] = "";
            message[1] = "Press any key to continue...";
            Textbox.Print(Screen.Center(Textbox.Generate(message, 50, 6, 1)));
            Console.ReadKey();

        }

        async public static void DeleteCardMenu(SqliteCommand masterCommand) {
            List<string> message = new List<string>() { "", "What card would you like to delete?", ""};
            Textbox.Print(Screen.Center(Textbox.Generate(message, 60, 5, 1)));
            string cardName = Textbox.PrintInputBox(30);

            try {
                SqlOperations.DeleteExistingCard(masterCommand, cardName);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            Console.Clear();
            message[1] = "Press any key to continue...";
            Textbox.Print(Screen.Center(Textbox.Generate(message, 60, 5, 1)));
            Console.ReadKey();
            return;
        }
    }

    public class HttpOperations {
        public static async Task GetCard(SqliteCommand masterCommand, HttpClient client) {

            CardDataSkeleton data = new CardDataSkeleton();

            string? card = Textbox.PrintInputBox(30);
            
            List<string> message = new List<string>() {"", "Please enter the name of the card you would like to add", "", ""};

            while (card.Length == 0) {
                Console.Clear();
                message[2] = "Input too short";
                Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
                card = Textbox.PrintInputBox(30);
            }

            message[2] = "";

            if(SqlOperations.CheckForExistingCard(masterCommand, card) == true) {
                Console.Clear();
                message[1] = "Card already exists! Please update the card instead";
                message[2] = "Press any key to continue...";
                Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
                Console.ReadKey();
                return;
            }

            message[1] = "How many copies do you have?";
            Console.Clear();
            Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
            string copiesRaw = Textbox.PrintInputBox(30);

            int copies = 0;
            bool validInt = false;

            while(validInt == false) {
                if(Information.IsNumeric(copiesRaw) == true) {
                    copies = Convert.ToInt32(copiesRaw);
                    break;
                } else {
                    Console.Clear();
                    message[1] = "Invalid characters detected, please only enter numbers";
                    Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
                    copiesRaw = Textbox.PrintInputBox(30);
                }
            }

            card = card.Replace(" ","%20");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try {
                string response = await client.GetStringAsync($"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={card}");
                JsonDocument unformatted = JsonSerializer.Deserialize<JsonDocument>(response);
                var dataArray = unformatted.RootElement.GetProperty("data");
                var restringified = JsonSerializer.Serialize(dataArray[0]);

                CardDataSkeleton newCard = JsonSerializer.Deserialize<CardDataSkeleton>(restringified);
                data = newCard;
                data.copies = copies;
                SqlOperations.InsertCard(masterCommand, data);                               

            } catch(Exception e) {
                Console.WriteLine(e);
            }
        }
    }

    public class CardDataSkeleton {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? frameType { get; set; }
        public string? desc { get; set; }
        public int? atk { get; set; }
        public int? def { get; set; }
        public int? level { get; set; }
        public string? race { get; set; }
        public string? attribute { get; set; }
        public int? copies { get; set; }
    }
}
