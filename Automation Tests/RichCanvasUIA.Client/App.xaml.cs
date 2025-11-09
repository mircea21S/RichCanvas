using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Windows;

using RichCanvasUIA.Client.IPC_Pipe;

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
                var pipeHandler = new RichCanvasUITestsPipeHandler();
                StartListeningToUITestsPipe(e.Args[0], pipeHandler);
            }
        }

        private void StartListeningToUITestsPipe(string pipeHandleName, RichCanvasUITestsPipeHandler pipeHandler)
        {
            Task.Run(() =>
            {
                using PipeStream pipeClient = new AnonymousPipeClientStream(PipeDirection.In, pipeHandleName);
                using var sr = new StreamReader(pipeClient);
                string pipeData;
                while ((pipeData = sr.ReadLine()) != null)
                {
                    Current.Dispatcher.BeginInvoke(() =>
                    {
                        pipeHandler.Process(pipeData, (MainWindowViewModel)MainWindow.DataContext);
                    });
                }
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
