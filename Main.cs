using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static lpr381Project.BranchAndBoundKnapsack;
using static lpr381Project.RevisedVariableTransformation;

namespace lpr381Project
{
    public partial class Main : Form
    {
        List<double> objFunc;
        List<List<double>> constraints;
        bool isMin;
        List<VariableSignType> varSigns = null;

        public Main()
        {
            InitializeComponent();
        }

        private void UpdateOutput()
        {
            string filePath = @"output.txt"; // your file path

            if (File.Exists(filePath))
            {
                mainTextDisplay.Text = File.ReadAllText(filePath);
            }
            else
            {
                MessageBox.Show("File not found: " + filePath);
            }
        }

        private void UpdateInput()
        {
            InputReader reader = new InputReader("input.txt");
            var data = reader.GetData();
            (objFunc, constraints, isMin, varSigns) = data;

            if (File.Exists("input.txt"))
            {
                mainInputBox.Text = File.ReadAllText("input.txt");
            }

            File.WriteAllText("input.txt", mainInputBox.Text);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Logger.Close();

            UpdateInput();

            // Configure RichTextBox
            mainTextDisplay.WordWrap = false;               // prevent wrapping
            mainTextDisplay.Font = new Font("Consolas", 10); // monospaced font
            mainTextDisplay.ScrollBars = RichTextBoxScrollBars.Both; // horizontal + vertical
        }

        private void enterInput_Click(object sender, EventArgs e)
        {
            File.WriteAllText("input.txt", mainInputBox.Text);
            UpdateInput();
        }

        private void primalSimplexBtn_Click(object sender, EventArgs e)
        {
            UpdateInput();

            Logger.Init("output.txt");
            try
            {
                Logger.WriteLine("Primal Simplex\n");
                PrimalSimplex primalSimplex = new PrimalSimplex(true);
                // test for >=
                int ctr = 0;
                foreach (var constraint in constraints)
                {
                    ctr++;
                    if (constraint.Count > 0 && constraint[constraint.Count - 1] == 1)
                    {
                        // Do something if the last element is 1
                        Logger.WriteLine($"Primal Simplex Cannot handle >= constraint Assuming You wanted <= for c{ctr}");
                        Logger.WriteLine("\n");
                    }
                }
                primalSimplex.RunPrimalSimplex(objFunc, constraints, isMin, varSigns);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"\nInvalid Input or Fatel Error!");
            }
            Logger.Close();

            UpdateOutput();
        }

        private void revisedPrimal_Click(object sender, EventArgs e)
        {
            UpdateInput();

            Logger.Init("output.txt");
            try
            {
                Logger.WriteLine("Revised Primal Simplex\n");
                RevisedPrimalSimplex revisedPrimalSimplex = new RevisedPrimalSimplex(true);
                revisedPrimalSimplex.RunRevisedPrimalSimplex(objFunc, constraints, isMin, varSigns);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"\nInvalid Input or Fatel Error!");
            }
            Logger.Close();

            UpdateOutput();
        }

        private void branchAndBoundBtn_Click(object sender, EventArgs e)
        {
            UpdateInput();

            Logger.Init("output.txt");
            Logger.WriteLine("Branch And Bound Simplex\n");
            BranchAndBound branchAndBound = new BranchAndBound(true);
            branchAndBound.RunBranchAndBound(objFunc, constraints, isMin, varSigns);
            Logger.Close();

            UpdateOutput();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateInput();

            Logger.Init("output.txt");
            try
            {
                Logger.WriteLine("Branch And Bound KnapSack\n");
                KnapSack knapSack = new KnapSack(true);
                knapSack.RunBranchAndBoundKnapSack(objFunc, constraints, isMin, varSigns);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"\nInvalid Input or Fatel Error!");
            }
            Logger.Close();

            UpdateOutput();
        }

        private void cuttingPlaneBtn_Click(object sender, EventArgs e)
        {
            UpdateInput();

            Logger.Init("output.txt");
            try
            {
                Logger.WriteLine("Cutting Plane\n");
                CuttingPlane cuttingPlane = new CuttingPlane(true);
                cuttingPlane.RunCuttingPlane(objFunc, constraints, isMin, varSigns);
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"\nInvalid Input or Fatel Error!");
            }
            Logger.Close();

            UpdateOutput();
        }

        private void sensitivityAnalysisBtn_Click(object sender, EventArgs e)
        {
            Logger.Init("output.txt");
            try
            {
                var analysis = new Analysis();
                analysis.SetProblem(objFunc, constraints, isMin, varSigns);
                analysis.Show();
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"\nInvalid Input or Fatel Error!");
            }
            Logger.Close();
        }
    }
}
