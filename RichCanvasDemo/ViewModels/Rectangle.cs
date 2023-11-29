using Newtonsoft.Json;
using RichCanvasDemo.ViewModels.Base;
using RichCanvasDemo.ViewModels.Grouping;
using System;

namespace RichCanvasDemo.ViewModels
{
    public class Rectangle : Drawable, ICloneable, IGroupable
    {
        private Group _group;

        public Group Group { get => _group; set => IsDraggable = value != null; }

        public object Clone() => JsonConvert.DeserializeObject<Rectangle>(JsonConvert.SerializeObject(this));
    }
}
