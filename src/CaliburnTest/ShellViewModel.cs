using System.Windows;
using Caliburn.Micro;
namespace CaliburnTest
{
    public class ShellViewModel : PropertyChangedBase, IShell
    {
        public void ButtonClick(object source)
        {
            if (source != null)
            {
                MessageBox.Show(source.GetType().ToString());
            }
        }
    }
}