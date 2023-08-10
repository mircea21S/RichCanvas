using System.Windows;
using System.Windows.Input;

namespace RichCanvas.States
{
    public class PanningState : CanvasState
    {
        private Point _initialPosition;

        public PanningState(RichItemsControl parent) : base(parent)
        {
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            _initialPosition = e.GetPosition(Parent.ScrollContainer);
            Parent.IsPanning = true;
            Parent.Cursor = Cursors.Hand;
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (Parent.IsPanning)
            {
                var currentPosition = e.GetPosition(Parent.ScrollContainer);
                Parent.ScrollContainer.Pan(_initialPosition, currentPosition);
                _initialPosition = currentPosition;
            }
        }

        public override void HandleMouseUp(MouseButtonEventArgs e)
        {
            Parent.IsPanning = false;
            Parent.Cursor = Cursors.Arrow;
        }
    }
}
