using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MineSweeper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MineSweeper.GetMenu();
        }

        static class MineSweeper
        {
            private const int EASY_MODE = 9;
            private const int NORMAL_MODE = 15;
            private const int HARD_MODE = 25;
            private const int HARD_MINES = 99;
            private const int NORMAL_MINES = 40;
            private const int EASY_MINES = 10;
            private  const char SAFE_CELL = '.';
            private  const char COVER = '■';
            private  const char FLAG_MARK = '►';
            private  const char MINE = '☻';

            private static int flagsNumber = 0;
            private static int fieldSize = 1;
            private static string difficulty = "easy";
            private static char[,] mineField;
            private static char[,] unexploredField;
            private static List<List<int>> boardPoints = new List<List<int>>();
            private static int minesNumber = 1;
            private static bool steppedOnMine = false;

            public static void GetMenu()
            {
                Console.Clear();
                Console.WriteLine("1.Start Game");
                Console.WriteLine("2.Exit");
                var input = Console.ReadLine();
                if (new Regex(@"(1)|(start)|(start game)").IsMatch(input) || input == "")
                {
                    StartGame();
                } else
                {
                    Console.WriteLine("Exiting...");
                    return;
                }
            }
            private static void StartGame()
            {
                Console.Clear();
                AskForDiffculty();
                mineField = SetBoard(fieldSize, true);
                unexploredField = SetBoard(fieldSize, false);
                DistributeMines();
                GenerateHints();

                while (true)
                {
                    GetFlagsNumber();
                    if (steppedOnMine) break;
                    if (IsWinningState()) break;
                    ShowBoard(unexploredField);
                    AskToSelectCell();
                    Console.Clear();
                }
                Console.Clear();
                if (steppedOnMine)
                {
                    ShowMines();
                    Console.WriteLine("You stepped on a mine and failed!");
                }
                if (IsWinningState())
                {
                    ShowMines();
                    Console.WriteLine("Congratulations! You found all the mines!"); 
                }
                AskForRestart();
            }

            private static void AskForRestart()
            {
                Console.WriteLine("Do you want to restart? (yes/no)");
                var input = Console.ReadLine();
                if (input == "" || input.ToLower() == "yes")
                {
                    GetMenu();
                }
                else
                {
                    Console.WriteLine("Exiting...");
                }
            }

            private static bool IsWinningState()
            {
                int trueFlags = 0;
                int coveredCells = 0;
                foreach (var cell in unexploredField)
                {
                    if (cell == COVER)
                    {
                        coveredCells++;
                    }
                }
                if (coveredCells == minesNumber && flagsNumber == 0) return true;
                for (int y = 0; y < fieldSize; y++)
                {
                    for (int x = 0; x < fieldSize; x++)
                    {
                        if (unexploredField[y, x] == FLAG_MARK)
                        {
                            if (mineField[y, x] == MINE)
                            {
                                trueFlags++;
                            }
                        }
                    }
                }
                if (trueFlags == minesNumber && trueFlags == flagsNumber) return true;
                return false;
            }
            private static char[,] SetBoard(int size, bool isMineField)
            {
                var field = new char[fieldSize, fieldSize];
                for (int y = 0; y < fieldSize; y++)
                {
                    for (int x = 0; x < fieldSize; x++)
                    {
                        field[y, x] = isMineField ? SAFE_CELL : COVER;
                    }
                }
                return field;
            }
            private static void AskForDiffculty()
            {
                Console.WriteLine($"Choose Difficulty [Easy, Normal, Hard]:");
                var input = "";
                while (true)
                {
                    input = Console.ReadLine().ToLower();
                    if (!new Regex(@"(easy|normal|hard)").IsMatch(input))
                    {
                        Console.WriteLine("Please enter valid command!");
                        continue;
                    }
                    break;
                }
                switch (input)
                {
                    case "easy":
                        DifficultyMode("easy"); break;
                    case "normal":
                        DifficultyMode("normal"); break;
                    case "hard":
                        DifficultyMode("hard"); break;
                }
            }
            private static void DifficultyMode(string mode)
            {
                difficulty = mode;
                switch (mode)
                {
                    case "easy":
                        minesNumber = EASY_MINES;
                        fieldSize = EASY_MODE;
                        break;
                    case "normal":
                        minesNumber = NORMAL_MINES;
                        fieldSize = NORMAL_MODE;
                        break;
                    case "hard":
                        minesNumber = HARD_MINES;
                        fieldSize = HARD_MODE;
                        break;
                }
            }
            private static void AskToSelectCell()
            {
                int y;
                int x;
                string command;
                while (true)
                {
                    Console.WriteLine("Flag cells or claim a cell as free[free/flag]:");
                    var input = Console.ReadLine();
                    string inputResult = Regex.Replace(input, @"\s+", " ");
                    if (!new Regex(@"\d+\s*\d+\s*(free|flag)").IsMatch(input))
                    {
                        Console.WriteLine("Invalid move, try again.");
                        continue;
                    }
                    var move = inputResult.Split(" ");
                    y = int.Parse(move[1]);
                    x = int.Parse(move[0]);
                    command = move[2];
                    var range = Enumerable.Range(1, fieldSize);
                    if (!(range.Contains(y) && range.Contains(x)))
                    {
                        Console.WriteLine($"your X and Y coordinates must be between [1 - {fieldSize}]");
                        continue;
                    }
                    break;
                }
                RevealCell(y, x, command);
            }
            private static void RevealCell(int row, int col, string command)
            {
                int y = row - 1;
                int x = col - 1;
                if (command == "free")
                {
                    if (unexploredField[y, x] == COVER || unexploredField[y, x] == FLAG_MARK)
                    {
                        FreeCell(y, x);
                        return;
                    } else
                    {
                        Console.WriteLine("This cell is exposed, choose diffrent cell.");
                        return;
                    }
                }
                else if (command == "flag")
                {
                    FlageCell(y, x);
                    return;
                }
            }
            private static void FlageCell(int y, int x)
            {
                if (unexploredField[y, x] != FLAG_MARK)
                {
                    unexploredField[y, x] = FLAG_MARK;
                }else
                {
                    unexploredField[y, x] = COVER;
                }
            }
            private static void FreeCell(int y, int x)
            {
                if (mineField[y, x] == MINE)
                {
                    steppedOnMine = true;
                    Console.WriteLine("You stepped on a mine and failed!");
                    return;
                }

                if (mineField[y, x] != SAFE_CELL)
                {
                    unexploredField[y, x] = mineField[y, x];
                    return;
                }
                var points = new List<List<int>> ();
                points.Add(new List<int> {y, x});
                ExploreField(points);
            }
            private static void ExploreField(List<List<int>> pointList)
            {
                if (pointList.Count == 0) return;
                var exploringPoints = new List<List<int>>();
                foreach (var point in pointList)
                {
                    unexploredField[point[0], point[1]] = mineField[point[0], point[1]];
                    var surroundPoints = GetSurroundingTiles(point);
                    foreach (var p in surroundPoints)
                    {
                        if (unexploredField[p[0], p[1]] == COVER || unexploredField[p[0], p[1]] == FLAG_MARK)
                        {
                            unexploredField[p[0], p[1]] = mineField[p[0], p[1]];
                            if (unexploredField[p[0], p[1]] == SAFE_CELL)
                            {
                                exploringPoints.Add(p);
                            }
                        }
                    }
                }
                ExploreField(exploringPoints);
            }
            private static List<List<int>> GetSurroundingTiles(List<int> point)
            {
                var limit = fieldSize;
                var pointList = new List<List<int>>();
                var y = point[0] - 1;
                var x = point[1] - 1;
                for (int i = y; i <= y + 2; i++)
                {
                    for (int j = x; j <= x + 2; j++)
                    {
                        var p = new List<int>() { i, j };
                        if (!p.Contains(-1) && !p.Contains(limit) && p != point)
                        {
                            pointList.Add(p);
                        }
                    }
                }
                return pointList;
            }
            private static void GenerateHints()
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    for (int x = 0; x < fieldSize; x++)
                    {
                        var mines = 0;
                        var surroundTiles = GetSurroundingTiles(new List<int> {y, x});
                        foreach (var tile in surroundTiles)
                        {
                            if (mineField[tile[0], tile[1]] == MINE)
                            {
                                mines++;
                            }
                        }
                        if (mines > 0 && mineField[y, x] == SAFE_CELL)
                        {
                            mineField[y, x] = $"{mines}".ToCharArray()[0];
                        }
                    }
                }
            }
            private static void GetBoardPoints()
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    for (int x = 0; x < fieldSize; x++)
                    {
                        boardPoints.Add(new List<int> { y, x });
                    }
                }
                boardPoints.RemoveAt(0);
            }
            private static void ShowBoard(char[,] board)
            {
                string topNumbers = MakeTopChartNumbers();
                Console.WriteLine($"Mode:{difficulty.ToUpper()}{new String(' ', (topNumbers.Length - 20))}" +
                    $"Flags({FLAG_MARK}):{minesNumber - flagsNumber}");
                Console.WriteLine($"{new String('=', topNumbers.Length + 4)}");
                Console.WriteLine($"  │{topNumbers}│");
                
                Console.WriteLine($"──│{ new String('─', topNumbers.Length)}│");
                 
                for (int y = 0; y < fieldSize; y++)
                {
                    var leftChart = (y + 1 < 10) ? $" {y + 1}" : $"{y + 1}";
                    Console.Write($"{leftChart}│");
                    for (int x = 0; x < fieldSize; x++)
                    {
                        var end = (x == fieldSize - 1) ? "│" : "";
                        Console.Write($" {board[y, x]} {end}"); ;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"──│{new String('─', topNumbers.Length)}│");
            }
            private static string MakeTopChartNumbers()
            {
                var intListChart = Enumerable.Range(1, fieldSize).ToList();
                var strListChart = new List<String>();
                foreach (var item in intListChart)
                {
                    if (item < 10)
                    {
                        strListChart.Add($" {item} ");
                    } else
                    {
                        strListChart.Add($"{item} ");
                    }
                }
                return String.Join("", strListChart);
            }
            private static void DistributeMines()
            {
                GetBoardPoints();
                Random random = new Random();
                var shuffledPoints = boardPoints.OrderBy(a => Guid.NewGuid()).ToList();
                var selectedPoints = shuffledPoints.Take(minesNumber);
                foreach (var p in selectedPoints)
                {
                    mineField[p[0], p[1]] = MINE;
                }
            }
            private static void GetFlagsNumber()
            {
                int flags = 0;
                foreach (var cell in unexploredField)
                {
                    if (cell == FLAG_MARK)
                    {
                        flags++;
                       
                    }
                }
                flagsNumber = flags;
            }
            private static void ShowMines()
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    for (int x = 0; x < fieldSize; x++)
                    {
                        if (mineField[y, x] == MINE) unexploredField[y, x] = MINE;
                    }
                }
                ShowBoard(unexploredField);
            }
        }
    }
}