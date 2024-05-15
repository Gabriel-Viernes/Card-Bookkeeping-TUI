using Utils;
using static Utils.LogUtils;
using Formatting;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.VisualBasic;


namespace CardBookkeepingTUI {

    class Menu {

        async public static void AddCardDialog(string connectionString) {

            List<string> message = new List<string>() {"", "Please enter the name of the card you would like to add", ""};
            
            Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
            string? card = Textbox.PrintInputBox(30);

            while (card.Length == 0) {
                Console.Clear();
                message[2] = "Input too short";
                Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
                card = Textbox.PrintInputBox(30);
            }

            if(SqlOperations.CheckForExistingCard(connectionString, card) == true) {
                Console.Clear();
                message[1] = "Card already exists! Please update the card instead";
                message[2] = "Press any key to continue...";
                Textbox.Print(Screen.Center(Textbox.Generate(message, 100, 6, 1)));
                Console.ReadKey();
                return;
            }

            message[1] = "How many copies do you have?";
            message[2] = "";

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
            List<string> converted = new List<string>();

            try {
                using(HttpClient client = new HttpClient()) {

                    CardDataSkeleton data = new CardDataSkeleton();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string response = await client.GetStringAsync($"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={card}");
                    Log(response);
                    //api response is data:{ 0: {(whatever key/value)}};
                    JsonDocument unformatted = JsonSerializer.Deserialize<JsonDocument>(response);
                    var dataArray = unformatted.RootElement.GetProperty("data");
                    var restringified = JsonSerializer.Serialize(dataArray[0]);

                    CardDataSkeleton newCard = JsonSerializer.Deserialize<CardDataSkeleton>(restringified);
                    data = newCard;
                    data.copies = copies;
                    converted = StringUtils.ConvertCardDataSkeletonToList(data);
                }
            } catch (Exception e) {
                Console.WriteLine("An error occurred");
                Console.WriteLine(e);
                AddCardDialog(connectionString);
            }

            string report = "Added new card:";
                for(int i = 0; i < converted.Count; i++) {
                    report += $"{converted[i]}|";
                }
            Log(report);
            SqlOperations.InsertCard(connectionString, converted);                               
        }

         public static void FindCardDialog(string connectionString) {
            List<string> message = new List<string>() {"", "What card are you looking for?", ""};
            Textbox.Print(Screen.Center(Textbox.Generate(message, 50, 5, 1)));
            string input = Textbox.PrintInputBox(30);
            List<List<string>> data = new List<List<string>>();

            try {
                data = SqlOperations.FindExistingCard(connectionString, input);
            } catch (Exception e) {
                Console.WriteLine(e);
                return;
            }

            Console.Clear();

            if(data.Count == 0) {
                List<string> notFound = new List<string>() {
                    "",
                    "No cards found with provided keyword",
                    "Press any key to continue...",
                    ""
                };
                Textbox.Print(Screen.Center(Textbox.Generate(notFound, 1)));
                Console.ReadKey();
                return;
            }          

            data.Insert(0, new List<string>() {"Name","Copies","Type","FrameType","Desc.","Atk","Def","Level","Race","Attr.","Card ID" });

            message[1] = "Press ESC to go back to the main menu";

            List<string> temp = Textbox.GenerateTable(data, data[0].Count * 10, 10, 1);
            int index = 2;
            temp[index] += "<<<";
            bool exit = false;
            while(exit == false) {
                Textbox.Print(Screen.Center(Textbox.Generate(message, 1)));
                Textbox.Print(temp);
                switch(Console.ReadKey().Key) {
                    case System.ConsoleKey.UpArrow:
                    case System.ConsoleKey.K:
                        Console.Clear();
                        if(index > 2) {
                            temp[index] = temp[index].Substring(0,temp[index].Length - 3);
                            index--;
                            temp[index] += "<<<";
                        }
                        break;

                    case System.ConsoleKey.DownArrow:
                    case System.ConsoleKey.J:
                        Console.Clear();
                        if(index < temp.Count-2) {
                            temp[index] = temp[index].Substring(0,temp[index].Length - 3);
                            index++;
                            temp[index] += "<<<";
                        }
                        break;
                    case System.ConsoleKey.Enter:
                        Console.Clear();
                        break;
                    case System.ConsoleKey.Escape:
                        exit = true;
                        break;
                   
                }
            }
        }

         public static void UpdateCardDialog(string connectionString) {

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

         public static void DeleteCardDialog(string connectionString) {
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
