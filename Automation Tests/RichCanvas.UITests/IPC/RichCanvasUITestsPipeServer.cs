using System;
using System.IO;
using System.IO.Pipes;

using FlaUI.Core.Input;

namespace RichCanvas.UITests.IPC
{
    public class RichCanvasUITestsPipeServer : IDisposable
    {
        private readonly AnonymousPipeServerStream _pipeServer;
        private readonly StreamWriter _pipeServerWriter;

        public RichCanvasUITestsPipeServer()
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

        internal void DisposeLocalCopyOfClientHandle() => _pipeServer.DisposeLocalCopyOfClientHandle();

        internal string GetClientHandleAsString()
            => _pipeServer.GetClientHandleAsString();
    }
}
