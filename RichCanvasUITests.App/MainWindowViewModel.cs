using RichCanvasUITests.App.States;
using RichCanvasUITests.App.TestMocks;
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

        private RelayCommand _resetViewportLocation;
        public ICommand ResetViewportLocation => _resetViewportLocation ??= new RelayCommand(PerformResetViewportLocation);

        private RelayCommand _setViewportLocationValue;
        public ICommand SetViewportLocationValue => _setViewportLocationValue ??= new RelayCommand(PerformSetViewportLocationValue);

        private RelayCommand _resetViewportZoomCommand;
        public ICommand ResetViewportZoomCommand => _resetViewportZoomCommand ??= new RelayCommand(ResetViewportZoom);

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

        private System.Windows.Point _viewportLocation;
        public System.Windows.Point ViewportLocation
        {
            get => _viewportLocation;
            set => SetProperty(ref _viewportLocation, value);
        }

        private System.Windows.Size _viewportSize;
        public System.Windows.Size ViewportSize
        {
            get => _viewportSize;
            set => SetProperty(ref _viewportSize, value);
        }

        private bool _enableGrid;
        public bool EnableGrid
        {
            get => _enableGrid;
            set => SetProperty(ref _enableGrid, value);
        }

        private bool _enableSnapping;
        public bool EnableSnapping
        {
            get => _enableSnapping;
            set => SetProperty(ref _enableSnapping, value);
        }

        public SingleSelectionStateViewModel SingleSelectionState { get; }
        public MultipleSelectionStateViewModel MultipleSelectionState { get; }
        public DrawingStateViewModel DrawingState { get; }

        public ZoomingViewModel ZoomingViewModel { get; }

        public MainWindowViewModel()
        {
            Items = [];
            SelectedItems = [];
            SingleSelectionState = new SingleSelectionStateViewModel(this);
            MultipleSelectionState = new MultipleSelectionStateViewModel(this);
            DrawingState = new DrawingStateViewModel(this);
            ZoomingViewModel = new ZoomingViewModel();
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

        private void PerformResetViewportLocation()
        {
            ViewportLocation = new System.Windows.Point(0, 0);
        }

        private void PerformSetViewportLocationValue()
        {
            ViewportLocation = PanningStateDataMocks.ViewportLocationMockValue;
        }

        private void ResetViewportZoom()
        {
            ZoomingViewModel.ViewportZoom = 1;
            ViewportLocation = new System.Windows.Point(0, 0);
        }
    }
}
