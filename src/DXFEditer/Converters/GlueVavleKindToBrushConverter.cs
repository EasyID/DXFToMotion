using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DXFViewer
{
    class GlueVavleKindToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GlueVavleKind glueVavle = (GlueVavleKind)value;

            SolidColorBrush brush;
            switch (glueVavle)
            {
                case GlueVavleKind.点胶阀2:
                    {
                        brush = Brushes.Green;
                    }
                    break;
                case GlueVavleKind.点胶阀3:
                    {
                        brush = Brushes.Orange;
                    }
                    break;
                case GlueVavleKind.点胶阀4:
                    {
                        brush = Brushes.Blue;
                    }
                    break;
                default:
                    {
                        brush = Brushes.White;
                    }
                    break;
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
