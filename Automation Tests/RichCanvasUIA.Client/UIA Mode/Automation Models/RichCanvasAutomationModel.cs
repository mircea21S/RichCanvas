using System.Collections.ObjectModel;

namespace RichCanvasUIA.Client.UIA_Mode.Automation_Models
{
    public class RichCanvasAutomationModel
    {
        public ObservableCollection<RichCanvasContainerAutomationModel> Items { get; } = [];
    }
}
