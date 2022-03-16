using System.Windows;
using System.Windows.Input;

namespace RichCanvas.Helpers
{
    internal static class DragBehavior
    {
        private static Point _initialPosition;
        internal static RichItemsControl ItemsControl { get; set; }

        internal static DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(RichItemContainer),
            new PropertyMetadata(OnIsDraggingChanged));

        internal static void SetIsDragging(UIElement element, bool value) => element.SetValue(IsDraggingProperty, value);
        internal static bool GetIsDragging(UIElement element) => (bool)element.GetValue(IsDraggingProperty);

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichItemContainer container)
            {
                bool isDragging = (bool)e.NewValue;
                ItemsControl.IsDragging = isDragging;
                if (isDragging)
                {
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
            _initialPosition = new Point(e.GetPosition(ItemsControl.ItemsHost).X, e.GetPosition(ItemsControl.ItemsHost).Y);

            container.IsSelected = true;
            container.RaiseDragStartedEvent(_initialPosition);
            container.CaptureMouse();

            ItemsControl.Cursor = Cursors.Hand;
        }

        private static void OnSelectedContainerReleased(object sender, MouseButtonEventArgs e)
        {
            var container = (RichItemContainer)sender;

            container.RaiseDragCompletedEvent(e.GetPosition(ItemsControl.ItemsHost));

            if (container.IsMouseCaptured)
            {
                container.ReleaseMouseCapture();
            }
        }

        private static void OnSelectedContainerMove(object sender, MouseEventArgs e)
        {
            var container = (RichItemContainer)sender;
            if (container.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(ItemsControl.ItemsHost);

                var offset = currentPosition - _initialPosition;

                if ((ItemsControl.ScrollContainer.NegativeVerticalScrollDisabled && offset.Y > 0 && ItemsControl.ItemsHost.BottomElement.IsSelected) ||
                    (ItemsControl.ScrollContainer.NegativeHorizontalScrollDisabled && offset.X > 0 && ItemsControl.ItemsHost.RightElement.IsSelected))
                {
                    _initialPosition = currentPosition;
                    return;
                }

                if (offset.X != 0 || offset.Y != 0)
                {
                    container.RaiseDragDeltaEvent(new Point(offset.X, offset.Y));
                }

                _initialPosition = currentPosition;
            }
        }
    }
}
