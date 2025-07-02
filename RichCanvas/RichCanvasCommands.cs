using System;
using System.Windows.Input;

using RichCanvas.Gestures;

namespace RichCanvas
{
    /// <summary>
    /// Holds pre-defined <see cref="RichItemsControl"/> routed commands.
    /// </summary>
    public class RichCanvasCommands
    {
        /// <summary>
        /// Zoom in relative to the canvas current mouse position.
        /// </summary>
        public static RoutedUICommand ZoomIn { get; } = new RoutedUICommand("Zoom in", nameof(ZoomIn), typeof(RichCanvasCommands), new InputGestureCollection
        {
           RichCanvasGestures.ZoomIn
        });

        /// <summary>
        /// Zoom out relative to the canvas current mouse position.
        /// </summary>
        public static RoutedUICommand ZoomOut { get; } = new RoutedUICommand("Zoom out", nameof(ZoomOut), typeof(RichCanvasCommands), new InputGestureCollection
        {
            RichCanvasGestures.ZoomOut
        });

        internal static void Register(Type type)
        {
            CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ZoomIn, OnZoomIn));
            CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ZoomOut, OnZoomOut));
        }

        private static void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is RichItemsControl richItemsControl)
            {
                richItemsControl.ZoomOut();
            }
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
