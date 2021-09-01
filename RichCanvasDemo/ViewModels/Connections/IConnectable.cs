using RichCanvasDemo.ViewModels.Base;
using System.Collections.Generic;

namespace RichCanvasDemo.ViewModels.Connections
{
    public interface IConnectable
    {
        ICollection<Drawable> Connections { get; }

        bool IsParent { get; }

        Drawable Parent { get; set; }

        void Move(double offsetX, double offsetY);
    }
}
