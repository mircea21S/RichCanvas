using RichCanvasDemo.Common;
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

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
        private Point _directionPoint;
        private RelayCommand<double> leftChangedCommand;

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
            set
            {
                SetProperty(ref _isSelected, value);
                OnIsSelectedChanged(value);
            }
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

        public Point DirectionPoint
        {
            get => _directionPoint;
            set => SetProperty(ref _directionPoint, value);
        }

        public ICommand LeftChangedCommand => leftChangedCommand ??= new RelayCommand<double>(OnLeftChanged);


        public Drawable()
        {
            VisualProperties = new VisualProperties();
        }
        protected virtual void OnLeftChanged(double delta) { }

        protected virtual void OnWidthUpdated() { }

        protected virtual void OnHeightUpdated() { }

        protected virtual void OnIsSelectedChanged(bool value) { }

        protected Drawable Clone<T>() where T : Drawable => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(this));

        public virtual Drawable Clone() => throw new NotImplementedException();

        public virtual void OnDrawingEnded(Action<object> callback = default) { }
    }
}
