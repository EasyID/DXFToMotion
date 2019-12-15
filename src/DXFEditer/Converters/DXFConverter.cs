using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using netDxf;
namespace DXFViewer
{
    public class DXFConverter
    {

        #region field

        DxfDocument dxfDocument;

        #endregion

        #region constructor

        public DXFConverter(string filePath)
        {
            dxfDocument = DxfDocument.Load(filePath);
        }

        #endregion

        #region method

        public List<MotionPath> ToMotionPath()
        {
            List<MotionPath> motionPaths = new List<MotionPath>();

            //Lines
            foreach (netDxf.Entities.Line line in dxfDocument.Lines)
            {
                motionPaths.Add(ConvertLine(line));
            }

            //Circles
            foreach (netDxf.Entities.Circle circle in dxfDocument.Circles)
            {
                motionPaths.Add(ConvertCircle(circle));
            }

            //Arcs
            foreach (netDxf.Entities.Arc arc in dxfDocument.Arcs)
            {
                motionPaths.Add(ConvertArc(arc));
            }

            //PolyLines
            foreach (netDxf.Entities.LwPolyline polyline in dxfDocument.LwPolylines)
            {
                motionPaths.AddRange(ConvertPolyline(polyline));
            }

            //Blocks
            foreach (netDxf.Blocks.Block block in dxfDocument.Blocks.Where(w => !w.Name.Contains("Space")))
            {
                motionPaths.AddRange(ConvertBlock(block));
            }

            return motionPaths;
        }

        private MotionLine ConvertLine(netDxf.Entities.Line line)
        {
            MotionLine path = new MotionLine()
            {
                PathType = MotionPathType.线,
                StartPoint = new Point(line.StartPoint.X, line.StartPoint.Y),
                EndPoint = new Point(line.EndPoint.X, line.EndPoint.Y)
            };
            return path;
        }

        private MotionArc ConvertCircle(netDxf.Entities.Circle circle)
        {
            MotionArc path = new MotionArc()
            {
                PathType = MotionPathType.圆,
                CenterPoint = new Point(circle.Center.X, circle.Center.Y),
                Radius = circle.Radius,
                StartAngle = 0,
                EndAngle = 360
            };
            return path;
        }

        private MotionArc ConvertArc(netDxf.Entities.Arc arc)
        {
            MotionArc path = new MotionArc()
            {
                PathType = MotionPathType.弧,
                CenterPoint = new Point(arc.Center.X, arc.Center.Y),
                Radius = arc.Radius,
                StartAngle = arc.StartAngle,
                EndAngle = arc.EndAngle,
                StartPoint = new Point()
                {
                    X = arc.Center.X + arc.Radius * Math.Cos(arc.StartAngle.ToRAD()),
                    Y = arc.Center.Y + arc.Radius * Math.Sin(arc.StartAngle.ToRAD())
                },
                EndPoint = new Point()
                {
                    X = arc.Center.X + arc.Radius * Math.Cos(arc.EndAngle.ToRAD()),
                    Y = arc.Center.Y + arc.Radius * Math.Sin(arc.EndAngle.ToRAD())
                }
            };
            return path;
        }

        private List<MotionPath> ConvertBlock(netDxf.Blocks.Block block)
        {
            List<MotionPath> blockSegments = new List<MotionPath>();

            foreach (netDxf.Entities.EntityObject entity in block.Entities)
            {
                if (entity is netDxf.Entities.Line) blockSegments.Add(ConvertLine(entity as netDxf.Entities.Line));
                else if (entity is netDxf.Entities.Circle) blockSegments.Add(ConvertCircle(entity as netDxf.Entities.Circle));
                else if (entity is netDxf.Entities.Arc) blockSegments.Add(ConvertArc(entity as netDxf.Entities.Arc));
            }

            return blockSegments;
        }

        private List<MotionPath> ConvertPolyline(netDxf.Entities.LwPolyline polyline)
        {
            List<MotionPath> polylineSegments = new List<MotionPath>();

            for (int i = 1; i < polyline.Vertexes.Count; i++)
            {
                MotionLine line = new MotionLine()
                {
                    PathType = MotionPathType.线,
                    StartPoint = new Point(polyline.Vertexes[i - 1].Position.X, polyline.Vertexes[i - 1].Position.Y),
                    EndPoint = new Point(polyline.Vertexes[i].Position.X, polyline.Vertexes[i].Position.Y)
                };
                polylineSegments.Add(line);
            }

            if (polyline.IsClosed)
            {
                MotionLine line = new MotionLine()
                {
                    PathType = MotionPathType.线,
                    StartPoint = new Point(polyline.Vertexes.Last().Position.X, polyline.Vertexes.Last().Position.Y),
                    EndPoint = new Point(polyline.Vertexes.First().Position.X, polyline.Vertexes.First().Position.Y)
                };
                polylineSegments.Add(line);
            }

            return polylineSegments;
        }

        #endregion
    }
}
