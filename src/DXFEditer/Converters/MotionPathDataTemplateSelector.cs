using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DXFViewer
{
    class MotionPathDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement framework = container as FrameworkElement;
            MotionPath motionPath = item as MotionPath;
            DataTemplate dt = null;

            if (motionPath != null && framework != null)
            {
                if (motionPath.PathType == MotionPathType.线)
                {
                    dt = framework.FindResource("MotionLineViewTemplate") as DataTemplate;
                }
                else if (motionPath.PathType == MotionPathType.圆)
                {
                    dt = framework.FindResource("MotionCircleViewTemplate") as DataTemplate;
                }
                else if (motionPath.PathType == MotionPathType.弧)
                {
                    dt = framework.FindResource("MotionArcViewTemplate") as DataTemplate;
                }
            }

            return dt;
        }
    }
}
