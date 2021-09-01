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
            Move(delta);
        }

        public override void OnDrawingEnded(Action<object> callback = default)
        {
            Line line;
            if (DirectionPoint.X < 1 && DirectionPoint.Y >= 1)
            {
                line = new Line { Top = Top + Height, Left = Left };
            }
            else if (DirectionPoint.X < 1 && DirectionPoint.Y < 1)
            {
                line = new Line { Top = Top, Left = Left };
            }
            else if (DirectionPoint.X >= 1 && DirectionPoint.Y < 1)
            {
                line = new Line { Top = Top, Left = Left + Width };
            }
            else
            {
                line = new Line { Top = Top + Height, Left = Left + Width };
            }

            if (Parent == null)
            {
                line.Parent = this;
                Connections.Add(line);
            }
            else
            {
                line.Parent = Parent;
                ((IConnectable)Parent).Connections.Add(line);
            }

            callback(line);
        }

        public void Move(double offsetX = 0, double offsetY = 0)
        {
            if (Parent != null)
            {
                foreach (Drawable connection in ((IConnectable)Parent).Connections)
                {
                    //if it's selected is already moving
                    if (!connection.IsSelected)
                    {
                        connection.Left -= offsetX;
                    }
                }
            }
            else
            {
                foreach (Drawable connection in Connections)
                {
                    //if it's selected is already moving
                    if (!connection.IsSelected)
                    {
                        connection.Left -= offsetX;
                    }
                }
            }
        }
    }
}
