using System;
using static System.Console;


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

        public static string PrintInputBox(int width) {

            //exit point for input textboxes that return input

            List<string> text = new List<string>() {"  > "};
            text = Screen.Center(Textbox.Generate(width, 3, text, 0));

            Textbox.Print(text);

            CursorLeft = (text[0].Length - width + 5);
            CursorTop = CursorTop - 2;

            string input = ReadLine();
            return input;

        }

        public static List<string> Generate(int width, int height, List<string> text, int alignment) {

            if((alignment > 2) || (alignment < 0)) {
                Exception e = new Exception("Invalid alignment specified. Please choose from a range of 0-2");
            }

             if(text.Count > (height - 2)) {
                Exception e = new Exception("Too many lines of text for given height");
                throw e;
            }

            for(int i = 0; i < text.Count; i++) {
                if(text[i].Length > width) {
                    Exception e = new Exception("Text is too long for given width");
                    throw e;
                }
            }           

            if(text.Count < (height - 2)) {
                for(int i = 0; i < (height - text.Count + 1); i++) {
                    text.Add("");
                }
            }

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
                    for(int i = 0; i < height-2; i++) {
                        generatedTextbox.Add(Align.LeftAlign(text[i], width));
                    }
                    break;
                case 1:
                    for(int i = 0; i < height-2; i++) {
                        generatedTextbox.Add(Align.CenterAlign(text[i], width));
                    }
                    break;
                case 2:
                    for(int i = 0; i < height-2; i++) {
                        generatedTextbox.Add(Align.RightAlign(text[i],width));
                        
                    }
                    break;
            }
            generatedTextbox.Add(bottom);
            return generatedTextbox;
        }

    }

    public static class Align {
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
