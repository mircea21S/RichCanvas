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

        private RelayCommand<bool> addPositionedRectangleCommand;

        public ICommand AddPositionedRectangleCommand
        {
            get
            {
                if (addPositionedRectangleCommand == null)
                {
                    addPositionedRectangleCommand = new RelayCommand<bool>(AddPositionedRectangle);
                }

                return addPositionedRectangleCommand;
            }
        }

        private void AddPositionedRectangle(bool isImmutable)
        {
            Items.Add(isImmutable ? RichItemContainerModelMocks.ImmutablePositionedRectangleMock : RichItemContainerModelMocks.PositionedRectangleMock);
        }

        private RelayCommand clearAllItemsCommand;

        public ICommand ClearAllItemsCommand
        {
            get
            {
                if (clearAllItemsCommand == null)
                {
                    clearAllItemsCommand = new RelayCommand(ClearAllItems);
                }

                return clearAllItemsCommand;
            }
        }

        private void ClearAllItems()
        {
            Items.Clear();
        }

        private RelayCommand addDrawnRectangleCommand;

        public ICommand AddDrawnRectangleCommand
        {
            get
            {
                if (addDrawnRectangleCommand == null)
                {
                    addDrawnRectangleCommand = new RelayCommand(AddDrawnRectangle);
                }

                return addDrawnRectangleCommand;
            }
        }

        private void AddDrawnRectangle()
        {
            Items.Add(RichItemContainerModelMocks.DrawnRectangleMock);
        }

        private RelayCommand addEmptyRectangleCommand;

        public ICommand AddEmptyRectangleCommand
        {
            get
            {
                if (addEmptyRectangleCommand == null)
                {
                    addEmptyRectangleCommand = new RelayCommand(AddEmptyRectangle);
                }

                return addEmptyRectangleCommand;
            }
        }

        private void AddEmptyRectangle()
        {
            Items.Add(new RichItemContainerModel());
        }
    }
}
