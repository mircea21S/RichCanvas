using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas.Helpers
{
    /// <summary>
    /// Helper wrapper on <see cref="VisualTreeHelper"/>
    /// </summary>
    public static class VisualHelper
    {
        /// <summary>
        /// Gets <see cref="RichItemContainer"/> parent of <paramref name="d"/>
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static RichItemContainer GetParentContainer(DependencyObject d)
        {
            if (d is RichItemContainer container)
            {
                return container;
            }
            var parent = VisualTreeHelper.GetParent(d);
            while (!(parent is RichItemContainer))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return (RichItemContainer)parent;
        }

        /// <summary>
        /// Checks whether <paramref name="reference"/> has <see cref="ScrollBar"/> parent
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static bool HasScrollBarParent(DependencyObject reference)
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
