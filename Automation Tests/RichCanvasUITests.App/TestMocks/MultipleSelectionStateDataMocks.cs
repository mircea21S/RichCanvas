using System.Collections.Generic;

namespace RichCanvasUITests.App.TestMocks
{
    public static class MultipleSelectionStateDataMocks
    {
        public static List<RichItemContainerModel> MultipleSelectionCloselyPositionedDummyItems =>
        [
            new()
            {
                Top = 50,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 150,
                Left = 150,
                Height = 50,
                Width = 50
            }
        ];

        public static List<RichItemContainerModel> MultipleSelectionDummyItems =>
        [
            new()
            {
                Top = 50,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 150,
                Left = 150,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 250,
                Left = 250,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 80,
                Left = 150,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 100,
                Left = 300,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 300,
                Left = 400,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 180,
                Left = 230,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 450,
                Left = 450,
                Height = 50,
                Width = 50
            }
        ];
    }
}
