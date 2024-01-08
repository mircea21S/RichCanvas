using RichCanvasTestApp.ViewModels;

namespace RichCanvasTestApp.ViewModelsMocks
{
    public static class RectangleMock
    {
        public static Rectangle FakePositionedRectangle => new Rectangle
        {
            Left = 100,
            Top = 100
        };

        public static Rectangle FakeImmutableRectangleWithTopAndLeftSet => new Rectangle
        {
            AllowScaleChangeToUpdatePosition = false,
            Top = 300,
            Left = 300
        };
    }
}
