namespace RichCanvasUIA.Client.Debug_Mode.RichCanvasItems
{
    public class RichCanvasEditorItem : ObservableObject
    {
        private double _top;

        public double Top
        {
            get => _top;
            set => SetProperty(ref _top, value);
        }

        private double _left;

        public double Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }

        private double _width;

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private double _height;

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private bool _allowScaleChangeToUpdatePosition = true;

        public bool AllowScaleChangeToUpdatePosition
        {
            get => _allowScaleChangeToUpdatePosition;
            set => SetProperty(ref _allowScaleChangeToUpdatePosition, value);
        }

        private double _angle;

        public double Angle
        {
            get => _angle;
            set => SetProperty(ref _angle, value);
        }

        private bool _isDraggable = true;

        public bool IsDraggable
        {
            get => _isDraggable;
            set => SetProperty(ref _isDraggable, value);
        }
    }
}
