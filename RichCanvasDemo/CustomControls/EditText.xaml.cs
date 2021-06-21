using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RichCanvasDemo.CustomControls
{
    /// <summary>
    /// Interaction logic for EditText.xaml
    /// </summary>
    public partial class EditText : Window
    {
        public EditText()
        {
            InitializeComponent();
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
