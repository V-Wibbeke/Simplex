using System;
using System.Collections.Generic;

namespace KI_Projekt
{
    class simplex
    {
        private Double[,] ShiftVariables;
        private Double[,] CalculatorVariables;
        private List<int> solutionsPosition;
        private int keyRow;
        private int keyColumn;
        private double keyElement;
        private bool isOptimal;
        private int NrOfRepetitions = 0;
        public Double[,] MainTable { get; set; }
        public int Constraints { get; set; }
        public int Variables { get; set; }
        public List<Double> Solutions { get; set; }

        public void Simplex()
        {
            SetStartingConditions();

            if (!isOptimal)
            {
                Iteration();
            }
            else
            {
                OptimalSolution();
            }
        }

        private void SetStartingConditions()
        {
            ShiftVariables = new Double[Constraints, 3];
            for (int i = 0; i < Constraints; i++)
            {
                ShiftVariables[i, 0] = MainTable[0, Variables + i];
                ShiftVariables[i, 1] = Solutions[i];
            }

            solutionsPosition = new List<int>();
            for (int count = 0; count < Constraints; count++)
            {
                solutionsPosition.Add(Variables + count);
            }

            SetCalculatorVariables();
            KeyColumn();
            Ratio();
            KeyRow();
            KeyElement();

            Console.WriteLine("\nMaximization Problem:");
            Status();
        }

        private void SetCalculatorVariables()
        {
            CalculatorVariables = new Double[2, Variables + Constraints];

            for (int column = 0; column < Constraints + Variables; column++)
            {
                double Zj = 0;
                for (int row = 0; row < Constraints; row++)
                {
                    Zj += (MainTable[row + 1, column] * ShiftVariables[row, 0]);
                }
                CalculatorVariables[0, column] = Zj;
                CalculatorVariables[1, column] = MainTable[0, column] - Zj;
            }
        }

        private void Ratio()
        {
            for (int vertical = 0; vertical < Constraints; vertical++)
            {
                ShiftVariables[vertical, 2] = ShiftVariables[vertical, 1] / MainTable[vertical + 1, Convert.ToInt32(keyColumn)];
            }
        }

        private void KeyColumn()
        {
            isOptimal = true;
            keyColumn = 0;
            double tempValue = CalculatorVariables[1, 0];
            for (int pos = 1; pos < Variables + Constraints; pos++)
            {
                if (tempValue > 0)
                {
                    isOptimal = false;
                }
                if (tempValue <= CalculatorVariables[1, pos])
                {
                    tempValue = CalculatorVariables[1, pos];
                    keyColumn = pos;
                }
            }
        }

        private void KeyRow()
        {
            keyRow = 0;
            double tempValue = ShiftVariables[0, 2];
            for (int pos = 1; pos < Constraints; pos++)
            {
                if (tempValue > ShiftVariables[pos, 2] && ShiftVariables[pos, 2] > 0)
                {
                    tempValue = ShiftVariables[pos, 2];
                    keyRow = pos;
                }
            }
        }

        private void KeyElement()
        {
            keyElement = MainTable[Convert.ToInt32(keyRow) + 1, Convert.ToInt32(keyColumn)];
        }

        private void Iteration()
        {
            NrOfRepetitions++;

            solutionsPosition[keyRow] = keyColumn;

            double[] tempArray = new double[Variables + Constraints + 1];
            for (int count = 0; count <= Variables + Constraints; count++)
            {
                if (count == 0)
                {
                    tempArray[count] = ShiftVariables[keyRow, 1];
                }
                else
                {
                    tempArray[count] = MainTable[keyRow + 1, count - 1];
                }
            }
            double temp;

            ShiftVariables[keyRow, 0] = MainTable[0, keyColumn];
            for (int count = 0; count < Variables + Constraints; count++)
            {
                MainTable[keyRow + 1, count] = MainTable[keyRow + 1, count] / keyElement; //changes all Elements within the keyrow
            }
            ShiftVariables[keyRow, 1] = ShiftVariables[keyRow, 1] / keyElement;

            for (int row = 0; row < Constraints; row++)
            {
                if (row != keyRow)
                {
                    temp = MainTable[row + 1, keyColumn];
                    for (int count = 0; count < Variables + Constraints; count++)
                    {
                        MainTable[row + 1, count] = MainTable[row + 1, count] - ((tempArray[count + 1] * temp) / keyElement);
                    }
                    ShiftVariables[row, 1] = ShiftVariables[row, 1] - ((tempArray[0] * temp) / keyElement);
                }
            }

            SetCalculatorVariables();
            KeyColumn();
            Ratio();
            KeyRow();
            KeyElement();

            Console.WriteLine("\n\n" + NrOfRepetitions + ". Iteration:");
            Status();

            OptimalSolution();
        }

        private void OptimalSolution()
        {
            string tempString = "";
            double tempvalue = 0;
            double optimalSolution = 0;
            for (int column = 0; column < Variables + Constraints; column++)
            {
                for (int row = 0; row < Constraints; row++)
                {
                    if (column == solutionsPosition[row])
                    {
                        tempvalue = ShiftVariables[row, 1];
                        optimalSolution += MainTable[0, column] * tempvalue;
                        break;
                    }
                    tempvalue = 0;
                }
                tempString += MainTable[0, column] + "(" + tempvalue + ")";
                if (column != Variables + Constraints - 1)
                {
                    tempString += " + ";
                }
            }

            if (!isOptimal)
            {
                Console.WriteLine("\nCurrent Solution:");
                Console.WriteLine(tempString + " = " + optimalSolution);
                Iteration();
            }
            else
            {
                Console.WriteLine("\nSolution was found:");
                Console.WriteLine(tempString + " = " + optimalSolution);

                Console.WriteLine("\nVariables of minimization problem:");
                Console.WriteLine("x0: " + CalculatorVariables[0, Variables + Constraints - 1]);
                for (int i = 1; i < Constraints; i++)
                {
                    Console.WriteLine("x" + i + ": " + CalculatorVariables[0, Variables + i - 1]);
                }

                Console.WriteLine("\n\nFinal solution: " + optimalSolution);
            }
        }

        private void Status()
        {
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");

            Console.WriteLine("Main Table:");
            for (int row = 0; row < MainTable.GetLength(0); row++)
            {
                for (int col = 0; col < MainTable.GetLength(1) - 1; col++)
                {
                    Console.Write(String.Format("{0}\t", MainTable[row, col]));
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nCalculatorvariables:");
            for (int row = 0; row < CalculatorVariables.GetLength(0); row++)
            {
                for (int col = 0; col < CalculatorVariables.GetLength(1); col++)
                {
                    Console.Write(String.Format("{0}\t", CalculatorVariables[row, col]));
                }
                Console.WriteLine();
            }

            Console.WriteLine("\nSchiftvariables:\nCB  Solution  Ratio");
            for (int row = 0; row < ShiftVariables.GetLength(0); row++)
            {
                for (int col = 0; col < ShiftVariables.GetLength(1); col++)
                {
                    Console.Write(String.Format("{0}\t", ShiftVariables[row, col]));
                }
                Console.WriteLine();
            }
        }
    }
}