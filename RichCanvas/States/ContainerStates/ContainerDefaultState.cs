using System.Windows.Input;

using RichCanvas.Gestures;

namespace RichCanvas.States.ContainerStates
{
    /// <summary>
    /// Defines the state called by <see cref="RichItemContainer.GetDefaultState()"/>.
    /// <br/>
    /// Note: <i>Used for orchestrating all states interactions with <see cref="RichItemContainer"/>.</i>
    /// </summary>
    public class ContainerDefaultState : ContainerState
    {
        /// <summary>
        /// Initializes a new <see cref="ContainerDefaultState"/>.
        /// </summary>
        /// <param name="container">Owner of the state.</param>
        public ContainerDefaultState(RichItemContainer container) : base(container)
        {
        }

        /// <inheritdoc/>
        public override void HandleMouseDown(MouseButtonEventArgs e)
        {
            if (RichCanvasGestures.Drag.Matches(e.Source, e) && Container.IsDraggable)
            {
                PushState(new DraggingContainerState(Container));
            }
        }
    }
}
