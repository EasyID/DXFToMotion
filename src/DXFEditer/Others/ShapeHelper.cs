using System;
using System.Windows;
using System.Windows.Shapes;

namespace DXFViewer
{
    public class ShapeHelper : DependencyObject
    {
        #region IsSelected

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(ShapeHelper), new PropertyMetadata(false));

        public static bool GetIsSelected(UIElement element)
        {
            return (bool)element.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(UIElement element, Boolean value)
        {
            element.SetValue(IsSelectedProperty, value);
        }

        #endregion
    }
}
