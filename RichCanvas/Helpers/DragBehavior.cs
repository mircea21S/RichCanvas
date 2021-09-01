using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RichCanvas.Helpers
{
    internal static class DragBehavior
    {
        internal delegate void DragDeltaEventHandler(Point point);
        internal static event DragDeltaEventHandler DragDelta;
        private static Point _initialPosition;

        internal static RichItemsControl ItemsControl { get; set; }

        public static bool IsDragging { get; private set; }


        internal static DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(RichItemContainer),
            new PropertyMetadata(OnIsDraggingChanged));

        internal static void SetIsDragging(UIElement element, bool value) => element.SetValue(IsDraggingProperty, value);
        internal static bool GetIsDragging(UIElement element) => (bool)element.GetValue(IsDraggingProperty);

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichItemContainer container)
            {
                bool isDragging = (bool)e.NewValue;
                IsDragging = isDragging;
                ItemsControl.IsDragging = isDragging;
                if (isDragging)
                {
                    //can raise routed events on container(drag started e.g)
                    container.MouseDown += OnSelectedContainerClicked;
                    container.MouseMove += OnSelectedContainerMove;
                    container.MouseUp += OnSelectedContainerReleased;
                }
                else
                {
                    container.MouseDown -= OnSelectedContainerClicked;
                    container.MouseMove -= OnSelectedContainerMove;
                    container.MouseUp -= OnSelectedContainerReleased;
                }
            }
        }

        private static void OnSelectedContainerClicked(object sender, MouseButtonEventArgs e)
        {
            var container = (RichItemContainer)sender;
            container.IsSelected = true;

            _initialPosition = new Point(e.GetPosition(ItemsControl.ItemsHost).X, e.GetPosition(ItemsControl.ItemsHost).Y);
            container.CaptureMouse();
            ItemsControl.Cursor = Cursors.Hand;
        }

        private static void OnSelectedContainerReleased(object sender, MouseButtonEventArgs e)
        {
            var container = (RichItemContainer)sender;
            container.ReleaseMouseCapture();

            TranslateTransform translateTransform = container.TranslateTransform;

            if (translateTransform != null)
            {
                container.Left += translateTransform.X;
                container.Top += translateTransform.Y;
                translateTransform.X = 0;
                translateTransform.Y = 0;
            }

            if (ItemsControl.EnableGrid && ItemsControl.EnableSnapping)
            {
                container.Left = Math.Round(container.Left / ItemsControl.GridSpacing) * ItemsControl.GridSpacing;
                container.Top = Math.Round(container.Top / ItemsControl.GridSpacing) * ItemsControl.GridSpacing;
            }


            if (ItemsControl.HasSelections)
            {
                ItemsControl.UpdateSelections(ItemsControl.EnableSnapping);
            }
            else
            {
                ItemsControl.NeedMeasure = true;
                ItemsControl.ItemsHost.InvalidateMeasure();
            }

            ItemsControl.Cursor = Cursors.Arrow;
        }

        private static void OnSelectedContainerMove(object sender, MouseEventArgs e)
        {
            var container = (RichItemContainer)sender;
            if (container.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(ItemsControl.ItemsHost);

                TranslateTransform translateTransform = container.TranslateTransform;

                if (!ItemsControl.HasSelections && translateTransform != null)
                {
                    translateTransform.X += currentPosition.X - _initialPosition.X;
                    translateTransform.Y += currentPosition.Y - _initialPosition.Y;
                }
                else
                {
                    DragDelta?.Invoke(new Point(currentPosition.X - _initialPosition.X, currentPosition.Y - _initialPosition.Y));
                }

                ItemsControl.NeedMeasure = true;
                ItemsControl.UpdateSelections();

                _initialPosition = currentPosition;
            }
        }
    }
}
