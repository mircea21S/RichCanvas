using System.Windows.Input;

namespace RichCanvas.States
{
    public abstract class ContainerState
    {
        protected RichItemContainer Container { get; }
        public ContainerState(RichItemContainer container)
        {
            Container = container;
        }
        public virtual void Enter() { }
        public virtual void HandleMouseDown(MouseButtonEventArgs e) { }
        public virtual void HandleMouseMove(MouseEventArgs e) { }
        public virtual void HandleMouseUp(MouseButtonEventArgs e) { }
    }
}
