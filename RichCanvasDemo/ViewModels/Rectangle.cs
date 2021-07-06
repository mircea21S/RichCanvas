using RichCanvasDemo.ViewModels.Base;

namespace RichCanvasDemo.ViewModels
{
    public class Rectangle : Drawable
    {
        public override Drawable Clone()
        {
            return new Rectangle
            {
                Height = this.Height,
                IsDraggable = this.IsDraggable,
                IsSelectable = this.IsSelectable,
                VisualProperties = new VisualProperties
                {
                    BorderColor = this.VisualProperties.BorderColor,
                    FillColor = this.VisualProperties.FillColor
                },
                Width = this.Width,
            };
        }
    }
}
