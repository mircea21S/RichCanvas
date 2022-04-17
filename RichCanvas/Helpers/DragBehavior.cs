using System;
using System.Windows;
using System.Windows.Input;

namespace RichCanvas.Helpers
{
    internal static class DragBehavior
    {
        private static Point _initialPosition;
        internal static RichItemsControl? ItemsControl { get; set; }

        internal static DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(RichItemContainer),
            new PropertyMetadata(OnIsDraggingChanged));

        internal static void SetIsDragging(UIElement element, bool value) => element.SetValue(IsDraggingProperty, value);
        internal static bool GetIsDragging(UIElement element) => (bool)element.GetValue(IsDraggingProperty);

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichItemContainer container)
            {
                bool isDragging = (bool)e.NewValue;
                if (ItemsControl != null)
                {
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
            _initialPosition = new Point(e.GetPosition(ItemsControl.ItemsHost).X, e.GetPosition(ItemsControl.ItemsHost).Y);
            var container = (RichItemContainer)sender;

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

                if (!ItemsControl.EnableNegativeScrolling)
                {
                    if (offset.Y > 0 && ItemsControl.ItemsHost.BottomElement.IsSelected && ItemsControl.ScrollContainer.NegativeVerticallOffset < 0 && ItemsControl.ScrollContainer.NegativeVerticallOffset - offset.Y <= 0)
                    {
                        _initialPosition = currentPosition;
                        return;
                    }

                    if (offset.X > 0 && ItemsControl.ItemsHost.RightElement.IsSelected && ItemsControl.ScrollContainer.NegativeHorizontalOffset < 0 && ItemsControl.ScrollContainer.NegativeHorizontalOffset - offset.X <= 0)
                    {
                        _initialPosition = currentPosition;
                        return;
                    }
                }

                if (!ItemsControl.ExtentSize.IsEmpty)
                {
                    if (offset.Y < 0 && ItemsControl.ItemsHost.TopElement.IsSelected && ItemsControl.ScrollContainer.VerticalOffset + (-offset.Y) >= ItemsControl.ExtentSize.Height)
                    {
                        _initialPosition = currentPosition;
                        return;
                    }

                    if (ItemsControl.ExtentSize.Height == 0)
                    {
                        if (offset.Y > 0 && ItemsControl.ItemsHost.BottomElement.IsSelected && ItemsControl.ScrollContainer.NegativeVerticallOffset < 0 && ItemsControl.ScrollContainer.NegativeVerticallOffset - offset.Y <= 0)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                    }
                    else
                    {
                        if (offset.Y > 0 && ItemsControl.ItemsHost.BottomElement.IsSelected && ItemsControl.ScrollContainer.NegativeVerticallOffset - offset.Y <= -ItemsControl.ExtentSize.Height)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                    }

                    if (offset.X < 0 && ItemsControl.ItemsHost.LeftElement.IsSelected && ItemsControl.ScrollContainer.HorizontalOffset + (-offset.X) >= ItemsControl.ExtentSize.Width)
                    {
                        _initialPosition = currentPosition;
                        return;
                    }

                    if (offset.X > 0 && ItemsControl.ItemsHost.RightElement.IsSelected && ItemsControl.ScrollContainer.NegativeHorizontalOffset - offset.X <= -ItemsControl.ExtentSize.Width)
                    {
                        _initialPosition = currentPosition;
                        return;
                    }
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
