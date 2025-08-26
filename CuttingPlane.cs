using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace lpr381Project
{
    public class CuttingPlane
    {
        private bool isConsoleOutput;
        private int precision = 4;
        private double tolerance = 1e-6;
        private int maxIterations = 10;

        private DualSimplex dual;

        private bool isMin;

        // simplex specific vars
        private string problemType;
        private string absProblemType;

        // dual constraints
        private int amtOfObjVars;
        private List<double> objFunc;

        private List<List<double>> constraints;

        public CuttingPlane(bool isConsoleOutput = false)
        {
            this.isConsoleOutput = isConsoleOutput;
            this.precision = 4;
            this.tolerance = 1e-6;
            this.maxIterations = 10;
            dual = new DualSimplex();

            isMin = false;

            // simplex specific vars
            problemType = "Max";
            absProblemType = "abs Off";

            // dual constraints
            amtOfObjVars = 2;
            objFunc = new List<double> { 0.0, 0.0 };

            constraints = new List<List<double>> { new List<double> { 0.0, 0.0, 0.0, 0.0 } };
        }

        public double roundValue(double value)
        {
            try
            {
                double rounded = Math.Round(value, precision);
                return rounded;
            }
            catch
            {
                return value;
            }
        }

        public void printTableau(List<List<double>> tableau, string title = "Tableau")
        {
            var tempHeaderStr = new List<string>();
            for (int i = 0; i < objFunc.Count; i++)
            {
                tempHeaderStr.Add($"x{i + 1}");
            }
            for (int i = 0; i < (tableau[tableau.Count - 1].Count - objFunc.Count - 1); i++)
            {
                tempHeaderStr.Add($"s/e{i + 1}");
            }
            tempHeaderStr.Add("rhs");


            Logger.WriteLine($"\n{title}");
            foreach (var header in tempHeaderStr)
            {
                Logger.Write($"{header,8}  ");
            }
            Logger.WriteLine();

            foreach (var row in tableau)
            {
                foreach (var val in row)
                {
                    Logger.Write($"{roundValue(val),8:F4}  ");
                }
                Logger.WriteLine();
            }
            Logger.WriteLine();

        }

        public List<List<double>> roundMatrix(List<List<double>> matrix)
        {
            return matrix.Select(row => row.Select(val => roundValue(val)).ToList()).ToList();
        }

        public double cleanValue(double value, double tolerance = 1e-10)
        {
            if (Math.Abs(value) < tolerance)
                return 0.0;

            return value;
        }

        public List<double> gomoryCut(List<double> row, bool verbose = false)
        {
            if (row.Count < 2)
            {
                throw new ArgumentException("Input must have at least one coefficient and RHS.");
            }

            // Clean the input row first
            List<double> cleanRow = row.Select(x => cleanValue(x)).ToList();

            // Use exact arithmetic simulation
            List<double> coefs = cleanRow.Take(cleanRow.Count - 1).ToList();
            double rhs = cleanRow[cleanRow.Count - 1];

            double getFractionalPart(double x)
            {
                int integer_part = (int)x;
                double fractional_part = x - integer_part;

                // If negative, adjust the fractional part to be positive
                if (fractional_part < 0)
                {
                    fractional_part += 1;
                }

                return fractional_part;
            }

            if (verbose)
            {
                Logger.WriteLine($"Original row: {string.Join(", ", coefs.Select(c => c.ToString("F4")))} | {rhs:F4}");
            }

            // Generate the Gomory cut using exact fraction arithmetic
            List<double> fracCoefs = new List<double>();
            for (int i = 0; i < coefs.Count; i++)
            {
                double frac_part = getFractionalPart(coefs[i]);
                fracCoefs.Add(-frac_part);
            }

            double rhsFrac = getFractionalPart(rhs);
            double negFracRhs = -rhsFrac;

            // Convert back to floats and clean
            List<double> result = fracCoefs.Select(x => cleanValue(x)).ToList();
            result.Add(cleanValue(negFracRhs));

            if (verbose)
            {
                Logger.WriteLine($"Gomory cut coefficients: {string.Join(", ", result.Select(r => r.ToString("F4")))}");
            }

            return result;
        }

        public List<int> getBasicVarSpots(List<List<List<double>>> tableaus)
        {
            // get the spots of the basic variables
            List<int> basicVarSpots = new List<int>();
            List<List<double>> tableau = tableaus[tableaus.Count - 1];

            for (int k = 0; k < tableau[0].Count; k++)
            {
                int columnIndex = k;
                List<double> column = new List<double>();
                for (int i = 0; i < tableau.Count; i++)
                {
                    column.Add(cleanValue(tableau[i][columnIndex]));
                }

                // Check if this is a unit vector (exactly one 1, rest 0s)
                int ones_count = column.Count(x => Math.Abs(x - 1.0) < tolerance);
                int zeros_count = column.Count(x => Math.Abs(x) < tolerance);

                if (ones_count == 1 && zeros_count == column.Count - 1)
                {
                    basicVarSpots.Add(k);
                }
            }

            return basicVarSpots;
        }

        public int findMostFractionalRow(List<List<double>> tableau)
        {
            double bestFractionalScore = -1;
            int selectedRow = 1;
            List<(int, double, double)> candidates = new List<(int, double, double)>();

            if (isConsoleOutput)
            {
                Logger.WriteLine($"\nAnalyzing fractional parts:");
            }

            for (int i = 1; i < tableau.Count; i++) // Skip objective row
            {
                double rhsValue = cleanValue(tableau[i][tableau[i].Count - 1]);

                // Calculate fractional part using floor method
                double fractionalPart = rhsValue - Math.Floor(rhsValue);

                // Calculate how "fractional" this is - distance from nearest 0 or 1
                // Most fractional is closest to 0.5
                double fractionalScore = Math.Min(fractionalPart, 1 - fractionalPart);

                if (isConsoleOutput)
                {
                    Logger.WriteLine($"Row {i}: RHS = {rhsValue:F4}, fractional part = {fractionalPart:F4}, score = {fractionalScore:F4}");
                }

                // Only consider rows with significant fractional parts
                if (fractionalPart > tolerance && fractionalPart < (1 - tolerance))
                {
                    if (fractionalScore > bestFractionalScore)
                    {
                        bestFractionalScore = fractionalScore;
                        candidates = new List<(int, double, double)> { (i, fractionalPart, rhsValue) };
                    }
                    else if (Math.Abs(fractionalScore - bestFractionalScore) < tolerance)
                    {
                        candidates.Add((i, fractionalPart, rhsValue));
                    }
                }
            }

            // When scores are tied, pick the row with the largest fractional part
            // If fractional parts are also tied, pick the smallest RHS value
            if (candidates.Count > 0)
            {
                // Sort by fractional part (desc), then by RHS value (asc) for tie-breaking
                candidates = candidates.OrderByDescending(x => x.Item2).ThenBy(x => x.Item3).ToList();
                selectedRow = candidates[0].Item1;

                if (isConsoleOutput)
                {
                    Logger.WriteLine($"Candidates with score {bestFractionalScore:F4}: {string.Join(", ", candidates.Select(c => $"({c.Item1}, RHS={c.Item3:F4}, frac={c.Item2:F4})"))}");
                }
            }

            if (isConsoleOutput)
            {
                Logger.WriteLine($"Selected row {selectedRow}");
            }

            return selectedRow;
        }

        public bool hasFractionalSolution(List<List<double>> tableau)
        {
            for (int i = 1; i < tableau.Count; i++) // Skip objective row
            {
                double rhsValue = cleanValue(tableau[i][tableau[i].Count - 1]);

                // Calculate fractional part using floor method
                double fractionalPart = rhsValue - Math.Floor(rhsValue);

                // Check if the fractional part is significant
                if (fractionalPart > tolerance)
                {
                    return true;
                }
            }

            return false;
        }

        public List<List<double>> cleanTableau(List<List<double>> tableau)
        {
            List<List<double>> cleanedTableau = new List<List<double>>();
            for (int i = 0; i < tableau.Count; i++)
            {
                List<double> cleanedRow = new List<double>();
                for (int j = 0; j < tableau[i].Count; j++)
                {
                    cleanedRow.Add(cleanValue(tableau[i][j]));
                }
                cleanedTableau.Add(cleanedRow);
            }
            return cleanedTableau;
        }

        public void PrintBasicVars(List<List<double>> tableau)
        {
            Logger.WriteLine("\n=== BASIC VARIABLE VALUES (Decision Variables Only) ===");

            List<int> basicVarColumns = new List<int>();

            // Only check decision variables (first objFunc.Count columns)
            for (int col = 0; col < objFunc.Count; col++)
            {
                List<double> column = new List<double>();

                // Extract the column values (skip objective row for basic variable identification)
                for (int row = 1; row < tableau.Count; row++)
                {
                    column.Add(cleanValue(tableau[row][col]));
                }

                // Check if this is a unit vector (exactly one 1, rest 0s)
                int onesCount = column.Count(x => Math.Abs(x - 1.0) < tolerance);
                int zerosCount = column.Count(x => Math.Abs(x) < tolerance);

                if (onesCount == 1 && zerosCount == column.Count - 1)
                {
                    basicVarColumns.Add(col);
                }
            }

            foreach (int col in basicVarColumns)
            {
                // Find which row has the "1" for this basic variable
                int basicRow = -1;
                for (int row = 1; row < tableau.Count; row++) // Skip objective row
                {
                    if (Math.Abs(cleanValue(tableau[row][col]) - 1.0) < tolerance)
                    {
                        basicRow = row;
                        break;
                    }
                }

                if (basicRow != -1)
                {
                    double rhsValue = cleanValue(tableau[basicRow][tableau[basicRow].Count - 1]);
                    string varName = $"x{col + 1}";
                    Logger.WriteLine($"{varName} = {roundValue(rhsValue)}");
                }
            }

            Logger.WriteLine();
        }

        public List<List<double>> doCuttingPlane(List<List<double>> workingTableau)
        {
            List<List<double>> currentTableau = cleanTableau(workingTableau);
            int iteration = 1;

            // Recursive cutting plane loop
            while (hasFractionalSolution(currentTableau) && iteration <= maxIterations)
            {
                Logger.WriteLine($"\n=== CUTTING PLANE ITERATION {iteration} ===");

                // Clean tableau before processing
                currentTableau = cleanTableau(currentTableau);

                // Find the most fractional row for Gomory cut
                int pickedRow = findMostFractionalRow(currentTableau);

                Logger.WriteLine($"Selected row {pickedRow} for Gomory cut: {string.Join(", ", currentTableau[pickedRow].Select(x => roundValue(x).ToString("F4")))}");

                // Generate Gomory cut from the selected row
                List<double> tempList = currentTableau[pickedRow].Skip(objFunc.Count).ToList(); // Remove objective function coefficients
                List<double> newCon = gomoryCut(tempList, verbose: true);

                // Clean the new constraint
                newCon = newCon.Select(x => cleanValue(x)).ToList();

                // Prepare the new constraint for tableau format
                for (int i = 0; i < objFunc.Count; i++)
                {
                    newCon.Insert(i, 0);
                }

                newCon.Insert(newCon.Count - 1, 1); // Add slack variable coefficient
                Logger.WriteLine($"Generated cutting plane constraint: {string.Join(", ", newCon.Select(x => roundValue(x).ToString("F4")))}");

                // Add new column for the slack variable
                Logger.WriteLine("Adding new slack variable column...");
                for (int i = 0; i < currentTableau.Count; i++)
                {
                    currentTableau[i].Insert(currentTableau[i].Count - 1, 0);
                }

                // Add the new constraint row
                currentTableau.Add(newCon);

                printTableau(currentTableau, title: $"Tableau with cutting plane constraint {iteration}");

                // Solve the new problem with dual simplex
                var result = dual.DoDualSimplex(new List<double>(), new List<List<double>>(), isMin, currentTableau);
                List<List<List<double>>> finalTableaus = result.Item1;
                var headerStr = result.Item6;

                for (int i = 0; i < finalTableaus.Count; i++)
                {
                    printTableau(finalTableaus[i], title: $"Iteration {iteration} - Tableau {i + 1}");
                }

                // Clean and update current tableau for next iteration
                currentTableau = cleanTableau(finalTableaus[finalTableaus.Count - 1]);
                iteration += 1;
            }

            // Final results
            if (!hasFractionalSolution(currentTableau))
            {
                Logger.WriteLine($"\n=== OPTIMAL INTEGER SOLUTION FOUND ===");
                Logger.WriteLine($"Solution achieved after {iteration - 1} cutting plane iterations");
            }
            else
            {
                Logger.WriteLine($"\n=== MAXIMUM ITERATIONS REACHED ===");
                Logger.WriteLine($"Stopped after {maxIterations} iterations");
            }

            printTableau(currentTableau, title: "Final Optimal Tableau");

            double optimalSolution = currentTableau[0][currentTableau[0].Count - 1];
            Logger.WriteLine($"Optimal Solution: {roundValue(optimalSolution)}");

            PrintBasicVars(currentTableau);

            return currentTableau;
        }

        public (List<double> objFunc, List<List<double>> constraints) SetUpProblem(
            List<double> objFunc,
            List<List<double>> constraints,
            List<VariableSignType> varSigns = null
        )
        {
            int numConstraints = objFunc.Count;
            int vectorLength = objFunc.Count + 3;    // columns including slack + RHS

            for (int i = 0; i < numConstraints; i++)
            {
                if (varSigns != null && varSigns[i] == VariableSignType.Binary)
                {
                    var row = new List<double>(new double[vectorLength]);

                    // Put a "1" at column i (the decision variable column)
                    row[i] = 1;

                    // Put a "1" in the slack column (vectorLength - 2)
                    row[vectorLength - 2] = 1;

                    // RHS (last element) stays 0
                    constraints.Add(row);
                }
            }

            return (objFunc, constraints);
        }

        public void RunCuttingPlane(List<double> objFuncPassed, List<List<double>> constraintsPassed, bool isMinPassed, List<VariableSignType> varSigns = null)
        {
            objFunc = objFuncPassed.ToList();
            constraints = constraintsPassed.Select(x => x.ToList()).ToList();
            isMin = isMinPassed;

            (objFunc, constraints) = SetUpProblem(objFunc, constraints, varSigns);

            CanonicalFormBuilder.BuildCanonicalForm(objFunc, constraints);

            // Initial dual simplex solution
            var result = dual.DoDualSimplex(objFunc, constraints, isMin);
            List<List<List<double>>> workingTableaus = result.Item1;

            for (int i = 0; i < workingTableaus.Count; i++)
            {
                printTableau(workingTableaus[i], title: $"Initial Tableau {i + 1}");
            }

            doCuttingPlane(workingTableaus[workingTableaus.Count - 1]);
        }
    }
}