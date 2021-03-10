using RichCanvasDemo.ViewModels.Base;

namespace RichCanvasDemo.ViewModels
{
    public class Line : Drawable
    {
        private double _x2;

        public double X2
        {
            get => _x2;
            set => SetProperty(ref _x2, value);
        }
        private double _y2;

        public double Y2
        {
            get => _y2;
            set => SetProperty(ref _y2, value);
        }
    }
}
