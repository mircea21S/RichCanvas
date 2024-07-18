using RichCanvasUITests.App.TestMocks;
using System.Windows.Input;

namespace RichCanvasUITests.App.States
{
    public class MultipleSelectionStateViewModel
    {
        public MainWindowViewModel Parent { get; }

        public MultipleSelectionStateViewModel(MainWindowViewModel parent)
        {
            Parent = parent;
        }

        private RelayCommand _addSelectableItemsCommmand;
        public ICommand AddSelectableItemsCommmand => _addSelectableItemsCommmand ??= new RelayCommand(AddSelectableItems);

        private void AddSelectableItems()
        {
            foreach (var item in MultipleSelectionStateDataMocks.PositionedSelectableItemsListMock)
            {
                Parent.Items.Add(item);
            }
        }
    }
}
