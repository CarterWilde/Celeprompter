using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Teleprompter_App {
  public partial class App : Application {
    public static int CharLength { get; set; } = 60;
    public static string MainText { get; set; }
    public static string FileText { get; set; }
    public static double ScrollSpeed { get; set; }
    public static string SpliceText(string text, int lineLength) {
      return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
    }
  }
}
