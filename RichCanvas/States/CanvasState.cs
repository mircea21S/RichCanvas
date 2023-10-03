using System;
using System.Windows;
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
        public virtual void Enter() { }
        public virtual void Cancel() { }
        public virtual void HandleMouseDown(MouseButtonEventArgs e) { }
        public virtual void HandleMouseMove(MouseEventArgs e) { }
        public virtual void HandleMouseUp(MouseButtonEventArgs e) { }
        public virtual void HandleAutoPanning(Point mousePosition, bool heightChanged = false) { }
    }
}
