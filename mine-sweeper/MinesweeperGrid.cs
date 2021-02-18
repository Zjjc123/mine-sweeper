using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    public class MinesweeperGrid
    {
        // stores the value (number of adj. mines) and whether it is a mine or not
        private bool isMine;
        private int value;

        public MinesweeperGrid(bool mine, int v)
        {
            isMine = mine;
            value = v;
        }

        public int Value { get => value; set => this.value = value; }
        public bool IsMine { get => isMine; set => this.isMine = value; }
    }
}
