using System;
using System.Windows.Input;

using RichCanvas.Gestures;

namespace RichCanvas
{
    /// <summary>
    /// Holds pre-defined <see cref="RichCanvas"/> routed commands.
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

        /// <summary>
        /// Change the viewport location and zoom to fit as many items as possible on screen.
        /// </summary>
        public static RoutedUICommand FitToScreen { get; } = new RoutedUICommand("Fit to screen", nameof(FitToScreen), typeof(RichCanvasCommands), new InputGestureCollection
        {
           RichCanvasGestures.FitToScreen
        });

        internal static void Register(Type type)
        {
            CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ZoomIn, OnZoomIn));
            CommandManager.RegisterClassCommandBinding(type, new CommandBinding(ZoomOut, OnZoomOut));
            CommandManager.RegisterClassCommandBinding(type, new CommandBinding(FitToScreen, OnFitToScreen));
        }

        private static void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is RichCanvas richItemsControl)
            {
                richItemsControl.ZoomOut();
            }
        }

        private static void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is RichCanvas richItemsControl)
            {
                richItemsControl.ZoomIn();
            }
        }

        private static void OnFitToScreen(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is RichCanvas richItemsControl)
            {
                richItemsControl.FitToScreen();
            }
        }
    }
}
