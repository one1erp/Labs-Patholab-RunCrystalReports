using CrystalDecisions.CrystalReports.Engine;
using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Patholab_Common;
using Patholab_DAL;
using Xceed.Wpf.Toolkit;

namespace RunCrystalReports
{
    /// <summary>
    /// Interaction logic for ReportsCtl.xaml
    /// </summary>
    public partial class ReportsCtl : UserControl
    {

        public bool DEBUG;

        #region Ctor


        public ReportsCtl()
        {
            InitializeComponent();
            ReportsA = new List<string>();
            ReportsB = new List<string>();
            ReportsC = new List<string>();
            this.DataContext = this;
        }
        public ReportsCtl(INautilusProcessXML _xmlProcessor,
    IExtensionWindowSite2 _ntlsSite,
    INautilusServiceProvider _sp,
    INautilusDBConnection _ntlsCon,
        INautilusUser _ntlsUser)
            : this()
        {
            this._xmlProcessor = _xmlProcessor;
            this._ntlsSite = _ntlsSite;
            this._sp = _sp;
            this._ntlsCon = _ntlsCon;
            this._ntlsUser = _ntlsUser;
            //InitializeComponent();
            //ReportsA = new List<string>();
            //ReportsB = new List<string>();
            //ReportsC = new List<string>();
            //this.DataContext = this;

        }
        #endregion

        #region Fields

        private DataLayer dal;
        private List<U_CRYSTAL_REPORT> allowedReports;
        private List<long> roleIds;
        private INautilusProcessXML _xmlProcessor;
        private IExtensionWindowSite2 _ntlsSite;
        private INautilusServiceProvider _sp;
        private INautilusDBConnection _ntlsCon;
        private INautilusUser _ntlsUser;
        #endregion

        #region Members
        public List<string> ReportsA { get; set; }
        public List<string> ReportsB { get; set; }
        public List<string> ReportsC { get; set; }
        #endregion


        public void InitializeData()
        {

            try
            {



                dal = new DataLayer();

                if (DEBUG)
                {
                  
                    dal.MockConnect();
                }
                else
                {
                    dal.Connect(_ntlsCon);
                }

                roleIds = GetUserRoles();

                var reports = from item in dal.GetAll<U_CRYSTAL_REPORT>().Include("U_CRYSTAL_REPORT_USER").Include("U_REPORT_PARAMS_USER") select item;
                allowedReports = new List<U_CRYSTAL_REPORT>();

                foreach (var item in reports)
                {
                    bool b = IsAllowedUser(item);
                    if (b)
                    {
                        allowedReports.Add(item);
                    }
                }


                ReportsA = GetReportsByLetter("A");
                ReportsB = GetReportsByLetter("B");
                ReportsC = GetReportsByLetter("C");


            }
            catch (Exception e)
            {

                System.Windows.Forms.MessageBox.Show("Error in  InitializeData " + "/n" + e.Message);
                Logger.WriteLogFile(e);
            }

        }
        bool IsAllowedUser(U_CRYSTAL_REPORT report)
        {
            try
            {
                var roleAllowed = report.U_CRYSTAL_REPORT_USER.U_ROLE_ALLOWED;
                if (string.IsNullOrEmpty(roleAllowed))
                {
                    return true;

                }
                else
                {
                    List<long> spliettedRoles = new List<long>();
                    var splited = roleAllowed.Split(';');
                    foreach (var role in splited)
                    {
                        if (!string.IsNullOrEmpty(role))
                        {
                            spliettedRoles.Add(int.Parse(role));
                        }

                    }
                    if (spliettedRoles.Count > 0 && roleIds.Count > 0)
                    {
                        var common = spliettedRoles.Intersect(roleIds);
                        if (common.Count() > 0)
                        {
                            return true;

                        }
                    }
                }
                return false;

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error in  IsAllowedUser " + "/n" + e.Message);
                Logger.WriteLogFile(e);
                return false;
            }

        }
        private List<long> GetUserRoles()
        {

            double operatorId;
            if (DEBUG)
                operatorId = 1;
            else
                operatorId = _ntlsUser.GetOperatorId();


            var currentOperator = dal.GetOperatorByIdIncludeRole(operatorId);
            var roleIds = currentOperator.LIMS_ROLES.Select(x => x.ROLE_ID);
            return roleIds.ToList();
        }

        private List<string> GetReportsByLetter(string letter)
        {
            return allowedReports.Where(x => x.U_CRYSTAL_REPORT_USER.U_LIST == letter).Select(x => x.U_CRYSTAL_REPORT_USER.U_HEBREW_NAME).ToList();
        }

        private void lbReportsA_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                spLabels.Children.Clear();
                spParams.Children.Clear();


                ListBox clb = sender as ListBox;
                if (clb != null)
                {
                    var hebrewName = clb.SelectedItems[0].ToString();
                    var reportParams = (from item in allowedReports where item.U_CRYSTAL_REPORT_USER.U_HEBREW_NAME == hebrewName select item).FirstOrDefault();
                    if (reportParams != null)
                    {
                        GenerateParameters(reportParams.U_REPORT_PARAMS_USER);
                    }


                }
            }

            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show("Error in  lbReportsA_SelectionChanged_1 " + "\n" + ex.Message);
                Logger.WriteLogFile(ex);

            }



        }

        void OpenCrystal()
        {
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(@"C:\Users\ashim\Desktop\CRYSTALtEST.rpt");
            //reportDocument.SetParameterValue("@id", "12");

        }

        void GetParameters()
        {

            for (int i = 0; i < spParams.Children.Count; i++)
            {
                var ctlType = spParams.Children[i].GetType();
                if (ctlType.Name == "DatePicker")
                {
                    var dp = spParams.Children.OfType<DatePicker>().FirstOrDefault();
                    var date = dp.SelectedDate;


                }
                else if (ctlType.Name == "TextBox")
                {
                    var dp = spParams.Children.OfType<TextBox>().FirstOrDefault();
                    var date = dp.Text;

                }
                else if (ctlType.Name == "CheckBox")
                {
                    var dp = spParams.Children.OfType<CheckBox>().FirstOrDefault();
                    var date = dp.IsChecked;

                }

            }

        }
        private void CreateLabel(string PrompText)
        {

            //create label
            var label = new TextBlock();
            label.Margin = new Thickness(5, 5, 5, 5);
            label.Text = PrompText;
            spLabels.Children.Add(label);
        }
        void GenerateControl(string type)
        {

        }
        private void GenerateParameters(IEnumerable<U_REPORT_PARAMS_USER> collection)
        {


            foreach (var item in collection)
            {

                switch (item.U_FIELD_TYPE)
                {
                    case "D":
                        CreateLabel(item.U_PARAM_HEBREW);
                        Control d = new DatePicker();
                        d.Margin = new Thickness(5, 5, 5, 5);
                        spParams.Children.Add(d);

                        break;
                    case "T":
                        CreateLabel(item.U_PARAM_HEBREW);
                        TextBox t = new TextBox();
                        t.Margin = new Thickness(5, 5, 5, 5);
                        spParams.Children.Add(t);
                        break;
                    case "N":
                        CreateLabel(item.U_PARAM_HEBREW);
                        IntegerUpDown numeric = new IntegerUpDown();
                        numeric.Margin = new Thickness(5, 5, 5, 5);
                        spParams.Children.Add(numeric);
                        break;
                    case "B":
                        CreateLabel(item.U_PARAM_HEBREW);
                        CheckBox c = new CheckBox();
                        c.Margin = new Thickness(5, 5, 5, 5);
                        spParams.Children.Add(c);

                        break;
                    default:
                        break;
                }

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GetParameters();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenCrystal();
        }
    }
}