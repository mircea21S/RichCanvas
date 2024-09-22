using System.Windows;
using System.Windows.Controls.Primitives;

namespace RichCanvas.States.ContainerStates
{
    public class DraggingStrategy
    {
        public RichItemsControl Parent { get; }

        public DraggingStrategy(RichItemsControl parent)
        {
            Parent = parent;
        }

        public virtual void OnItemsDragStarted() { }
        public virtual void OnItemsDragDelta(Point offsetPoint) { }
        public virtual void OnItemsDragCompleted() { }
    }
}