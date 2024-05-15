using static System.Console;
using CardBookkeepingTUI;
using static Utils.StringUtils;


namespace Formatting {

    public class Screen {
        public static List<string> Center(List<string> text) {

            //this method assumes that all strings within the list are the same length
            //adds empty space to the left of every string in a list to "center" a textbox

            int originalLength = 0;
            for(int i = 0; i < text.Count; i++) {
                originalLength = text[i].Length;
                for(int j = 0; j < ((WindowWidth/2)-(originalLength/2)); j++) {
                    text[i] = " " + text[i];
                }
            }
            return text;
        }
    }

    public class Textbox {

        public static void Print(List<string> text) {

            //meant to be usual exit point for textboxes
            //prints a List<string> that's been manipulated to user requirements

            for(int i = 0; i < text.Count; i++) {
                WriteLine(text[i]);
            }
        }

        public static void Print(List<string> text, int x, int y) {
            CursorLeft = x; CursorTop = y;

            for(int i = 0; i < text.Count; i++) {
                CursorLeft = x;
                WriteLine(text[i]);
            }

        }

        public static string PrintInputBox(int width) {

            //exit point for input textboxes that return input
            
            CursorVisible = true;
            List<string> text = new List<string>() {"  > "};
            text = Screen.Center(Textbox.Generate(text, width, 3,  0));

            Textbox.Print(text);

            CursorLeft = (text[0].Length - width + 5);
            CursorTop = CursorTop - 2;

            string input = ReadLine();
            CursorVisible = false;
            return input;

        }

        public static List<string> GenerateTable(List<List<string>> cards, int width, int cellWidth, int alignment) {

            string top = "┌";
            string bottom = "└";
            int count = 0;

            for(int i = 0; i < cards[0].Count-1; i++) {
                for(int j = 0; j < cellWidth; j++) {
                    top += "─";
                    bottom += "─";
                }
                top += "┬";
                bottom += "┴";
            }
            for(int i = 0; i < cellWidth; i++) {
                top += "─";
                bottom += "─";
            }

            top += "┐";
            bottom += "┘";

            List<string> rendered = new List<string>();
            rendered.Add(top);
            foreach (List<string> card in cards) {
                string temp = "│";
                for(int i = 0; i < card.Count; i++) {
                    if(card[i].Length > cellWidth) {
                        temp += tententen(card[i], cellWidth);    
                    } else {
                        temp += card[i];
                        for(int j = card[i].Length-1; j < cellWidth - 1; j++) {
                            temp += " ";
                        }
                    }
                    temp += "│";
                }
                rendered.Add(temp);
            } 
            rendered.Add(bottom);
        
            return rendered;
        }

        public static List<string> GenerateMenu(List<string> text, int index, int width, int alignment) {
            List<string> modified = new List<string>();
            for(int i = 0; i < text.Count; i++) {
               if(i == index) {
                   modified.Add($"[[{text[i]}]]");
               } else {
                   modified.Add(text[i]);
               } 
            }

            return Generate(modified, width, modified.Count+2, alignment);
        }

        public static List<string> Generate(List<string> text, int alignment) {

            int maxWidth = 0;

            for(int i = 0; i < text.Count; i++) {
                if(text[i].Length > WindowWidth) {
                    string cut = text[i].Substring(WindowWidth-3);
                    text[i] = text[i].Remove(WindowWidth-3);
                    text[i] = text[i] + "-";
                    List<string> temp = text.Slice(i+1, (text.Count-i-1));
                    text.RemoveRange(i+1, (text.Count-i-1));

                    text.Add(cut);
                    text.AddRange(temp);
                    maxWidth = text[i].Length;

                } else if(text[i].Length > maxWidth) {
                    maxWidth = text[i].Length;
                } 

            }
            maxWidth += 2;
            int height = text.Count+2;

            return Generate(text, maxWidth, height, alignment);
        }

        public static List<string> Generate(List<string> text, int width, int height, int alignment) {


            if((alignment > 2) || (alignment < 0)) {
                Exception e = new Exception("Invalid alignment specified. Please choose from a range of 0-2");
            }

            for(int i = 0; i < text.Count; i++) {
                if(text[i].Length > width) {
                    string cut = text[i].Substring(width-3);
                    text[i] = text[i].Remove(width-3);
                    text[i] = text[i] + "-";
                    List<string> temp = text.Slice(i+1, (text.Count-i-1));
                    text.RemoveRange(i+1, (text.Count-i-1));

                    text.Add(cut);
                    text.AddRange(temp);
                }
            }


            if(text.Count > (height - 2)) {
                Exception e = new Exception($"Attempting to put {text.Count} lines of text in a textbox of height {height}");
                throw e;
            } else if(text.Count > WindowHeight) {
                Exception e = new Exception("Textbox too tall for current window height");
                throw e; 
            }

            //if(text.Count < (height - 2)) {

            //    for(int i = text.Count; i < (height); i++) {
            //        text.Add("");
            //    }
            //}

            string top = "┌";
            string bottom = "└";

            for(int i = 0; i < width - 2; i++) {
                top = top + "─";
                bottom = bottom + "─";
            }

            top = top + "┐";
            bottom = bottom + "┘";

            List<string> generatedTextbox = new List<string>() {top};
            
            switch(alignment) {
                case 0:
                    for(int i = 0; i < text.Count; i++) {
                        generatedTextbox.Add(Align.LeftAlign(text[i], width));
                    }
                    break;
                case 1:
                    for(int i = 0; i < text.Count; i++) {
                        generatedTextbox.Add(Align.CenterAlign(text[i], width));
                    }
                    break;
                case 2:
                    for(int i = 0; i < text.Count; i++) {
                        generatedTextbox.Add(Align.RightAlign(text[i],width));
                        
                    }
                    break;
            }

            if(generatedTextbox.Count < (height-2)) {
                for(int i = generatedTextbox.Count; i < height; i++) {
                    generatedTextbox.Add("");
                }
            }

            generatedTextbox.Add(bottom);
            return generatedTextbox;
        }

    }

    public static class Align {

        public static string SelectAlign(string text, int width, int alignment) {
             if(text.Length > (width-2)) {
                Exception e = new Exception("Text too long for specified width");
                throw e;
            }           
            switch(alignment) {
                case 0:
                    return Align.LeftAlign(text, width);
                case 1:
                    return Align.CenterAlign(text, width);
                case 2:
                    return Align.RightAlign(text, width);
                default:
                    Exception e = new Exception("Invalid alignment specified. Please use an integer from 0-2");
                    throw e;

            }

        }

        public static string LeftAlign(string text, int width) {
            if(text.Length > (width-2)) {
                Exception e = new Exception("Text too long for specified width");
                throw e;
            }
            width = width - text.Length - 2;
            text = "\u2502" + text;
            for (int i = 0; i < width; i++) {
                text = text + $" ";
            }
            text = text + "\u2502";
            return text;
        }

        public static string CenterAlign(string text, int width) {
            if(text.Length > (width-2)) {
                Exception e = new Exception("Text too long for specified width");
                throw e;
            }
            width = width - text.Length - 2;
            if((width%2) != 0) {
                string leftPipe = "\u2502";
                for (int i = 0; i < ((width/2) + (width%2)); i++) {
                    leftPipe = leftPipe + " ";
                }
                text = leftPipe + text;
                for (int i = 0; i < ((width/2)); i++) {
                    text = text + " ";
                }
                text = text + "\u2502";
            } else {
                string leftPipe = "\u2502";
                for (int i = 0; i < (width/2); i++) {
                    leftPipe = leftPipe + " ";
                    text = text + " ";
                }
                text = leftPipe + text + "\u2502";
            }
            return text;
        }

        public static string RightAlign(string text, int width) {
            if(text.Length > (width-2)) {
                Exception e = new Exception("Text too long for specified width");
                throw e;
            }
            width = width - text.Length - 2;
            string leftPipe = "\u2502";
            for (int i = 0; i < width; i++) {
                leftPipe = leftPipe + " ";
            }
            text = leftPipe + text + "\u2502";
            return text;
        }
    }
}
