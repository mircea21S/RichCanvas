using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

using RichCanvasUIA.Client.Debug_Mode;
using RichCanvasUIA.Client.UIA_Mode;
using RichCanvasUIA.Client.UIA_Mode.IPC_Pipe;

namespace RichCanvasUIA.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFromCommandLineArguments();
        }

        private void InitializeFromCommandLineArguments()
        {
            // skipping the first argument - as it's the path to Debug build outuput exe file.
            var commandLineArguments = Environment.GetCommandLineArgs()?.Skip(1)?.ToArray();
            if (commandLineArguments == null || commandLineArguments.Length > 1) return;

            var startMode = commandLineArguments[0];
            if (StartUIAMode(startMode, out string pipeHandlerName))
            {
                mainContent.Content = new UIAModeMainControl
                {
                    DataContext = new RichCanvasClientUIAModeViewModel()
                };
                if (!string.IsNullOrEmpty(pipeHandlerName))
                {
                    var pipeHandler = new RichCanvasUITestsPipeHandler();
                    StartListeningToUITestsPipe(pipeHandlerName, pipeHandler);
                }
            }
            // always start Debug for now
            else
            {
                mainContent.Content = new DebugModeMainWindow();
            }
        }

        private bool StartUIAMode(string startMode, out string pipeHandlerName)
        {
            var regex = new Regex(@"^(?<first>UIA)(?:\+(?<pipeHandlerName>[A-Za-z0-9]+))?$");

            var match = regex.Match(startMode);
            if (match.Success)
            {
                pipeHandlerName = match.Groups["pipeHandlerName"].Success ? match.Groups["pipeHandlerName"].Value : null;
                return true;
            }
            pipeHandlerName = null;
            return false;
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
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        pipeHandler.Process(pipeData, (RichCanvasClientUIAModeViewModel)((UIAModeMainControl)mainContent.Content).DataContext);
                    });
                }
            });
        }
    }
}
