using RichCanvas.Adorners;
using RichCanvas.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas
{
    public class RichItemContainer : ContentControl
    {
        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RichItemContainer));
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(RichItemContainer));
        public double Top
        {
            get => (double)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }
        public static DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(RichItemContainer));
        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }
        public static DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }
        public static DependencyProperty IsDraggableProperty = DependencyProperty.Register("IsDraggable", typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        private HighlightAdorner _currentAdorner;

        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }
        static RichItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemContainer), new FrameworkPropertyMetadata(typeof(RichItemContainer)));
        }

        internal bool IsDrawn { get; set; }
        internal RichItemsControl Host { get; set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            // Attach new adorner to current ListBoxItem
            var adorner = new HighlightAdorner(this);
            _currentAdorner = adorner;
            adorner.Container.Content = this;
            adorner.Container.ContentTemplate = Host.HighlightTemplate;
            layer.Add(adorner);

            if (IsDraggable)
            {
                DragBehavior.SetIsDragging((RichItemContainer)e.OriginalSource, true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            if (_currentAdorner != null)
            {
                layer.Remove(_currentAdorner);
            }
            if (IsDraggable)
            {
                DragBehavior.SetIsDragging((RichItemContainer)e.OriginalSource, false);
            }
        }

        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            var intersection = hitTestParameters.HitGeometry.FillContainsWithDetail(new RectangleGeometry(new Rect(this.Left, this.Top, this.Width, this.Height)));
            return new GeometryHitTestResult(this, intersection);
        }

        internal bool IsValid()
        {
            return Height != 0 && Width != 0 && !double.IsNaN(Height) && !double.IsNaN(Width);
        }
    }
}
