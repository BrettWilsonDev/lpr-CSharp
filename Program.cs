using lpr381Project;
using static lpr381Project.RevisedVariableTransformation;

namespace lpr381Project
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //InputReader reader = new InputReader("input.txt");
            //var data = reader.GetData();

            //Logger.Init("output.txt");


            //Logger.WriteLine("Program started!");

            ////var instance = new RevisedPrimalSimplex(true);
            ////instance.Test();

            //var instance = new BranchAndBound(true);
            //instance.Test();

            //Logger.Close();

            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }
    }
}