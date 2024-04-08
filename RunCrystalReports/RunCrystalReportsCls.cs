using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;

using System.Runtime.InteropServices;
using System.Windows.Forms;
using Patholab_Common;


namespace RunCrystalReports
{


    [ComVisible(true)]
    [ProgId("RunCrystalReports.RunCrystalReportsCls")]
    public partial class RunCrystalReportsCls : UserControl, IExtensionWindow
    {

        INautilusProcessXML xmlProcessor;
        INautilusUser _ntlsUser;
        private IExtensionWindowSite2 _ntlsSite;
        private INautilusServiceProvider sp;
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
            _ntlsSite.SetWindowTitle("Crystal Reports");
        }

        public bool DEBUG;
        public void PreDisplay()
        {

            xmlProcessor = Utils.GetXmlProcessor(sp);

            _ntlsUser = Utils.GetNautilusUser(sp);

            ReportsCtl reportsCtl1 = new ReportsCtl(xmlProcessor, _ntlsSite, sp, _ntlsCon, _ntlsUser);
            elementHost1.Child = reportsCtl1;
            reportsCtl1.DEBUG = DEBUG;
            reportsCtl1.InitializeData();
        }

        public WindowButtonsType GetButtons()
        {
            return LSExtensionWindowLib.WindowButtonsType.windowButtonsNone;
        }

        public bool SaveData()
        {
            return false;
        }

        public void SetServiceProvider(object serviceProvider)
        {
            sp = serviceProvider as NautilusServiceProvider;
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

        private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
    }
}


        #endregion
