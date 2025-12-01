using System;
using System.Windows;

namespace RichCanvasUIA.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;
            Console.WriteLine(e.Exception);
            _ = MessageBox.Show($"{e.Exception.Message}{Environment.NewLine}{e.Exception.StackTrace}", "Error", button, icon);
        }
    }
}
