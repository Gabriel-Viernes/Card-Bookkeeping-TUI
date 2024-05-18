using Utils;
using static Utils.LogUtils;
using Dialogs;
using Formatting;
using DeserializeClasses;
using System.Text.Json;

class Entry {
    static void Main(string[] args) {
        Console.Clear();

        string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        bool doILog = true;
        string currentDb = "yugioh.db";
        string connectionString = $"Data Source={homeFolder}/.config/cbt/db/{currentDb}";
        Settings currentSettings = new Settings();

        try {
            Directory.CreateDirectory($"{homeFolder}/.config/cbt/logs");
            Directory.CreateDirectory($"{homeFolder}/.config/cbt/db");
        } catch(Exception e) {
            Console.WriteLine(e);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }

        if(File.Exists($"{homeFolder}/.config/cbt/settings.json")) {
            using (StreamReader reader = new StreamReader($"{homeFolder}/.config/cbt/settings.json")) {
                currentSettings = JsonSerializer.Deserialize<Settings>(reader.ReadToEnd());
                Log("Settings Initialized");
            }
        } else {
            Settings defalt = new Settings();
            defalt.WriteLogs = false; defalt.CurrentDb = "yugioh.db"; defalt.ConnectionString = $"{homeFolder}/.config/cbt/db/{defalt.CurrentDb}";
            string serialized = JsonSerializer.Serialize(defalt);
            using(File.Create($"{homeFolder}/.config/cbt/settings.json"));
            using (StreamWriter writer = new StreamWriter($"{homeFolder}/.config/cbt/settings.json")) {
                writer.Write(JsonSerializer.Serialize(defalt));
            }
            currentSettings = defalt;
        }




        SqlOperations.DatabaseCheck(connectionString);

        bool quit = false;
        List<string> splash = new List<string>() {"", "Welcome to the Card Bookkeeping TUI!", ""};
        List<string> options = new List<string>() {"Add Card", "Find Card", "Change Card", "Delete Card", "Exit"};
        int index = 0;


        while (quit == false) {
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
                            Menu.AddCardDialog(connectionString);
                            break;

                        case 1:
                            Console.Clear();
                            Menu.FindCardDialog(connectionString);
                            break;

                        case 2:
                            Console.Clear();
                            Menu.UpdateCardDialog(connectionString);
                            break;

                        case 3:
                            Console.Clear();
                            Menu.DeleteCardDialog(connectionString);
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


