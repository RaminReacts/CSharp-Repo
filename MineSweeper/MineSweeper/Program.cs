using System.Text.RegularExpressions;

namespace MineSweeper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MineSweeper.Run();
        }

        static class MineSweeper
        {
            private  const char SAFE_CELL = '.';
            private  const char MINE = 'X';
            private  const int FIELD_SIZE = 9;
            private static char[,] mineField = new char[FIELD_SIZE, FIELD_SIZE];
            private static List<List<int>> boardPoints = new List<List<int>>();
            private static int minesNumber = 1;

            static MineSweeper()
            {
                SetBoard();
            }

            public static void Run()
            {
                AskForMinesNumber();
                DistributeMines();
                ShowBoard(mineField);
            }
            private static void AskForMinesNumber()
            {
                var limit = Math.Pow(FIELD_SIZE, 2) - 1;
                Console.WriteLine($"How many mines do you want on the field [1 - {limit}]?");
                var input = "";

                do
                {
                    input = Console.ReadLine();
                    if (!new Regex(@"\d+").IsMatch(input))
                    {
                        Console.WriteLine($"Please enter the number between [1 - {limit}]:");
                        continue;
                    }
                    if (!(0 < int.Parse(input) && int.Parse(input) < limit))
                    {
                        Console.WriteLine($"Please enter the number between [1 - {limit}]:");
                        continue;
                    }
                    minesNumber = int.Parse(input);
                    break;
                } while (true);
            }

            private static void GetBoardPoints()
            {
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    for (int x = 0; x < FIELD_SIZE; x++)
                    {
                        boardPoints.Add(new List<int> { y, x });
                    }
                }
                boardPoints.RemoveAt(0);
            }

            public static void Greet()
            {
                Console.WriteLine("Welcome to minsweeper!");
            }

            private static void SetBoard()
            {
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    for (int x = 0; x < FIELD_SIZE; x++)
                    {
                        mineField[y, x] = SAFE_CELL;
                    }
                }
            }

            private static void ShowBoard(char[,] board)
            {
                var topChartList = Enumerable.Range(1, FIELD_SIZE).ToList();
                string topNumbers = String.Join(" ", topChartList);
                Console.WriteLine($" │ {topNumbers} │");
                Console.WriteLine($"─│{ new String('─', FIELD_SIZE * 2 + 1)}│");
                 
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    Console.Write($"{y + 1}│");
                    for (int x = 0; x < FIELD_SIZE; x++)
                    {
                        var end = (x == FIELD_SIZE - 1) ? " │" : "";
                        Console.Write($" {board[y, x]}{end}"); ;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"─│{new String('─', FIELD_SIZE * 2 + 1)}│");

            }

            private static void DistributeMines()
            {
                GetBoardPoints();
                Random random = new Random();
                //var shuffledPoints = boardPoints.OrderBy(x => random.Next()).ToArray();
                var shuffledPoints = boardPoints.OrderBy(a => Guid.NewGuid()).ToList();
                var selectedPoints = shuffledPoints.Take(minesNumber);
                foreach (var p in selectedPoints)
                {
                    mineField[p[0], p[1]] = MINE;
                }

            }


        }
    }
}