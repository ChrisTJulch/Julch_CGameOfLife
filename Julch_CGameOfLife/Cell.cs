using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Julch_CGameOfLife
{
    public class Cell
    {
        public bool isAlive;
        public int generation;

        public Cell(bool active = false) {
            this.isAlive = active;
            this.generation = 0;
        }
    }
}
