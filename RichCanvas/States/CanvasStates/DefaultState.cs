using System.Windows;
using System.Windows.Input;

using RichCanvas.Gestures;
using RichCanvas.Helpers;

namespace RichCanvas.States
{
    /// <summary>
    /// Defines the state called by <see cref="RichCanvas.GetDefaultState()"/>.
    /// <br/>
    /// Note: <i>Used for orchestrating all states interactions with <see cref="RichCanvas"/>.</i>
    /// </summary>
    public class DefaultState : CanvasState
    {
        /// <summary>
        /// Initializes a new <see cref="DefaultState"/>.
        /// </summary>
        /// <param name="parent">Owner of the state.</param>
        public DefaultState(RichCanvas parent) : base(parent)
        {
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
