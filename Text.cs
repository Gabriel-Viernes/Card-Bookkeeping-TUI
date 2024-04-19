using System;
using static System.Console;


namespace Formatting {
    public class Textbox {
        public static void Print(int width, int height, List<string> text, int alignment) {

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
            
            WriteLine($"{text.Count} || {height}");

            WriteLine(top);
            switch(alignment) {
                case 0:
                    for(int i = 0; i < height-2; i++) {
                        WriteLine(Align.LeftAlign(text[i], width));
                    }
                    break;
                case 1:
                    for(int i = 0; i < height-2; i++) {
                        WriteLine(Align.CenterAlign(text[i], width));
                    }
                    break;
                case 2:
                    for(int i = 0; i < height-2; i++) {
                        WriteLine(Align.RightAlign(text[i],width));
                        
                    }
                    break;
            }
            WriteLine(bottom);
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
