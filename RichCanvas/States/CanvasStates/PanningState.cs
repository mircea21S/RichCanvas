using System.Windows;
using System.Windows.Input;

namespace RichCanvas.States
{
    /// <summary>
    /// Defines a new state used when panning action happens on <see cref="RichCanvas"/>.
    /// </summary>
    public class PanningState : CanvasState
    {
        private Point _initialPosition;

        /// <summary>
        /// Initializes a new <see cref="PanningState"/>.
        /// </summary>
        /// <param name="parent">Owner of the state.</param>
        public PanningState(RichCanvas parent) : base(parent)
        {
        }

        /// <inheritdoc/>
        public override void Enter()
        {
            _initialPosition = Mouse.GetPosition(Parent);
            Parent.IsPanning = true;
            Parent.Cursor = Cursors.Hand;
        }

        /// <inheritdoc/>
        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (Parent.IsPanning)
            {
                Point currentPosition = e.GetPosition(Parent);
                Vector delta = currentPosition - _initialPosition;

                Parent.ViewportLocation -= delta / Parent.ViewportZoom;

                _initialPosition = currentPosition;
            }
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            Parent.IsPanning = false;
            Parent.Cursor = Cursors.Arrow;
        }
    }
}
