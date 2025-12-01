using System.Windows.Input;

using RichCanvasUIA.Client.UIA_Mode.Automation_Models;

namespace RichCanvasUIA.Client.UIA_Mode
{
    public class RichCanvasClientUIAModeViewModel : ObservableObject
    {
        public RichCanvasAutomationModel RichCanvasAutomationModel { get; } = new RichCanvasAutomationModel();

        private string _pipeDataInfo;

        public string PipeDataInfo
        {
            get => _pipeDataInfo;
            set => SetProperty(ref _pipeDataInfo, value);
        }

        public ICommand TestCommand { get; }

        public RichCanvasClientUIAModeViewModel()
        {
            TestCommand = new RelayCommand(() =>
            {
                RichCanvasAutomationModel.Items.Add(PreDefinedAutomationItemModels.MutablePositionedRectangleWithSize);
            });
        }
    }
}
