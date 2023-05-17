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

            if (!ItemsControl.CanSelectMultipleItems)
            {
                ItemsControl.UpdateSelectedItem(container);
            }
            container.IsSelected = true;

            if (ItemsControl.CanSelectMultipleItems || (container.IsSelected && !ItemsControl.CanSelectMultipleItems))
            {
                container.RaiseDragStartedEvent(_initialPosition);
            }
            container.CaptureMouse();

            if (!ItemsControl.IsPanning)
            {
                ItemsControl.Cursor = Cursors.Hand;
            }
        }

        private static void OnSelectedContainerReleased(object sender, MouseButtonEventArgs e)
        {
            var container = (RichItemContainer)sender;

            if ((container.IsSelected && !ItemsControl.CanSelectMultipleItems) || ItemsControl.CanSelectMultipleItems)
            {
                container.RaiseDragCompletedEvent(e.GetPosition(ItemsControl.ItemsHost));
            }

            if (container.IsMouseCaptured)
            {
                container.ReleaseMouseCapture();
            }
        }

        private static void OnSelectedContainerMove(object sender, MouseEventArgs e)
        {
            var container = (RichItemContainer)sender;
            if (container.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed && ((container.IsSelected && !ItemsControl.CanSelectMultipleItems) || ItemsControl.CanSelectMultipleItems))
            {
                Point currentPosition = e.GetPosition(ItemsControl.ItemsHost);
                var offset = currentPosition - _initialPosition;

                if (offset.X != 0 || offset.Y != 0)
                {
                    if (!ItemsControl.EnableNegativeScrolling)
                    {
                        if (offset.Y > 0 && ItemsControl.ItemsHost.BottomElement.IsSelected && ItemsControl.ScrollContainer.BottomLimit < ItemsControl.ItemsHost.BottomElement.BoundingBox.Bottom + offset.Y)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }

                        if (offset.X > 0 && ItemsControl.ItemsHost.RightElement.IsSelected && ItemsControl.ScrollContainer.RightLimit < ItemsControl.ItemsHost.RightElement.BoundingBox.Right + offset.X)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                    }

                    if (!ItemsControl.ExtentSize.IsEmpty)
                    {
                        if (offset.Y < 0 && ItemsControl.ItemsHost.TopElement.IsSelected
                            && ItemsControl.ItemsHost.TopElement.BoundingBox.Top + offset.Y < ItemsControl.ScrollContainer.TopLimit - ItemsControl.ExtentSize.Height)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                        if (offset.Y > 0 && ItemsControl.ItemsHost.BottomElement.IsSelected
                            && ItemsControl.ItemsHost.BottomElement.BoundingBox.Bottom + offset.Y > ItemsControl.ScrollContainer.BottomLimit + ItemsControl.ExtentSize.Height)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                        if (offset.X < 0 && ItemsControl.ItemsHost.LeftElement.IsSelected
                            && ItemsControl.ItemsHost.LeftElement.BoundingBox.Left + offset.X < ItemsControl.ScrollContainer.LeftLimit - ItemsControl.ExtentSize.Width)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                        if (offset.X > 0 && ItemsControl.ItemsHost.RightElement.IsSelected
                            && ItemsControl.ItemsHost.RightElement.BoundingBox.Right + offset.X > ItemsControl.ScrollContainer.RightLimit + ItemsControl.ExtentSize.Width)
                        {
                            _initialPosition = currentPosition;
                            return;
                        }
                    }

                    container.RaiseDragDeltaEvent(new Point(offset.X, offset.Y));
                    _initialPosition = currentPosition;
                }
            }
        }
    }
}
