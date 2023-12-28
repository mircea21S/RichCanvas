using RichCanvasTestApp.ViewModels.Base;
using RichCanvasTestApp.ViewModels.Connections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace RichCanvasTestApp.ViewModels
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

        public override void OnDrawingEnded(Point drawEndedMousePosition, Action<object> callback = default)
        {
            var createdLine = new Line
            {
                Top = drawEndedMousePosition.Y,
                Left = drawEndedMousePosition.X
            };

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
            //TODO: offsetX is actually the new Left value, update this events and the sent data
            //if (Parent != null)
            //{
            //    if (!Parent.IsSelected)
            //    {
            //        Parent.Left -= offsetX;
            //        Parent.Top -= offsetY;
            //    }
            //    foreach (Drawable connection in ((IConnectable)Parent).Connections)
            //    {
            //        //if it's selected is already moving
            //        if (!connection.IsSelected)
            //        {
            //            connection.Left -= offsetX;
            //            connection.Top -= offsetY;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (Drawable connection in Connections)
            //    {
            //        //if it's selected is already moving
            //        if (!connection.IsSelected)
            //        {
            //            connection.Left -= offsetX;
            //            connection.Top -= offsetY;
            //        }
            //    }
            //}
        }
    }
}
