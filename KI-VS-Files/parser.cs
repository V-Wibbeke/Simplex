using System;
using System.Collections.Generic;
using System.Linq;

namespace KI_Projekt
{
    class parser
    {
        private List<string> benchmarkRows = new List<string>();
        private List<double> originalSolutions = new List<double>();
        public List<double> SolutionSpace = new List<double>();
        private double[,] MaxBoard;
        private double[,] MinBoard;
        public double[,] MainBoard;
        public string FullText { get; set; }
        public int Constraints { get; set; }
        public int Variables { get; set; }

       public void Parser()
        {
            BaseFormatting();
            CanonToStandardFormat();
            mainTab();
        }
        
        private void BaseFormatting()
        {
            benchmarkRows = FullText.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList(); // ToList() necessary => otherwise saved as an array
            benchmarkRows.ForEach(item => Console.WriteLine(item));
            Console.WriteLine();

            foreach (string item in benchmarkRows.ToList())                                      // ToList saves change into another list until iteration is finished, otherwise there is an error
            {
                if (item.Contains("//"))
                {
                    benchmarkRows.Remove(item);                                                  //delete comments
                }
            } 

            for (int i = 0; i < benchmarkRows.Count; i++)
            {
                benchmarkRows[i] = benchmarkRows[i].Remove(0, benchmarkRows[i].IndexOf("+") + 2);
                benchmarkRows[i] = benchmarkRows[i].Replace(";", string.Empty);
            }                                                                                   // format beginning/end of line

            benchmarkRows.ForEach(item => Console.WriteLine(item));
            Console.WriteLine();

            Constraints = benchmarkRows.Count - 1;
            Variables = benchmarkRows[0].Split('x').Length - 1;
        }

        private void CanonToStandardFormat()
        {
            for (int temp = 0; temp < benchmarkRows.Count; temp++)
            {
                if (benchmarkRows[temp].Contains("="))
                {
                    benchmarkRows[temp] = benchmarkRows[temp].Remove(benchmarkRows[temp].IndexOf("=") - 1, 1);
                    for (int i = 0; i < Constraints; i++)
                    {
                        if (temp == i + 1)
                        {
                            benchmarkRows[temp] = benchmarkRows[temp].Insert(benchmarkRows[temp].IndexOf("=") - 1, " + 1*s" + i);
                        }
                        else
                        {
                            benchmarkRows[temp] = benchmarkRows[temp].Insert(benchmarkRows[temp].IndexOf("=") - 1, " + 0*s" + i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Constraints; i++)
                    {
                        benchmarkRows[temp] = benchmarkRows[temp].Insert(benchmarkRows[temp].Length, " + 0*s" + i);
                    }
                }
            }
            benchmarkRows.ForEach(item => Console.WriteLine(item));
            Console.WriteLine();
        }

        private void mainTab()
        {
            MinBoard = new double[Constraints + 1, Variables + Constraints + 1];

            for (int row = 0; row < benchmarkRows.Count; row++)
            {
                for (int column = 0; column < Variables + Constraints; column++)
                {
                    string temp = benchmarkRows[row];
                    MinBoard[row, column] = Convert.ToDouble(temp.Substring(0, benchmarkRows[row].IndexOf("*")));
                    benchmarkRows[row] = benchmarkRows[row].Remove(0, benchmarkRows[row].IndexOf("+") + 2);
                }
            }

            Console.WriteLine("Main Table:");
            for (int row = 0; row < MinBoard.GetLength(0); row++)
            {
                for (int col = 0; col < MinBoard.GetLength(1) - 1; col++)
                {
                    Console.Write(String.Format("{0}\t", MinBoard[row, col]));
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            OriginalSolutions();

            Console.WriteLine("Solutions for Constraints: ");
            foreach (var item in originalSolutions) { Console.Write(item + " "); }
            Console.WriteLine("\n");

            if (!FullText.Contains("max"))
            {
                Console.WriteLine("Minimization problem detected. Problem was transposed.\n");
                MinToMaxProblem();
            }
            else
            {
                MainBoard = MinBoard;
                SolutionSpace = originalSolutions;
            }
        }

        private void MinToMaxProblem()
        {
            List<double> tempSolutions = new List<double>();
            tempSolutions.Add(1); // Cost-function
            tempSolutions.AddRange(originalSolutions);

            for (int count = 0; count < tempSolutions.Count; count++)
            {
                MinBoard[count, Variables] = tempSolutions[count];
            }

            double[] SwitchRows = new double[Variables + 1];

            for (int column = 0; column <= Variables; column++)
            {
                SwitchRows[column] = MinBoard[0, column];
                MinBoard[0, column] = MinBoard[Constraints, column];
                MinBoard[Constraints, column] = SwitchRows[column];
            }

            int tempVariable = Constraints;
            Constraints = Variables;
            Variables = tempVariable;

            MaxBoard = new double[Constraints + 1, Constraints + Variables + 1];

            for (int row = 0; row < Constraints + 1; row++)
            {
                for (int column = 0; column <= Variables; column++)
                {
                    MaxBoard[row, column] = MinBoard[column, row];
                }
            }

            for (int column = 0; column <= Variables; column++)
            {
                SwitchRows[column] = MaxBoard[0, column];
                MaxBoard[0, column] = MaxBoard[Constraints, column];
                MaxBoard[Constraints, column] = SwitchRows[column];
            }

            double[] SwitchColumns = new double[Constraints + Variables];

            for (int row = 0; row <= Constraints; row++)
            {
                SwitchColumns[row] = MaxBoard[row, Variables];
                MaxBoard[row, Variables] = MaxBoard[row, Variables + Constraints];
                MaxBoard[row, Variables + Constraints] = SwitchColumns[row];
            }

            for (int row = 1; row <= Constraints; row++)
            {
                MaxBoard[row, row + Variables - 1] = 1;
            }

            SolutionSpaceAndMainBoard();
        }

        private List<double> OriginalSolutions()
        {
            for (int i = 1; i < benchmarkRows.Count; i++)
            {
                originalSolutions.Add(Convert.ToDouble(benchmarkRows[i].Split("=").Last().Trim()));
            }
            return originalSolutions;
        }

        private void SolutionSpaceAndMainBoard()
        {
            for (int i = 0; i < Constraints; i++)
            {
                SolutionSpace.Add(MaxBoard[i + 1, Variables + Constraints]);
            }
            MainBoard = MaxBoard;
        }
    }
}


