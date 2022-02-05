using RichCanvasDemo.ViewModels.Base;
using RichCanvasDemo.ViewModels.Grouping;
using System.Collections.Generic;
using System.Linq;

namespace RichCanvasDemo.ViewModels
{
    public class Group : Drawable
    {
        public List<Drawable> Elements { get; private set; }

        protected override void OnLeftChanged(double delta) => Elements.ForEach(e => e.Left -= delta);
        protected override void OnTopChanged(double delta) => Elements.ForEach(e => e.Top -= delta);

        public void SetGroupedElements(params Drawable[] elements)
        {
            Elements = new List<Drawable>();
            elements.ToList().ForEach(e => ((IGroupable)e).Group = this);
            Elements.AddRange(elements);
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
