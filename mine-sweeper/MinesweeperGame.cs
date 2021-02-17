using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public static class MinesweeperGame
    {
        // public class constants
        internal const int ROWS = 10;
        internal const int COLS = 16;
        internal const int BUTTON_SIZE = 35;
        internal const int NUM_MINES = 20;

        // private class variables
        private static Form form;
        private static Button control_button;
        private static Button[,] buttons;

        private static int VERTICAL_OFFSET = 60;

        public static Button Control_button { get => control_button; }

        // initializes form buttons and sets form width and height
        public static void Initialize(Form f)
        {
            // set up the form
            MinesweeperGame.form = f;
            int titleHeight = f.Height - f.ClientRectangle.Height + VERTICAL_OFFSET;
            f.Size = new Size(BUTTON_SIZE * COLS + COLS,
                              BUTTON_SIZE * ROWS + ROWS + titleHeight);

            control_button = new Button();
            control_button.Left = (f.Width - control_button.Width) / 2;
            control_button.Top = 10;
            control_button.Width = 40;
            control_button.Height = 40;
            control_button.Font = new Font(control_button.Font.Name, Convert.ToInt32(control_button.Height * 0.3333333333333333));
            control_button.Text = "\uD83D\uDE42";
            control_button.AutoSize = false;
            control_button.TextAlign = ContentAlignment.MiddleCenter;
            control_button.Dock = DockStyle.None;
            control_button.Click += new EventHandler(MinesweeperGame.Reset);
            f.Controls.Add(control_button); 

            // create the buttons on the form
            buttons = new Button[ROWS, COLS];
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    // create a new button control
                    Button b = new Button();
                    buttons[i, j] = b;
                    b.Width = BUTTON_SIZE;
                    b.Height = BUTTON_SIZE;
                    b.Top = i * BUTTON_SIZE + VERTICAL_OFFSET;
                    b.Left = j * BUTTON_SIZE;
                    b.Text = String.Empty;
                    b.Name = i + "_" + j;
                    b.FlatStyle = FlatStyle.Popup;
                    b.MouseDown += new MouseEventHandler(MinesweeperGame.Click);
                    // add the button control to the form
                    f.Controls.Add(b);
                }
            }

            MinesweeperBoard.Initialize(ROWS, COLS, NUM_MINES);
            MinesweeperBoard.DebugBoard();
        }

        // event handler for all minesweeper button click events
        private static void Click(object sender, MouseEventArgs e) {
            Button b = (Button)sender;
            int index = b.Name.IndexOf("_");
            int i = int.Parse(b.Name.Substring(0, index));
            int j = int.Parse(b.Name.Substring(index + 1));

            switch (e.Button)
            {
                case MouseButtons.Left:
                    MinesweeperBoard.Click(i, j);
                    break;

                case MouseButtons.Right:
                    MinesweeperBoard.RightClick(i, j);
                    break;
            }
        }

        // event handler for all minesweeper button click events
        private static void Reset(object sender, EventArgs e)
        {
            MinesweeperBoard.Reset();
        }

        // retrieve a button control at row "i" and column "j"
        public static Button GetButton(int i, int j)
        {
            if (i < 0 || i >= ROWS)
            {
                throw new ArgumentException("row index out of range");
            }
            if (j < 0 || j >= COLS)
            {
                throw new ArgumentException("column index out of range");
            }
            return buttons[i, j];
        }
    }
}
