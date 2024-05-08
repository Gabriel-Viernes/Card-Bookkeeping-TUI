﻿using System.Net.Http.Headers;
using System.Text.Json;
using Utils;
using static Utils.LogUtils;
using Microsoft.VisualBasic;
using CardBookkeepingTUI;
using Formatting;

class Entry {
    static void Main(string[] args) {


        //Console.WriteLine("Testing break");
        //Console.ReadKey();
        Console.Clear();

        bool doILog = true;
        string currentDb = "yugioh.db";
        string connectionString = $"Data Source=./db/{currentDb}";

        int menuIndex = 0;

        try {
            Directory.CreateDirectory("./logs");
        } catch(Exception e) {
            Log($"{e}", doILog);
        }

        try {
            Directory.CreateDirectory("./config");
            Directory.CreateDirectory("./db");
        } catch(Exception e) {
            Log($"{e}", doILog);
        }


        SqlOperations.DatabaseCheck(connectionString);

        bool quit = false;
        List<string> splash = new List<string>() {"", "Welcome to the Card Bookkeeping TUI!", ""};
        List<string> options = new List<string>() {"Add Card", "Find Card", "Change Card", "Delete Card", "Exit"};
        int index = 0;


        while (quit == false) {
            GC.Collect();
            Console.CursorVisible = false;
            Console.Clear();
            Textbox.Print(Screen.Center(Textbox.Generate(splash, 50, 5, 1)));
            Textbox.Print(Screen.Center(Textbox.GenerateMenu(options, index, 20, 1)));


            switch(Console.ReadKey().Key) {
                case System.ConsoleKey.UpArrow:
                case System.ConsoleKey.K:
                    Console.Clear();
                    if(index > 0) {
                        index--;
                    }
                    break;

                case System.ConsoleKey.DownArrow:
                case System.ConsoleKey.J:
                    Console.Clear();
                    if(index < options.Count-1) {
                        index++;
                    }
                    break;

                case System.ConsoleKey.Enter:
                    switch(index) {
                        case 0:
                            Console.Clear();
                            Menu.AddCardMenu(connectionString);
                            break;

                        case 1:
                            Console.Clear();
                            Menu.FindCardMenu(connectionString);
                            break;

                        case 2:
                            Console.Clear();
                            Menu.UpdateCardMenu(connectionString);
                            break;

                        case 3:
                            Console.Clear();
                            Menu.DeleteCardMenu(connectionString);
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
        List<string> splash = new List<string>() {"", "Welcome to the Card Bookkeeping TUI!", ""};


        public Menu(List<string> items) {
            index = 0;
            menuItems = items;
        }

        public void Display() {

            Textbox.Print(Screen.Center(Textbox.Generate(splash, 50, 5, 1)));
            Textbox.Print(Screen.Center(Textbox.GenerateMenu(menuItems, index, 20, 1)));
            return;
        }       

        async public static void AddCardMenu(string connectionString) {

            List<string> message = new List<string>() {"", "Please enter the name of the card you would like to add", ""};
            
            Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));

            try {
                await HttpOperations.GetCard(connectionString);
            } catch (Exception e) {
                Console.WriteLine("An error occurred");
                Console.WriteLine(e);
                AddCardMenu(connectionString);
            }
        }

        async public static void FindCardMenu(string connectionString) {

            
            List<string> message = new List<string>() {"", "What card are you looking for?", ""};
            Textbox.Print(Screen.Center(Textbox.Generate(message, 50, 5, 1)));
            string input = Textbox.PrintInputBox(30);

            List<CardDataSkeleton> data = new List<CardDataSkeleton>();

            //if(SqlOperations.CheckForExistingCard(connectionString, input) == false) {
            //    Console.Clear();
            //    message[1] = "Card not found! Please make sure that you entered the card's name correctly";
            //    message[2] = "Press any key to continue...";
            //    message.Add("");
            //    Textbox.Print(Screen.Center(Textbox.Generate(message ,1)));
            //    Console.ReadKey();
            //    return;
            //}

            try {
                data = SqlOperations.FindExistingCard(connectionString, input);
            } catch (Exception e) {
                Console.WriteLine(e);
                return;
            }


            Console.Clear();
            message.Clear();

            Console.WriteLine($"{data.Count}");
            for(int i = 0; i < data.Count; i++) {
                Console.WriteLine(data[i].name);
            }
            Console.ReadKey();
           // message.Add("");
           // message.Add($"You have {data.copies} copies of {data.name}");
           // switch(data.type) {
           //     case "Spell Card":
           //         message.Add($"Type: {data.type}");
           //         message.Add($"{data.desc}");
           //         break;
           //     case "Trap Card":
           //         message.Add($"Type: {data.type}");
           //         message.Add($"{data.desc}");
           //         break;
           //     default:
           //         message.Add($"Type: {data.type}");
           //         message.Add($"|ATK:    |DEF:    |LEVEL:  |");
           //         message.Add($"|{StringUtils.MakeLengthUniform(data.atk)}|{StringUtils.MakeLengthUniform(data.def)}|{StringUtils.MakeLengthUniform(data.level)}|");
           //         message.Add($"{data.desc}");
           //         break;
           // }
           // 
           // message.Add($"Press any key to continue...");
           // message.Add("");
            Textbox.Print(Screen.Center(Textbox.Generate(message, 40, 25, 1)));
            Console.ReadKey();
        }

        async public static void UpdateCardMenu(string connectionString) {

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
                SqlOperations.UpdateExistingCard(connectionString, cardName, newCopiesConverted);
            } catch(Exception e) {
                Console.WriteLine(e);
            }

            Console.Clear();
            message[2] = "";
            message[1] = "Press any key to continue...";
            Textbox.Print(Screen.Center(Textbox.Generate(message, 50, 6, 1)));
            Console.ReadKey();

        }

        async public static void DeleteCardMenu(string connectionString) {
            List<string> message = new List<string>() { "", "What card would you like to delete?", ""};
            Textbox.Print(Screen.Center(Textbox.Generate(message, 60, 5, 1)));
            string cardName = Textbox.PrintInputBox(30);

            try {
                SqlOperations.DeleteExistingCard(connectionString, cardName);
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

        public static async Task GetCard(string connectionString) {

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

            if(SqlOperations.CheckForExistingCard(connectionString, card) == true) {
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

            using(HttpClient client = new HttpClient()) {

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try {
                    string response = await client.GetStringAsync($"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={card}");
                    //api response is data:{ 0: {(whatever key/value)}};
                    JsonDocument unformatted = JsonSerializer.Deserialize<JsonDocument>(response);
                    var dataArray = unformatted.RootElement.GetProperty("data");
                    var restringified = JsonSerializer.Serialize(dataArray[0]);

                    CardDataSkeleton newCard = JsonSerializer.Deserialize<CardDataSkeleton>(restringified);
                    data = newCard;
                    data.copies = copies;
                    List<string> test = StringUtils.ConvertCardDataSkeletonToList(data);
                    for(int i = 0; i < test.Count; i++) {
                        Log($"{test[i]}", true);
                    }
                    SqlOperations.InsertCard(connectionString, test);                               

                } catch(Exception e) {
                    Log($"{e}", true);
                }

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
