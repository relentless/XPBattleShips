using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class Bot
    {
        public int GridLength { get; set; }
        public string GridLetter { get; set; }
        public List<string> Positions;
        public List<string> MyShipPositions { get; set; }
        public List<string> AttackedPositions { get; set; }
        public int MaxTurns { get; set; }
        public string[] Ships { get; set; }
        public int MineCount { get; set; }
        public int GridLengthDifference { get; set; }
        private int _placedShips;
        private char _initialChar = 'H';
        public List<string> MinePositions = new List<string>
        {
            "B1", "C3", "G1"
        };

        public List<ShipPosition> MyPlacedShips = new List<ShipPosition>
        {
            new ShipPosition {GridLetter = "B", GridNumber = 4, Orientation = "horizontal"},
            new ShipPosition {GridLetter = "H", GridNumber = 1, Orientation = "vertical"},
            new ShipPosition {GridLetter = "A", GridNumber = 3, Orientation = "vertical"},
            new ShipPosition {GridLetter = "B", GridNumber = 2, Orientation = "horizontal"},
            new ShipPosition {GridLetter = "G", GridNumber = 4, Orientation = "vertical"},
            new ShipPosition {GridLetter = "B", GridNumber = 6, Orientation = "horizontal"},
            new ShipPosition {GridLetter = "A", GridNumber = 8, Orientation = "horizontal"},
            new ShipPosition {GridLetter = "C", GridNumber = 3, Orientation = "horizontal"},
        };

        public Bot()
        {
            MyShipPositions = new List<string>();
            AttackedPositions = new List<string>();
            _placedShips = 0;
        }


        public ShipPosition PlaceShip()
        {
            if (_placedShips == MyPlacedShips.Count && GridLength > 8)
            {
                var random = new Random();

                var randomGridNumber = random.Next(1, GridLength - 4);
                _initialChar++;
                return new ShipPosition { GridLetter = _initialChar.ToString(), GridNumber = randomGridNumber, Orientation = "vertical" };
            }

            var ship = MyPlacedShips[_placedShips];
            ship.GridNumber += GridLengthDifference;
            _placedShips++;
            return ship;
        }

        public void SetupGrid(string gridSize, int maxTurns, string[] ships, int mineCount)
        {
            MaxTurns = maxTurns;
            Ships = ships;
            MineCount = mineCount;
            SetupGrid(gridSize);
        }

        public void MyShips(string[] myShip)
        {
            for (int i = 0; i < myShip.Length; i++)
            {
                MyShipPositions.Add(myShip[i]);
            }
        }

        public void SetupGrid(string gridSize)
        {
            Positions = new List<string>();
            GridLetter = gridSize.Substring(0, 1);
            GridLength = int.Parse(gridSize.Substring(1, gridSize.Length - 1));
            GridLengthDifference = GridLength > 8 ? GridLength - 8 : 0;

            for (char c = 'A'; c <= Convert.ToChar(GridLetter); c++)
            {
                //do something with letter 
                for (var i = 1; i < GridLength + 1; i++)
                {
                    Positions.Add(c + i.ToString());
                }
            }
        }

        public Move NextMoveByType()
        {
            if (MinePositions.Any())
            {
                var minePosition = MinePositions[0];
                MinePositions.Remove(minePosition);
                return new Move { Type = "MINE", GridReference = minePosition };
            }

            var move = string.Empty;
            if (MyShipPositions.Any())
            {
                move = MyShipPositions[0];
                MyShipPositions.Remove(move);
            }
            else
            {
                move = Positions[0];
            }

            Positions.Remove(move);

            return new Move { GridReference = move, Type = "ATTACK" };
        }

        public string NextMove()
        {
            var move = string.Empty;
            if (MyShipPositions.Any())
            {
                move = MyShipPositions[0];
                MyShipPositions.Remove(move);
            }
            else
            {
                move = Positions[0];
            }

            Positions.Remove(move);

            return move;
        }
    }

    public class Move
    {
        public string Type { get; set; }
        public string GridReference { get; set; }
    }

    public class ShipPosition
    {
        public string GridLetter { get; set; }
        public int GridNumber { get; set; }
        public string Orientation { get; set; }
    }
}
