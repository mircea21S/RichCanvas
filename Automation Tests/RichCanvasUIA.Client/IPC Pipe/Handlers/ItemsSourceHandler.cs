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

                case PipeHandlerNames.ItemsSource.AddItemTopOutsideViewport:
                    _mainWindowDataContext.Items.Add(new RichItemContainerModel
                    {
                        Top = _mainWindowDataContext.ViewportLocation.Y - 100,
                        Left = _mainWindowDataContext.ViewportLocation.X + 10,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.AddItemLeftOutsideViewport:
                    _mainWindowDataContext.Items.Add(new RichItemContainerModel
                    {
                        Top = _mainWindowDataContext.ViewportLocation.Y + 100,
                        Left = _mainWindowDataContext.ViewportLocation.X - 100,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.AddItemBottomOutsideViewport:
                    _mainWindowDataContext.Items.Add(new RichItemContainerModel
                    {
                        Top = _mainWindowDataContext.ViewportSize.Height,
                        Left = _mainWindowDataContext.ViewportLocation.X + 10,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.AddItemRightOutsideViewport:
                    _mainWindowDataContext.Items.Add(new RichItemContainerModel
                    {
                        Top = _mainWindowDataContext.ViewportLocation.Y + 10,
                        Left = _mainWindowDataContext.ViewportSize.Width,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.ClearAllItems:
                    _mainWindowDataContext.Items.Clear();
                    break;
            }
        }
    }
}
