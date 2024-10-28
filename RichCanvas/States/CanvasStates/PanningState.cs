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

        public override void Enter()
        {
            _initialPosition = Mouse.GetPosition(Parent.ScrollContainer);
            Parent.IsPanning = true;
            Parent.Cursor = Cursors.Hand;
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (Parent.IsPanning)
            {
                var currentPosition = e.GetPosition(Parent.ScrollContainer);
                var delta = currentPosition - _initialPosition;

                Parent.ScrollContainer.Scroll((Point)delta);
                Parent.ViewportLocation += -delta / Parent.Scale;

                _initialPosition = currentPosition;
            }
        }

        public override void Exit()
        {
            Parent.IsPanning = false;
            Parent.Cursor = Cursors.Arrow;
        }
    }
}
