using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Julch_CGameOfLife
{
    public partial class Form1 : Form
    {
        public static int universeWidth = 25, universeHeight = 25;
        private Cell[,] universe = new Cell[universeWidth, universeHeight];
        private Cell[,] scratchPad = new Cell[universeWidth, universeHeight];

        private Color gridColor = Color.Black;
        private Color cellColor = Color.Gray;

        private Timer timer = new Timer();

        private int generations = 0;

        private bool _paused = true;
        private bool _firstRunTime = true;

        public Form1()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeUniverse();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            UniverseNext();

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }

        private void InitializeTimer()
        {
            // Setup the timer.

            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = true; // start timer running
        }

        private void InitializeUniverse() {
            // Setup the universe.

            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    universe[x, y] = new Cell();
                }
            }

            scratchPad = universe;

        }

        private void UniverseNext() {
            //Update the universe's next generation.

            //Apply changes based on the rules of conways game of life.
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    UpdateCell(x, y);
                }
            }

            //Update the universe with the scratch pad modifications.
            Cell[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;
        }

        private void UpdateCell(int x, int y) {

            int neighborCount = GetCellNeighborCount(x, y);

            if (universe[x, y].isAlive)
            {
                //Check for neighbors. method that returns number of neighbors.
                //Run through rules and then determine the current cells state.

                //RULE 1: Any living cell in the current universe with less than 2 living neighbors
                //dies in the next generation as if by under - population.If a cell meets this
                //criteria in the universe array then make the same cell dead in the scratch
                //pad array.

                //RULE 2: Any living cell with more than 3 living neighbors will die in the next generation as
                //if by over - population.If so in the universe then kill it in the scratch pad.


                //RULE 3: Any living cell with 2 or 3 living neighbors will live on into the next generation.
                //If this is the case in the universe then the same cell lives in the scratch pad.

                //RULE 4: Any dead cell with exactly 3 living neighbors will be born into the next generation
                //as if by reproduction. If so in the universe then make that cell alive in the scratch pad.


                if (neighborCount < 2) //RULE 1:
                {
                    scratchPad[x, y].isAlive = false;
                    scratchPad[x, y].generation = 0;
                }
                if (neighborCount > 3) //RULE 2:
                {
                    scratchPad[x, y].isAlive = false;
                    scratchPad[x, y].generation = 0;
                }
                if (neighborCount == 2 || neighborCount == 3) //RULE 3:
                {
                    scratchPad[x, y].isAlive = true;
                    scratchPad[x, y].generation += 1;
                }
            }
            else
            {
                if (neighborCount == 3) //RULE 4:
                {
                    scratchPad[x, y].isAlive = true;
                    scratchPad[x, y].generation = 0;
                }
            }
        }

        private int GetCellNeighborCount(int x, int y)
        {
            int count = 0;

            int minX = Math.Max(x - 1, universe.GetLowerBound(0));
            int maxX = Math.Min(x + 1, universe.GetUpperBound(0));
            int minY = Math.Max(y - 1, universe.GetLowerBound(1));
            int maxY = Math.Min(y + 1, universe.GetUpperBound(1));

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    if (universe[i, j].isAlive)
                    {
                        count += GetCellInBounds(i, j);
                    }
                }
            }
            return count;
        }

        private int GetCellInBounds(int x, int y)
        {
            if (x > universe.GetLength(0) - 1 || x < 0) { return 0; }
            if (y > universe.GetLength(1) - 1 || y < 0) { return 0; }
            return 1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_paused == false && _firstRunTime == false) {
                NextGeneration();
            }
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y].isAlive == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //Start Button.
            //If its the first time running the simulation,
            //unpause the game and set first time to false.

            if (_firstRunTime) {
                _paused = false;
                _firstRunTime = false;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //Pause Button.
            //If the game is currently paused, unpause it.
            //If the game is currently unpaused, pause it.

            if (_firstRunTime == false) {
                if (_paused) {
                    _paused = false;
                }
                else
                {
                    _paused = true;
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //Next Button.
            //If the game is currrently paused allow the button to initiate
            //the next generation.

            if (_firstRunTime) {
                if (_paused) {
                    NextGeneration();
                }
            }
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y].isAlive = !universe[x, y].isAlive;

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
    }
}
