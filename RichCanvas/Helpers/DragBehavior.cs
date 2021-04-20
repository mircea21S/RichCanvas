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
        internal static void SetIsDragging(UIElement element, bool value)
        {
            element.SetValue(IsDraggingProperty, value);
        }
        internal static bool GetIsDragging(UIElement element)
        {
            return (bool)element.GetValue(IsDraggingProperty);
        }
        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichItemContainer container)
            {
                bool isDragging = (bool)e.NewValue;
                IsDragging = isDragging;
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
            RichItemContainer container = (RichItemContainer)sender;
            _initialPosition = new Point(e.GetPosition(ItemsControl.ItemsHost).X, e.GetPosition(ItemsControl.ItemsHost).Y);
            container.CaptureMouse();
            ItemsControl.Cursor = Cursors.Hand;
        }

        private static void OnSelectedContainerReleased(object sender, MouseButtonEventArgs e)
        {
            RichItemContainer container = (RichItemContainer)sender;
            container.ReleaseMouseCapture();

            var transformGroup = (TransformGroup)container.RenderTransform;
            var translateTransform = (TranslateTransform)transformGroup.Children[1];
            container.Top += translateTransform.Y;
            container.Left += translateTransform.X;

            translateTransform.X = 0;
            translateTransform.Y = 0;
            ItemsControl.UpdateSelections();
            ItemsControl.UpdateLimits();
            ItemsControl.ItemsHost.InvalidateArrange();
            ItemsControl.Cursor = Cursors.Arrow;
        }

        private static void OnSelectedContainerMove(object sender, MouseEventArgs e)
        {
            RichItemContainer container = (RichItemContainer)sender;
            if (container.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(ItemsControl.ItemsHost);

                var transformGroup = (TransformGroup)container.RenderTransform;
                var translateTransform = (TranslateTransform)transformGroup.Children[1];

                if (!ItemsControl.HasSelections)
                {
                    translateTransform.X += currentPosition.X - _initialPosition.X;
                    translateTransform.Y += currentPosition.Y - _initialPosition.Y;
                }

                DragDelta?.Invoke(new Point(currentPosition.X - _initialPosition.X, currentPosition.Y - _initialPosition.Y));

                if (ItemsControl.HasSelections)
                {
                    ItemsControl.UpdateSelectionsLimit();
                }
                _initialPosition = currentPosition;
            }
        }
    }
}
