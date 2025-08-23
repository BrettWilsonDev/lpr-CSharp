using System;
using System.Collections.Generic;
using System.IO;

namespace lpr381Project
{
    internal class InputReader
    {
        public List<double> ObjFunc { get; private set; } = new List<double>();
        public List<List<double>> Constraints { get; private set; } = new List<List<double>>();
        public bool IsMin { get; private set; } = false;
        public List<VariableSignType> VarSigns { get; private set; } = new List<VariableSignType>();

        public InputReader(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                // ---- Parse objective function ----
                string[] firstLine = lines[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                IsMin = firstLine[0].Equals("min", StringComparison.OrdinalIgnoreCase);

                for (int i = 1; i < firstLine.Length; i++)
                    ObjFunc.Add(double.Parse(firstLine[i]));

                // ---- Parse constraints (lines except first & last) ----
                for (int i = 1; i < lines.Length - 1; i++)
                {
                    string[] parts = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    List<double> row = new List<double>();

                    double rhs = 0;
                    int inequalityFlag = 0; // 0 for <=, 1 for >=

                    for (int j = 0; j < parts.Length; j++)
                    {
                        if (parts[j].Contains("<="))
                        {
                            rhs = double.Parse(parts[j].Replace("<=", ""));
                            inequalityFlag = 0;
                            break;
                        }
                        else if (parts[j].Contains(">="))
                        {
                            rhs = double.Parse(parts[j].Replace(">=", ""));
                            inequalityFlag = 1;
                            break;
                        }

                        row.Add(double.Parse(parts[j]));
                    }

                    // Append RHS and inequality flag
                    row.Add(rhs);
                    row.Add(inequalityFlag);

                    Constraints.Add(row);
                }

                // ---- Parse variable signs (last line) ----
                string[] lastLine = lines[lines.Length - 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string sign in lastLine)
                {
                    switch (sign.ToLower())
                    {
                        case "bin":
                            VarSigns.Add(VariableSignType.Binary);
                            break;
                        case "int":
                            VarSigns.Add(VariableSignType.Integer);
                            break;
                        case "-":
                            VarSigns.Add(VariableSignType.Negative);
                            break;
                        case "urs":
                            VarSigns.Add(VariableSignType.Unrestricted);
                            break;
                        case "+":
                            VarSigns.Add(VariableSignType.Positive);
                            break;
                        default: // fallback: treat anything else as positive
                            VarSigns.Add(VariableSignType.Positive);
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        public (List<double> ObjFunc, List<List<double>> Constraints, bool IsMin, List<VariableSignType> VarSigns) GetData()
        {
            return (ObjFunc, Constraints, IsMin, VarSigns);
        }

        // Debug print
        public void Print()
        {
            Logger.WriteLine("isMin: " + IsMin);
            Logger.WriteLine("Objective: " + string.Join(", ", ObjFunc));
            Logger.WriteLine("Constraints:");
            foreach (var row in Constraints)
                Logger.WriteLine("  " + string.Join(", ", row));
            Logger.WriteLine("Var Signs: " + string.Join(", ", VarSigns));
        }
    }
}
