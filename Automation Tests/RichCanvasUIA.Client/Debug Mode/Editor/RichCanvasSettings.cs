namespace RichCanvasUIA.Client.Debug_Mode.Editor
{
    public class RichCanvasSettings : ObservableObject
    {
        private bool _realTimeSelectionEnabled;

        public bool RealTimeSelectionEnabled
        {
            get => _realTimeSelectionEnabled;
            set => SetProperty(ref _realTimeSelectionEnabled, value);
        }

        private bool _canSelectMultipleItems;

        public bool CanSelectMultipleItems
        {
            get => _canSelectMultipleItems;
            set => SetProperty(ref _canSelectMultipleItems, value);
        }
    }
}
