using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace RichCanvas.States
{
    public static class StateManager
    {
        private static List<CanvasState> _canvasStates = new List<CanvasState>();

        public static DependencyProperty CanvasStatesProperty = DependencyProperty.RegisterAttached("CanvasStates", typeof(IList<CanvasState>), typeof(RichItemsControl), new FrameworkPropertyMetadata(default(IList<CanvasState>), OnCanvasStatesChanged));
        public static void SetCanvasStatesProperty(UIElement element, IList<CanvasState> value) => element.SetValue(CanvasStatesProperty, value);
        public static IList<CanvasState> GetApplyTransform(UIElement element) => (IList<CanvasState>)element.GetValue(CanvasStatesProperty);


        private static void OnCanvasStatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValues = e.OldValue as IList<CanvasState>;
            if (oldValues != null)
            {
                foreach (var oldValue in oldValues)
                {
                    if (_canvasStates.Contains(oldValue))
                    {
                        _canvasStates.Remove(oldValue);
                    }
                }
            }

            var newValues = e.NewValue as IList<CanvasState>;
            if (newValues != null)
            {
                _canvasStates.AddRange(newValues);
            }
        }

        public static void RegisterCanvasState(CanvasState state)
        {

        }
    }
}
