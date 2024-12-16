using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class RichCanvasGestures
    {
        public static InputGesture Select { get; set; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drawing { get; set; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drag { get; set; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture ZoomIn { get; set; } = new KeyGesture(Key.OemPlus, ModifierKeys.Control);
        public static InputGesture ZoomOut { get; set; } = new KeyGesture(Key.OemMinus, ModifierKeys.Control);
        public static InputGesture Pan { get; set; } = new MouseKeyGesture(new MouseGesture(MouseAction.LeftClick), new KeyGesture(Key.Space));
        //TODO: add a bool DP to change from wheel zoom to a state with a custom gesture
        public static ModifierKeys ZoomModifierKey { get; set; } = ModifierKeys.Control;
    }
}
