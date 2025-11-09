using RichCanvasUIA.Client.Models;

namespace RichCanvasUIA.Client.IPC_Pipe.Handlers
{
    public class DrawingStateHandler : IPipeHandler
    {
        private readonly MainWindowViewModel _mainWindowDataContext;

        public DrawingStateHandler(MainWindowViewModel mainWindowDataContext)
        {
            _mainWindowDataContext = mainWindowDataContext;
        }

        public void Process(string pipeDataName)
        {
            switch (pipeDataName)
            {
                case PipeHandlerNames.Drawing.AddEmptyRectangle:
                    _mainWindowDataContext.Items.Add(new RichItemContainerModel());
                    break;

                case PipeHandlerNames.Drawing.AddEmptyLine:
                    _mainWindowDataContext.Items.Add(new Line());
                    break;

                case PipeHandlerNames.Drawing.RemoveFirstItem:
                    _mainWindowDataContext.Items.RemoveAt(0);
                    break;
            }
        }
    }
}
