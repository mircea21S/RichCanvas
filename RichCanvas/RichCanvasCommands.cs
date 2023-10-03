using RichCanvas.Gestures;
using System;
using System.Windows.Input;

namespace RichCanvas
{
    public class RichCanvasCommands
    {
        /// <summary>
        /// Zoom in relative to the editor's viewport center.
        /// </summary>
        public static RoutedUICommand ZoomIn { get; } = new RoutedUICommand("Zoom in", nameof(ZoomIn), typeof(RichCanvasCommands), new InputGestureCollection
        {
           RichCanvasGestures.ZoomIn
        });

        /// <summary>
        /// Zoom out relative to the editor's viewport center.
        /// </summary>
        public static RoutedUICommand ZoomOut { get; } = new RoutedUICommand("Zoom out", nameof(ZoomOut), typeof(RichCanvasCommands), new InputGestureCollection
        {
            RichCanvasGestures.ZoomOut
        });

        internal static void Register(Type type)
        {
            CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ZoomIn, OnZoomIn));
        }

        private static void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is RichItemsControl richItemsControl)
            {
                richItemsControl.ZoomIn();
            }
        }
    }
}
