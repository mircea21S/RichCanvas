namespace RichCanvasUITests.App.TestMocks
{
    public static class DrawingStateDataMocks
    {
        public static RichItemContainerModel PositionedRectangleMockWithoutSize => new RichItemContainerModel
        {
            Top = 100,
            Left = 100
        };
        public static RichItemContainerModel ImmutablePositionedRectangleMockWithoutSize => new RichItemContainerModel
        {
            AllowScaleChangeToUpdatePosition = false,
            Top = 300,
            Left = 300
        };
        public static RichItemContainerModel DrawnRectangleMock => new RichItemContainerModel
        {
            Top = 100,
            Left = 100,
            Height = 100,
            Width = 100
        };
    }
}
