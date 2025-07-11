using System.Windows;

namespace RichCanvas.States.ContainerStates
{
    internal class DraggingStrategy
    {
        protected RichCanvas Parent { get; }
        protected RichCanvasContainer Container { get; }

        internal DraggingStrategy(RichCanvasContainer container)
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