using Microsoft.Xaml.Behaviors;
using RichCanvas;
using RichCanvas.Helpers;
using RichCanvasDemo.ViewModels;
using RichCanvasDemo.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RichCanvasDemo.Helpers.Behaviors
{
    public class GroupBehavior : Behavior<System.Windows.Shapes.Rectangle>
    {
        RichItemContainer ItemContainer => VisualHelper.GetParentContainer(AssociatedObject);
        Group GroupDataContext => (Group)ItemContainer.DataContext;
        protected override void OnAttached()
        {
            ItemContainer.Host.DrawingEnded += Host_DrawingEnded;
        }

        private void Host_DrawingEnded(object sender, RoutedEventArgs e)
        {
            List<object> intersectedElements = ItemContainer.Host.GetElementsInArea(new Rect(ItemContainer.Left, ItemContainer.Top, ItemContainer.Width, ItemContainer.Height));
            GroupDataContext.SetGroupedElements(intersectedElements.OfType<Drawable>().Where(d => !(d is Group)).ToList());
            GroupDataContext.SetGroupSize();
        }

    }
}
