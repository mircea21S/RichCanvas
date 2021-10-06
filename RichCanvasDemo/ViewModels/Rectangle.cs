using RichCanvasDemo.ViewModels.Base;
using System;
using System.Text.Json;

namespace RichCanvasDemo.ViewModels
{
    public class Rectangle : Drawable, ICloneable
    {
        public object Clone() => JsonSerializer.Deserialize<Rectangle>(JsonSerializer.Serialize(this));
    }
}
