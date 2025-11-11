using RichCanvasUIA.Client.Models;
using RichCanvasUIA.Client.TestMocks;

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

                case PipeHandlerNames.Drawing.EnableDrawingEndedCommandExecution:
                    _mainWindowDataContext.DrawingState.ShouldExecuteDrawingEndedCommand = true;
                    break;

                case PipeHandlerNames.Drawing.DisableDrawingEndedCommandExecution:
                    _mainWindowDataContext.DrawingState.ShouldExecuteDrawingEndedCommand = false;
                    break;

                case PipeHandlerNames.Drawing.AddImmutableRectangle:
                    _mainWindowDataContext.Items.Add(DrawingStateDataMocks.ImmutablePositionedRectangleMockWithoutSize);
                    break;

                case PipeHandlerNames.Drawing.AddPositionedRectangle:
                    _mainWindowDataContext.Items.Add(DrawingStateDataMocks.PositionedRectangleMockWithoutSize);
                    break;

                case PipeHandlerNames.Drawing.AddDrawnRectangle:
                    _mainWindowDataContext.Items.Add(DrawingStateDataMocks.DrawnRectangleMock);
                    break;
            }
        }
    }
}
