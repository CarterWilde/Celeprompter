using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TeleprompterApp {
  public partial class App : Application {
    public static int CharLength { get; set; } = 60;
    public static string MainText { get; set; }
    public static string FileText { get; set; }
    public static string SpliceText(string text, int lineLength) {
      string tempStr = Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
      //This is so the smooth scroll doesn't chop up
      int i = 0;
      while (i < MainText.Length + 55) {
        tempStr += Environment.NewLine;
        i++;
      }
      return tempStr;
    }
  }
}
