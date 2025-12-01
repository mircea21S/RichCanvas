using System.Collections.Generic;

using RichCanvasUIA.Client.UIA_Mode.IPC_Pipe.Handlers;

namespace RichCanvasUIA.Client.UIA_Mode.IPC_Pipe
{
    internal class RichCanvasUITestsPipeHandler
    {
        private readonly Dictionary<string, PipeHandlerDefinition> _nameToPipeHandlerMap = [];

        internal void Process(string pipeData, RichCanvasClientUIAModeViewModel uiaModeViewModel)
        {
            uiaModeViewModel.PipeDataInfo = $"{pipeData}";
            if (pipeData.Split('.').Length != 2)
            {
                return;
            }

            string pipeHandlerName = pipeData.Split('.')[0];
            PipeHandlerDefinition handler = GetHandler(pipeHandlerName, uiaModeViewModel);
            string pipeHandlerOperationName = pipeData.Split('.')[1];
            handler.Process(pipeHandlerOperationName);
        }

        internal PipeHandlerDefinition GetHandler(string pipeHandlerName, RichCanvasClientUIAModeViewModel uiaModeViewModel)
        {
            if (_nameToPipeHandlerMap.TryGetValue(pipeHandlerName, out PipeHandlerDefinition pipeHandler))
            {
                return pipeHandler;
            }
            else
            {
                pipeHandler = pipeHandlerName switch
                {
                    nameof(PipeHandlerNames.Drawing) => new DrawingStateHandler(uiaModeViewModel),
                    nameof(PipeHandlerNames.ItemsSource) => new ItemsSourceHandler(uiaModeViewModel),
                    nameof(PipeHandlerNames.Selection) => new SelectionHandler(uiaModeViewModel),
                    _ => throw new System.NotImplementedException()
                };
                _nameToPipeHandlerMap.Add(pipeHandlerName, pipeHandler);
                return pipeHandler;
            }
        }
    }
}
