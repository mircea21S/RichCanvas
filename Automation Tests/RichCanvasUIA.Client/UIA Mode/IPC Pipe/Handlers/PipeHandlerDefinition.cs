using RichCanvasUIA.Client.UIA_Mode.Automation_Models;

namespace RichCanvasUIA.Client.UIA_Mode.IPC_Pipe.Handlers
{
    internal abstract class PipeHandlerDefinition(RichCanvasClientUIAModeViewModel clientUIAModeViewModel)
    {
        protected RichCanvasClientUIAModeViewModel ClientUIAModeViewModel { get; } = clientUIAModeViewModel;
        protected RichCanvasAutomationModel RichCanvasAutomationModel => ClientUIAModeViewModel.RichCanvasAutomationModel;

        internal abstract void Process(string pipeDataName);
    }
}
