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

namespace TeleprompterApp {
  public partial class MainWindow : Window {
    private bool isMoving = false;                  //False - ignore mouse movements and don't scroll
    private bool isDeferredMovingStarted = false;   //True - Mouse down -> Mouse up without moving -> Move; False - Mouse down -> Move
    private Point? startPosition = null;
    private double slowdown = 200;

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
      if (openFileDialog.ShowDialog() == true) {
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

    private void ScrollSpeedEvent(object sender, KeyEventArgs e) {
      if (e.Key == Key.Return) {
        if (int.TryParse(ScrollInput.Text, out int n)) {
          if (n != 0) {
            slowdown = n;
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
      fullscreenWindows.Add(new FullscreenWindow(ScrollView.VerticalOffset, TeleText.FontSize));
      fullscreenWindows.Last().InitializeComponent();
      fullscreenWindows.Last().Show();
    }

    private void ScrollChanged(object sender, ScrollChangedEventArgs e) {
      TeleText.Text = App.MainText;
      if (fullscreenWindows != null) {
        foreach (FullscreenWindow window in fullscreenWindows) {
          window.ScrollSync(sender, e);
        }
      }
    }

    private void CloseAll(object sender, RoutedEventArgs e) {
      foreach (FullscreenWindow screen in fullscreenWindows) {
        screen.Close();
      }
      this.Close();
    }




    private void ScrollViewer_MouseDown(object sender, MouseButtonEventArgs e) {
      if (this.isMoving == true) //Moving with a released wheel and pressing a button
        this.CancelScrolling();
      else if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed) {
        if (this.isMoving == false) //Pressing a wheel the first time
        {
          this.isMoving = true;
          this.startPosition = e.GetPosition(sender as IInputElement);
          this.isDeferredMovingStarted = true; //the default value is true until the opposite value is set
        }
      }
    }

    private void ScrollViewer_MouseUp(object sender, MouseButtonEventArgs e) {
      if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Released && this.isDeferredMovingStarted != true)
        this.CancelScrolling();
    }

    private void CancelScrolling() {
      this.isMoving = false;
      this.startPosition = null;
      this.isDeferredMovingStarted = false;
    }

    private void ScrollViewer_MouseMove(object sender, MouseEventArgs e) {
      var sv = sender as ScrollViewer;

      if (this.isMoving && sv != null) {
        this.isDeferredMovingStarted = false; //standard scrolling (Mouse down -> Move)

        var currentPosition = e.GetPosition(sv);
        var offset = currentPosition - startPosition.Value;
        offset.Y /= slowdown;
        offset.X /= slowdown;

        //if(Math.Abs(offset.Y) > 25.0/slowdown)  //Some kind of a dead space, uncomment if it is neccessary
        sv.ScrollToVerticalOffset(sv.VerticalOffset + offset.Y);
        sv.ScrollToHorizontalOffset(sv.HorizontalOffset + offset.X);
      }
    }
  }
}
