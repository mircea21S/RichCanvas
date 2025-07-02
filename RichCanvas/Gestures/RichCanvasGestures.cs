using System.Windows.Input;

using RichCanvas.States;
using RichCanvas.States.ContainerStates;

namespace RichCanvas.Gestures
{
    /// <summary>
    /// Holds all default <see cref="InputGesture"/>s used to match their associated state.
    /// </summary>
    public class RichCanvasGestures
    {
        /// <summary>
        /// Gets or sets the <see cref="InputGesture"/> used to match both <see cref="SingleSelectionState"/> or <see cref="MultipleSelectionState"/>.
        /// </summary>
        public static InputGesture Select { get; set; } = new MouseGesture(MouseAction.LeftClick);

        /// <summary>
        /// Gets or sets the <see cref="InputGesture"/> used to match the <see cref="DrawingState"/>.
        /// </summary>
        public static InputGesture Drawing { get; set; } = new MouseGesture(MouseAction.LeftClick);

        /// <summary>
        /// Gets or sets the <see cref="InputGesture"/> used to match the <see cref="DraggingContainerState"/>.
        /// </summary>
        public static InputGesture Drag { get; set; } = new MouseGesture(MouseAction.LeftClick);

        /// <summary>
        /// Gets or sets the <see cref="InputGesture"/> used to invoke <see cref="RichCanvasCommands.ZoomIn"/> routed command.
        /// </summary>
        public static InputGesture ZoomIn { get; set; } = new KeyGesture(Key.OemPlus, ModifierKeys.Control);

        /// <summary>
        /// Gets or sets the <see cref="InputGesture"/> used to invoke <see cref="RichCanvasCommands.ZoomOut"/> routed command.
        /// </summary>
        public static InputGesture ZoomOut { get; set; } = new KeyGesture(Key.OemMinus, ModifierKeys.Control);

        /// <summary>
        /// Gets or sets the <see cref="InputGesture"/> used to match the <see cref="PanningState"/>.
        /// </summary>
        public static InputGesture Pan { get; set; } = new MouseKeyGesture(new MouseGesture(MouseAction.LeftClick), new KeyGesture(Key.Space));

        //TODO: add a bool DP to change from wheel zoom to a state with a custom gesture
        /// <summary>
        /// Gets or sets the <see cref="ModifierKeys"/> used together with MouseWheel for zooming.
        /// <br/>
        /// Default is <see cref="ModifierKeys.Control"/>.
        /// </summary>
        public static ModifierKeys ZoomModifierKey { get; set; } = ModifierKeys.Control;
    }
}
