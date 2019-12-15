using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public static class DoubleHelper
    {

        /// <summary>
        /// 由角度转换为弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double ToRAD(this double angle)
        {
            return angle * (Math.PI / 180);
        }

        /// <summary>
        /// 由弧度转换为角度
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double ToAngle(this double rad)
        {
            return rad * (180 / Math.PI);
        }

    }
}
