using RichCanvas.Gestures;
using RichCanvas.States.SelectionStates;
using System.Windows.Input;

namespace RichCanvas.States
{
    public class DefaultCanvasState : CanvasState
    {
        public DefaultCanvasState(RichItemsControl parent) : base(parent)
        {
        }
        public override void Enter(InputEventArgs e)
        {
            //if (RichCanvasGestures.Drawing.Matches(e.Source, e) && Parent.CurrentDrawingIndexes.Count > 0)
            //{
            //    Parent.SetCurrentState(new DrawingState(Parent));
            //}
            //else
            if (RichCanvasGestures.Select.Matches(e.Source, e))
            {
                Parent.SetCurrentState(new SelectingState(Parent));
            }
        }
    }
}
