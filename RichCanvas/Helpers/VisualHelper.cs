using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas.Helpers
{
    internal static class VisualHelper
    {
        internal static DependencyObject GetParentByName(string name, FrameworkElement reference)
        {
            if (reference.Name != name)
            {
                var parent = VisualTreeHelper.GetParent(reference);
                GetParentByName(name, (FrameworkElement)parent);
                return null;
            }
            else
            {
                return reference;
            }
        }
        internal static bool IsScrollBarParent(DependencyObject reference)
        {
            if (!(reference is ScrollBar))
            {
                if (reference != null)
                {
                    var parent = VisualTreeHelper.GetParent(reference);
                    return IsScrollBarParent(parent);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
