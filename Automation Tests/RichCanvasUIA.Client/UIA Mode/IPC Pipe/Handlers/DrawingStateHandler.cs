using RichCanvasUIA.Client.UIA_Mode.Automation_Models;

namespace RichCanvasUIA.Client.UIA_Mode.IPC_Pipe.Handlers
{
    internal class DrawingStateHandler(RichCanvasClientUIAModeViewModel clientUIAModeViewModel) : PipeHandlerDefinition(clientUIAModeViewModel)
    {
        internal override void Process(string pipeDataName)
        {
            switch (pipeDataName)
            {
                case PipeHandlerNames.Drawing.AddEmptyRectangle:
                    RichCanvasAutomationModel.Items.Add(new RichCanvasContainerAutomationModel());
                    break;

                case PipeHandlerNames.Drawing.AddEmptyLine:
                    RichCanvasAutomationModel.Items.Add(new LineAutomationModel());
                    break;

                case PipeHandlerNames.Drawing.EnableDrawingEndedCommandExecution:
                    RichCanvasAutomationModel.ShouldExecuteDrawingEndedCommand = true;
                    break;

                case PipeHandlerNames.Drawing.DisableDrawingEndedCommandExecution:
                    RichCanvasAutomationModel.ShouldExecuteDrawingEndedCommand = false;
                    break;

                case PipeHandlerNames.Drawing.AddImmutableRectangle:
                    RichCanvasAutomationModel.Items.Add(PreDefinedAutomationItemModels.ImmutablePositionedRectangleWithoutSize);
                    break;

                case PipeHandlerNames.Drawing.AddPositionedRectangle:
                    RichCanvasAutomationModel.Items.Add(PreDefinedAutomationItemModels.MutablePositionedRectangleWithSize);
                    break;

                case PipeHandlerNames.Drawing.AddDrawnRectangle:
                    RichCanvasAutomationModel.Items.Add(PreDefinedAutomationItemModels.FullyDrawnRectangle);
                    break;
            }
        }
    }
}
