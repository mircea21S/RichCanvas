using System;

namespace RichCanvas.Automation.ControlInformations
{
    public class RichItemContainerData
    {
        public Type? DataContextType { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public bool IsSelected { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
    }
}
