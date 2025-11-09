namespace RichCanvasUIA.Client.IPC_Pipe
{
    public class PipeHandlerNames
    {
        public static class ItemsSource
        {
            public const string MoveFirstItemToTheEnd = "MoveFirstItemToTheEnd";
            public const string RemoveFirstItem = "RemoveFirstItem";
        }

        public static class Drawing
        {
            public const string AddEmptyRectangle = "AddEmptyRectangle";
            public const string AddEmptyLine = "AddEmptyLine";
            public const string DisableDrawingEndedCommandExecution = "DisableDrawingEndedCommandExecution";
            public const string EnableDrawingEndedCommandExecution = "EnableDrawingEndedCommandExecution";
            public const string AddImmutableRectangle = "AddImmutableDrawnRectangle";
        }
    }
}
