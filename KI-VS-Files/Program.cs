using System;
using System.IO;

namespace KI_Projekt
{
    class Program
    {
        static void Main()
        {
            var benchmarkSource = "";
            parser benchmark = new parser();

#if PUBLISH
            while (!File.Exists(Directory.GetCurrentDirectory() + "\\Benchmarks\\" + benchmarkSource + ".txt"))
            {
                Console.WriteLine("Please enter the name of the benchmark (exact casing & without \".txt\") --> Example: KI_30");
                benchmarkSource = Console.ReadLine();
                Console.WriteLine();
            }

            benchmark.FullText = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Benchmarks\\" + benchmarkSource+ ".txt");
#else
            while (!File.Exists(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + "\\Benchmarks\\" + benchmarkSource + ".txt"))
            {
                Console.WriteLine("Please enter the name of the benchmark (exact casing & without \".txt\") --> Example: KI_30");
                benchmarkSource = Console.ReadLine();
                Console.WriteLine();
            }

            benchmark.FullText = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName + "\\Benchmarks\\" + benchmarkSource + ".txt");
#endif

            benchmark.Parser();

            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Parser finished. Press any key to start simplex.");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------\n");
            Console.ReadKey();

            simplex solver = new simplex();
            solver.MainTable = benchmark.MainBoard;
            solver.Solutions = benchmark.SolutionSpace;
            solver.Constraints = benchmark.Constraints;
            solver.Variables = benchmark.Variables;
            solver.Simplex();

            Console.WriteLine("\n\n-----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Solver finished. Press any key to end program.");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
            Console.ReadKey();
        }
    }
}
