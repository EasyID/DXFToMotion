using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
namespace DXFViewer
{
    public class MotionPathConverter
    {
        public MotionPathConverter() { }

        public double ScaleX { get; set; } = 96 / 25.4;

        public double ScaleY { get; set; } = 96 / 25.4;

        public Path CreatFromLine(MotionLine line)
        {
            Path path = new Path();

            path.Data = new LineGeometry(
                new Point(line.StartPoint.X * ScaleX, line.StartPoint.Y * ScaleY),
                new Point(line.EndPoint.X * ScaleX, line.EndPoint.Y * ScaleY)
                );

            return path;
        }

        public Path CreatFromCircle(MotionArc circle)
        {
            Path path = new Path();

            path.Data = new EllipseGeometry(
                new Point(circle.CenterPoint.X * ScaleX, circle.CenterPoint.Y * ScaleY),
                circle.Radius * ScaleX,
                circle.Radius * ScaleY
                );

            return path;
        }

        public Path CreatFromArc(MotionArc arc)
        {
            Path path = new Path();

            Point startPoint = new Point()
            {
                X = arc.StartPoint.X * ScaleX,
                Y = arc.StartPoint.Y * ScaleY
            };

            Point endPoint = new Point()
            {
                X = arc.EndPoint.X * ScaleX,
                Y = arc.EndPoint.Y * ScaleY
            };

            Point centerPoint = new Point()
            {
                X = arc.CenterPoint.X * ScaleX,
                Y = arc.CenterPoint.Y * ScaleY
            };

            path.Data = new PathGeometry(new PathFigureCollection()
            {
                new PathFigure(
                    startPoint,
                    new PathSegmentCollection()
                    {
                        new ArcSegment(
                            endPoint,
                            new Size(arc.Radius * ScaleX, arc.Radius * ScaleY),
                            arc.EndAngle - arc.StartAngle,
                            (endPoint.X - startPoint.X) * (centerPoint.Y - startPoint.Y) - (endPoint.Y - startPoint.Y) * (centerPoint.X - startPoint.X) > 0 ? false : true,
                            SweepDirection.Clockwise,
                            true)
                    },
                    false
                    )
            });

            return path;
        }

    }
}
