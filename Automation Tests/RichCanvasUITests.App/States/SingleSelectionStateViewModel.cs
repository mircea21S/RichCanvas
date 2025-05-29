using RichCanvasUITests.App.TestMocks;
using System;
using System.Windows.Input;

namespace RichCanvasUITests.App.States
{
    public class SingleSelectionStateViewModel
    {
        public MainWindowViewModel Parent { get; }

        private ICommand _addSingleSelectionItemsForTest;
        public ICommand AddSingleSelectionItemsForTest => _addSingleSelectionItemsForTest ??= new RelayCommand(AddTestSingleSelectionItems);

        private RelayCommand<Int64> _setSelectedItemCommand;
        public ICommand SetSelectedItemCommand => _setSelectedItemCommand ??= new RelayCommand<Int64>(SetSelectedItem);

        public SingleSelectionStateViewModel(MainWindowViewModel parent)
        {
            Parent = parent;
        }

        private void AddTestSingleSelectionItems()
        {
            foreach (var item in SingleSelectionStateDataMocks.SingleSelectionItems)
            {
                Parent.Items.Add(item);
            }
        }

        private void SetSelectedItem(Int64 index)
        {
            Parent.SelectedItem = Parent.Items[(int)index];
        }
    }
}
