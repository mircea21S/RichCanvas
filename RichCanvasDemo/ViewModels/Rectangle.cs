using RichCanvasDemo.ViewModels.Base;
using RichCanvasDemo.ViewModels.Grouping;
using System;
using System.Text.Json;

namespace RichCanvasDemo.ViewModels
{
    public class Rectangle : Drawable, ICloneable, IGroupable
    {
        private Group _group;

        public Group Group { get => _group; set => IsDraggable = value != null; }

        public object Clone() => JsonSerializer.Deserialize<Rectangle>(JsonSerializer.Serialize(this));
    }
}
