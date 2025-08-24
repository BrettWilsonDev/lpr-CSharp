using lpr381Project;
using static lpr381Project.RevisedVariableTransformation;

namespace lpr381Project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }
    }
}