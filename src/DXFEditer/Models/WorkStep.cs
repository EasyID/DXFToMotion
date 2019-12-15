using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFViewer
{
    public class WorkStep
    {
        #region field


        #endregion

        #region construction

        public WorkStep() { }

        #endregion

        #region property

        public GlueVavleKind GlueVavle { get; set; } = GlueVavleKind.点胶阀1;

        public string Remark { get; set; } = "Undefine";

        public ObservableCollection<MotionPath> PathList { get; set; } = new ObservableCollection<MotionPath>();

        #endregion

        #region method



        #endregion
    }

    public enum GlueVavleKind
    {
        点胶阀1,
        点胶阀2,
        点胶阀3,
        点胶阀4
    }
}
