using System.Collections.Generic;

namespace RichCanvasUITests.App.TestMocks
{
    public static class SingleSelectionStateDataMocks
    {
        public static List<RichItemContainerModel> SingleSelectionItems =>
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
                Left = 90,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 250,
                Left = 150,
                Height = 50,
                Width = 50
            }
       ];
    }
}
