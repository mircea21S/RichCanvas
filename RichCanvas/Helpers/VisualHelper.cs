using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas.Helpers
{
    internal static class VisualHelper
    {
        internal static bool HasAdornerThumbParent(DependencyObject reference)
        {
            if (!(reference is Thumb))
            {
                if (reference != null)
                {
                    var parent = VisualTreeHelper.GetParent(reference);
                    if (parent is ScrollViewer)
                    {
                        return false;
                    }
                    return HasAdornerThumbParent(parent);
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
        internal static RichItemContainer GetNeedLineBoundingBoxContainer(DependencyObject d)
        {
            var parent = VisualTreeHelper.GetParent(d);
            while (!(parent is RichItemContainer))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return (RichItemContainer)parent;
        }
        internal static RichItemContainer GetParentContainer(DependencyObject d)
        {
            var parent = VisualTreeHelper.GetParent(d);
            while (!(parent is RichItemContainer))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return (RichItemContainer)parent;
        }
        internal static bool HasScrollBarParent(DependencyObject reference)
        {
            if (!(reference is ScrollBar))
            {
                if (reference != null)
                {
                    var parent = VisualTreeHelper.GetParent(reference);
                    if (parent is ScrollViewer)
                    {
                        return false;
                    }
                    return HasScrollBarParent(parent);
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
