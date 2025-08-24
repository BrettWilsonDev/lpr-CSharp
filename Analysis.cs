using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace lpr381Project
{
    public partial class Analysis : Form
    {
        SensitivityAnalysis sensitivityAnalysis = new SensitivityAnalysis(true);
        int currentItem = -1;

        List<double> objFunc;
        List<List<double>> constraints;
        bool isMin;
        List<VariableSignType> varSigns;

        public Analysis()
        {
            InitializeComponent();
        }

        public void SetProblem(List<double> objFuncPassed, List<List<double>> constraintsPassed, bool isMinPassed, List<VariableSignType> varSignsPassed = null)
        {
            objFunc = objFuncPassed.ToList();
            constraints = constraintsPassed.Select(x => x.ToList()).ToList();
            isMin = isMinPassed;
            varSigns = varSignsPassed;
        }

        private void Analysis_Load(object sender, EventArgs e)
        {
            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();
            sendInput.Hide();
            inputBox.Hide();
            inputChoicelbl.Hide();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            try
            {
                Logger.WriteLine("SensitivityAnalysis started!");
                sensitivityAnalysis.RunSensitivityAnalysis(objFunc, constraints, isMin, varSigns);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"\nInvalid Input or Fatel Error!");
            }
            Logger.Close();

            // Configure RichTextBox
            outputBox.WordWrap = false;               // prevent wrapping
            outputBox.Font = new Font("Consolas", 10); // monospaced font
            outputBox.ScrollBars = RichTextBoxScrollBars.Both; // horizontal + vertical

            UpdateOutput();
        }

        private void UpdateOutput()
        {
            string filePath = @"SensitivityAnalysis.txt"; // your file path

            if (File.Exists(filePath))
            {
                outputBox.Text = File.ReadAllText(filePath);
            }
            else
            {
                MessageBox.Show("File not found: " + filePath);
            }
        }

        private void sendInput_Click(object sender, EventArgs e)
        {
            string input = inputBox.Text.Trim();
            string val = valueInput.Text.Trim();
            string row = inputRow.Text.Trim();

            if (!(float.TryParse(val, out float valIn)) && ((currentItem == 2) || currentItem == 4 || currentItem == 6 || currentItem == 7 || currentItem == 8))
            {
                if (currentItem == 7)
                {
                    MessageBox.Show("Please Enter a Row Index");
                }
                else
                {
                    MessageBox.Show("Please Enter a Value");
                }
                valIn = -1;
            }


            int rowIdx = -1; // declare rowIdx here

            if (currentItem == 8)
            {
                if (!(int.TryParse(row, out rowIdx)))
                {
                    MessageBox.Show("Please Enter a Row Index");
                    rowIdx = -1;
                }
            }

            if (int.TryParse(input, out int varIdx))
            {
                switch (currentItem)
                {
                    case 1:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleNonBasicVariableRange(varIdx);
                        Logger.Close();
                        break;
                    case 2:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleNonBasicVariableChange(valIn, varIdx);
                        Logger.Close();
                        break;
                    case 3:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleBasicVariableRange(varIdx);
                        Logger.Close();
                        break;
                    case 4:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleBasicVariableChange(valIn, varIdx);
                        Logger.Close();
                        break;
                    case 5:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleRHSRange(varIdx);
                        Logger.Close();
                        break;
                    case 6:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleRHSChange(valIn, varIdx);
                        Logger.Close();
                        break;
                    case 7:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleNonBasicColumnRange(valIn, varIdx);
                        Logger.Close();
                        break;
                    case 8:
                        Logger.Init("SensitivityAnalysis.txt");
                        sensitivityAnalysis.HandleNonBasicColumnChange(rowIdx, valIn, varIdx);
                        Logger.Close();
                        break;
                }


                UpdateOutput();
            }
            else
            {
                MessageBox.Show("Please Enter a Choice");
            }

            inputBox.Clear();
            valueInput.Clear();
            inputRow.Clear();
        }

        private void HandleNonBasicVariableRangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 1;

            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            //inputActsLbl.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleNonBasicVariableRange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleNonBasicVariableChangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 2;

            inputValuelbl.Show();
            valueInput.Show();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            //inputActsLbl.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleNonBasicVariableChange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleBasicVariableRangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 3;

            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleBasicVariableRange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleBasicVariableChangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 4;

            inputValuelbl.Show();
            valueInput.Show();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleBasicVariableRange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleRHSRangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 5;

            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleRHSRange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleRHSChangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 6;

            inputValuelbl.Show();
            valueInput.Show();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleRHSChange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleNonBasicColumnRangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 7;

            inputValuelbl.Show();
            valueInput.Show();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            inputValuelbl.Text = "Input Row index";

            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleNonBasicColumnRange();
            Logger.Close();

            UpdateOutput();
        }

        private void HandleNonBasicColumnChangeBtn_Click(object sender, EventArgs e)
        {
            currentItem = 8;

            inputValuelbl.Show();
            valueInput.Show();
            inputRow.Show();
            InputRowLbl.Show();

            inputBox.Show();
            sendInput.Show();
            inputChoicelbl.Show();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();



            Logger.Init("SensitivityAnalysis.txt");
            sensitivityAnalysis.HandleNonBasicColumnChange();
            Logger.Close();

            UpdateOutput();
        }

        private void addActivityBtn_Click(object sender, EventArgs e)
        {
            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();
            inputBox.Hide();
            sendInput.Hide();
            inputChoicelbl.Hide();

            dataGridViewActs.Show();
            actsBtn.Show();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            dataGridViewActs.Rows.Clear();

            dataGridViewActs.ColumnCount = sensitivityAnalysis.GetConstraints().Count + 1;

            dataGridViewActs.Columns[0].HeaderText = "x";
            for (int i = 1; i < sensitivityAnalysis.GetConstraints().Count + 1; i++)
            {
                dataGridViewActs.Columns[i].HeaderText = $"c{i}";
            }

            for (int i = 0; i < sensitivityAnalysis.GetConstraints().Count + 1; i++)
            {
                dataGridViewActs.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridViewActs.Columns[i].Width = 50; // set the width to 50 pixels
            }

            //dataGridViewActs.RowCount = 1;
            dataGridViewActs.AllowUserToAddRows = false;
            dataGridViewActs.Rows.Add();
        }

        private void addConstraintBtn_Click(object sender, EventArgs e)
        {
            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();
            inputBox.Hide();
            sendInput.Hide();
            inputChoicelbl.Hide();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Show();
            inputConsLbl.Show();
            consBtn.Show();

            dataGridViewCons.Rows.Clear();

            dataGridViewCons.ColumnCount = sensitivityAnalysis.GetColCount() + 1;
            dataGridViewCons.AllowUserToAddRows = true; // users can add inner lists
            dataGridViewCons.AllowUserToDeleteRows = true;

            for (int i = 0; i < sensitivityAnalysis.GetColCount() + 1; i++)
            {
                dataGridViewCons.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dataGridViewCons.Columns[i].Width = 50; // set the width to 50 pixels
            }

            dataGridViewCons.Columns[sensitivityAnalysis.GetColCount() - 1].HeaderText = "Sign";
            dataGridViewCons.Columns[sensitivityAnalysis.GetColCount()].HeaderText = "Rhs";

            for (int i = 0; i < sensitivityAnalysis.GetColCount() - 1; i++)
            {
                dataGridViewCons.Columns[i].HeaderText = $"x{i + 1}";
            }
        }

        private void actsConsBtn_Click(object sender, EventArgs e)
        {
            List<double> activityIn = new List<double>();

            foreach (DataGridViewRow row in dataGridViewActs.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null) continue;

                    string cellValue = cell.Value.ToString().Trim();

                    if (double.TryParse(cellValue, out double value))
                    {
                        activityIn.Add(value);
                    }
                    else
                    {
                        Logger.WriteLine($"Invalid double value: {cellValue}");
                        // Optionally, you can skip or throw an exception
                    }
                }
            }

            // Now pass it to your sensitivityAnalysis logic
            Logger.Init("SensitivityAnalysis.txt");
            try
            {
                var displayCol = sensitivityAnalysis.DoAddActivity(
                    activityIn
                );

                Logger.WriteLine("The New Col to be Added to the tableau");
                foreach (var item in displayCol)
                {
                    Logger.WriteLine($"{item}");
                }

                Logger.WriteLine($"\n");
                var tab = sensitivityAnalysis.GetRevisedTab();

                for (int i = 0; i < displayCol.Count; i++)
                {
                    int insertIndex = Math.Max(tab[i].Count - 1, 0); // second last, or 0 if row is too short
                    tab[i].Insert(insertIndex, displayCol[i]);
                }

                //for (int row = 0; row < tab.Count; row++)
                //{
                //    Logger.WriteLine(string.Join(" ", tab[row]));
                //}

                sensitivityAnalysis.PrintTableau(tab, "New Activity Added");
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Invalid Input");
            }
            finally
            {
                Logger.Close();
            }

            UpdateOutput();
        }

        private void consBtn_Click(object sender, EventArgs e)
        {
            List<List<double>> parsedData = new List<List<double>>();

            foreach (DataGridViewRow row in dataGridViewCons.Rows)
            {
                if (row.IsNewRow) continue;

                List<double> innerList = new List<double>();

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null) continue;

                    string cellValue = cell.Value.ToString().Trim();

                    if (cellValue == "<=")
                    {
                        innerList.Add(0);
                    }
                    else if (cellValue == ">=")
                    {
                        innerList.Add(1);
                    }
                    else if (double.TryParse(cellValue, out double number))
                    {
                        innerList.Add(number);
                    }
                    else
                    {
                        // Handle invalid input if necessary
                        innerList.Add(0); // or throw exception
                    }
                }

                // Swap second-to-last and last elements if there are at least 2 elements
                if (innerList.Count >= 2)
                {
                    int lastIndex = innerList.Count - 1;
                    int secondLastIndex = innerList.Count - 2;

                    double temp = innerList[secondLastIndex];
                    innerList[secondLastIndex] = innerList[lastIndex];
                    innerList[lastIndex] = temp;
                }

                parsedData.Add(innerList);
            }

            Logger.Init("SensitivityAnalysis.txt");
            foreach (List<double> innerList in parsedData)
            {
                // Join the numbers with spaces for printing
                string line = string.Join(" ", innerList);
                Logger.WriteLine(line);
            }

            var (fixedTab, unfixedTab) = sensitivityAnalysis.DoAddConstraint(parsedData, sensitivityAnalysis.GetRevisedTab());

            Logger.Close();
            UpdateOutput();
        }

        private void doDuality_Click(object sender, EventArgs e)
        {
            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Hide();
            sendInput.Hide();
            inputChoicelbl.Hide();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Duality duality = new Duality(true);

            Logger.Init("SensitivityAnalysis.txt");

            duality.RunDuality(sensitivityAnalysis.GetObjFunc(), sensitivityAnalysis.GetConstraints(), sensitivityAnalysis.GetIsMin());
            //duality.Test();
            Logger.Close();

            UpdateOutput();
        }

        private void shadowPriceBtn_Click(object sender, EventArgs e)
        {
            inputValuelbl.Hide();
            valueInput.Hide();
            inputRow.Hide();
            InputRowLbl.Hide();

            inputBox.Hide();
            sendInput.Hide();
            inputChoicelbl.Hide();

            dataGridViewActs.Hide();
            actsBtn.Hide();
            dataGridViewCons.Hide();
            inputConsLbl.Hide();
            consBtn.Hide();

            Duality duality = new Duality(false);

            Logger.Init("SensitivityAnalysis.txt");
            duality.RunDuality(sensitivityAnalysis.GetObjFunc(), sensitivityAnalysis.GetConstraints(), sensitivityAnalysis.GetIsMin());

            duality.PrintShadowPrice();

            Logger.Close();

            UpdateOutput();
        }
    }
}
