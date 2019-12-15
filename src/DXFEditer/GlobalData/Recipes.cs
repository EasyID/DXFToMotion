using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public class Recipes : NotificationObject
    {
        #region Singleton

        public static Recipes Instance { get; set; } = new Recipes();

        private Recipes() { }

        #endregion

        public ObservableCollection<WorkStep> WorkSteps { get; set; } = new ObservableCollection<WorkStep>();

    }
}
