﻿using RichCanvasUITests.App.Models;
using RichCanvasUITests.App.TestMocks;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private RelayCommand<Type> _addEmptyItemCommand;

        public ICommand AddEmptyItemCommand
        {
            get
            {
                if (_addEmptyItemCommand == null)
                {
                    _addEmptyItemCommand = new RelayCommand<Type>(AddEmptyRectangle);
                }

                return _addEmptyItemCommand;
            }
        }

        private void AddEmptyRectangle(Type itemType)
        {
            if (itemType == typeof(RichItemContainerModel))
            {
                Items.Add(new RichItemContainerModel());
            }
            else if (itemType == typeof(Line))
            {
                Items.Add(new Line());
            }
        }

        private RelayCommand<NotifyCollectionChangedAction> _updateItemsSourceCommand;

        public ICommand UpdateItemsSourceCommand
        {
            get
            {
                if (_updateItemsSourceCommand == null)
                {
                    _updateItemsSourceCommand = new RelayCommand<NotifyCollectionChangedAction>(UpdateItemsSource);
                }

                return _updateItemsSourceCommand;
            }
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
