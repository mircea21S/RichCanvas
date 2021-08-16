using RichCanvasDemo.ViewModels.Base;
using RichCanvasDemo.ViewModels.Connections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace RichCanvasDemo.ViewModels
{
    public class Line : Drawable, IConnectable
    {
        private ObservableCollection<Drawable> _connections;
        private double _x2;
        private double _y2;

        public double X2
        {
            get => _x2;
            set => SetProperty(ref _x2, value);
        }

        public double Y2
        {
            get => _y2;
            set => SetProperty(ref _y2, value);
        }

        public ICollection<Drawable> Connections => _connections;

        public bool IsParent => Parent == null;

        public Drawable Parent { get; set; }

        public Line()
        {
            _connections = new ObservableCollection<Drawable>();
            _connections.CollectionChanged += ConnectionsChanged;
        }

        private void ConnectionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //connect before and after line
            }
        }
        protected override void OnLeftChanged(double delta)
        {
            foreach (Drawable connection in Connections)
            {
                //if it's selected is already moving
                if (!connection.IsSelected)
                {
                    connection.Left -= delta;
                }
            }
        }

        public override void OnDrawingEnded(Action<object> callback = default)
        {
            if (DirectionPoint.X < 1 && DirectionPoint.Y >= 1)
            {
                var line = new Line { Top = Top + Height, Left = Left };
                callback(line);
            }
            else if (DirectionPoint.X < 1 && DirectionPoint.Y < 1)
            {
                var line = new Line { Top = Top, Left = Left };
                callback(line);
            }
            else if (DirectionPoint.X >= 1 && DirectionPoint.Y < 1)
            {
                var line = new Line { Top = Top, Left = Left + Width };
                callback(line);
            }
            else
            {
                var line = new Line { Top = Top + Height, Left = Left + Width };
                callback(line);
            }
        }
    }
}
