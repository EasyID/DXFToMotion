using System.ComponentModel;

namespace DXFViewer
{
    public class GlobalParam : NotificationObject
    {
        #region Singleton

        public static GlobalParam Instance { get; set; } = new GlobalParam();

        private GlobalParam() { }

        #endregion

        public string LoadFilePath { get; set; }

        public double MaxXLenght { get; set; } = 150;

        public double MaxYLenght { get; set; } = 150;
    }
}
