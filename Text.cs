using System;

namespace Formatting {
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
