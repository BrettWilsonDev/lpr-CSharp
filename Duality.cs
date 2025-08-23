using System;
using System.Collections.Generic;
using System.Linq;

namespace lpr381Project
{
    public class Duality
    {
        private bool isConsoleOutput;
        private DualSimplex dual;


        private List<double> objFunc;
        private double optimalSolution;
        private List<double> changingVars;
        private List<List<double>> constraintsLhs;
        private List<double> cellRef;
        private List<double> dualObjFunc;
        private double dualOptimalSolution;
        private List<double> dualChangingVars;

        private List<List<double>> dualConstraintsLhs;
        private List<double> dualCellRef;

        // dual constraints
        private List<List<double>> constraints;
        private List<string> headerString;
        private List<string> dualHeaderString;
        private List<double> tObjFunc;
        private List<double> tDualObjFunc;
        private string strMin;
        private string strDualMin;

        public Duality(bool isConsoleOutput = false)
        {
            this.isConsoleOutput = isConsoleOutput;
            dual = new DualSimplex();

            objFunc = new List<double>();
            optimalSolution = 0;
            changingVars = new List<double>();
            constraintsLhs = new List<List<double>>();
            cellRef = new List<double>();
            dualObjFunc = new List<double>();
            dualOptimalSolution = 0;
            dualChangingVars = new List<double>();
            dualConstraintsLhs = new List<List<double>>();
            dualCellRef = new List<double>();

            objFunc = new List<double> { 0.0, 0.0 };

            constraints = new List<List<double>> { new List<double> { 0.0, 0.0, 0.0, 0.0 } };
            headerString = new List<string>();
            dualHeaderString = new List<string>();

            tObjFunc = new List<double>();
            tDualObjFunc = new List<double>();

            strMin = "";
            strDualMin = "";
        }

        public List<List<T>> TransposeMat<T>(List<List<T>> matrix)
        {
            if (matrix.Count == 0) return new List<List<T>>();

            int rows = matrix.Count;
            int cols = matrix[0].Count;

            var result = new List<List<T>>();
            for (int j = 0; j < cols; j++)
            {
                var row = new List<T>();
                for (int i = 0; i < rows; i++)
                {
                    row.Add(matrix[i][j]);
                }
                result.Add(row);
            }
            return result;
        }

        public void PrintDuality()
        {
            //try
            //{
            // for display reasons
            tObjFunc = new List<double>(objFunc);
            tDualObjFunc = new List<double>(dualObjFunc);
            tObjFunc.Add(optimalSolution);
            tDualObjFunc.Add(dualOptimalSolution);

            // print header
            for (int i = 0; i < headerString.Count; i++)
            {
                Logger.Write($"{headerString[i],8}  ");
            }
            Logger.WriteLine("\n");

            // display the objective function
            Logger.Write($"{strDualMin + " z",8}  ");
            for (int i = 0; i < tObjFunc.Count; i++)
            {
                Logger.Write($"{tObjFunc[i],8:F3}  ");
            }
            Logger.WriteLine("\n");

            var displayCons = constraintsLhs.Select(row => new List<double>(row)).ToList();

            // build display cons
            for (int i = 0; i < constraintsLhs.Count; i++)
            {
                displayCons[i].Add(cellRef[i]);
                if (constraints[i][constraints[i].Count - 1] == 0)
                {
                    displayCons[i].Add(0); // placeholder for "<=" (will be handled in display)
                }
                else
                {
                    displayCons[i].Add(1); // placeholder for ">=" (will be handled in display)
                }
                displayCons[i].Add(constraints[i][constraints[i].Count - 2]);

                double tSlack = displayCons[i][displayCons[i].Count - 1] - displayCons[i][displayCons[i].Count - 3];
                displayCons[i].Add(tSlack);
            }

            // display the constraints
            for (int i = 0; i < displayCons.Count; i++)
            {
                Logger.Write($"{"c" + (i + 1),8}  ");
                for (int j = 0; j < displayCons[i].Count; j++)
                {
                    if (j == displayCons[i].Count - 3) // sign position
                    {
                        string sign = displayCons[i][j] == 0 ? "<=" : ">=";
                        Logger.Write($"{sign,8}  ");
                    }
                    else
                    {
                        Logger.Write($"{displayCons[i][j],8:F3}  ");
                    }
                }
                Logger.WriteLine();
            }
            Logger.WriteLine("\n");

            // optimal variables
            Logger.Write($"{"opt",8}  ");
            for (int i = 0; i < changingVars.Count; i++)
            {
                Logger.Write($"{changingVars[i],8:F3}  ");
            }
            Logger.WriteLine("\n");

            // dual ==============================================
            Logger.WriteLine(new string('=', 100) + "\n");

            // print dual header
            for (int i = 0; i < dualHeaderString.Count; i++)
            {
                Logger.Write($"{dualHeaderString[i],8}  ");
            }
            Logger.WriteLine("\n");

            // display the dual objective function
            Logger.Write($"{strMin + " z",8}  ");
            for (int i = 0; i < tDualObjFunc.Count; i++)
            {
                Logger.Write($"{tDualObjFunc[i],8:F3}  ");
            }
            Logger.WriteLine("\n");

            var dualDisplayCons = dualConstraintsLhs.Select(row => new List<double>(row)).ToList();

            // build dual display cons
            for (int i = 0; i < dualConstraintsLhs.Count; i++)
            {
                dualDisplayCons[i].Add(dualCellRef[i]);
                dualDisplayCons[i].Add(objFunc[i]);

                double tSlack = dualDisplayCons[i][dualDisplayCons[i].Count - 1] - dualDisplayCons[i][dualDisplayCons[i].Count - 3];
                dualDisplayCons[i].Add(tSlack);
            }

            // display the dual constraints
            for (int i = 0; i < dualDisplayCons.Count; i++)
            {
                Logger.Write($"{"c" + (i + 1),8}  ");
                for (int j = 0; j < dualDisplayCons[i].Count; j++)
                {
                    if (j == dualDisplayCons[i].Count - 3) // sign position
                    {
                        string sign = dualDisplayCons[i][j] == 0 ? "<=" : ">=";
                        Logger.Write($"{sign,8}  ");
                    }
                    else
                    {
                        Logger.Write($"{dualDisplayCons[i][j],8:F3}  ");
                    }
                }
                Logger.WriteLine();
            }
            Logger.WriteLine("\n");

            // optimal dual variables
            Logger.Write($"{"opt",8}  ");
            for (int i = 0; i < dualChangingVars.Count; i++)
            {
                Logger.Write($"{dualChangingVars[i],8:F3}  ");
            }
            Logger.WriteLine("\n");
            //}
            //catch (Exception e)
            //{
            //    Logger.WriteLine("Error: " + e.Message);
            //    throw;
            //}
        }

        public (List<double> objFunc, double optimalSolution, List<double> changingVars, List<List<double>> constraintsLhs, List<double> cellRef, List<double> dualObjFunc, double dualOptimalSolution, List<double> dualChangingVars, List<List<double>> dualConstraintsLhs, List<double> dualCellRef) DoDuality(List<double> objFunc, List<List<double>> constraints, bool isMin)
        {
            var a = new List<double>(objFunc);
            var b = constraints.Select(row => new List<double>(row)).ToList();

            var (tableaus, changingVars, optimalSolution, _, _, _) = dual.DoDualSimplex(a, b, isMin);

            var constraintsLhs = constraints.Select(row => new List<double>(row)).ToList();

            for (int i = 0; i < constraintsLhs.Count; i++)
            {
                constraintsLhs[i].RemoveAt(constraintsLhs[i].Count - 1);
                constraintsLhs[i].RemoveAt(constraintsLhs[i].Count - 1);
            }

            var cellRef = new List<double>();

            for (int i = 0; i < constraintsLhs.Count; i++)
            {
                double tSum = changingVars.Zip(constraintsLhs[i], (x, y) => x * y).Sum();
                cellRef.Add(tSum);
            }

            // fix slack and excess mix
            int slackCtr = 0;
            int excessCtr = 0;
            for (int i = 0; i < constraints.Count; i++)
            {
                if (constraints[i][constraints[i].Count - 1] == 1)
                {
                    excessCtr++;
                }
                else
                {
                    slackCtr++;
                }
            }

            if (slackCtr != constraints.Count && excessCtr != constraints.Count)
            {
                for (int i = 0; i < constraints.Count; i++)
                {
                    if (constraints[i][constraints[i].Count - 1] == 1)
                    {
                        constraints[i][constraints[i].Count - 1] = 0;
                        for (int j = 0; j < constraints[i].Count - 1; j++)
                        {
                            constraints[i][j] = -constraints[i][j];
                        }
                    }
                }
            }

            var dualConstraints = constraints.Select(row => new List<double>(row)).ToList();

            // duality
            var dualObjFunc = new List<double>();
            for (int i = 0; i < constraintsLhs.Count; i++)
            {
                dualObjFunc.Add(constraints[i][constraints[i].Count - 2]);
            }

            dualConstraints = TransposeMat(constraintsLhs);

            for (int i = 0; i < dualConstraints.Count; i++)
            {
                dualConstraints[i].Add(objFunc[i]);
            }

            for (int i = 0; i < constraints.Count; i++)
            {
                if (i >= dualConstraints.Count)
                {
                    break;
                }
                if (constraints[i][constraints[i].Count - 1] == 1)
                {
                    dualConstraints[i].Add(0);
                }
                else
                {
                    dualConstraints[i].Add(1);
                }
            }

            for (int i = 0; i < dualObjFunc.Count; i++)
            {
                if (dualObjFunc[i] < 0)
                {
                    for (int j = 0; j < dualConstraints.Count; j++)
                    {
                        dualConstraints[j][i] = -dualConstraints[j][i];
                    }
                }
            }

            isMin = !isMin;

            var a2 = new List<double>(dualObjFunc);
            var b2 = dualConstraints.Select(row => new List<double>(row)).ToList();

            var (tableaus2, dualChangingVars, dualOptimalSolution, _, _, _) = dual.DoDualSimplex(a2, b2, isMin);

            var dualConstraintsLhs = dualConstraints.Select(row => new List<double>(row)).ToList();

            for (int i = 0; i < dualConstraintsLhs.Count; i++)
            {
                dualConstraintsLhs[i].RemoveAt(dualConstraintsLhs[i].Count - 1);
                dualConstraintsLhs[i].RemoveAt(dualConstraintsLhs[i].Count - 1);
            }

            var dualCellRef = new List<double>();

            for (int i = 0; i < dualConstraintsLhs.Count; i++)
            {
                double tSum = dualChangingVars.Zip(dualConstraintsLhs[i], (x, y) => x * y).Sum();
                dualCellRef.Add(tSum);
            }

            // save primal results
            this.optimalSolution = (double)optimalSolution;
            this.changingVars = changingVars;
            this.constraintsLhs = constraintsLhs;
            this.cellRef = cellRef;

            // save dual results
            this.dualObjFunc = dualObjFunc;
            this.dualOptimalSolution = (double)dualOptimalSolution;
            this.dualChangingVars = dualChangingVars;
            this.dualConstraintsLhs = dualConstraintsLhs;
            this.dualCellRef = dualCellRef;

            // headers (optional: build them from objFunc sizes)
            headerString = Enumerable.Range(0, this.objFunc.Count).Select(i => $"x{i + 1}").ToList();
            headerString.Add("z");
            dualHeaderString = Enumerable.Range(0, this.dualObjFunc.Count).Select(i => $"y{i + 1}").ToList();
            dualHeaderString.Add("z");

            headerString.Insert(0, "primal");
            dualHeaderString.Insert(0, "dual");

            // print result tables
            if (isConsoleOutput)
            {
                PrintDuality();

                if (optimalSolution == dualOptimalSolution)
                {
                    Logger.WriteLine($"Strong Duality {optimalSolution} is equal to {dualOptimalSolution}");
                }
                else
                {
                    Logger.WriteLine($"Weak Duality {optimalSolution} is not equal to {dualOptimalSolution}");
                }
            }

            return (objFunc, (double)optimalSolution, changingVars, constraintsLhs, cellRef, dualObjFunc, (double)dualOptimalSolution, dualChangingVars, dualConstraintsLhs, dualCellRef);
        }

        public void RunDuality(List<double> objFuncPassed, List<List<double>> constraintsPassed, bool isMin)
        {
            objFunc = objFuncPassed.ToList();
            constraints = constraintsPassed.Select(x => x.ToList()).ToList();
            DoDuality(objFunc, constraints, isMin);
        }

        public void PrintShadowPrice()
        {
            Logger.Write($"{"Shadow Price:",8}  ");
            for (int i = 0; i < dualChangingVars.Count; i++)
            {
                Logger.Write($"{dualChangingVars[i],8:F3}  ");
            }
            Logger.WriteLine("\n");
        }
    }
}