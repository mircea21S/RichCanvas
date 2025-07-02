using System.Windows;

namespace RichCanvas.States.ContainerStates
{
    internal class DraggingStrategy
    {
        protected RichItemsControl Parent { get; }
        protected RichItemContainer Container { get; }

        internal DraggingStrategy(RichItemContainer container)
        {
            Parent = container.Host;
            Container = container;
        }

        internal virtual void OnItemsDragStarted()
        {
        }

        internal virtual void OnItemsDragDelta(Point offsetPoint)
        {
        }

        internal virtual void OnItemsDragCompleted()
        {
        }
    }
}