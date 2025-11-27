using System.Collections.Generic;

using RichCanvasUIA.Client.IPC_Pipe.Handlers;

namespace RichCanvasUIA.Client.IPC_Pipe
{
    internal class RichCanvasUITestsPipeHandler
    {
        private readonly Dictionary<string, IPipeHandler> _nameToPipeHandlerMap = [];

        internal void Process(string pipeData, MainWindowViewModel mainWindowDataContext)
        {
            mainWindowDataContext.PipeDataInfo = $"{pipeData}";
            if (pipeData.Split('.').Length != 2)
            {
                return;
            }

            string pipeHandlerName = pipeData.Split('.')[0];
            IPipeHandler handler = GetHandler(pipeHandlerName, mainWindowDataContext);
            string pipeHandlerOperationName = pipeData.Split('.')[1];
            handler.Process(pipeHandlerOperationName);
        }

        internal IPipeHandler GetHandler(string pipeHandlerName, MainWindowViewModel mainWindowDataContext)
        {
            if (_nameToPipeHandlerMap.TryGetValue(pipeHandlerName, out IPipeHandler pipeHandler))
            {
                return pipeHandler;
            }
            else
            {
                pipeHandler = pipeHandlerName switch
                {
                    nameof(PipeHandlerNames.Drawing) => new DrawingStateHandler(mainWindowDataContext),
                    nameof(PipeHandlerNames.ItemsSource) => new ItemsSourceHandler(mainWindowDataContext),
                    nameof(PipeHandlerNames.Selection) => new SelectionHandler(mainWindowDataContext),
                    _ => throw new System.NotImplementedException()
                };
                _nameToPipeHandlerMap.Add(pipeHandlerName, pipeHandler);
                return pipeHandler;
            }
        }
    }
}
