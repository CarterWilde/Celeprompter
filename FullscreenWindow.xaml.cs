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
  public partial class FullscreenWindow : Window {
    public FullscreenWindow(double verticalOffset) {
      InitializeComponent();
      Scroll.ScrollToVerticalOffset(verticalOffset);
      TeleText.Text = App.MainText;
    }

    public void TextSync(){
      TeleText.Text = App.MainText;
    }

    public void FontSync(int i){
      TeleText.FontSize = i;
      LineHighLight.Height = i + 6;
    }

    public void CharSync(int i) {
      TeleText.Text = App.MainText;
    }

    public void ScrollSync(object sender, RoutedEventArgs e) {
      ScrollChangedEventArgs se = (ScrollChangedEventArgs) e;
      Scroll.ScrollToVerticalOffset(se.VerticalOffset);
    }
  }
}
