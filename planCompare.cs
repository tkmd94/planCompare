using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Controls;
using System.Windows.Media;

[assembly: AssemblyVersion("1.0.0.1")]

namespace VMS.TPS
{
    public class Script
    {
        public Script()
        {
        }
        private void AddCheckTitle(string titleText, Panel panel)
        {
            var label = new Label();
            label.Content = titleText;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.FontSize = 13;
            label.FontWeight = FontWeights.Bold;
            label.Foreground = Brushes.Blue;
            panel.Children.Add(label);
        }
        private void AddColumn(string columnText, Grid grid, int columnIndex, FontWeight fontWeightStyle, Brush brushFG, Brush brushBG)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var label = new Label();
            label.Content = columnText;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.SetValue(Grid.RowProperty, 0);
            label.SetValue(Grid.ColumnProperty, columnIndex);
            label.FontWeight = fontWeightStyle;
            label.Foreground = brushFG;
            label.Background = brushBG;
            grid.Children.Add(label);

        }
        private void AddRow(string rowText, Grid grid, bool addRowStatus, int columnIndex, int rowIndex, FontWeight fontWeightStyle, Brush brushFG, Brush brushBG)
        {
            if (addRowStatus == true)
                grid.RowDefinitions.Add(new RowDefinition());
            var label = new Label();
            label.Content = rowText;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.SetValue(Grid.RowProperty, rowIndex);
            label.SetValue(Grid.ColumnProperty, columnIndex);
            label.FontWeight = fontWeightStyle;
            label.Foreground = brushFG;
            label.Background = brushBG;
            grid.Children.Add(label);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(ScriptContext context /*, System.Windows.Window window, ScriptEnvironment environment*/)
        {
            // TODO : Add here the code that is called when the script is launched from Eclipse.


            if (context.PlanSetup == null)
            {
                MessageBox.Show("Warning: no plan is loaded.");
                return;
            }
            if (context.PlanSetup.ProtocolID.Count() == 0)
            {
                MessageBox.Show("Warning: no clinical protocol is attached the plan.");
                return;
            }

            int prescriptionRowCount = 0;
            int prescriptionColCount = 0;
            int qualityIndicesRowCount = 0;
            int qualityIndicesColCount = 0;

            //define pop-up window.
            var window = new Window();
            var scrollView = new ScrollViewer();
            var listView = new ListView();

            scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            var panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.Background = Brushes.White;

            //define labels of patient & protocol Information
            AddCheckTitle("Patient Information ", panel);
            var labelPat = new Label();
            labelPat.Content = "";
            labelPat.HorizontalAlignment = HorizontalAlignment.Left;
            labelPat.FontSize = 15;
            labelPat.FontWeight = FontWeights.Bold;
            labelPat.Foreground = Brushes.Black;
            panel.Children.Add(labelPat);
            var labelProtocol = new Label();
            labelProtocol.Content = "";
            labelProtocol.HorizontalAlignment = HorizontalAlignment.Left;
            labelProtocol.FontSize = 15;
            labelProtocol.FontWeight = FontWeights.Bold;
            labelProtocol.Foreground = Brushes.Black;
            panel.Children.Add(labelProtocol);


            //define labels of clinical protocol.
            AddCheckTitle("Prescriptions ", panel);
            var gridPrescription = new Grid();
            AddColumn(" Structure ", gridPrescription, 0, FontWeights.Normal, Brushes.Black, Brushes.White);
            gridPrescription.RowDefinitions.Add(new RowDefinition());
            gridPrescription.ShowGridLines = true;
            panel.Children.Add(gridPrescription);
            prescriptionRowCount++;
            prescriptionColCount++;

            AddCheckTitle("Quality Indices ", panel);
            var gridQualityIndices = new Grid();
            AddColumn(" Structure ", gridQualityIndices, 0, FontWeights.Normal, Brushes.Black, Brushes.White);
            gridQualityIndices.RowDefinitions.Add(new RowDefinition());
            gridQualityIndices.ShowGridLines = true;
            panel.Children.Add(gridQualityIndices);
            qualityIndicesRowCount++;
            qualityIndicesColCount++;

            //define close button.
            var OKbutton = new Button();
            OKbutton.Content = "Close";
            //OKbutton.FontSize = 14;
            OKbutton.IsCancel = true;
            OKbutton.IsDefault = true;
            OKbutton.HorizontalAlignment = HorizontalAlignment.Right;
            OKbutton.Margin = new Thickness(0, 10, 30, 10);
            OKbutton.Width = 80;
            OKbutton.Height = 25;
            panel.Children.Add(OKbutton);


            scrollView.Content = panel;
            window.Content = scrollView;
            window.Title = "PlanCompare(Clinical Protocol)";
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //////////////////////////////////////////////////////////////////

            Patient patient = context.Patient;
            Course course = context.Course;

            string modifier = "";

            //set label patient ID & name,protocol ID.
            labelPat.Content = string.Format("Patiet ID: {0} / Name: {1},{2} {3}", patient.Id, patient.LastName, patient.FirstName, patient.MiddleName);
            labelProtocol.Content = string.Format("Protocol ID: {0}", context.PlanSetup.ProtocolID.Replace("_", "__"));

            //set column title of priscrition grid.
            prescriptionColCount++;
            AddColumn(" Prescription ", gridPrescription, 1, FontWeights.Normal, Brushes.Black, Brushes.White);
            prescriptionColCount++;
            AddColumn(" FractionDose ", gridPrescription, 2, FontWeights.Normal, Brushes.Black, Brushes.White);
            prescriptionColCount++;
            AddColumn(" TotalDose ", gridPrescription, 3, FontWeights.Normal, Brushes.Black, Brushes.White);

            //set column title of quality indices grid.
            qualityIndicesColCount++;
            AddColumn(" Index ", gridQualityIndices, 1, FontWeights.Normal, Brushes.Black, Brushes.White);
            qualityIndicesColCount++;
            AddColumn(" TargetValue ", gridQualityIndices, 2, FontWeights.Normal, Brushes.Black, Brushes.White);

            //get clinical protocol information.
            var prescriptions = new List<ProtocolPhasePrescription>();
            var measures = new List<ProtocolPhaseMeasure>();
            context.PlanSetup.GetProtocolPrescriptionsAndMeasures(ref prescriptions, ref measures);

            //set prescriptions on each row. 
            foreach (ProtocolPhasePrescription prescription in prescriptions)
            {
                AddRow(prescription.StructureId,
                    gridPrescription, true, 0, prescriptionRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                // replace with eclipse UI notation. 
                if (prescription.PrescModifier.ToString() == "PrescriptionModifierAtLeast")
                {
                    modifier = " At least " + prescription.PrescParameter.ToString() + " % recievs more than ";
                }
                else if (prescription.PrescModifier.ToString() == "PrescriptionModifierAtMost")
                {
                    modifier = " At most " + prescription.PrescParameter.ToString() + " % recievs more than ";
                }
                else if (prescription.PrescModifier.ToString() == "PrescriptionModifierMinDose")
                {
                    modifier = " Minimum dose is ";
                }
                else if (prescription.PrescModifier.ToString() == "PrescriptionModifierMaxDose")
                {
                    modifier = " Maximum dose is ";
                }
                else if (prescription.PrescModifier.ToString() == "PrescriptionModifierMeanDose")
                {
                    modifier = " Mean dose is ";
                }
                else
                {
                    modifier = prescription.PrescModifier.ToString();
                }

                AddRow(modifier,
                    gridPrescription, false, 1, prescriptionRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                AddRow(prescription.TargetFractionDose.ToString(),
                    gridPrescription, false, 2, prescriptionRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                AddRow(prescription.TargetTotalDose.ToString(),
                    gridPrescription, false, 3, prescriptionRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                prescriptionRowCount++;
            }
            //set quality indices on each row. 
            foreach (ProtocolPhaseMeasure measure in measures)
            {
                AddRow(measure.StructureId,
                    gridQualityIndices, true, 0, qualityIndicesRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                // replace with eclipse UI notation. 
                if (measure.Modifier.ToString() == "MeasureModifierAtMost")
                {
                    modifier = " is less than ";
                }
                else if (measure.Modifier.ToString() == "MeasureModifierAtLeast")
                {
                    modifier = " is more than ";
                }
                else if (measure.Modifier.ToString() == "MeasureModifierTarget")
                {
                    modifier = " is ";
                }
                else
                {
                    modifier = measure.Modifier.ToString();
                }

                AddRow(measure.TypeText.ToString() + modifier,
                    gridQualityIndices, false, 1, qualityIndicesRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                AddRow(measure.TargetValue.ToString(),
                    gridQualityIndices, false, 2, qualityIndicesRowCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                qualityIndicesRowCount++;
            }
            //set actual values  each plan. 
            foreach (PlanSetup plan in course.PlanSetups)
            {
                //exclude plan without clinical protocol.
                if (plan.ProtocolID.Count() > 0)
                {
                    AddColumn(plan.Id, gridPrescription, prescriptionColCount, FontWeights.Normal, Brushes.Black, Brushes.White);
                    AddColumn(plan.Id, gridQualityIndices, qualityIndicesColCount, FontWeights.Normal, Brushes.Black, Brushes.White);

                    //get clinical protocol information of each plan.
                    var prescriptions2 = new List<ProtocolPhasePrescription>();
                    var measures2 = new List<ProtocolPhaseMeasure>();
                    plan.GetProtocolPrescriptionsAndMeasures(ref prescriptions2, ref measures2);

                    prescriptionRowCount = 1;
                    //set prescription on each row. 
                    foreach (ProtocolPhasePrescription prescription in prescriptions2)
                    {
                        //set color in case 
                        if (prescription.TargetIsMet == true)
                        {
                            AddRow(prescription.ActualTotalDose.ToString(),
                                gridPrescription,
                                false,
                                prescriptionColCount,
                                prescriptionRowCount,
                                FontWeights.Bold,
                                Brushes.Black,
                                Brushes.LightGreen);
                        }
                        else
                        {
                            AddRow(prescription.ActualTotalDose.ToString(),
                                gridPrescription,
                                false,
                                prescriptionColCount,
                                prescriptionRowCount,
                                FontWeights.Bold,
                                Brushes.Black,
                                Brushes.HotPink);
                        }
                        prescriptionRowCount++;
                    }

                    qualityIndicesRowCount = 1;
                    //set quality indices on each row. 
                    foreach (ProtocolPhaseMeasure measure in measures2)
                    {
                        //set color in case 
                        if (measure.TargetIsMet == true)
                        {
                            AddRow(string.Format(" {0:f2}", Math.Round(measure.ActualValue, 2)),
                                gridQualityIndices,
                                false,
                                qualityIndicesColCount,
                                qualityIndicesRowCount,
                                FontWeights.Bold,
                                Brushes.Black,
                                Brushes.LightGreen);
                        }
                        else
                        {
                            AddRow(string.Format(" {0:f2}", Math.Round(measure.ActualValue, 2)),
                                gridQualityIndices,
                                false,
                                qualityIndicesColCount,
                                qualityIndicesRowCount,
                                FontWeights.Bold,
                                Brushes.Black,
                                Brushes.HotPink);
                        }
                        qualityIndicesRowCount++;
                    }
                    prescriptionColCount++;
                    qualityIndicesColCount++;
                }
            }
            //show window
            window.ShowDialog();
        }
    }
}
