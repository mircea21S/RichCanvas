using RichCanvas.Gestures;
using RichCanvas.Helpers;
using RichCanvas.States.SelectionStates;
using System.Windows;
using System.Windows.Input;

namespace RichCanvas.States
{
    internal class DefaultState : CanvasState
    {
        public DefaultState(RichItemsControl parent) : base(parent)
        {
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (RichCanvasGestures.Drawing.Matches(e.Source, e)
                && Parent.CurrentDrawingIndexes.Count > 0
                && !VisualHelper.HasScrollBarParent((DependencyObject)e.OriginalSource))
            {
                PushState(new DrawingState(Parent));
            }
            else if (RichCanvasGestures.Pan.Matches(e.Source, e))
            {
                PushState(new PanningState(Parent));
            }
            else if (RichCanvasGestures.Select.Matches(e.Source, e) && Parent.SelectionEnabled)
            {
                PushState(new SelectingState(Parent));
            }
            
        }
    }
}
