using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public class MotionPath
    {
        public MotionPathType PathType { get; set; } = MotionPathType.线;

        public MotionPathDirection PathDirection { get; set; } = MotionPathDirection.起点至终点;
    }
}
