namespace RichCanvasUIA.Client.IPC_Pipe
{
    public class PipeHandlerNames
    {
        public static class ItemsSource
        {
            public const string MoveFirstItemToTheEnd = "MoveFirstItemToTheEnd";
            public const string RemoveFirstItem = "RemoveFirstItem";
            public const string AddItemTopOutsideViewport = "AddItemTopOutsideViewport";
            public const string AddItemLeftOutsideViewport = "AddItemLeftOutsideViewport";
            public const string AddItemBottomOutsideViewport = "AddItemBottomOutsideViewport";
            public const string AddItemRightOutsideViewport = "AddItemRightOutsideViewport";
        }

        public static class Drawing
        {
            public const string AddEmptyRectangle = "AddEmptyRectangle";
            public const string AddEmptyLine = "AddEmptyLine";
            public const string DisableDrawingEndedCommandExecution = "DisableDrawingEndedCommandExecution";
            public const string EnableDrawingEndedCommandExecution = "EnableDrawingEndedCommandExecution";
            public const string AddImmutableRectangle = "AddImmutableDrawnRectangle";
            public const string AddPositionedRectangle = "AddPositionedRectangle";
            public const string AddDrawnRectangle = "AddDrawnRectangle";
        }

        public static class Selection
        {
            public const string AddSelectableItems = "AddSelectableItems";
            public const string AddConsecutiveItemsForRealTimeSelection = "AddConsecutiveItemsForRealTimeSelection";
        }
    }
}
