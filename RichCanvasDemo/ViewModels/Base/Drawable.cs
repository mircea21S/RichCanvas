using RichCanvasDemo.Common;

namespace RichCanvasDemo.ViewModels.Base
{
    public abstract class Drawable : ObservableObject
    {
        private double _top;
        private double _left;
        private bool _isSelected;
        private bool _isSelectable = true;
        private bool _isDraggable = true;
        private double _width;
        private double _height;
        private VisualProperties _visualProperties;
        private bool _shouldBringIntoView;

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

        public VisualProperties VisualProperties
        {
            get => _visualProperties;
            set => SetProperty(ref _visualProperties, value);
        }

        public bool IsSelectable
        {
            get => _isSelectable;
            set => SetProperty(ref _isSelectable, value);
        }

        public bool IsDraggable
        {
            get => _isDraggable;
            set => SetProperty(ref _isDraggable, value);
        }
        public bool ShouldBringIntoView
        {
            get => _shouldBringIntoView;
            set => SetProperty(ref _shouldBringIntoView, value);
        }
        public Drawable()
        {
            VisualProperties = new VisualProperties();
        }

        protected virtual void OnWidthUpdated() { }
        protected virtual void OnHeightUpdated() { }

        public virtual Drawable Clone() => null;
    }
}
