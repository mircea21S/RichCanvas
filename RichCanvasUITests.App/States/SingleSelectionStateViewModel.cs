using RichCanvasUITests.App.TestMocks;
using System.Windows.Input;

namespace RichCanvasUITests.App.States
{
    public class SingleSelectionStateViewModel
    {
        public MainWindowViewModel Parent { get; }

        private ICommand _addSingleSelectionItemsForTest;
        public ICommand AddSingleSelectionItemsForTest => _addSingleSelectionItemsForTest ??= new RelayCommand(AddTestSingleSelectionItems);

        public SingleSelectionStateViewModel(MainWindowViewModel parent)
        {
            Parent = parent;
        }

        private void AddTestSingleSelectionItems()
        {
            foreach (var item in SingleSelectionStateDataMocks.SingleSelectionRealTimeDragTestItems)
            {
                Parent.Items.Add(item);
            }
        }
    }
}
