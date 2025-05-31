using System.Windows;
using System.Windows.Input;

using RichCanvas.Gestures;
using RichCanvas.Helpers;
using RichCanvas.States.CanvasStates;

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
            else if (RichCanvasGestures.Select.Matches(e.Source, e))
            {
                if (Parent.CanSelectMultipleItems)
                {
                    PushState(new MultipleSelectionState(Parent));
                }
                else
                {
                    PushState(new SingleSelectionState(Parent));
                }
            }
        }

        public override bool MatchesPreviewMouseDownState(MouseButtonEventArgs e, out CanvasState? matchingState)
        {
            if (RichCanvasGestures.Pan.Matches(e.Source, e))
            {
                matchingState = new PanningState(Parent);
                return true;
            }
            matchingState = null;
            return false;
        }
    }
}
