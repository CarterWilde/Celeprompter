using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Teleprompter_App {
  public partial class MainWindow : Window {
    IList<FullscreenWindow> fullscreenWindows = new List<FullscreenWindow>();
    DispatcherTimer timer = new DispatcherTimer();
    public MainWindow() {
      InitializeComponent();
      for (int i = 1; i <= 100; i++) {
        App.MainText += i.ToString() + "\n";
        App.FileText += i.ToString() + "\n";
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
            string FileText = reader.ReadToEnd();
            App.MainText = App.SpliceText(FileText, App.CharLength);
            App.FileText = FileText;
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

    private void FontInputEvent(object sender, KeyEventArgs e) {
      if (e.Key == Key.Return) {
        string FontSizeValue = FontInput.Text;
        if (int.TryParse(FontSizeValue, out int n)) {
          TeleText.FontSize = n;
          LineHighLight.Height = n + 6;
          if (fullscreenWindows != null) {
            foreach (FullscreenWindow window in fullscreenWindows) {
              window.FontSync(n);
            }
          }
        }
      }
    }

    private void CharInputEvent(object sender, KeyEventArgs e) {
      if (e.Key == Key.Return) {
        string CharSizeValue = CharInput.Text;
        if (int.TryParse(CharSizeValue, out int n)) {
          App.CharLength = n;
          App.MainText = App.SpliceText(App.FileText, App.CharLength);
          TeleText.Text = App.SpliceText(App.MainText, App.CharLength);
          if (fullscreenWindows != null) {
            foreach (FullscreenWindow window in fullscreenWindows) {
              window.CharSync(n);
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
      TeleText.Text = App.MainText;
      if (fullscreenWindows != null) {
        foreach (FullscreenWindow window in fullscreenWindows) {
          window.ScrollSync(sender, e);
        }
      }
    }

    private void WindowKeyDown(object sender, KeyEventArgs e) {
      if(e.Key == Key.Down){
        App.ScrollSpeed += Math.Pow(.25, App.ScrollSpeed);
      }
      if (e.Key == Key.Up) {
        App.ScrollSpeed -= Math.Pow(.25, App.ScrollSpeed);
      }
    }
  }
}
