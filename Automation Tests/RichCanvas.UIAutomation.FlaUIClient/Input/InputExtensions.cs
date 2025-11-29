using System.Windows.Input;

using FlaUI.Core.WindowsAPI;

using MouseButton = FlaUI.Core.Input.MouseButton;

namespace RichCanvas.UIAutomation.FlaUIClient.Input
{
    public static class InputExtensions
    {
        public static VirtualKeyShort ToVirtualKeyShort(this Key key) => key switch
        {
            Key.Q => VirtualKeyShort.KEY_Q,
            Key.Space => VirtualKeyShort.SPACE,
            _ => throw new MissingMemberException("Key not mapped")
        };

        public static MouseButton ToMouseButton(this MouseAction mouseAction) => mouseAction switch
        {
            MouseAction.LeftClick => MouseButton.Left,
            _ => throw new MissingMemberException("Key not mapped")
        };

        public static VirtualKeyShort ToVirtualKeyShort(this ModifierKeys modifierKey) => modifierKey switch
        {
            ModifierKeys.Control => VirtualKeyShort.CONTROL,
            ModifierKeys.Shift => VirtualKeyShort.SHIFT,
            ModifierKeys.Windows => VirtualKeyShort.LWIN,
            ModifierKeys.Alt => VirtualKeyShort.ALT,
            ModifierKeys.None => VirtualKeyShort.NONAME,
            _ => throw new MissingMemberException("Key not mapped")
        };
    }
}
