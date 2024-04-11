// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using Utils;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using YugiohLocalDatabase;
using Microsoft.Data.Sqlite;

class Entry {
    static void Main(string[] args) {
        Console.Clear();
        string putThisInAConfigRetard = "Data Source=yugioh.db";
        //TODO: lrn to use configs
        SqliteConnection connection = new SqliteConnection(putThisInAConfigRetard);
        Console.WriteLine("Connecting to data server...");
        try {
            connection.Open();
        } catch(Exception e) {
            Console.WriteLine(e);
        }
        Console.WriteLine("Success!");
        Console.WriteLine("Initializing Master Command");
        var masterCommand = connection.CreateCommand();
        Console.WriteLine(masterCommand);
        SqlOperations.DatabaseCheck(masterCommand);
        using HttpClient client = new();
        bool quit = false;
        string[] mainMenuOptions = {"Add Card", "Find Card", "Change Card", "Delete Card", "Exit"};
        Menu mainMenu = new Menu(mainMenuOptions);
        //menu code begins here
        while (quit == false) {
            mainMenu.Display();
            switch(Console.ReadKey().Key) {
                case System.ConsoleKey.UpArrow:
                    if(mainMenu.index > 0) {
                        mainMenu.index--;
                    }
                    Console.Clear();
                    break;
                case System.ConsoleKey.DownArrow:
                    if(mainMenu.index < mainMenu.menuItems.Length-1) {
                        mainMenu.index++;
                    }
                    Console.Clear();
                    break;
                case System.ConsoleKey.Enter:
                    switch(mainMenu.index) {
                        case 0:
                            Menu.AddCardMenu(client, masterCommand);
                            break;
                        case 1:
                            Menu.FindCardMenu(masterCommand);
                            break;
                        case 4:
                            Console.Clear();
                            Console.WriteLine("Goodbye!");
                            quit = true;
                        break;
                    }
                    break;
            }
        }
    }
}

namespace YugiohLocalDatabase {
    public class Menu {
        public int index;
        public string[] menuItems;
        public Menu(string[] items) {
            index = 0;
            menuItems = items;
        }
        public void Display() {
            Console.WriteLine("======================================================");
            Console.WriteLine("         Welcome to the Yugioh Local Database!        ");
            Console.WriteLine("======================================================");
            for(int i = 0; i < menuItems.Length; i++) {

                if(i == index) {
                    Console.WriteLine($"[[{menuItems[i]}]]");
                } else {
                    Console.WriteLine($"{menuItems[i]}");
                }
            }
            return;
        }       
        async public static void AddCardMenu(HttpClient client, SqliteCommand masterCommand) {
            CardLookup findCard = new CardLookup(client);
            Console.WriteLine("Please enter the name of the card you would like to add");
            try {
                await findCard.GetCard(masterCommand);
            } catch (Exception e) {
                Console.WriteLine("An error occurred");
                AddCardMenu(client, masterCommand);
            }
        }
        async public static void FindCardMenu(SqliteCommand masterCommand) {
            Console.WriteLine("Card Name?");
            string input = Console.ReadLine();
            CardDataSkeleton data = SqlOperations.FindExistingCard(masterCommand, input);
            Console.WriteLine($"{data.name} :");
            Console.WriteLine($"You have {data.copies} copies");
        }

    }

    public class CardLookup {
        public string? card;
        public HttpClient client;
        public CardDataSkeleton data;
        public CardLookup(HttpClient inputClient) {
            client = inputClient;
            data = new CardDataSkeleton();
        }
        public async Task GetCard(SqliteCommand masterCommand) {
            card = Console.ReadLine();
            if(SqlOperations.CheckForExistingCard(masterCommand, card) == true) {
                Console.WriteLine("Card already exists!");
                return;
            }
            Console.WriteLine("How many copies do you have?");
            string copiesRaw = Console.ReadLine();
            int copies = 0;
            bool validInt = false;
            while(validInt == false) {
                if(Information.IsNumeric(copiesRaw) == true) {
                    copies = Convert.ToInt32(copiesRaw);
                    break;
                } else {
                    Console.WriteLine("Invalid characters detected, please only enter numbers");
                    copiesRaw = Console.ReadLine();
                }
            }
            card = card.Replace(" ","%20");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try {
                string response = await client.GetStringAsync($"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={card}");
                CardSkeleton unformatted = JsonSerializer.Deserialize<CardSkeleton>(response);
                var restringified = JsonSerializer.Serialize(unformatted.data[0]);
                CardDataSkeleton newCard = JsonSerializer.Deserialize<CardDataSkeleton>(restringified);
                data = newCard;
                data.copies = copies;
                SqlOperations.InsertCard(masterCommand, data);                               
            } catch(Exception e) {
                Console.WriteLine(e);
            }
        }
    }
    public class CardSkeleton {
        public object[] data { get; set; }
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
