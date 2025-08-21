﻿using RichCanvasTestApp.ViewModels.Base;
using System.Collections.Generic;
using System.Windows;

namespace RichCanvasTestApp.ViewModels
{
    public class Bezier : Drawable
    {
        private Point _point1;
        private Point _point2;
        private Point _point3;

        public List<Point> Points { get; set; }
        public Point Point1
        {
            get => _point1;
            set => SetProperty(ref _point1, value);
        }

        public Point Point2
        {
            get => _point2;
            set => SetProperty(ref _point2, value);
        }

        public Point Point3
        {
            get => _point3;
            set => SetProperty(ref _point3, value);
        }

        public Bezier()
        {
            SetPoints();
        }

        protected override void OnWidthUpdated()
        {
            SetPoints();
        }

        protected override void OnHeightUpdated()
        {
            SetPoints();
        }

        public void SetPoints()
        {
            Point1 = new Point(Width - 20, 0);
            Point2 = new Point(Width - 10, 0);
            Point3 = new Point(Width, Height);
        }
    }
}
