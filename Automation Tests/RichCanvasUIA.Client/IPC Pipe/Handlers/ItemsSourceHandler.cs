namespace RichCanvasUIA.Client.IPC_Pipe.Handlers
{
    public class ItemsSourceHandler : IPipeHandler
    {
        private readonly MainWindowViewModel _mainWindowDataContext;

        public ItemsSourceHandler(MainWindowViewModel mainWindowDataContext)
        {
            _mainWindowDataContext = mainWindowDataContext;
        }

        public void Process(string pipeDataName)
        {
            switch (pipeDataName)
            {
                case PipeHandlerNames.ItemsSource.RemoveFirstItem:
                    _mainWindowDataContext.Items.RemoveAt(0);
                    break;

                case PipeHandlerNames.ItemsSource.MoveFirstItemToTheEnd:
                    _mainWindowDataContext.Items.Move(0, _mainWindowDataContext.Items.Count - 1);
                    break;
            }
        }
    }
}
