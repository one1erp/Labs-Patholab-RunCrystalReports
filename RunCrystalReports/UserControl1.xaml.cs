using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using Patholab.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RunCrystalReports._2
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    [ComVisible(true)]
    [ProgId("RunCrystalReports.RunCrystalReportsCls")]
    public partial class RunCrystalReportsCls : UserControl, IExtensionWindow
    {
        private INautilusDBConnection _ntlsCon;
        public RunCrystalReportsCls()
        {
            InitializeComponent();
        }
        #region Implementation of IExtensionWindow

        public bool CloseQuery()
        {
            return true;
        }

        public void Internationalise() { }

        public void SetSite(object site)
        {
            _ntlsSite = (IExtensionWindowSite2)site;
            _ntlsSite.SetWindowInternalName("");
            _ntlsSite.SetWindowRegistryName("");
            _ntlsSite.SetWindowTitle("test wpf");
        }


        public void PreDisplay()
        {
            //Utils.CreateConstring(_ntlsCon);


        }

        public WindowButtonsType GetButtons()
        {
            return LSExtensionWindowLib.WindowButtonsType.windowButtonsNone;
        }

        public bool SaveData()
        {
            return false;
        }
        private IExtensionWindowSite2 _ntlsSite;

        public void SetServiceProvider(object serviceProvider)
        {
        NautilusServiceProvider sp = serviceProvider as NautilusServiceProvider;
           _ntlsCon = Utils.GetNtlsCon(sp);

        }

        public void SetParameters(string parameters)
        {

        }

        public void Setup()
        {

        }

        public WindowRefreshType DataChange()
        {
            return LSExtensionWindowLib.WindowRefreshType.windowRefreshNone;
        }

        public WindowRefreshType ViewRefresh()
        {
            return LSExtensionWindowLib.WindowRefreshType.windowRefreshNone;
        }

        public void refresh() { }

        public void SaveSettings(int hKey) { }

        public void RestoreSettings(int hKey) { }
        public void Close()
        {

        }
    }
}


        #endregion
