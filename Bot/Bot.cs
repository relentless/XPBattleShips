using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class Bot
    {
        public class Bot
    {
        public int GridLength { get; set; }
        public string GridLetter { get; set; }
        public List<string> Positions;

        public void SetupGrid(string gridSize)
        {
            Positions = new List<string>();
            GridLetter = gridSize.Substring(0, 1);
            GridLength = int.Parse(gridSize.Substring(1, gridSize.Length-1));

            for (char c = 'A'; c <= Convert.ToChar(GridLetter); c++)
            {
                //do something with letter 
                for (var i = 1; i < GridLength + 1; i++)
                {
                    Positions.Add(c + i.ToString());
                }
            } 


        }

        public string NextMove()
        {
            var move = Positions[0];
            Positions.Remove(move);
            return move;
        }
    }
}
