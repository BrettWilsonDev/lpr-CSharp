using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace lpr381Project
{

    public enum VariableSignType
    {
        Positive,      // '+' - x >= 0
        Negative,      // '-' - x <= 0
        Unrestricted,  // 'urs' - x unrestricted
        Integer,       // 'int' - x integer
        Binary         // 'bin' - x binary
    }

    public class VariableTransformation
    {
        public string Type { get; set; }
        public int OriginalIndex { get; set; }
        public int TransformedIndex { get; set; }
        public int? TransformedIndexNeg { get; set; } // For unrestricted variables

        public VariableTransformation(string type, int originalIndex, int transformedIndex, int? transformedIndexNeg = null)
        {
            Type = type;
            OriginalIndex = originalIndex;
            TransformedIndex = transformedIndex;
            TransformedIndexNeg = transformedIndexNeg;
        }
    }

    public class PrimalSimplex
    {
        private bool isConsoleOutput;

        private List<int> imPivotCols;
        private List<int> imPivotRows;
        private List<string> imHeaderRow;

        private List<double> objFunc;
        private List<List<double>> constraints;
        private List<int> phases;

        // For variable transformations
        private int originalVarCount;
        private List<VariableTransformation> varTransformations;
        private List<string> transformedVarNames;

        public PrimalSimplex(bool isConsoleOutput = false)
        {
            this.isConsoleOutput = isConsoleOutput;
            imPivotCols = new List<int>();
            imPivotRows = new List<int>();
            imHeaderRow = new List<string>();
            objFunc = new List<double> { 0.0, 0.0 };
            constraints = new List<List<double>> { new List<double> { 0.0, 0.0, 0.0 } };
            phases = new List<int>();
        }

        public (List<double> objFunc, List<List<double>> constraints) TransformVariableSignRestrictions(
    List<double> objFunc, List<List<double>> constraints, List<VariableSignType> varSigns)
        {
            if (varSigns == null)
            {
                varSigns = Enumerable.Repeat(VariableSignType.Positive, objFunc.Count).ToList();
            }

            // Store original variable information for solution interpretation
            originalVarCount = objFunc.Count;
            varTransformations = new List<VariableTransformation>();

            var newObjFunc = new List<double>();
            var newConstraints = new List<List<double>>();

            // Track binary and integer variables for later constraint addition
            var binaryVarIndices = new List<int>();
            var integerVarIndices = new List<int>();

            // Initialize new constraints (copy original constraints structure)
            for (int i = 0; i < constraints.Count; i++)
            {
                newConstraints.Add(new List<double>());
            }

            int varIndex = 0;
            transformedVarNames = new List<string>();

            // Process each original variable
            for (int i = 0; i < varSigns.Count; i++)
            {
                var sign = varSigns[i];

                switch (sign)
                {
                    case VariableSignType.Positive:
                        // Standard non-negative variable
                        newObjFunc.Add(objFunc[i]);
                        varTransformations.Add(new VariableTransformation("standard", i, varIndex));
                        transformedVarNames.Add($"x{i + 1}");

                        // Update constraints
                        for (int j = 0; j < constraints.Count; j++)
                        {
                            newConstraints[j].Add(constraints[j][i]);
                        }
                        varIndex++;
                        break;

                    case VariableSignType.Negative:
                        // Non-positive variable: x <= 0 → substitute x = -x' where x' >= 0
                        newObjFunc.Add(-objFunc[i]); // Negate coefficient
                        varTransformations.Add(new VariableTransformation("negative", i, varIndex));
                        transformedVarNames.Add($"x{i + 1}'");

                        // Update constraints (negate coefficients)
                        for (int j = 0; j < constraints.Count; j++)
                        {
                            newConstraints[j].Add(-constraints[j][i]);
                        }
                        varIndex++;
                        break;

                    case VariableSignType.Unrestricted:
                        // Unrestricted variable: x = x+ - x- where x+, x- >= 0
                        newObjFunc.Add(objFunc[i]);
                        newObjFunc.Add(-objFunc[i]);
                        varTransformations.Add(new VariableTransformation("unrestricted", i, varIndex, varIndex + 1));
                        transformedVarNames.Add($"x{i + 1}+");
                        transformedVarNames.Add($"x{i + 1}-");

                        // Update constraints
                        for (int j = 0; j < constraints.Count; j++)
                        {
                            newConstraints[j].Add(constraints[j][i]);      // x+
                            newConstraints[j].Add(-constraints[j][i]);     // x-
                        }
                        varIndex += 2;
                        break;

                    case VariableSignType.Integer:
                        // Integer variable
                        newObjFunc.Add(objFunc[i]);
                        varTransformations.Add(new VariableTransformation("integer", i, varIndex));
                        transformedVarNames.Add($"x{i + 1}(int)");
                        integerVarIndices.Add(varIndex); // Track for later constraint addition

                        for (int j = 0; j < constraints.Count; j++)
                        {
                            newConstraints[j].Add(constraints[j][i]);
                        }
                        varIndex++;
                        break;

                    case VariableSignType.Binary:
                        // Binary variable
                        newObjFunc.Add(objFunc[i]);
                        varTransformations.Add(new VariableTransformation("binary", i, varIndex));
                        transformedVarNames.Add($"x{i + 1}(bin)");
                        binaryVarIndices.Add(varIndex); // Track for later constraint addition

                        for (int j = 0; j < constraints.Count; j++)
                        {
                            newConstraints[j].Add(constraints[j][i]);
                        }
                        varIndex++;
                        break;
                }
            }

            // Add RHS to existing constraints
            for (int i = 0; i < constraints.Count; i++)
            {
                newConstraints[i].Add(constraints[i][constraints[i].Count - 1]); // Add original RHS
            }

            // Now add binary constraints (x <= 1) - diagonal pattern
            foreach (int binaryVarIndex in binaryVarIndices)
            {
                var binaryConstraint = new List<double>();

                // Initialize constraint with zeros for all variables
                for (int k = 0; k < varIndex; k++)
                {
                    if (k == binaryVarIndex)
                        binaryConstraint.Add(1); // Diagonal: 1 for the binary variable
                    else
                        binaryConstraint.Add(0); // 0 for all other variables
                }

                // Add RHS = 1 for x <= 1 constraint
                binaryConstraint.Add(1);

                newConstraints.Add(binaryConstraint);
            }

            // Add integer constraints if needed (similar pattern)
            // Note: Integer constraints might need different handling depending on your requirements
            foreach (int integerVarIndex in integerVarIndices)
            {
                // Add integer-specific constraints here if needed
                // For now, just treating them as non-negative
            }

            return (newObjFunc, newConstraints);
        }

        public List<double> InterpretSolution(List<double> transformedSolution)
        {
            if (varTransformations == null || varTransformations.Count == 0)
            {
                return transformedSolution;
            }

            var originalSolution = new List<double>(new double[originalVarCount]);

            foreach (var transformation in varTransformations)
            {
                switch (transformation.Type)
                {
                    case "standard":
                        // x_original = x_transformed
                        originalSolution[transformation.OriginalIndex] = transformedSolution[transformation.TransformedIndex];
                        break;

                    case "negative":
                        // x_original = -x_transformed
                        originalSolution[transformation.OriginalIndex] = -transformedSolution[transformation.TransformedIndex];
                        break;

                    case "unrestricted":
                        // x_original = x+ - x-
                        originalSolution[transformation.OriginalIndex] =
                            transformedSolution[transformation.TransformedIndex] -
                            transformedSolution[transformation.TransformedIndexNeg.Value];
                        break;

                    case "integer":
                    case "binary":
                        // Same as standard for LP relaxation
                        originalSolution[transformation.OriginalIndex] = transformedSolution[transformation.TransformedIndex];
                        break;
                }
            }

            return originalSolution;
        }

        public List<List<double>> DoFormulationOperation(List<double> objFunc, List<List<double>> constraints)
        {
            // For primal simplex, all constraints are assumed to be <= with non-negative RHS
            int tableSizeH = constraints.Count + 1;

            // Build the display header row using transformed variable names if available
            imHeaderRow.Clear();
            if (transformedVarNames != null && transformedVarNames.Count > 0)
            {
                imHeaderRow.AddRange(transformedVarNames);
            }
            else
            {
                for (int i = 0; i < objFunc.Count; i++)
                {
                    imHeaderRow.Add($"x{i + 1}");
                }
            }

            // Add slack variables
            for (int i = 0; i < constraints.Count; i++)
            {
                imHeaderRow.Add($"s{i + 1}");
            }
            imHeaderRow.Add("rhs");

            int tableSizeW = objFunc.Count + constraints.Count + 1;
            var opTable = new List<List<double>>();

            for (int i = 0; i < tableSizeH; i++)
            {
                opTable.Add(new List<double>(new double[tableSizeW]));
            }

            // For maximization, negate the objective function coefficients
            for (int i = 0; i < objFunc.Count; i++)
            {
                opTable[0][i] = -objFunc[i];
            }

            // Add constraint coefficients and RHS
            for (int i = 0; i < constraints.Count; i++)
            {
                for (int j = 0; j < constraints[i].Count - 1; j++)
                {
                    opTable[i + 1][j] = constraints[i][j];
                }
                opTable[i + 1][opTable[i + 1].Count - 1] = constraints[i][constraints[i].Count - 1];
            }

            // Add slack variables (identity matrix)
            for (int i = 1; i < opTable.Count; i++)
            {
                int slackCol = objFunc.Count + i - 1;
                opTable[i][slackCol] = 1;
            }

            return opTable;
        }

        public (List<List<double>> newTab, List<double> thetaCol) DoPrimalPivotOperation(List<List<double>> tab, bool isMin = false)
        {
            var thetasCol = new List<double>();

            // Find entering variable (most negative for maximization, most positive for minimization)
            var testRow = tab[0].Take(tab[0].Count - 1).ToList();

            int colIndex = -1;
            if (isMin)
            {
                // For minimization, choose most positive coefficient
                if (testRow.All(num => num <= 0))
                {
                    return (null, null); // Optimal solution found
                }
                double largestPositiveNumber = testRow.Where(num => num > 0).Max();
                colIndex = tab[0].ToList().IndexOf(largestPositiveNumber);
            }
            else
            {
                // For maximization, choose most negative coefficient
                if (testRow.All(num => num >= 0))
                {
                    return (null, null); // Optimal solution found
                }
                double largestNegativeNumber = testRow.Where(num => num < 0).Min();
                colIndex = tab[0].ToList().IndexOf(largestNegativeNumber);
            }

            // Find leaving variable using minimum ratio test
            var thetas = new List<double>();
            for (int i = 1; i < tab.Count; i++)
            {
                if (tab[i][colIndex] > 0)
                {
                    thetas.Add(tab[i][tab[i].Count - 1] / tab[i][colIndex]);
                }
                else
                {
                    thetas.Add(double.PositiveInfinity);
                }
            }

            thetasCol = new List<double>(thetas);

            // Check for unbounded solution
            if (thetas.All(theta => double.IsPositiveInfinity(theta)))
            {
                if (isConsoleOutput)
                {
                    Logger.WriteLine("Unbounded solution");
                }
                return (null, null);
            }

            // Find minimum positive ratio
            double minTheta = thetas.Where(theta => theta >= 0 && !double.IsPositiveInfinity(theta)).Min();
            int rowIndex = thetas.IndexOf(minTheta) + 1;

            // Perform pivot operation
            var oldTab = tab.Select(row => new List<double>(row)).ToList();
            var newTab = oldTab.Select(row => new List<double>(new double[row.Count])).ToList();

            double divNumber = oldTab[rowIndex][colIndex];

            if (Math.Abs(divNumber) < 1e-10)
            {
                return (null, null);
            }

            // Normalize pivot row
            for (int j = 0; j < oldTab[rowIndex].Count; j++)
            {
                newTab[rowIndex][j] = oldTab[rowIndex][j] / divNumber;
                if (Math.Abs(newTab[rowIndex][j]) < 1e-10)
                {
                    newTab[rowIndex][j] = 0.0;
                }
            }

            // Eliminate other rows
            for (int i = 0; i < oldTab.Count; i++)
            {
                if (i == rowIndex)
                    continue;

                for (int j = 0; j < oldTab[i].Count; j++)
                {
                    double mathItem = oldTab[i][j] - (oldTab[i][colIndex] * newTab[rowIndex][j]);
                    newTab[i][j] = mathItem;
                    if (Math.Abs(newTab[i][j]) < 1e-10)
                    {
                        newTab[i][j] = 0.0;
                    }
                }
            }

            if (isConsoleOutput)
            {
                Logger.WriteLine($"Pivot column: {colIndex + 1}, Pivot row: {rowIndex + 1}");
            }

            imPivotCols.Add(colIndex);
            imPivotRows.Add(rowIndex);

            return (newTab, thetasCol);
        }

        public (List<List<double>> tab, bool isMin, int amtOfE, int amtOfS, int lenObj) GetInput(
            List<double> objFunc, List<List<double>> constraints, bool isMin, List<VariableSignType> varSigns = null)
        {
            // Transform variables based on sign restrictions
            if (varSigns != null)
            {
                var transformed = TransformVariableSignRestrictions(objFunc, constraints, varSigns);
                objFunc = transformed.objFunc;
                constraints = transformed.constraints;
            }

            var tab = DoFormulationOperation(objFunc, constraints);
            return (tab, isMin, 0, constraints.Count, objFunc.Count);
        }

        public (List<List<List<double>>> tableaus, List<double> changingVars, double optimalSolution,
                List<int> pivotCols, List<int> pivotRows, List<string> headerRow) DoPrimalSimplex(
            List<double> objFunc, List<List<double>> constraints, bool isMin = false,
            List<VariableSignType> varSigns = null, List<List<double>> tabOverride = null)
        {
            var thetaCols = new List<List<double>>();
            var tableaus = new List<List<List<double>>>();

            var (tab, isMinParam, amtOfE, amtOfS, lenObj) = GetInput(objFunc, constraints, isMin, varSigns);

            // For use in other tools
            if (tabOverride != null)
            {
                tab = tabOverride;
                imPivotCols.Clear();
                imPivotRows.Clear();
                if (imHeaderRow.Count > 0 && imHeaderRow[imHeaderRow.Count - 1] == "rhs")
                {
                    imHeaderRow.RemoveAt(imHeaderRow.Count - 1);
                }
            }

            tableaus.Add(tab.Select(row => new List<double>(row)).ToList());

            int iteration = 0;
            int maxIterations = 100; // Prevent infinite loops

            while (iteration < maxIterations)
            {
                iteration++;

                // Clean up near-zero values
                for (int i = 0; i < tableaus[tableaus.Count - 1].Count; i++)
                {
                    for (int j = 0; j < tableaus[tableaus.Count - 1][i].Count; j++)
                    {
                        if (Math.Abs(tableaus[tableaus.Count - 1][i][j]) < 1e-10)
                        {
                            tableaus[tableaus.Count - 1][i][j] = 0.0;
                        }
                    }
                }

                // Check optimality condition
                var testRow = tableaus[tableaus.Count - 1][0].Take(tableaus[tableaus.Count - 1][0].Count - 1);
                bool isOptimal;
                if (isMin)
                {
                    isOptimal = testRow.All(num => num <= 0);
                }
                else
                {
                    isOptimal = testRow.All(num => num >= 0);
                }

                if (isOptimal)
                {
                    if (isConsoleOutput)
                    {
                        Logger.WriteLine($"\nOptimal Solution Found after {iteration - 1} iterations");
                        Logger.WriteLine($"Optimal value: {tableaus[tableaus.Count - 1][0][tableaus[tableaus.Count - 1][0].Count - 1]}");
                    }
                    break;
                }

                // Perform pivot operation
                var (newTab, thetaCol) = DoPrimalPivotOperation(tableaus[tableaus.Count - 1], isMin);

                if (newTab == null)
                {
                    if (isConsoleOutput)
                    {
                        if (thetaCol == null)
                        {
                            Logger.WriteLine("\nUnbounded solution or no feasible solution found");
                        }
                        else
                        {
                            Logger.WriteLine("\nOptimal solution found");
                        }
                    }
                    break;
                }

                thetaCols.Add(thetaCol?.ToList() ?? new List<double>());
                tableaus.Add(newTab);
                phases.Add(1);
            }

            if (iteration >= maxIterations)
            {
                if (isConsoleOutput)
                {
                    Logger.WriteLine($"\nMax iterations ({maxIterations}) reached");
                }
            }

            // Calculate changing variables (in transformed space)
            int transformedVarCount = lenObj;
            var transformedChangingVars = new List<double>(new double[transformedVarCount]);

            for (int k = 0; k < transformedVarCount; k++)
            {
                var columnValues = new List<double>();
                for (int i = 0; i < tableaus[tableaus.Count - 1].Count; i++)
                {
                    columnValues.Add(tableaus[tableaus.Count - 1][i][k]);
                }

                if (columnValues.Count == 0)
                {
                    throw new InvalidOperationException("Column values list is empty");
                }

                // Check if this is a basic variable (exactly one 1 and rest 0s)
                int? oneIndex = null;
                for (int i = 0; i < columnValues.Count; i++)
                {
                    if (Math.Abs(columnValues[i] - 1) < 1e-9)
                    {
                        oneIndex = i;
                        break;
                    }
                }

                if (oneIndex.HasValue && oneIndex > 0) // Not in objective row
                {
                    transformedChangingVars[k] = tableaus[tableaus.Count - 1][oneIndex.Value][tableaus[tableaus.Count - 1][oneIndex.Value].Count - 1];
                }
            }

            // Convert back to original variables if transformations were applied
            List<double> changingVars;
            if (varTransformations != null && varTransformations.Count > 0)
            {
                changingVars = InterpretSolution(transformedChangingVars);
            }
            else
            {
                changingVars = transformedChangingVars;
            }

            double optimalSolution = tableaus[tableaus.Count - 1][0][tableaus[tableaus.Count - 1][0].Count - 1];

            if (isConsoleOutput)
            {
                var topRow = new List<string>(imHeaderRow.Take(imHeaderRow.Count - 1));
                topRow.Add("RHS");

                for (int i = 0; i < tableaus.Count; i++)
                {
                    Logger.WriteLine($"\nTableau {i + 1}");
                    Logger.WriteLine(string.Join("", topRow.Select(val => $"{val,10}")));
                    for (int j = 0; j < tableaus[i].Count; j++)
                    {
                        for (int k = 0; k < tableaus[i][j].Count; k++)
                        {
                            Logger.Write($"{tableaus[i][j][k],10:F3}");
                        }
                        Logger.WriteLine();
                    }
                }

                Logger.WriteLine($"\nOriginal Variable Values: [{string.Join(", ", changingVars.Select(v => v.ToString("F3")))}]");
                Logger.WriteLine($"Optimal Solution: {optimalSolution:F3}");

                // Print variable interpretations
                if (varTransformations != null && varTransformations.Count > 0)
                {
                    Logger.WriteLine("\nVariable Interpretations:");
                    for (int i = 0; i < changingVars.Count; i++)
                    {
                        Logger.WriteLine($"  x{i + 1} = {changingVars[i]:F3}");
                    }
                }
            }

            phases.Add(-1);

            return (tableaus, changingVars, optimalSolution, imPivotCols, imPivotRows, imHeaderRow);
        }

        public void RunPrimalSimplex(List<double> objFuncPassed, List<List<double>> constraintsPassed, bool isMin, List<VariableSignType> varSigns = null)
        {
            objFunc = objFuncPassed.ToList();
            constraints = constraintsPassed.Select(x => x.ToList()).ToList();

            var simplex = new PrimalSimplex(isConsoleOutput: true);

            Logger.WriteLine("=============================================================");
            if (isMin)
            {
                Logger.WriteLine($"Objective Function: Minimize {string.Join(" + ", objFunc.Select((c, i) => $"{c}x{i + 1}"))}");
            }
            else
            {
                Logger.WriteLine($"Objective Function: Maximize {string.Join(" + ", objFunc.Select((c, i) => $"{c}x{i + 1}"))}");
            }
            
            Logger.WriteLine("\nOriginal Constraints:");

            for (int i = 0; i < constraints.Count; i++)
            {
                var constraintTerms = new List<string>();
                for (int j = 0; j < constraints[i].Count - 2; j++)
                {
                    if (constraints[i][j] != 0)
                    {
                        string sign = constraints[i][j] > 0 && constraintTerms.Count > 0 ? "+" : "";
                        constraintTerms.Add($"{sign}{constraints[i][j]}x{j + 1}");
                    }
                }
                string constraintStr = string.Join(" ", constraintTerms);
                string operator_str =  "<=";
                Logger.WriteLine($"  {constraintStr} {operator_str} {constraints[i][constraints[i].Count - 2]}");
            }

            for (int i = 0; i < constraints.Count; i++)
            {
                var constraintTerms = new List<string>();
                for (int j = 0; j < constraints[i].Count - 1; j++)
                {
                    if (constraints[i][j] != 0)
                    {
                        constraintTerms.Add($"{constraints[i][j]}x{j + 1}");
                    }
                }
                constraintTerms.RemoveAt(constraintTerms.Count - 1);
                constraintTerms.RemoveAt(constraintTerms.Count - 1);

                //string constraintStr = string.Join(" + ", constraintTerms);
                //// Logger.WriteLine($"  {constraintStr} <= {constraints[i][constraints[i].Count - 1]}");
                //Logger.WriteLine($"  {constraintStr} <= {constraints[i][constraints[i].Count - 2]}");
            }

            Logger.WriteLine("\n");
            CanonicalFormBuilder.BuildCanonicalForm(objFunc, constraints, isMin);
            Logger.WriteLine("\n");

            Logger.WriteLine("=" + new string('=', 49));

            List<List<double>> copyCons = constraints.Select(innerList => innerList.ToList()).ToList();
            foreach (var innerList in copyCons)
            {
                innerList.RemoveAt(innerList.Count - 1);
            }

            var result = simplex.DoPrimalSimplex(objFunc, copyCons, isMin, varSigns);
            var (tableaus, changingVars, optimalSolution, pivotCols, pivotRows, headerRow) = result;

            Logger.WriteLine($"\nHeader Row: [{string.Join(", ", headerRow)}]");
        }
    }
}