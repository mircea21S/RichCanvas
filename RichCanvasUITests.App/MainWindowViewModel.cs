using RichCanvasUITests.App.States;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace RichCanvasUITests.App
{
    public class MainWindowViewModel : ObservableObject
    {
        public ObservableCollection<RichItemContainerModel> Items { get; }
        public ObservableCollection<RichItemContainerModel> SelectedItems { get; }

        private RichItemContainerModel _selectedItem;
        public RichItemContainerModel SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

        private RelayCommand _clearAllItemsCommand;
        public ICommand ClearAllItemsCommand => _clearAllItemsCommand ??= new RelayCommand(ClearAllItems);

        private RelayCommand<NotifyCollectionChangedAction> _updateItemsSourceCommand;
        public ICommand UpdateItemsSourceCommand => _updateItemsSourceCommand ??= new RelayCommand<NotifyCollectionChangedAction>(UpdateItemsSource);

        private RelayCommand _switchItemsPositionCommand;
        public ICommand SwitchItemsPositionCommand => _switchItemsPositionCommand ??= new RelayCommand(SwitchItemsPosition);

        private bool _realTimeSelectionEnabled;
        public bool RealTimeSelectionEnabled
        {
            get => _realTimeSelectionEnabled;
            set => SetProperty(ref _realTimeSelectionEnabled, value);
        }

        private bool _canSelectMultipleItems;
        public bool CanSelectMultipleItems
        {
            get => _canSelectMultipleItems;
            set => SetProperty(ref _canSelectMultipleItems, value);
        }

        private bool _realTimeDraggingEnabled;
        public bool RealTimeDraggingEnabled
        {
            get => _realTimeDraggingEnabled;
            set => SetProperty(ref _realTimeDraggingEnabled, value);
        }

        public SingleSelectionStateViewModel SingleSelectionState { get; }
        public MultipleSelectionStateViewModel MultipleSelectionState { get; }
        public DrawingStateViewModel DrawingState { get; }

        public MainWindowViewModel()
        {
            Items = [];
            SelectedItems = [];
            SingleSelectionState = new SingleSelectionStateViewModel(this);
            MultipleSelectionState = new MultipleSelectionStateViewModel(this);
            DrawingState = new DrawingStateViewModel(this);
        }

        private void ClearAllItems()
        {
            Items.Clear();
        }

        private void UpdateItemsSource(NotifyCollectionChangedAction actionType)
        {
            if (actionType == NotifyCollectionChangedAction.Move)
            {
                Items.Move(0, Items.Count - 1);
            }
            else if (actionType == NotifyCollectionChangedAction.Remove)
            {
                Items.RemoveAt(0);
            }
        }

        private void SwitchItemsPosition()
        {
            // changable method to test stuff
            Items[0] = Items[1];
        }
    }
}
