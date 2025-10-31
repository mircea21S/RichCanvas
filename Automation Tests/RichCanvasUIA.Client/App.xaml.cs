using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                StartListeningToUITestsPipe(e.Args[0]);
            }
        }

        private async Task StartListeningToUITestsPipe(string pipeHandleName)
        {
            await Task.Run(() =>
            {
                using PipeStream pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeHandleName);
                using var sr = new StreamReader(pipeClient);
                string temp;
                while ((temp = sr.ReadLine()) != null)
                {
                    //RichCanvasUITestsPipeHandler.Process(temp);
                    Current.Dispatcher.BeginInvoke(() =>
                    {
                        (MainWindow.DataContext as MainWindowViewModel).PipeDataInfo = $"{temp}";
                    });
                }
                (MainWindow.DataContext as MainWindowViewModel).PipeDataInfo = "Te-ai inchis ceau";
            });
        }

        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Error;
            MessageBoxResult result;
            Console.WriteLine(e.Exception);
            result = MessageBox.Show(e.Exception.Message, "Error", button, icon);
        }
    }
}
