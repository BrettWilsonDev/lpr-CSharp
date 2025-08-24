using System.Data;

namespace lpr381Project
{
    public class BranchAndBound
    {
        private bool isConsoleOutput;
        private int precision = 4;
        private double tolerance = 1e-6;

        private DualSimplex dual;

        private List<double> objFunc = new List<double> { 0.0, 0.0 };

        private List<List<double>> constraints = new List<List<double>> { new List<double> { 0.0, 0.0, 0.0, 0.0 } };

        private List<List<List<double>>> newTableaus = new List<List<List<double>>>();

        private bool isMin = false;

        // Tree traversal specific variables
        private List<double> bestSolution = null;
        private double bestObjective;
        private int nodeCounter = 0;
        private List<(List<double> solution, double objective)> allSolutions = new List<(List<double>, double)>();
        private bool enablePruning = false;

        private string bestSolutionNodeNum = null;
        private List<List<double>> bestSolutionTableau = null;

        private List<object> displayTableausMin = new List<object>();

        public BranchAndBound(bool isConsoleOutput = false)
        {
            this.isConsoleOutput = isConsoleOutput;
            dual = new DualSimplex(); // You'll need to implement this
            objFunc = new List<double> { 0.0, 0.0 };

            constraints = new List<List<double>> { new List<double> { 0.0, 0.0, 0.0, 0.0 } };
            newTableaus = new List<List<List<double>>>();

            isMin = false;

            bestSolution = null;
            bestObjective = isMin ? double.PositiveInfinity : double.NegativeInfinity;
            nodeCounter = 0;
            allSolutions = new List<(List<double>, double)>();
            enablePruning = false;

            bestSolutionNodeNum = null;
            bestSolutionTableau = null;

            displayTableausMin = new List<object>();
        }


        public double RoundValue(double value)
        {
            try
            {
                return Math.Round(value, precision);
            }
            catch
            {
                return value;
            }
        }

        public List<List<double>> RoundMatrix(List<List<double>> matrix)
        {
            if (matrix == null) return matrix;

            var result = new List<List<double>>();
            foreach (var row in matrix)
            {
                var roundedRow = new List<double>();
                foreach (var val in row)
                {
                    roundedRow.Add(RoundValue(val));
                }
                result.Add(roundedRow);
            }
            return result;
        }

        public List<double> RoundArray(List<double> array)
        {
            if (array == null) return array;

            var result = new List<double>();
            foreach (var val in array)
            {
                result.Add(RoundValue(val));
            }
            return result;
        }

        public List<List<List<double>>> RoundTableaus(List<List<List<double>>> tableaus)
        {
            if (tableaus == null || tableaus.Count == 0)
                return tableaus;

            var roundedTableaus = new List<List<List<double>>>();
            foreach (var tableau in tableaus)
            {
                var roundedTableau = RoundMatrix(tableau);
                roundedTableaus.Add(roundedTableau);
            }
            return roundedTableaus;
        }

        public bool IsIntegerValue(double value)
        {
            double roundedVal = RoundValue(value);
            return Math.Abs(roundedVal - Math.Round(roundedVal)) <= tolerance;
        }

        public void PrintTableau(List<List<double>> tableau, string title = "Tableau")
        {
            var tempHeaderStr = new List<string>();
            for (int i = 0; i < objFunc.Count; i++)
            {
                tempHeaderStr.Add($"x{i + 1}");
            }
            for (int i = 0; i < (tableau[tableau.Count - 1].Count - objFunc.Count - 1); i++)
            {
                tempHeaderStr.Add($"s{i + 1}");
            }
            tempHeaderStr.Add("rhs");

            if (isConsoleOutput)
            {
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
                        Logger.Write($"{RoundValue(val),8:F4}  ");
                    }
                    Logger.WriteLine();
                }
                Logger.WriteLine();
            }
        }

        public List<int> GetBasicVarSpots(List<List<List<double>>> tableaus)
        {
            var basicVarSpots = new List<int>();

            for (int k = 0; k < tableaus[tableaus.Count - 1][tableaus[tableaus.Count - 1].Count - 1].Count; k++)
            {
                int columnIndex = k;
                var tCVars = new List<double>();

                for (int i = 0; i < tableaus[tableaus.Count - 1].Count; i++)
                {
                    double columnValue = RoundValue(tableaus[tableaus.Count - 1][i][columnIndex]);
                    tCVars.Add(columnValue);
                }

                double sumVals = RoundValue(tCVars.Sum());
                if (Math.Abs(sumVals - 1.0) <= tolerance)
                {
                    basicVarSpots.Add(k);
                }
            }

            var basicVarCols = new List<List<double>>();
            for (int i = 0; i < tableaus[tableaus.Count - 1][tableaus[tableaus.Count - 1].Count - 1].Count; i++)
            {
                var tLst = new List<double>();
                if (basicVarSpots.Contains(i))
                {
                    for (int j = 0; j < tableaus[tableaus.Count - 1].Count; j++)
                    {
                        double roundedVal = RoundValue(tableaus[tableaus.Count - 1][j][i]);
                        tLst.Add(roundedVal);
                    }
                    basicVarCols.Add(tLst);
                }
            }

            // Sort the cbv according to basic var positions
            var zippedCbv = basicVarCols.Zip(basicVarSpots, (col, spot) => new { Col = col, Spot = spot }).ToList();
            var sortedCbvZipped = zippedCbv.OrderBy(x => x.Col.Contains(1.0) ? x.Col.IndexOf(1.0) : x.Col.Count).ToList();

            if (sortedCbvZipped.Any())
            {
                basicVarSpots = sortedCbvZipped.Select(x => x.Spot).ToList();
            }
            else
            {
                basicVarSpots = new List<int>();
            }

            return basicVarSpots;
        }

        public (List<List<double>> displayTab, List<List<double>> newTab) DoAddConstraint(List<List<double>> addedConstraints, List<List<double>> overRideTab = null)
        {
            List<List<double>> changingTable;
            List<int> basicVarSpots;

            if (overRideTab != null)
            {
                changingTable = DeepCopy(overRideTab);
                changingTable = RoundMatrix(changingTable);
                var tempTabs = new List<List<List<double>>> { changingTable };
                basicVarSpots = GetBasicVarSpots(tempTabs);
            }
            else
            {
                Logger.WriteLine("needs an input table");
                return (null, null);
            }

            var newTab = DeepCopy(changingTable);

            // Add new constraint rows to the tableau
            for (int k = 0; k < addedConstraints.Count; k++)
            {
                // Add a column for each new constraint's slack/surplus variable
                for (int i = 0; i < changingTable.Count; i++)
                {
                    newTab[i].Insert(newTab[i].Count - 1, 0.0);
                }

                // Create the new constraint row
                var newCon = new List<double>();
                for (int i = 0; i < changingTable[0].Count + addedConstraints.Count; i++)
                {
                    newCon.Add(0.0);
                }

                // Fill in the coefficients for the constraint
                for (int i = 0; i < addedConstraints[k].Count - 2; i++)
                {
                    newCon[i] = RoundValue(addedConstraints[k][i]);
                }

                // Set the RHS value
                newCon[newCon.Count - 1] = RoundValue(addedConstraints[k][addedConstraints[k].Count - 2]);

                // Add slack or surplus variable
                int slackSpot = ((newCon.Count - addedConstraints.Count) - 1) + k;
                if (addedConstraints[k][addedConstraints[k].Count - 1] == 1) // >= constraint
                {
                    newCon[slackSpot] = -1.0; // surplus variable
                }
                else // <= constraint
                {
                    newCon[slackSpot] = 1.0; // slack variable
                }

                newTab.Add(newCon);
            }

            // Round the new tableau
            newTab = RoundMatrix(newTab);
            PrintTableau(newTab, "unfixed tab");

            var displayTab = DeepCopy(newTab);

            // Fix tableau to maintain basic feasible solution
            for (int k = 0; k < addedConstraints.Count; k++)
            {
                int constraintRowIndex = newTab.Count - addedConstraints.Count + k;

                // Check each basic variable column
                foreach (int colIndex in basicVarSpots)
                {
                    // Get the coefficient in the new constraint row for this basic variable
                    double coefficientInNewRow = RoundValue(displayTab[constraintRowIndex][colIndex]);

                    if (Math.Abs(coefficientInNewRow) > tolerance)
                    {
                        // Find the row where this basic variable has coefficient 1
                        int? pivotRow = null;
                        for (int rowIndex = 0; rowIndex < displayTab.Count - addedConstraints.Count; rowIndex++)
                        {
                            if (Math.Abs(RoundValue(displayTab[rowIndex][colIndex]) - 1.0) <= tolerance)
                            {
                                pivotRow = rowIndex;
                                break;
                            }
                        }

                        if (pivotRow.HasValue)
                        {
                            // Auto-detect if we need to reverse the row operation based on constraint type
                            int constraintType = (int)addedConstraints[k][addedConstraints[k].Count - 1];
                            bool autoReverse = (constraintType == 1);

                            // Perform row operation to eliminate the coefficient
                            for (int col = 0; col < displayTab[0].Count; col++)
                            {
                                double pivotVal = RoundValue(displayTab[pivotRow.Value][col]);
                                double constraintVal = RoundValue(displayTab[constraintRowIndex][col]);

                                double newVal;
                                if (autoReverse)
                                {
                                    newVal = pivotVal - coefficientInNewRow * constraintVal;
                                }
                                else
                                {
                                    newVal = constraintVal - coefficientInNewRow * pivotVal;
                                }

                                displayTab[constraintRowIndex][col] = RoundValue(newVal);
                            }
                        }
                    }
                }
            }

            // Round the final tableau
            displayTab = RoundMatrix(displayTab);
            PrintTableau(displayTab, "fixed tab");

            return (displayTab, newTab);
        }

        public (int? xSpot, double? rhsVal) TestIfBasicVarIsInt(List<List<List<double>>> tabs)
        {
            var decisionVars = new List<double>();

            // Only check the ORIGINAL decision variables (not slack/surplus variables)
            for (int i = 0; i < objFunc.Count; i++) // Only original decision variables
            {
                bool found = false;
                for (int j = 0; j < tabs[tabs.Count - 1].Count; j++)
                {
                    double val = RoundValue(tabs[tabs.Count - 1][j][i]);
                    if (Math.Abs(val - 1.0) <= tolerance)
                    {
                        double rhsVal = RoundValue(tabs[tabs.Count - 1][j][tabs[tabs.Count - 1][j].Count - 1]);
                        decisionVars.Add(rhsVal);
                        found = true;
                        break; // Found the basic variable, move to next
                    }
                }
                if (!found)
                {
                    // If no basic variable found for this decision variable, it's 0
                    decisionVars.Add(0.0);
                }
            }

            // Find the fractional variable closest to 0.5 among DECISION VARIABLES ONLY
            int bestXSpot = -1;
            double? bestRhsVal = null;
            double minDistanceToHalf = double.PositiveInfinity;

            for (int i = 0; i < decisionVars.Count; i++)
            {
                if (!IsIntegerValue(decisionVars[i]))
                {
                    double fractionalPart = decisionVars[i] - Math.Floor(decisionVars[i]);
                    double distanceToHalf = Math.Abs(fractionalPart - 0.5);

                    if (distanceToHalf < minDistanceToHalf)
                    {
                        minDistanceToHalf = distanceToHalf;
                        bestXSpot = i; // This will be within range of original variables
                        bestRhsVal = decisionVars[i];
                    }
                }
            }

            if (bestXSpot == -1)
            {
                return (null, null);
            }
            else
            {
                return (bestXSpot, bestRhsVal);
            }
        }

        public (List<double> newConMin, List<double> newConMax) MakeBranch(List<List<List<double>>> tabs)
        {
            var (xSpot, rhsVal) = TestIfBasicVarIsInt(tabs);

            if (xSpot == null && rhsVal == null)
            {
                return (null, null);
            }

            if (isConsoleOutput)
            {
                Logger.WriteLine($"Branching on x{xSpot + 1} = {RoundValue(rhsVal.Value)}");
            }

            int maxInt = (int)Math.Ceiling(rhsVal.Value);
            int minInt = (int)Math.Floor(rhsVal.Value);

            // Create constraint for x <= floor(value)
            var newConMin = new List<double>();
            for (int i = 0; i < objFunc.Count; i++)
            {
                if (i != xSpot)
                {
                    newConMin.Add(0);
                }
                else
                {
                    newConMin.Add(1);
                }
            }
            newConMin.Add(minInt);
            newConMin.Add(0); // <= constraint

            // Create constraint for x >= ceil(value)
            var newConMax = new List<double>();
            for (int i = 0; i < objFunc.Count; i++)
            {
                if (i != xSpot)
                {
                    newConMax.Add(0);
                }
                else
                {
                    newConMax.Add(1);
                }
            }
            newConMax.Add(maxInt);
            newConMax.Add(1); // >= constraint

            return (newConMin, newConMax);
        }

        public double? GetObjectiveValue(List<List<List<double>>> tabs)
        {
            if (tabs == null || tabs.Count == 0)
                return null;
            // Last element of first row in final tableau
            return RoundValue(tabs[tabs.Count - 1][0][tabs[tabs.Count - 1][0].Count - 1]);
        }

        public List<double> GetCurrentSolution(List<List<List<double>>> tabs)
        {
            var solution = new List<double>();
            for (int i = 0; i < objFunc.Count; i++)
            {
                solution.Add(0.0);
            }

            for (int i = 0; i < objFunc.Count; i++)
            {
                for (int j = 0; j < tabs[tabs.Count - 1].Count; j++)
                {
                    double val = RoundValue(tabs[tabs.Count - 1][j][i]);
                    if (Math.Abs(val - 1.0) <= tolerance)
                    {
                        solution[i] = RoundValue(tabs[tabs.Count - 1][j][tabs[tabs.Count - 1][j].Count - 1]);
                        break;
                    }
                }
            }

            return solution;
        }

        public bool IsIntegerSolution(List<double> solution)
        {
            foreach (double val in solution)
            {
                if (!IsIntegerValue(val))
                {
                    return false;
                }
            }
            return true;
        }

        public bool UpdateBestSolution(List<List<List<double>>> tabs, string nodeLabel = null)
        {
            var objVal = GetObjectiveValue(tabs);
            var solution = GetCurrentSolution(tabs);

            if (objVal == null)
                return false;

            if (IsIntegerSolution(solution))
            {
                // Store ALL integer solutions found
                allSolutions.Add((new List<double>(solution), objVal.Value));

                if (isMin)
                {
                    if (objVal.Value < bestObjective)
                    {
                        bestObjective = objVal.Value;
                        bestSolution = solution;
                        bestSolutionTableau = tabs[tabs.Count - 1];
                        bestSolutionNodeNum = nodeLabel;
                        if (isConsoleOutput)
                        {
                            Logger.WriteLine($"New best integer solution Candidate found: [{string.Join(", ", solution)}] with objective {objVal.Value}");
                        }
                        return true;
                    }
                    else
                    {
                        if (isConsoleOutput)
                        {
                            Logger.WriteLine($"Integer solution Candidate found: [{string.Join(", ", solution)}] with objective {objVal.Value} (not better than current best)");
                        }
                    }
                }
                else
                {
                    if (objVal.Value > bestObjective)
                    {
                        bestObjective = objVal.Value;
                        bestSolution = solution;
                        bestSolutionTableau = tabs[tabs.Count - 1];
                        bestSolutionNodeNum = nodeLabel;
                        if (isConsoleOutput)
                        {
                            Logger.WriteLine($"New best integer solution Candidate found: [{string.Join(", ", solution)}] with objective {objVal.Value}");
                        }
                        return true;
                    }
                    else
                    {
                        if (isConsoleOutput)
                        {
                            Logger.WriteLine($"Integer solution Candidate found: [{string.Join(", ", solution)}] with objective {objVal.Value} (not better than current best)");
                        }
                    }
                }
            }
            return false;
        }

        public bool ShouldPrune(List<List<List<double>>> tabs)
        {
            // Only prune if pruning is enabled
            if (!enablePruning)
                return false;

            var objVal = GetObjectiveValue(tabs);

            if (objVal == null)
                return true; // Infeasible solution

            if (bestSolution != null)
            {
                if (isMin)
                    return objVal.Value >= bestObjective;
                else
                    return objVal.Value <= bestObjective;
            }

            return false;
        }

        public (List<double> bestSolution, double bestObjective) DoBranchAndBound(List<List<List<double>>> initialTabs, bool enablePruning = false)
        {
            this.enablePruning = enablePruning;

            if (isConsoleOutput)
            {
                Logger.WriteLine("Starting Branch and Bound Algorithm");
                if (enablePruning)
                    Logger.WriteLine("Pruning: ENABLED (standard branch and bound)");
                else
                    Logger.WriteLine("Pruning: DISABLED (complete tree exploration)");
                Logger.WriteLine(new string('=', 50));
            }

            // Round initial tableaus
            initialTabs = RoundTableaus(initialTabs);

            // Initialize best solution tracking
            bestSolution = null;
            bestObjective = isMin ? double.PositiveInfinity : double.NegativeInfinity;
            nodeCounter = 0;
            allSolutions = new List<(List<double>, double)>();

            // Stack to store nodes to process
            var nodeStack = new Stack<(List<List<List<double>>> tabs, int depth, string nodeLabel, List<string> constraintsPath, string parentLabel)>();
            nodeStack.Push((initialTabs, 0, "0", new List<string>(), null));

            // Keep track of child counters for each parent
            var childCounters = new Dictionary<string, int>();

            int ctr = 0;
            while (nodeStack.Count > 0)
            {
                ctr++;

                if (ctr > 20)
                {
                    Logger.WriteLine("something is very wrong");
                    break;
                }

                var (currentTabs, depth, nodeLabel, constraintsPath, parentLabel) = nodeStack.Pop();
                nodeCounter++;

                // Round current tableaus
                currentTabs = RoundTableaus(currentTabs);

                if (isConsoleOutput)
                {
                    Logger.WriteLine($"\n--- Processing Node {nodeLabel} (Depth {depth}) ---");
                    if (parentLabel != null)
                        Logger.WriteLine($"Parent: {parentLabel}");
                    Logger.WriteLine($"Constraints path: [{string.Join(", ", constraintsPath)}]");
                }

                // Check if current solution should be pruned (only if pruning enabled)
                if (ShouldPrune(currentTabs))
                {
                    if (isConsoleOutput)
                        Logger.WriteLine($"Node {nodeLabel} pruned by bound");
                    continue;
                }

                // Update best solution if current is integer and better
                UpdateBestSolution(currentTabs, nodeLabel);

                // Try to branch further
                var (newConMin, newConMax) = MakeBranch(currentTabs);

                if (newConMin == null && newConMax == null)
                {
                    // Integer solution found - already handled in UpdateBestSolution
                    if (isConsoleOutput)
                    {
                        var solution = GetCurrentSolution(currentTabs);
                        var objVal = GetObjectiveValue(currentTabs);
                        Logger.WriteLine($"Node {nodeLabel}: Integer solution [{string.Join(", ", solution)}] with objective {objVal}");
                    }
                    continue;
                }

                // Initialize child counter for this node if not exists
                if (!childCounters.ContainsKey(nodeLabel))
                    childCounters[nodeLabel] = 0;

                // Create two child nodes with hierarchical labels
                var childNodes = new List<(List<List<List<double>>>, int, string, List<string>, string)>();

                // Process "less than or equal" branch (newConMin) - Child 1
                try
                {
                    childCounters[nodeLabel]++;
                    string childLabel = nodeLabel == "0" ? "1" : $"{nodeLabel}.{childCounters[nodeLabel]}";

                    if (isConsoleOutput)
                    {
                        Logger.Write($"\nTrying MIN branch (Node {childLabel}): {string.Join(", ", newConMin)} ");
                        for (int i = 0; i < newConMin.Count - 2; i++)
                        {
                            if (newConMin[i] == 0 || newConMin[i] == 0.0)
                                continue;
                            if (newConMin[i] == 1 || newConMin[i] == 1.0)
                                Logger.Write($"x{i + 1} ");
                            else
                                Logger.Write($"{newConMin[i]}*x{i + 1} ");
                        }
                        if (newConMin[newConMin.Count - 1] == 0)
                            Logger.Write("<= ");
                        else
                            Logger.Write(">= ");
                        Logger.Write($"{newConMin[newConMin.Count - 2]} ");
                    }

                    var tempConsMin = new List<List<double>> { newConMin };
                    var (displayTabMin, newTabMin) = DoAddConstraint(tempConsMin, currentTabs[currentTabs.Count - 1]);

                    // Note: You'll need to implement DualSimplex.DoDualSimplex method
                    var (newTableausMin, changingVarsMin, optimalSolutionMin, IMPivotColsMin, IMPivotRowsMin, IMHeaderRowMin) =
                        dual.DoDualSimplex(new List<double>(), new List<List<double>>(), isMin, displayTabMin);

                    if (optimalSolutionMin == null)
                    {
                        for (int i = 0; i < IMHeaderRowMin.Count; i++)
                        {
                            Logger.Write($"{IMHeaderRowMin[i],8:F4}  ");
                        }
                        PrintTableau(newTableausMin[0], $"Node {childLabel}: Infeasible tableau");
                        newTableausMin = new List<List<List<double>>>();
                    }

                    if (optimalSolutionMin != null)
                    {
                        if (newTableausMin != null && newTableausMin.Count > 0)
                        {
                            // Round the new tableaus
                            newTableausMin = RoundTableaus(newTableausMin);
                            string constraintDesc = $"x{newConMin.Take(newConMin.Count - 2).ToList().IndexOf(1) + 1} <= {newConMin[newConMin.Count - 2]}";
                            var newConstraintsPath = new List<string>(constraintsPath) { constraintDesc };
                            childNodes.Add((newTableausMin, depth + 1, childLabel, newConstraintsPath, nodeLabel));
                        }
                        else if (isConsoleOutput)
                        {
                            Logger.WriteLine($"MIN branch (Node {childLabel}) infeasible");
                        }
                    }

                    if (isConsoleOutput && newTableausMin != null && newTableausMin.Count > 0)
                    {
                        for (int i = 0; i < newTableausMin.Count - 1; i++)
                        {
                            PrintTableau(newTableausMin[i], $"Node {childLabel} MIN branch Tableau {i + 1}");
                            displayTableausMin.Add(newTableausMin[i]);
                        }
                        PrintTableau(newTableausMin[newTableausMin.Count - 1], $"Node {childLabel} MIN branch final tableau");
                        displayTableausMin.Add(newTableausMin[newTableausMin.Count - 1]);
                    }
                }
                catch (Exception e)
                {
                    if (isConsoleOutput)
                        Logger.WriteLine($"MIN branch (Node {childCounters[nodeLabel]}) failed: {e}");
                }

                // Process "greater than or equal" branch (newConMax) - Child 2
                try
                {
                    childCounters[nodeLabel]++;
                    string childLabel = nodeLabel == "0" ? "2" : $"{nodeLabel}.{childCounters[nodeLabel]}";

                    if (isConsoleOutput)
                    {
                        Logger.Write($"\nTrying MAX branch (Node {childLabel}): {string.Join(", ", newConMax)} ");
                        for (int i = 0; i < newConMax.Count - 2; i++)
                        {
                            if (newConMax[i] == 0 || newConMax[i] == 0.0)
                                continue;
                            if (newConMax[i] == 1 || newConMax[i] == 1.0)
                                Logger.Write($"x{i + 1} ");
                            else
                                Logger.Write($"{newConMax[i]}*x{i + 1} ");
                        }
                        if (newConMax[newConMax.Count - 1] == 0)
                            Logger.Write("<= ");
                        else
                            Logger.Write(">= ");
                        Logger.Write($"{newConMax[newConMax.Count - 2]} ");
                    }

                    var tempConsMax = new List<List<double>> { newConMax };
                    var (displayTabMax, newTabMax) = DoAddConstraint(tempConsMax, currentTabs[currentTabs.Count - 1]);

                    var (newTableausMax, changingVarsMax, optimalSolutionMax, IMPivotColsMax, IMPivotRowsMax, IMHeaderRowMax) =
                        dual.DoDualSimplex(new List<double>(), new List<List<double>>(), isMin, displayTabMax);

                    if (optimalSolutionMax == null)
                    {
                        PrintTableau(newTableausMax[0], $"Node {childLabel}: Infeasible tableau");
                        newTableausMax = new List<List<List<double>>>();
                    }

                    //Logger.WriteLine($"Cols: {string.Join(", ", IMPivotColsMax)}");
                    //Logger.WriteLine($"Rows: {string.Join(", ", IMPivotRowsMax)}");

                    if (optimalSolutionMax != null)
                    {
                        if (newTableausMax != null && newTableausMax.Count > 0)
                        {
                            // Round the new tableaus
                            newTableausMax = RoundTableaus(newTableausMax);
                            string constraintDesc = $"x{newConMax.Take(newConMax.Count - 2).ToList().IndexOf(1) + 1} >= {newConMax[newConMax.Count - 2]}";
                            var newConstraintsPath = new List<string>(constraintsPath) { constraintDesc };
                            childNodes.Add((newTableausMax, depth + 1, childLabel, newConstraintsPath, nodeLabel));
                        }
                        else if (isConsoleOutput)
                        {
                            Logger.WriteLine($"MAX branch (Node {childLabel}) infeasible");
                        }
                    }

                    if (isConsoleOutput && newTableausMax != null && newTableausMax.Count > 0)
                    {
                        for (int i = 0; i < newTableausMax.Count - 1; i++)
                        {
                            PrintTableau(newTableausMax[i], $"Node {childLabel} MAX branch Tableau {i + 1}");
                        }
                        PrintTableau(newTableausMax[newTableausMax.Count - 1], $"Node {childLabel} MAX branch final tableau");
                    }
                }
                catch (Exception e)
                {
                    if (isConsoleOutput)
                        Logger.WriteLine($"MAX branch (Node {childCounters[nodeLabel]}) failed: {e}");
                }

                // Add child nodes to stack (reverse order for depth-first search)
                for (int i = childNodes.Count - 1; i >= 0; i--)
                {
                    nodeStack.Push(childNodes[i]);
                }
            }

            // Print final results
            if (isConsoleOutput)
            {
                Logger.WriteLine("\n" + new string('=', 50));
                Logger.WriteLine("BRANCH AND BOUND COMPLETED");
                Logger.WriteLine(new string('=', 50));
                if (bestSolution != null)
                {
                    PrintTableau(bestSolutionTableau, $"Best Candidate solution tableau at node {bestSolutionNodeNum}");
                    Logger.WriteLine($"Node of best solution: {bestSolutionNodeNum}");
                    Logger.WriteLine($"Best integer solution: [{string.Join(", ", bestSolution)}]");
                    Logger.WriteLine($"Best objective value: {bestObjective}");
                    Logger.WriteLine($"Best solution:");
                    PrintBasicVars(bestSolutionTableau);
                }
                else
                {
                    Logger.WriteLine("No integer solution found");
                }
                Logger.WriteLine($"Total nodes processed: {nodeCounter}");

                // Print all integer solutions found
                if (allSolutions.Count > 0)
                {
                    Logger.WriteLine($"\nAll integer solutions found ({allSolutions.Count}):");
                    for (int i = 0; i < allSolutions.Count; i++)
                    {
                        var (sol, obj) = allSolutions[i];
                        Logger.WriteLine($"  {i + 1}. Solution: [{string.Join(", ", sol)}], Objective: {obj}");
                    }
                }
            }

            return (bestSolution, bestObjective);
        }

        public (List<double> objFunc, List<List<double>> constraints) SetUpProblem(
            List<double> objFunc,
            List<List<double>> constraints)
        {
            // Generate c2 .. cN automatically
            int numConstraints = objFunc.Count;      // e.g. 6
            int vectorLength = objFunc.Count + 3;    // columns including slack + RHS

            for (int i = 0; i < numConstraints; i++)
            {
                var row = new List<double>(new double[vectorLength]);

                // Put a "1" at column i (the decision variable column)
                row[i] = 1;

                // Put a "1" in the slack column (vectorLength - 2)
                row[vectorLength - 2] = 1;

                // RHS (last element) stays 0
                constraints.Add(row);
            }

            return (objFunc, constraints);
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
                    column.Add((tableau[row][col]));
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
                    if (Math.Abs((tableau[row][col]) - 1.0) < tolerance)
                    {
                        basicRow = row;
                        break;
                    }
                }

                if (basicRow != -1)
                {
                    double rhsValue = (tableau[basicRow][tableau[basicRow].Count - 1]);
                    string varName = $"x{col + 1}";
                    Logger.WriteLine($"{varName} = {(rhsValue)}");
                }
            }

            Logger.WriteLine();
        }

        public void RunBranchAndBound(List<double> objFuncPassed, List<List<double>> constraintsPassed, bool isMin, List<VariableSignType> varSigns = null)
        {
            bool enablePruning = false;
            try
            {
                objFunc = objFuncPassed.ToList();
                constraints = constraintsPassed.Select(x => x.ToList()).ToList();
                (objFunc, constraints) = SetUpProblem(objFunc, constraints);

                var a = new List<double>(objFunc);
                var b = new List<List<double>>();
                foreach (var constraint in constraints)
                {
                    b.Add(new List<double>(constraint));
                }

                CanonicalFormBuilder.BuildCanonicalForm(objFunc, constraints, isMin);

                try
                {
                    var (newTableaus, changingVars, optimalSolution, _, _, _) = dual.DoDualSimplex(a, b, isMin);

                    // Round the initial tableaus
                    this.newTableaus = RoundTableaus(newTableaus);

                    if (isConsoleOutput)
                    {
                        Logger.WriteLine("Initial LP relaxation solved");
                        for (int i = 0; i < this.newTableaus.Count - 1; i++)
                        {
                            PrintTableau(this.newTableaus[i], $"Initial Tableau {i + 1}");
                        }
                        PrintTableau(this.newTableaus[this.newTableaus.Count - 1], "Initial tableau solved");
                        var solution = GetCurrentSolution(this.newTableaus);
                        var objVal = GetObjectiveValue(this.newTableaus);
                        Logger.WriteLine($"Initial solution: [{string.Join(", ", solution)}]");
                        Logger.WriteLine($"Initial objective: {objVal}");
                    }



                    // Start branch and bound
                    var (bestSolution, bestObjective) = DoBranchAndBound(this.newTableaus, enablePruning);
                }
                catch (Exception e)
                {
                    if (isConsoleOutput)
                        Logger.WriteLine($"Error in dual simplex: {e}");
                    throw;
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine($"math error: {e}");
                throw;
            }
        }

        // Helper method for deep copying lists
        private List<List<double>> DeepCopy(List<List<double>> original)
        {
            if (original == null) return null;

            var copy = new List<List<double>>();
            foreach (var row in original)
            {
                copy.Add(new List<double>(row));
            }
            return copy;
        }
    }
}