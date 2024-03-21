// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;
using Utils;
using MySql.Data.MySqlClient;
namespace YugiohLocalDatabase {
    class Entry {
        static void Main(string[] args) {
            Console.Clear();
            string putThisInAConfigRetard = "server=localhost;userid=notsito;password=minifigure1-is-king";
            //TODO: lrn to use configs
            MySqlConnection sqlConnection = new MySqlConnection(putThisInAConfigRetard);
            Console.WriteLine("Connecting to MySQL server...");
            try {
                sqlConnection.Open();
            } catch(Exception e) {
                Console.WriteLine(e);
            }
            Console.WriteLine("Success!");
            Console.WriteLine("Initializing Master Command");
            MySqlCommand masterCommand = new MySqlCommand("", sqlConnection);
            SqlOperations.DatabaseCheck(masterCommand);
            using HttpClient client = new();
            bool quit = false;
            string[] mainMenuOptions = {"Add Card", "Find Card", "Change Card", "Delete Card", "Exit"};
            Menu mainMenu = new Menu(mainMenuOptions);
            mainMenu.Display();
            while (quit == false) {
                switch(Console.ReadKey().Key) {
                    case System.ConsoleKey.UpArrow:
                        if(mainMenu.index > 0) {
                            mainMenu.index--;
                        }
                        Console.Clear();
                        mainMenu.Display();
                        break;
                    case System.ConsoleKey.DownArrow:
                        if(mainMenu.index < mainMenu.menuItems.Length-1) {
                            mainMenu.index++;
                        }
                        Console.Clear();
                        mainMenu.Display();
                        break;
                    case System.ConsoleKey.Enter:
                        switch(mainMenu.index) {
                            case 0:
                                Console.Clear();
                                Console.WriteLine("Please enter the name of the card you wish to add");
                                string input = Console.ReadLine();
                                input = input.Replace(" ","%20");
                                CardLookup findCard = new CardLookup(input, client);
                                findCard.GetCard().ContinueWith((data) => {
                                    Console.WriteLine(findCard.data.id);
                                    SqlOperations.InsertCard(masterCommand, findCard.data);                               
                                });
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

    }

    public class CardLookup {
        public string card;
        public HttpClient client;
        public CardDataSkeleton data;
        public CardLookup(string input, HttpClient inputClient) {
            card = input;
            client = inputClient;
            data = new CardDataSkeleton();
        }
        public async Task GetCard() {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            card = card.Replace(" ","%20");
            try {
                string response = await client.GetStringAsync($"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={card}");
                CardSkeleton unformatted = JsonSerializer.Deserialize<CardSkeleton>(response);
                var restringified = JsonSerializer.Serialize(unformatted.data[0]);
                CardDataSkeleton newCard = JsonSerializer.Deserialize<CardDataSkeleton>(restringified);
                data = newCard;
                Console.WriteLine(data);
                Console.WriteLine("hello!!?!?!?" + data.name);
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
    }
}
