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
        public static MinesweeperGrid[,] grid;

        public static bool gameOver = false;

        private static int r;
        private static int c;
        private static int m;

        public static void Initialize(int r_, int c_, int m_)
        {
            r = r_;
            c = c_;
            m = m_;

            grid = new MinesweeperGrid[r, c];

            for (int i = 0; i < r; i++) 
            { 
                for (int j = 0; j < c; j++)
                {
                    grid[i, j] = new MinesweeperGrid(false, 0);
                }
            }

            GenerateMines();
            GenerateNumbers();

        }

        public static void Click(int i, int j)
        {
            if (gameOver)
                return;

            Reveal(i, j);
            if (grid[i, j].IsMine)
            {
                EndGame(i, j);
            }
            else if (grid[i, j].Value == 0)
            {
                RevealEmpties(i, j);
            }
            CheckEnd();
        }

        public static void RightClick(int i, int j)
        {
            if (gameOver)
                return;

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

        private static void GenerateMines()
        {
            Random random = new Random();
            // generate random mine
            for (int i = 0; i < m; i++)
            {
                MinesweeperGrid picked_grid;
                do
                {
                    int mine_row = random.Next(r);
                    int mine_column = random.Next(c);

                    picked_grid = grid[mine_row, mine_column];
                } while (picked_grid.IsMine);
                picked_grid.IsMine = true;
            }
        }

        private static void GenerateNumbers()
        {
            // generate number grids
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    MinesweeperGrid picked_grid = grid[i, j];
                    if (!picked_grid.IsMine)
                    {
                        int count = 0;
                        // 3 x 3 scan but clamp max and min within array
                        for (int k = Math.Max(0, i - 1); k < Math.Min(i + 2, r); k++)
                        {
                            for (int l = Math.Max(0, j - 1); l < Math.Min(j + 2, c); l++)
                            {
                                if (grid[k, l].IsMine)
                                    count++;
                            }
                        }
                        picked_grid.Value = count;
                    }
                }
            }
        }

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

        private static void RevealEmpties(int i, int j)
        {
            MinesweeperGrid g = grid[i, j];
            Reveal(i, j);

            if (g.Value != 0)
                return;
            
            // make sure recursion doesn't go back
            g.Value = -1;

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

            foreach (Tuple<int, int> adj in adjs)
            {
                RevealEmpties(adj.Item1, adj.Item2);
            }
        }

        private static void Reveal(int i, int j)
        {
            MinesweeperGrid g = grid[i, j];
            Button b = MinesweeperGame.GetButton(i, j);
            b.FlatStyle = FlatStyle.Flat;
            b.BackColor = System.Drawing.Color.LightGray;
            if (g.IsMine)
            {
                b.Text = "\uD83D\uDCA3";

            }
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

        // lose
        private static void EndGame(int i, int j)
        {
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
            MinesweeperGame.Control_button.Text = "\u2620";
            gameOver = true;
        }

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

            if (count == (r * c - m))
            {
                EndGame();
            }
        }

        // win
        private static void EndGame()
        {
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
            MinesweeperGame.Control_button.Text = "\uD83D\uDE0E";
            gameOver = true;
        }

        public static void Reset()
        {
            Initialize(r, c, m);
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
            MinesweeperGame.Control_button.Text = "\uD83D\uDE42";
            gameOver = false;
        }
    }
}
