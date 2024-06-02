using RichCanvas.Gestures;
using System.Windows.Input;

namespace RichCanvas.States.ContainerStates
{
    public class ContainerDefaultState : ContainerState
    {
        public ContainerDefaultState(RichItemContainer container) : base(container)
        {
        }

        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (RichCanvasGestures.Drag.Matches(e.Source, e) && Container.IsDraggable)
            {
                PushState(new DraggingContainerState(Container));
            }
        }
    }
}
