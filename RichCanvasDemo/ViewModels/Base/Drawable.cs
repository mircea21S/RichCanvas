using RichCanvasDemo.Common;

namespace RichCanvasDemo.ViewModels.Base
{
    public abstract class Drawable : ObservableObject
    {
        private double _top;
        private double _left;
        private bool _isSelected;
        private double _width;
        private double _height;

        public double Top
        {
            get => _top;
            set => SetProperty(ref _top, value);
        }
        public double Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
        public double Width
        {
            get => _width;
            set
            {
                SetProperty(ref _width, value);
                OnWidthUpdated();
            }
        }
        public double Height
        {
            get => _height;
            set
            {
                SetProperty(ref _height, value);
                OnHeightUpdated();
            }
        }

        protected virtual void OnWidthUpdated() { }
        protected virtual void OnHeightUpdated() { }
    }
}
