// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;

namespace yugiohLocalDatabase {
    class mainMenu {
        static void Main(string[] args) {
            string[] mainMenuOptions = {"Add Card", "Find Card", "Change Card", "Delete Card", "Exit"};
            Console.WriteLine("======================================================");
            Console.WriteLine("         Welcome to the Yugioh Local Database!        ");
            Console.WriteLine("======================================================");
            MenuDisplay(mainMenuOptions);
        }
        static void MenuDisplay(string[] menuItems) {
            for(int i = 0; i < menuItems.Length; i++) {
                Console.WriteLine($"{menuItems[i]}");
            }
        }

    }
}
