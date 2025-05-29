namespace RichCanvasUITests.App.States
{
    public class ZoomingViewModel : ObservableObject
    {
        private double _maxScale = 2d;
        public double MaxScale
        {
            get => _maxScale;
            set => SetProperty(ref _maxScale, value);
        }

        private double _minScale = 0.1d;
        public double MinScale
        {
            get => _minScale;
            set => SetProperty(ref _minScale, value);
        }

        private double _scaleFactor = 1.1d;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set => SetProperty(ref _scaleFactor, value);
        }

        private double _viewportZoom = 1;
        public double ViewportZoom
        {
            get => _viewportZoom;
            set => SetProperty(ref _viewportZoom, value);
        }
    }
}
