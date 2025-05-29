using RichCanvasUITests.App;
using RichCanvasUITests.App.TestMocks;
using System;
using System.Linq;
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

        private RelayCommand<Int64> _addSelectableItemsCommmand;
        public ICommand AddSelectableItemsCommmand => _addSelectableItemsCommmand ??= new RelayCommand<Int64>(AddSelectableItems);

        private void AddSelectableItems(Int64 collectionType)
        {
            if (collectionType == 1)
            {
                foreach (var item in MultipleSelectionStateDataMocks.MultipleSelectionCloselyPositionedDummyItems)
                {
                    Parent.Items.Add(item);
                }
            }
            else if (collectionType == 2)
            {
                foreach (var item in MultipleSelectionStateDataMocks.MultipleSelectionDummyItems)
                {
                    Parent.Items.Add(item);
                }
            }
        }

        private RelayCommand<Int64> _addSelectedItemsCommand;
        public ICommand AddSelectedItemsCommand => _addSelectedItemsCommand ??= new RelayCommand<Int64>(AddSelectedItems);

        private void AddSelectedItems(Int64 itemsCount)
        {
            var toSelect = Parent.Items.Skip(Parent.SelectedItems.Count).Take((int)itemsCount);
            foreach (var item in toSelect)
            {
                Parent.SelectedItems.Add(item);
            }
        }

        private RelayCommand _selectAllItemsCommand;
        public ICommand SelectAllItemsCommand => _selectAllItemsCommand ??= new RelayCommand(SelectAllItems);

        private void SelectAllItems()
        {
            foreach (var item in Parent.Items)
            {
                Parent.SelectedItems.Add(item);
            }
        }
    }
}
