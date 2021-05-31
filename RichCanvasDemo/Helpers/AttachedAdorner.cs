using RichCanvas;
using RichCanvasDemo.Adorners;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RichCanvasDemo.Helpers
{
    public class AttachedAdorner
    {
        private static Adorner _currentAdorner;
        private static Adorner _currentLineAdorner;
        private static List<ResizeAdorner> _resizeAdorner = new List<ResizeAdorner>();

        public static readonly DependencyProperty HasLineHoverAdornerProperty = DependencyProperty.RegisterAttached("HasLineHoverAdorner", typeof(bool), typeof(AttachedAdorner),
            new FrameworkPropertyMetadata(false, OnHasLineHoverChanged));
        public static void SetHasLineHoverAdorner(UIElement element, bool value) => element.SetValue(HasLineHoverAdornerProperty, value);
        public static bool GetHasLineHoverAdorner(UIElement element) => (bool)element.GetValue(HasLineHoverAdornerProperty);


        public static readonly DependencyProperty HasHoverAdornerProperty = DependencyProperty.RegisterAttached("HasHoverAdorner", typeof(bool), typeof(AttachedAdorner),
            new FrameworkPropertyMetadata(false, OnHasHoverChanged));
        public static void SetHasHoverAdorner(UIElement element, bool value) => element.SetValue(HasHoverAdornerProperty, value);
        public static bool GetHasHoverAdorner(UIElement element) => (bool)element.GetValue(HasHoverAdornerProperty);

        public static readonly DependencyProperty HasResizeAdornerProperty = DependencyProperty.RegisterAttached("HasResizeAdorner", typeof(bool), typeof(AttachedAdorner),
            new FrameworkPropertyMetadata(false, OnHasResizeAdornerChanged));
        public static void SetHasResizeAdorner(UIElement element, bool value) => element.SetValue(HasResizeAdornerProperty, value);
        public static bool GetHasResizeAdorner(UIElement element) => (bool)element.GetValue(HasResizeAdornerProperty);

        private static void OnHasResizeAdornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (RichItemContainer)d;
            var value = (bool)e.NewValue;
            if (value)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
                var adorner = new ResizeAdorner(element);
                layer.Add(adorner);
                _resizeAdorner.Add(adorner);
            }
            else
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
                if (layer != null)
                {
                    foreach (var adorner in _resizeAdorner)
                    {
                        layer.Remove(adorner);
                    }
                    _resizeAdorner.Clear();
                }
            }
        }

        private static void OnHasLineHoverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var line = (Line)d;
            var value = (bool)e.NewValue;
            if (value)
            {
                line.MouseEnter += OnMouseEnterLine;
                line.MouseLeave += OnMouseLeaveLine;
            }
            else
            {
                line.MouseEnter -= OnMouseEnterLine;
                line.MouseLeave -= OnMouseLeaveLine;
            }
        }

        private static void OnMouseLeaveLine(object sender, MouseEventArgs e)
        {
            var element = (Line)sender;
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
            if (layer != null)
            {
                layer.Remove(_currentLineAdorner);
            }
        }

        private static void OnMouseEnterLine(object sender, MouseEventArgs e)
        {
            var line = (Line)sender;

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(line);

            var adorner = new LineHoverAdorner(line);
            var highlightLine = new Line
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 3,
                X1 = line.X1,
                X2 = line.X2,
                Y1 = line.Y1,
                Y2 = line.Y2
            };
            adorner.Container.Content = highlightLine;
            layer.Add(adorner);
            _currentLineAdorner = adorner;
        }


        private static void OnHasHoverChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (RichItemContainer)d;

            var value = (bool)e.NewValue;
            if (value)
            {
                element.MouseEnter += OnMouseEnter;
                element.MouseLeave += OnMouseLeave;
            }
            else
            {
                element.MouseEnter -= OnMouseEnter;
                element.MouseLeave -= OnMouseLeave;
            }
        }

        private static void OnMouseEnter(object sender, MouseEventArgs e)
        {
            var element = (RichItemContainer)sender;
            var template = (DataTemplate)element.FindResource("HoverAdornerTemplate");

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);

            HoverAdorner adorner = new HoverAdorner(element);
            adorner.Container.ContentTemplate = template;
            layer.Add(adorner);
            _currentAdorner = adorner;
        }

        private static void OnMouseLeave(object sender, MouseEventArgs e)
        {
            var element = (RichItemContainer)sender;
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(element);
            if (layer != null)
            {
                layer.Remove(_currentAdorner);
            }
        }

    }
}
