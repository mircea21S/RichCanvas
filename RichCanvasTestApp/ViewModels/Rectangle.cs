using Newtonsoft.Json;
using RichCanvasTestApp.ViewModels.Base;
using RichCanvasTestApp.ViewModels.Grouping;
using System;

namespace RichCanvasTestApp.ViewModels
{
    public class Rectangle : Drawable, ICloneable, IGroupable
    {
        private Group _group;

        public Group Group { get => _group; set => IsDraggable = value != null; }

        public object Clone() => JsonConvert.DeserializeObject<Rectangle>(JsonConvert.SerializeObject(this));
    }
}
