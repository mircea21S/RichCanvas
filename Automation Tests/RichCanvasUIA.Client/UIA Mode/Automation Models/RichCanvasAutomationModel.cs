using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace RichCanvasUIA.Client.UIA_Mode.Automation_Models
{
    public class RichCanvasAutomationModel : ObservableObject
    {
        public ObservableCollection<RichCanvasContainerAutomationModel> Items { get; } = [];

        public bool ShouldExecuteDrawingEndedCommand { get; set; } = true;

        private Point _viewportLocation;

        public Point ViewportLocation
        {
            get => _viewportLocation;
            set => SetProperty(ref _viewportLocation, value);
        }

        private Size _viewportSize;

        public Size ViewportSize
        {
            get => _viewportSize;
            set => SetProperty(ref _viewportSize, value);
        }

        private RelayCommand<Point> _drawingEndedCommand;
        public ICommand DrawingEndedCommand => _drawingEndedCommand ??= new RelayCommand<Point>(DrawingEnded, () => ShouldExecuteDrawingEndedCommand);

        private void DrawingEnded(Point mousePositon)
        {
            Items.Add(new DrawingEndedVisualAutomationModel
            {
                Top = mousePositon.Y,
                Left = mousePositon.X,
                Width = 100,
                Height = 100
            });
        }
    }
}
