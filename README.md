# LPR381 Linear and Integer Programming Project

## Overview
This project was developed to fulfill the specifications of the **LPR38 (Linear Programming 381)** course at my college. Its main goal is to create a program that can solve **Linear Programming (LP)** and **Integer Programming (IP)** models and perform sensitivity analysis to examine how changes in LP parameters affect the optimal solution.

The project demonstrates the practical application of **operations research** principles, where decision-making is modeled mathematically to optimize the allocation of scarce resources.

---

## Features

### Core Functionality
- Solve **maximization** and **minimization** linear programming models.
- Solve **binary integer programming models**, specifically Knapsack problems.
- Accepts a dynamic number of **decision variables** and **constraints**.
- Handles input/output via text files for flexibility and reproducibility.
- Implements proper programming practices, including comprehensive comments and clear code structure.

### Supported Algorithms
1. **Primal Simplex Algorithm**
   - Displays canonical form and all tableau iterations.
2. **Revised Primal Simplex Algorithm**
   - Displays Product Form and Price Out iterations.
3. **Branch & Bound Simplex Algorithm**
   - Supports backtracking, node fathoming, and displays all subproblem table iterations.
4. **Cutting Plane Algorithm**
   - Includes Product Form and Price Out iterations.
5. **Branch & Bound Knapsack Algorithm**
   - Supports backtracking and displays best candidate solutions.

### Sensitivity Analysis
- Analyze and modify:
  - **Non-basic variables**
  - **Basic variables**
  - **Constraint right-hand-side values**
- Perform **shadow price analysis**.
- Add new activities or constraints to an optimal solution.
- Solve **dual programming models** and verify strong/weak duality.

### Special Cases
- Detects and resolves **infeasible** or **unbounded** programming models.
- Bonus functionality for **non-linear problems** (e.g., `f(x)=x^2`).

---

## Input File Format

- **First line:** Problem type (`max` or `min`) followed by operators and coefficients of decision variables.
- **Subsequent lines:** Constraints with operators, coefficients, relation (`=`, `<=`, `>=`), and right-hand-side value.
- **Sign restrictions:** Listed after all constraints (`+`, `-`, `bin`, `int`) in the same order as decision variables.

**Example Input File:**

max +2 +3 +3 +5 +2 +4 <br>
+11 +8 +6 +14 +10 +10 <=40 <br>
bin bin bin bin bin bin <br>



---

## Output
- Outputs the **canonical form** and all tableau iterations of the selected algorithm.
- All decimal values are rounded to three decimal places.
- Includes sensitivity analysis results and dual problem solutions where applicable.

---

## Project Requirements
- Visual Studio project using any .NET programming language.
- Menu-driven executable (`solve.exe`) with options to choose the algorithm and perform sensitivity analysis.
- Error handling and special case detection.
- Proper programming best practices.

---

## Mark Allocation
| Criteria | Weight |
|----------|--------|
| Content Outline | 2 |
| Input File | 3 |
| Output File | 2 |
| Primal Simplex | 4 |
| Revised Primal Simplex | 4 |
| Branch & Bound Simplex | 20 |
| Branch & Bound Knapsack | 16 |
| Cutting Plane Algorithm | 14 |
| Sensitivity Analysis | 25 |
| Error Handling | 5 |
| Interface Presentation | 5 |
| Non-linear Problem (Bonus) | 10 |
| **Total** | 100 |

---

