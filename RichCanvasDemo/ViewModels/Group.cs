using RichCanvasDemo.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RichCanvasDemo.ViewModels
{
    public class Group : Drawable
    {
        public List<Drawable> Elements { get; private set; }

        public Group()
        {
            IsDraggable = false;
            HasCustomBehavior = true;
        }

        public void SetGroupedElements(List<Drawable> elements)
        {
            Elements = new List<Drawable>();
            Elements.AddRange(elements);
            Console.WriteLine(Elements.Count);
        }

        internal void SetGroupSize()
        {
            if (Elements.Count > 0)
            {
                Left = Elements.Min(d => d.Left);
                Top = Elements.Min(d => d.Top);
                Width = Elements.Max(d => d.Left + d.Width) - Left;
                Height = Elements.Max(d => d.Top + d.Height) - Top;
            }
        }
    }
}
