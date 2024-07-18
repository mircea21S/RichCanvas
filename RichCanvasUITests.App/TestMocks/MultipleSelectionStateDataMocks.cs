using System.Collections.Generic;

namespace RichCanvasUITests.App.TestMocks
{
    public static class MultipleSelectionStateDataMocks
    {
        public static List<RichItemContainerModel> PositionedSelectableItemsListMock =>
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
    }
}
