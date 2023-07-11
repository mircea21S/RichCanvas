using System.Windows.Controls.Primitives;

namespace RichCanvas.States.Dragging
{
    public class DraggingStrategy
    {
        public RichItemsControl Parent { get; }

        public DraggingStrategy(RichItemsControl parent)
        {
            Parent = parent;
        }

        public virtual void OnItemsDragStarted(object sender, DragStartedEventArgs e) { }
        public virtual void OnItemsDragDelta(object sender, DragDeltaEventArgs e) { }
        public virtual void OnItemsDragCompleted(object sender, DragCompletedEventArgs e) { }
    }
}