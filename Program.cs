// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.Json;

namespace yugiohLocalDatabase {
    class Entry {
        static void Main(string[] args) {

            Console.Clear();
            using HttpClient client = new();
            CardLookup test = new CardLookup("Koitsu", client);;
            test.GetCard();
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
                                Console.WriteLine(input);
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
        public static string card;
        public static HttpClient client;
        public CardLookup(string input, HttpClient inputClient) {
            card = input;
            client = inputClient;

        }
        public async Task GetCard() {
            Console.WriteLine(client);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            card = card.Replace(" ","%20");
            Console.WriteLine(card);
            try {
                string response = await client.GetStringAsync($"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={card}");
                Console.WriteLine(response);
            } catch(Exception e) {
                Console.WriteLine(e);
                Console.WriteLine(e.GetType().GetProperties());
            }
        }

    }

    public class Util {
        
    }
}
