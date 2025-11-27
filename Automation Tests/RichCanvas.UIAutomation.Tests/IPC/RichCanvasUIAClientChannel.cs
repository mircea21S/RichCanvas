using System;
using System.IO;
using System.IO.Pipes;

using FlaUI.Core.Input;

using RichCanvasUIA.Client.IPC_Pipe;

namespace RichCanvas.UIAutomation.Tests.IPC
{
    public class RichCanvasUIAClientChannel : IDisposable
    {
        private readonly AnonymousPipeServerStream _pipeServer;
        private readonly StreamWriter _pipeServerWriter;

        public RichCanvasUIAClientChannel()
        {
            _pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            _pipeServerWriter = new StreamWriter(_pipeServer)
            {
                AutoFlush = true
            };
        }

        public void Dispose()
        {
            _pipeServer.Dispose();
            _pipeServerWriter.Dispose();
        }

        public void Send(string data)
        {
            try
            {
                _pipeServerWriter.WriteLine(data);
                _pipeServer.WaitForPipeDrain();
                Wait.UntilInputIsProcessed();
            }
            catch (IOException)
            {
                throw;
            }
        }

        internal void AddDrawnRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddDrawnRectangle);
        }

        internal void AddPositionedRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddPositionedRectangle);
        }

        internal void AddImmutableRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddImmutableRectangle);
        }

        internal void MoveFirstItemToTheEnd()
        {
            SendToAppWithItemsSourcePrefix(PipeHandlerNames.ItemsSource.MoveFirstItemToTheEnd);
        }

        internal void RemoveFirstItem()
        {
            SendToAppWithItemsSourcePrefix(PipeHandlerNames.ItemsSource.RemoveFirstItem);
        }

        internal void AddEmptyRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddEmptyRectangle);
        }

        internal void AddEmptyLine()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddEmptyLine);
        }

        internal void DisableDrawingEndedCommandExecution()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.DisableDrawingEndedCommandExecution);
        }

        internal void EnableDrawingEndedCommandExecution()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.EnableDrawingEndedCommandExecution);
        }

        internal void DisposeLocalCopyOfClientHandle() => _pipeServer.DisposeLocalCopyOfClientHandle();

        internal string GetClientHandleAsString()
            => _pipeServer.GetClientHandleAsString();

        internal void AddSelectableItems()
        {
            SendToAppWithSelectionPrefix(PipeHandlerNames.Selection.AddSelectableItems);
        }

        private void SendToAppWithSelectionPrefix(string operationName)
        {
            Send($"{nameof(PipeHandlerNames.Selection)}.{operationName}");
        }

        private void SendToAppWithDrawingPrefix(string operationName)
        {
            Send($"{nameof(PipeHandlerNames.Drawing)}.{operationName}");
        }

        private void SendToAppWithItemsSourcePrefix(string operationName)
        {
            Send($"{nameof(PipeHandlerNames.ItemsSource)}.{operationName}");
        }
    }
}
