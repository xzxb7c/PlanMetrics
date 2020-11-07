using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace PlanMetrics
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public StructureSet set = null;
        public PlanningItem plan = null;
        public PlanSetup PS = null;
        public PlanSum SumPlan = null;
        public int comboindex = 0;
        public bool isPsum = false;
        public string stType = null;
        public DoseValue planTD = new DoseValue();
        public double sumDose = 0;
        public double checkedDose = 0;

        double outputDose = 0;
        double dose = 0;
        double volume = 0;
        double result = 0;
        double volume2 = 0;
        public DoseValue.DoseUnit doseUnit = DoseValue.DoseUnit.cGy;
        public DoseValuePresentation dosePres = DoseValuePresentation.Absolute;
        public VolumePresentation volPres = VolumePresentation.AbsoluteCm3;
        string volPresTxt = "cc";
        string testResult = "PASS";
        string teststring = "V0";

        List<dvh_stats> dvhs = new List<dvh_stats>();
        List<string> planT = new List<string>();
        List<PlanSumClass> PlanSumList = new List<PlanSumClass>();
        List<PlanMets> pm = new List<PlanMets>();
        List<PlanMetData> pmdata = new List<PlanMetData>();
        List<PlanMetData> pmdata_sorted = new List<PlanMetData>();

        public class PlanSumClass
        {
            public string PlanName { get; set; }
            public DoseValue PlanDose { get; set; }
        }

        public class dvh_stats
        {
            public SolidColorBrush stLine { get; set; }
            public string StructureID { get; set; }

            public string structType { get; set; }

            public string V100 { get; set; }

            public string V95 { get; set; }

            public string V90 { get; set; }

            public string MaxDose { get; set; }
            public string MinDose { get; set; }
            public string MeanDose { get; set; }
            public SolidColorBrush dataColor { get; set; }
        }

        public class PlanMets
        {
            public string OAR { get; set; }

            public string conType { get; set; }

            public string input_st { get; set; }

            public string inputunit { get; set; }

            public string outMax_st { get; set; }

            public string outUnit { get; set; }
        }

        public class PlanMetData
        {
            public string Structure { get; set; }
            public string Test { get; set; }

            public string Value { get; set; }

            public string TestResult { get; set; }

            public SolidColorBrush TRColor { get; set; }

        }
        public MainControl()
        {
            InitializeComponent();

        }

        public void BuildList()
        {
            PlanSumList.Clear();
            planT.Clear();


            if (!isPsum)
            {

                PS = (PlanSetup)plan;
                planT.Add(PS.TargetVolumeID);

            }
            else
            {

                SumPlan = (PlanSum)plan;

                foreach (var pss in SumPlan.PlanSetups)
                {
                    PlanSumList.Add(new PlanSumClass
                    {
                        PlanName = pss.Id,
                        PlanDose = pss.TotalDose
                    });

                    planT.Add(pss.TargetVolumeID);

                    Double.TryParse(pss.TotalDose.ToString().Split(' ').First(), out checkedDose);
                    sumDose = sumDose + checkedDose;

                }
                tbRxdose.Text = sumDose.ToString();
                listBoxZone.DataContext = PlanSumList;
                listBoxZone.Items.Refresh();

            }

        }

        public void ComputeTargets()
        {


            Double.TryParse(tbRxdose.Text, out outputDose);

            DoseValue totalDose = new DoseValue(outputDose, DoseValue.DoseUnit.cGy);

            pmdata.Clear();
            pmdata_sorted.Clear();


            if (File.Exists("\\\\TDC-ARi-FIL-001\\va_data2$\\ProgramData\\Vision\\PublishedScripts\\pm30.txt") == true)
            {

                foreach (string line in File.ReadAllLines("\\\\TDC-ARi-FIL-001\\va_data2$\\ProgramData\\Vision\\PublishedScripts\\pm30.txt"))
                {
                    pm.Add(new PlanMets
                    {
                        OAR = line.Split(',')[0],
                        conType = line.Split(',')[1],
                        input_st = line.Split(',')[2],
                        inputunit = line.Split(',')[3],
                        outMax_st = line.Split(',')[4],
                        outUnit = line.Split(',')[5]
                    }); ;
                }


                foreach (var st in plan.StructureSet.Structures)
                {


                    if (!st.IsEmpty)
                    {
                        if (st.Id.Contains("BrainStem"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Brainstem"))
                                {

                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("OpticNerve") || st.Id.Contains("Chiasm"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Optic Pathway"))
                                {

                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("Eye"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Eye"))
                                {

                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("Lens"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Lens"))
                                {

                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("Cochlea"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Cochlea"))
                                {

                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("Kidneys"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Kidneys"))
                                {

                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("Kidney") && !st.Id.Contains("Kidneys"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Kidney") && !x.OAR.Contains("Kidneys"))
                                {
                                    PerformTest(st, x);

                                }
                            }

                        }
                        else if (st.Id.Contains("LargeBowel"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Colon"))
                                {
                                    PerformTest(st, x);


                                }
                            }
                        }
                        else if (st.Id.Contains("SmallBowel"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Jejunum"))
                                {
                                    PerformTest(st, x);


                                }
                            }
                        }
                        else if (st.Id.Contains("Bladder"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Bladder"))
                                {
                                    PerformTest(st, x);


                                }
                            }
                        }
                        else if (st.Id.Contains("Rectum"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Rectum"))
                                {
                                    PerformTest(st, x);

                                }
                            }
                        }
                        else if (st.Id.Contains("Femur"))
                        {
                            foreach (var x in pm)
                            {
                                if (x.OAR.Contains("Femoral"))
                                {
                                    PerformTest(st, x);

                                }
                            }
                        }
                    }
                }

                pmdata_sorted = pmdata.OrderBy(o => o.Structure).ToList();
                dvh_sp.DataContext = pmdata_sorted;
                dvh_sp.Items.Refresh();

            }
        }

        private void PerformTest(Structure st, PlanMets x)
        {
            Double.TryParse(x.input_st, out dose);
            Double.TryParse(x.outMax_st, out result);

            volPres = x.outUnit == "cc" ? VolumePresentation.AbsoluteCm3 : VolumePresentation.Relative;

            DoseValue doseVal = new DoseValue(100 * dose, doseUnit);

            volume = Math.Round(plan.GetVolumeAtDose(st, doseVal, volPres), 2);

            volume2 = Math.Round(plan.GetVolumeAtDose(st, new DoseValue(0, doseUnit), volPres), 2);

            if (x.conType == "Vmin")
            {
                testResult = volume2 - volume <= result ? "PASS" : "FAIL";
                teststring = String.Format("V0 Gy  - V{0} Gy < {1} {2} ", x.input_st, x.outMax_st, x.outUnit);

                pmdata.Add(new PlanMetData
                {
                    Structure = st.Id,
                    Test = teststring,
                    Value = (volume2 - volume).ToString() + " " + x.outUnit,
                    TestResult = testResult,
                    TRColor = testResult == "PASS" ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red)
                });

            }
            else if (x.conType == "Vmax")
            {
                testResult = volume <= result ? "PASS" : "FAIL";
                teststring = String.Format("V{0} Gy < {1} {2} ", x.input_st, x.outMax_st, x.outUnit);

                pmdata.Add(new PlanMetData
                {
                    Structure = st.Id,
                    Test = teststring,
                    Value = volume.ToString() + " " + x.outUnit,
                    TestResult = testResult,
                    TRColor = testResult == "PASS" ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.Red)
                });

            }

        }


        private void print_btn_Click(object sender, RoutedEventArgs e)
        {

            PrintDialog pd = new PrintDialog();
            pd.PrintVisual(this, "Plan Metrics Report");
        }





        private void btnComputePTV_Click(object sender, RoutedEventArgs e)
        {
            ComputeTargets();
        }

        private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            Double.TryParse(chkZone.Tag.ToString().Split(' ').First(), out checkedDose);
            sumDose = sumDose + checkedDose;
            //    ZoneText.Text = "Plan Name = " + chkZone.Content.ToString();
            //   ZoneValue.Text = "Values = " + sumDose.ToString();
            tbRxdose.Text = sumDose.ToString();


        }

        private void CheckBoxZone_UnChecked(object sender, RoutedEventArgs e)
        {

            CheckBox chkZone = (CheckBox)sender;
            Double.TryParse(chkZone.Tag.ToString().Split(' ').First(), out checkedDose);
            sumDose = sumDose - checkedDose;
            //ZoneText.Text = "Plan Name = " + chkZone.Content.ToString();
            //   ZoneValue.Text = "Values = " + sumDose.ToString();
            tbRxdose.Text = sumDose.ToString();

        }


    }
}

