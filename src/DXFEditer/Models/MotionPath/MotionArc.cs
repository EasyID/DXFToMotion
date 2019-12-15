using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXFViewer
{
    public class MotionArc : MotionPath
    {
        public double StartAngle { get; set; }

        public double EndAngle { get; set; }

        public double Radius { get; set; }

        public Point CenterPoint { get; set; }

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
    }
}
