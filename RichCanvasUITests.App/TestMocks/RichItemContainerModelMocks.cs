using RichCanvasUITests.App.Models;
using System.Collections.Generic;

namespace RichCanvasUITests.App.TestMocks
{
    public static class RichItemContainerModelMocks
    {
        public static RichItemContainerModel PositionedRectangleMock => new RichItemContainerModel
        {
            Top = 100,
            Left = 100
        };
        public static RichItemContainerModel ImmutablePositionedRectangleMock => new RichItemContainerModel
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
        public static List<RichItemContainerModel> PositionedSelectableItemListMock => new()
        {
            new()
            {
                Top = 50,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new ()
            {
                Top = 100,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new ()
            {
                Top = 50,
                Left = 100,
                Height = 50,
                Width = 50
            },
            new ()
            {
                Top = 150,
                Left = 150,
                Height = 50,
                Width = 50
            },
            new Line
            {
                Top = 200,
                Left = 200,
                Width = 250,
                Height = 250
            },
            new Line
            {
                Top = 200,
                Left = 200,
                Width = 250,
                Height = 200
            }
        };
    }
}
