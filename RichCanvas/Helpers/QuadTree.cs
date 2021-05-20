using System.Collections.Generic;
using System.Windows;

namespace RichCanvas.Helpers
{
    internal class QuadTree
    {
        private Rect _bounds;

        internal Rect Bounds => _bounds;
        internal List<RichItemContainer> Nodes { get; set; }
        internal QuadTree TopLeft { get; set; }
        internal QuadTree TopRight { get; set; }
        internal QuadTree BottomLeft { get; set; }
        internal QuadTree BottomRight { get; set; }
        public QuadTree(Rect bounds)
        {
            _bounds = bounds;
        }
        internal void Insert(RichItemContainer node)
        {

        }
        internal void SubDivide() { }
        //internal List<RichItemContainer> QueryRange(Rect range)
        //{

        //}
    }
}
