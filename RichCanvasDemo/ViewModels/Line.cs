using RichCanvasDemo.ViewModels.Base;
using RichCanvasDemo.ViewModels.Connections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
        }

        protected override void OnLeftChanged(double delta)
        {
            Move(delta);
        }
        protected override void OnTopChanged(double delta)
        {
            Move(0, delta);
        }

        public override void OnDrawingEnded(Action<object> callback = default)
        {
            Line createdLine = Scale.X < 1 && Scale.Y >= 1
                ? new Line { Top = Top + Height, Left = Left }
                : Scale.X < 1 && Scale.Y < 1
                    ? new Line { Top = Top, Left = Left }
                    : Scale.X >= 1 && Scale.Y < 1
                                    ? new Line { Top = Top, Left = Left + Width }
                                    : new Line { Top = Top + Height, Left = Left + Width };
            if (Parent == null)
            {
                createdLine.Parent = this;
                Connections.Add(createdLine);
            }
            else
            {
                createdLine.Parent = Parent;
                ((IConnectable)Parent).Connections.Add(createdLine);
            }

            callback(createdLine);
        }

        public void Move(double offsetX = 0, double offsetY = 0)
        {
            if (Parent != null)
            {
                if (!Parent.IsSelected)
                {
                    Parent.Left -= offsetX;
                    Parent.Top -= offsetY;
                }
                foreach (Drawable connection in ((IConnectable)Parent).Connections)
                {
                    //if it's selected is already moving
                    if (!connection.IsSelected)
                    {
                        connection.Left -= offsetX;
                        connection.Top -= offsetY;
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
                        connection.Top -= offsetY;
                    }
                }
            }
        }
    }
}
