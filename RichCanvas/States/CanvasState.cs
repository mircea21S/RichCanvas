using System;
using System.Windows.Input;

namespace RichCanvas.States
{
    public abstract class CanvasState
    {
        protected RichItemsControl Parent { get; }
        public CanvasState(RichItemsControl parent)
        {
            Parent = parent;
        }
        public virtual void Enter(InputEventArgs? e = default) { }
        public virtual void Exit() { }
        public virtual void HandleMouseDown(MouseButtonEventArgs e) { }
        public virtual void HandleMouseMove(MouseEventArgs e) { }
        public virtual void HandleMouseUp(MouseButtonEventArgs e) { }
        protected void SetState(CanvasState state) => Parent.SetCurrentState(state);
    }
}
