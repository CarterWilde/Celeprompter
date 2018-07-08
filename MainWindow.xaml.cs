using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Teleprompter_App {
  public partial class MainWindow : Window {
    IList<FullscreenWindow> fullscreenWindows = new List<FullscreenWindow>();

    public MainWindow() {
      InitializeComponent();
      for (int i = 1; i <= 100; i++) {
        App.MainText += i.ToString() + "\n";
      }
      TeleText.Text = App.MainText;
    }

    private void OpenNewFileClick(object sender, RoutedEventArgs e) {
      OpenFileDialog openFileDialog = new OpenFileDialog {
        Filter = "Text files (*.txt)|*.txt"
      };
      if (openFileDialog.ShowDialog() == true){
        if (openFileDialog.CheckFileExists && openFileDialog.CheckPathExists) {
          using (Stream fileStream = openFileDialog.OpenFile()) {
            StreamReader reader = new StreamReader(fileStream);
            App.MainText = reader.ReadToEnd();
            TeleText.Text = App.MainText;
            if (fullscreenWindows != null) {
              foreach (FullscreenWindow window in fullscreenWindows) {
                window.TextSync();
              }
            }
          }
        }
      }
    }

    private void EnterKeyUp(object sender, KeyEventArgs e) {
      if (e.Key == Key.Return) {
        string FontSizeValue = FontInput.Text;
        if (int.TryParse(FontSizeValue, out int n)) {
          FontSizeValue = n.ToString();
          TeleText.FontSize = n;
          LineHighLight.Height = n;
          if (fullscreenWindows != null) {
            foreach (FullscreenWindow window in fullscreenWindows) {
              window.FontSync(n);
            }
          }
        }
      }
    }

    private void NewWindow(object sender, RoutedEventArgs e) {
      fullscreenWindows.Add(new FullscreenWindow(ScrollView.VerticalOffset));
      fullscreenWindows.Last().InitializeComponent();
      fullscreenWindows.Last().Show();
    }

    private void ScrollChanged(object sender, ScrollChangedEventArgs e){
      if (fullscreenWindows != null) {
        foreach (FullscreenWindow window in fullscreenWindows) {
          window.ScrollSync(sender, e);
        }
      }
    }
  }
}
