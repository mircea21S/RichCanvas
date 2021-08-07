using RichCanvasDemo.ViewModels.Base;

namespace RichCanvasDemo.ViewModels
{
    public class Rectangle : Drawable
    {
        public override Drawable Clone()
        {
            return Clone<Rectangle>();
        }
    }
}
