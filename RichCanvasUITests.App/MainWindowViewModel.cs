using RichCanvasUITests.App.TestMocks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RichCanvasUITests.App
{
    public class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<RichItemContainerModel> Items { get; }

        private RichItemContainerModel _selectedItem;
        public RichItemContainerModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<RichItemContainerModel>();
        }

        private RelayCommand<bool> _addPositionedRectangleCommand;

        public ICommand AddPositionedRectangleCommand
        {
            get
            {
                if (_addPositionedRectangleCommand == null)
                {
                    _addPositionedRectangleCommand = new RelayCommand<bool>(AddPositionedRectangle);
                }

                return _addPositionedRectangleCommand;
            }
        }

        private void AddPositionedRectangle(bool isImmutable)
        {
            Items.Add(isImmutable ? RichItemContainerModelMocks.ImmutablePositionedRectangleMock : RichItemContainerModelMocks.PositionedRectangleMock);
        }

        private RelayCommand _clearAllItemsCommand;

        public ICommand ClearAllItemsCommand
        {
            get
            {
                if (_clearAllItemsCommand == null)
                {
                    _clearAllItemsCommand = new RelayCommand(ClearAllItems);
                }

                return _clearAllItemsCommand;
            }
        }

        private void ClearAllItems()
        {
            Items.Clear();
        }

        private RelayCommand _addDrawnRectangleCommand;

        public ICommand AddDrawnRectangleCommand
        {
            get
            {
                if (_addDrawnRectangleCommand == null)
                {
                    _addDrawnRectangleCommand = new RelayCommand(AddDrawnRectangle);
                }

                return _addDrawnRectangleCommand;
            }
        }

        private void AddDrawnRectangle()
        {
            Items.Add(RichItemContainerModelMocks.DrawnRectangleMock);
        }

        private RelayCommand _addEmptyRectangleCommand;

        public ICommand AddEmptyRectangleCommand
        {
            get
            {
                if (_addEmptyRectangleCommand == null)
                {
                    _addEmptyRectangleCommand = new RelayCommand(AddEmptyRectangle);
                }

                return _addEmptyRectangleCommand;
            }
        }

        private void AddEmptyRectangle()
        {
            Items.Add(new RichItemContainerModel());
        }

        private RelayCommand _deleteItemCommand;

        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand(DeleteItem);
                }

                return _deleteItemCommand;
            }
        }

        private void DeleteItem()
        {
            // testing purpose: delete frist item, before drawing anything (entering DrawingState)
            Items.Remove(Items[1]);
            Items.Remove(Items[2]);
        }

        private RelayCommand _testCommand;

        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new RelayCommand(Test);
                }

                return _testCommand;
            }
        }

        private void Test()
        {
            // changable method to test stuff
            Items[0] = Items[1];
        }
    }
}
