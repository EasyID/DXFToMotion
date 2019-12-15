using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Caliburn.Micro;
using Microsoft.Win32;
namespace DXFViewer
{
    public class MainViewModel : PropertyChangedBase
    {
        #region field

        private readonly IEventAggregator _eventAggregator;

        private OpenFileDialog openFileDialog;

        private MotionPathConverter motionPathConverter = new MotionPathConverter();
        private DXFConverter dxfConverter;

        private Point startPosition;
        private Rectangle rect = null;
        private double scaleValue = 1;
        private bool isSelecting;

        #endregion

        #region property

        private Dictionary<Path, MotionPath> ShapeDic { get; set; } = new Dictionary<Path, MotionPath>();

        /// <summary>
        /// X方向缩放
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// Y方向缩放
        /// </summary>
        public double ScaleY { get; set; }

        /// <summary>
        /// 控件 WorkStepList 的选中项
        /// </summary>
        public WorkStep SelectWorkStep { get; set; }

        /// <summary>
        /// 控件 PathCollection 的选中项
        /// </summary>
        public MotionPath SelectMotionPath { get; set; }

        /// <summary>
        /// 控件 WorkStepList 的选中项
        /// </summary>
        public ObservableCollection<Shape> ChartChildren { get; set; } = new ObservableCollection<Shape>();

        #endregion

        #region constructor

        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            openFileDialog = new OpenFileDialog() { InitialDirectory = @"C:\Document\Instance\Desay\DXFToMotion\Example", Filter = "DXF文件|*.dxf" };
        }

        #endregion

        #region bindingMethod

        public void OpenFile()
        {
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    GlobalParam.Instance.LoadFilePath = openFileDialog.FileName;

                    dxfConverter = new DXFConverter(openFileDialog.FileName);

                    CreatView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"文件加载失败:{ex.ToString()}");
                }
            }
        }


        public void ChartMouseLeftButtonDown(Canvas chart, MouseButtonEventArgs e)
        {
            startPosition = e.GetPosition(chart);
            isSelecting = true;
            chart.Focus();
        }

        public void ChartMouseMove(Canvas chart, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isSelecting)
            {
                Point currentPosition = e.GetPosition(chart);

                if (rect == null)
                {
                    rect = new Rectangle() { Style = FindResource<Style>("SelectRectStyle") };
                    ChartChildren.Add(rect);
                }

                rect.Width = Math.Abs(currentPosition.X - startPosition.X);
                rect.Height = Math.Abs(currentPosition.Y - startPosition.Y);

                Canvas.SetLeft(rect, Math.Min(currentPosition.X, startPosition.X));
                Canvas.SetTop(rect, Math.Min(currentPosition.Y, startPosition.Y));
            }
        }

        public void ChartMouseLeftButtonUp(Canvas chart, MouseButtonEventArgs e)
        {
            if (isSelecting)
            {
                Point endPosition = e.GetPosition(chart);

                VisualTreeHelper.HitTest(chart, null,
                                        r =>
                                        {
                                            Path selectedShape = r.VisualHit as Path;
                                            if (selectedShape != null)
                                            {
                                                ShapeHelper.SetIsSelected(selectedShape, true);
                                            }

                                            return HitTestResultBehavior.Continue;
                                        },
                                        new GeometryHitTestParameters(new RectangleGeometry(new Rect(startPosition, endPosition))));

                isSelecting = false;
            }

            ChartChildren.Remove(rect);
            rect = null;
        }

        public void ChartKeyDown(Canvas chart, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && !isSelecting)
            {
                foreach (Shape shape in ChartChildren.OfType<Shape>())
                {
                    ShapeHelper.SetIsSelected(shape, false);
                }
            }
        }

        public void ChartMouseWheel(MouseWheelEventArgs e)
        {
            if (ChartChildren.Count > 0)
            {
                if (e.Delta > 0)
                {
                    scaleValue += 0.1;
                    scaleValue = scaleValue < 1.85 ? scaleValue : 1.8;
                }
                if (e.Delta < 0)
                {
                    scaleValue -= 0.1;
                    scaleValue = scaleValue > 0.75 ? scaleValue : 0.7;
                }

                //view.ChartScaleTransform.ScaleX = scaleValue;
                //view.ChartScaleTransform.ScaleY = scaleValue;
                ScaleX = scaleValue;
                ScaleY = scaleValue;
            }
        }


        public void CreateWorkStep()
        {
            WorkStep workStep = new WorkStep();
            foreach (Path path in ChartChildren.OfType<Path>())
            {
                if (ShapeHelper.GetIsSelected(path))
                {
                    workStep.PathList.Add(ShapeDic[path]);
                }
            }

            if (workStep.PathList.Count > 0)
            {
                Recipes.Instance.WorkSteps.Add(workStep);
            }
            else
            {
                MessageBox.Show("生成失败：未选择任何图形！", "提示");
            }
        }

        public void WorkStepListSelectionChanged()
        {
            if (SelectWorkStep != null)
            {
                List<Path> pathList = ShapeDic.Where(kv => { return SelectWorkStep.PathList.Contains(kv.Value); })
                          .Select(kv => kv.Key)
                          .ToList();

                foreach (Path path in ChartChildren.OfType<Path>())
                {
                    if (ShapeDic.ContainsKey(path))
                    {
                        ShapeHelper.SetIsSelected(path, pathList.Contains(path) ? true : false);
                    }
                }
            }
        }

        public void WorkStepListKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (SelectWorkStep != null)
                {
                    List<Path> pathList = ShapeDic.Where(kv => SelectWorkStep.PathList.Contains(kv.Value))
                                                  .Select(kv => kv.Key)
                                                  .ToList();

                    foreach (Path path in pathList)
                    {
                        path.Style = FindResource<Style>("GlueVavleOne");
                    }
                }

                Recipes.Instance.WorkSteps.Remove(SelectWorkStep);
            }
        }

        public void WorkStepUp(int workStepIndex)
        {
            if (workStepIndex <= 0) return;

            Recipes.Instance.WorkSteps.Move(workStepIndex, workStepIndex - 1);
        }

        public void WorkStepDown(int workStepIndex)
        {
            if (workStepIndex >= Recipes.Instance.WorkSteps.Count - 1) return;

            Recipes.Instance.WorkSteps.Move(workStepIndex, workStepIndex + 1);
        }


        public void PathListSelectionChanged()
        {
            if (SelectMotionPath != null)
            {
                foreach (Path path in ChartChildren.OfType<Path>())
                {
                    if (ShapeDic.ContainsKey(path))
                    {
                        ShapeHelper.SetIsSelected(path, ShapeDic[path] == SelectMotionPath ? true : false);
                    }
                }
            }
        }

        public void PathUp(int pathIndex)
        {
            if (pathIndex <= 0) return;

            SelectWorkStep.PathList.Move(pathIndex, pathIndex - 1);
        }

        public void PathDown(int pathIndex)
        {
            if (pathIndex >= SelectWorkStep.PathList.Count - 1) return;

            SelectWorkStep.PathList.Move(pathIndex, pathIndex + 1);
        }

        public void GlueVavleSelectionChanged()
        {
            if (SelectWorkStep != null)
            {
                List<Path> pathList = ShapeDic.Where(kv => SelectWorkStep.PathList.Contains(kv.Value))
                                              .Select(kv => kv.Key)
                                              .ToList();

                Style style = null;
                switch (SelectWorkStep.GlueVavle)
                {
                    case GlueVavleKind.点胶阀2:
                        style = FindResource<Style>("GlueVavleTwo");
                        break;
                    case GlueVavleKind.点胶阀3:
                        style = FindResource<Style>("GlueVavleThree");
                        break;
                    case GlueVavleKind.点胶阀4:
                        style = FindResource<Style>("GlueVavleFour");
                        break;
                    default:
                        style = FindResource<Style>("GlueVavleOne");
                        break;
                }

                foreach (Path path in pathList)
                {
                    path.Style = style;
                }
            }
        }


        #endregion

        #region other

        /// <summary>
        /// 生成视图
        /// </summary>
        public void CreatView()
        {
            MainView view = IoC.Get<MainView>();

            ChartChildren.Clear();
            ShapeDic.Clear();
            Recipes.Instance.WorkSteps.Clear();

            foreach (MotionPath motionPath in dxfConverter.ToMotionPath())
            {
                if (motionPath is MotionLine)
                {
                    MotionLine motionLine = motionPath as MotionLine;

                    Path linePath = motionPathConverter.CreatFromLine(motionLine);

                    linePath.Style = (Style)view.FindResource("GlueVavleOne");

                    ChartChildren.Add(linePath);

                    ShapeDic.Add(linePath, motionPath);
                }
                else if (motionPath is MotionArc)
                {
                    MotionArc motionArc = motionPath as MotionArc;

                    if (motionPath.PathType == MotionPathType.圆)
                    {
                        Path circlePath = motionPathConverter.CreatFromCircle(motionArc);

                        circlePath.Style = (Style)view.FindResource("GlueVavleOne");

                        ChartChildren.Add(circlePath);

                        ShapeDic.Add(circlePath, motionPath);
                    }
                    else
                    {
                        Path arcPath = motionPathConverter.CreatFromArc(motionArc);

                        arcPath.Style = (Style)view.FindResource("GlueVavleOne");

                        ChartChildren.Add(arcPath);

                        ShapeDic.Add(arcPath, motionPath);
                    }
                }
            }
        }

        bool IsOverRange = false;
        private void IsShapeOutRange(object state)
        {
            //Path limitPath = new Path();
            //limitPath.Data = new RectangleGeometry(new Rect(new Size(GlobalParam.Instance.MaxXLenght, GlobalParam.Instance.MaxYLenght)));
            //limitPath.Stroke = Brushes.Yellow;
            //view.ChartChildren.Add(limitPath);

            //VisualTreeHelper.HitTest(view.Chart, null,
            //    r =>
            //    {
            //        IntersectionDetail intersectionDetail = ((GeometryHitTestResult)r).IntersectionDetail;

            //        Path hitShape = r.VisualHit as Path;
            //        if (hitShape != null)
            //        {
            //            if (intersectionDetail != IntersectionDetail.FullyInside)
            //            {
            //                IsOverRange = true;
            //                return HitTestResultBehavior.Stop;
            //            }
            //        }
            //        return HitTestResultBehavior.Continue;
            //    },
            //    new GeometryHitTestParameters(limitPath.Data));

            //if (IsOverRange)
            //{
            //    view.ChartChildren.Clear();
            //    IsOverRange = false;
            //    MessageBox.Show($"检测到超出设置范围({GlobalParam.Instance.MaxXLenght},{GlobalParam.Instance.MaxYLenght})的图形，请在检查后重试！", "提示");
            //}
        }

        /// <summary>
        /// 根据名称查找指定控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="controlName">控件名称</param>
        public T FindControl<T>(string controlName)
        {
            MainView view = IoC.Get<MainView>();
            return (T)view.FindName(controlName);
        }

        /// <summary>
        /// 根据名称查找指定资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resourceName">资源名称</param>
        public T FindResource<T>(string resourceName)
        {
            MainView view = IoC.Get<MainView>();
            return (T)view.FindResource(resourceName);
        }

        #endregion
    }
}
