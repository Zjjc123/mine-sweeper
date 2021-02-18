using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public static class MinesweeperBoard
    {
        // stores a matrix of game state
        public static MinesweeperGrid[,] grid;

        // track if the game is over or not
        public static bool gameOver = false;

        private static int _r;
        private static int _c;
        private static int _m;

        // initialize game board
        public static void Initialize(int r, int c, int m)
        {
            _r = r;
            _c = c;
            _m = m;

            grid = new MinesweeperGrid[_r, _c];

            // create an empty r by c matrix
            for (int i = 0; i < _r; i++) 
            { 
                for (int j = 0; j < _c; j++)
                {
                    grid[i, j] = new MinesweeperGrid(false, 0);
                }
            }

            GenerateMines();
            GenerateNumbers();

        }

        // handle clicks
        public static void Click(int i, int j)
        {
            // disable if game over
            if (gameOver)
                return;

            // show grid
            Reveal(i, j);

            // end the game if the grid is a mine
            if (grid[i, j].IsMine)
            {
                EndGame(i, j);
            }
            // if the grid is empty recursively show other empty and border number tiles
            else if (grid[i, j].Value == 0)
            {
                RevealEmpties(i, j);
            }
            CheckEnd();
        }

        // handles right click (places flag)
        public static void RightClick(int i, int j)
        {
            if (gameOver)
                return;

            // if it is not clicked --> place a flag
            Button b = MinesweeperGame.GetButton(i, j);
            if (b.FlatStyle == FlatStyle.Popup)
            {
                if (b.Text.Equals("\u2691"))
                {
                    b.Text = "";
                    b.ForeColor = System.Drawing.Color.Black;
                }
                else
                {
                    b.Text = "\u2691";
                    b.ForeColor = System.Drawing.Color.Red;
                }
            }
        }


        // generate mines randomly
        private static void GenerateMines()
        {
            Random random = new Random();
            // generate random mine
            for (int i = 0; i < _m; i++)
            {
                // ensures no repeats
                MinesweeperGrid picked_grid;
                do
                {
                    int mine_row = random.Next(_r);
                    int mine_column = random.Next(_c);

                    picked_grid = grid[mine_row, mine_column];
                } while (picked_grid.IsMine);
                picked_grid.IsMine = true;
            }
        }

        // generate number tiles
        private static void GenerateNumbers()
        {
            // generate number grids
            for (int i = 0; i < _r; i++)
            {
                for (int j = 0; j < _c; j++)
                {
                    MinesweeperGrid picked_grid = grid[i, j];
                    if (!picked_grid.IsMine)
                    {
                        int count = 0;
                        // 3 x 3 scan but clamp max and min within array
                        for (int k = Math.Max(0, i - 1); k < Math.Min(i + 2, _r); k++)
                        {
                            for (int l = Math.Max(0, j - 1); l < Math.Min(j + 2, _c); l++)
                            {
                                // count the total of adjacent mines
                                if (grid[k, l].IsMine)
                                    count++;
                            }
                        }
                        picked_grid.Value = count;
                    }
                }
            }
        }


        // function to print the board to the console
        public static void DebugBoard()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j].IsMine)
                    {
                        Console.Write("[M] ");
                    }
                    else if (grid[i, j].Value > 0)
                    {
                        Console.Write("[{0}] ", grid[i, j].Value);
                    }
                    else
                    {
                        Console.Write("[ ] ");
                    }
                }
                Console.WriteLine();
            }
        }

        // recursively reveal empty tiles
        private static void RevealEmpties(int i, int j)
        {
            MinesweeperGrid g = grid[i, j];
            Reveal(i, j);

            // base case of number tile --> exit
            if (g.Value != 0)
                return;
            
            // make sure recursion doesn't go back
            g.Value = -1;

            // get adjacent tiles
            List<Tuple<int, int>> adjs = new List<Tuple<int, int>>();
            if (i > 0) {
                MinesweeperGrid adj_g = grid[i - 1, j];
                adjs.Add(new Tuple<int, int>(i - 1, j));
            }
            if (i < grid.GetLength(0) - 1)
            {
                MinesweeperGrid adj_g = grid[i + 1, j];
                adjs.Add(new Tuple<int, int>(i + 1, j));
            }
            if (j > 0)
            {
                MinesweeperGrid adj_g = grid[i, j - 1];
                adjs.Add(new Tuple<int, int>(i, j - 1));
            }
            if (j < grid.GetLength(1) - 1)
            {
                MinesweeperGrid adj_g = grid[i, j + 1];
                adjs.Add(new Tuple<int, int>(i, j + 1));
            }

            // reveal each adjacent tiles
            foreach (Tuple<int, int> adj in adjs)
            {
                RevealEmpties(adj.Item1, adj.Item2);
            }
        }

        // reveal tiles based on tile state
        private static void Reveal(int i, int j)
        {
            MinesweeperGrid g = grid[i, j];
            Button b = MinesweeperGame.GetButton(i, j);
            b.FlatStyle = FlatStyle.Flat;
            b.BackColor = System.Drawing.Color.LightGray;

            // show bomb if its a mine
            if (g.IsMine)
            {
                b.Text = "\uD83D\uDCA3";

            }
            // show number with colors
            else if (g.Value > 0)
            {
                b.Text = g.Value.ToString();
                switch (g.Value)
                {
                    case 1:
                        b.ForeColor = System.Drawing.Color.Blue;
                        break;
                    case 2:
                        b.ForeColor = System.Drawing.Color.Green;
                        break;
                    case 3:
                        b.ForeColor = System.Drawing.Color.Red;
                        break;
                    case 4:
                        b.ForeColor = System.Drawing.Color.DarkBlue;
                        break;
                    case 5:
                        b.ForeColor = System.Drawing.Color.DarkRed;
                        break;
                    case 6:
                        b.ForeColor = System.Drawing.Color.DarkCyan;
                        break;
                }

            }
        }

        // lost the game
        private static void EndGame(int i, int j)
        {
            // reveal all mines
            Button b = MinesweeperGame.GetButton(i, j);
            for (int k = 0; k < grid.GetLength(0); k++)
            {
                for (int l = 0; l < grid.GetLength(1); l++)
                {
                    if (grid[k, l].IsMine)
                        Reveal(k, l);
                }
            }
            b.BackColor = System.Drawing.Color.Red;
            // made dead face
            MinesweeperGame.Control_button.Text = "\u2620";
            gameOver = true;
        }

        // check to see if the player has won
        private static void CheckEnd()
        {
            int count = 0;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (MinesweeperGame.GetButton(i, j).FlatStyle == FlatStyle.Flat && grid[i, j].IsMine == false)
                        count++;
                }
            }

            // if total opened equals the total - mines
            if (count == (_r * _c - _m))
            {
                EndGame();
            }
        }

        // handle win
        private static void EndGame()
        {
            // reveal all mines and set them green
            for (int k = 0; k < grid.GetLength(0); k++)
            {
                for (int l = 0; l < grid.GetLength(1); l++)
                {
                    if (grid[k, l].IsMine)
                    {
                        Reveal(k, l);
                        MinesweeperGame.GetButton(k, l).ForeColor = System.Drawing.Color.Black;
                        MinesweeperGame.GetButton(k, l).BackColor = System.Drawing.Color.Green;
                    }
                }
            }
            // make cool face
            MinesweeperGame.Control_button.Text = "\uD83D\uDE0E";
            gameOver = true;
        }

        // reset game
        public static void Reset()
        {
            // reset board state
            Initialize(_r, _c, _m);
            // reset button styles
            for(int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Button b = MinesweeperGame.GetButton(i, j);
                    b.FlatStyle = FlatStyle.Popup;
                    b.ForeColor = System.Drawing.Color.Black;
                    b.BackColor = default(System.Drawing.Color);
                    b.Text = String.Empty;
                }
            }
            // reset control face
            MinesweeperGame.Control_button.Text = "\uD83D\uDE42";
            // enable control
            gameOver = false;
        }
    }
}
