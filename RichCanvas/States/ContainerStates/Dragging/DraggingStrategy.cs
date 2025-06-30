using System.Windows;

namespace RichCanvas.States.ContainerStates
{
    public class DraggingStrategy
    {
        protected RichItemsControl Parent { get; }
        protected RichItemContainer Container { get; }

        public DraggingStrategy(RichItemContainer container)
        {
            Parent = container.Host;
            Container = container;
        }

        public virtual void OnItemsDragStarted()
        {
        }

        public virtual void OnItemsDragDelta(Point offsetPoint)
        {
        }

        public virtual void OnItemsDragCompleted()
        {
        }
    }
}