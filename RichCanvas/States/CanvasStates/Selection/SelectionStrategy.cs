namespace RichCanvas.States
{
    public class SelectionStrategy
    {
        public RichItemsControl Parent { get; }
        public SelectionStrategy(RichItemsControl parent)
        {
            Parent = parent;
        }

        public virtual void MouseMoveOnRealTimeSelection() { }

        public virtual void MouseMoveOnDeferredSelection() { }

        public virtual void MouseUpOnRealTimeSelection() { }

        public virtual void MouseUpOnDeferredSelection() { }
    }
}