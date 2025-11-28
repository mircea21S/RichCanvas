using RichCanvasUIA.Client.TestMocks;

namespace RichCanvasUIA.Client.IPC_Pipe.Handlers
{
    public class SelectionHandler : IPipeHandler
    {
        private readonly MainWindowViewModel _mainWindowDataContext;

        public SelectionHandler(MainWindowViewModel mainWindowDataContext)
        {
            _mainWindowDataContext = mainWindowDataContext;
        }

        public void Process(string pipeDataName)
        {
            if (pipeDataName == PipeHandlerNames.Selection.AddSelectableItems)
            {
                foreach (RichItemContainerModel item in MultipleSelectionStateDataMocks.MultipleSelectionDummyItems)
                {
                    _mainWindowDataContext.Items.Add(item);
                }
            }
            else if (pipeDataName == PipeHandlerNames.Selection.AddConsecutiveItemsForRealTimeSelection)
            {
                foreach (RichItemContainerModel item in MultipleSelectionStateDataMocks.MultipleSelectionCloselyPositionedDummyItems)
                {
                    _mainWindowDataContext.Items.Add(item);
                }
            }
        }
    }
}
