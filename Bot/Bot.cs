using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot
{
    public class Bot
    {
        public List<string> MinePositions;

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

        public List<string> Positions;
        private char _initialChar = 'H';
        private int _placedShips;

        public Bot()
        {
            MinePositions = new List<string>();
            MyShipPositions = new List<string>();
            AttackedPositions = new List<string>();
            HitProximity = new List<string>();
            _placedShips = 0;
        }

        public int GridLength { get; set; }
        public string GridLetter { get; set; }
        public List<string> MyShipPositions { get; set; }
        public List<string> AttackedPositions { get; set; }
        public List<string> HitProximity { get; set; }
        public int MaxTurns { get; set; }
        public string[] Ships { get; set; }
        public int MineCount { get; set; }
        public int GridLengthDifference { get; set; }
        public MoveResult LastResult { get; set; }

        public void SetLastResult(string result, string move)
        {
            var moveResult = new MoveResult
            {
                Letter = move.Substring(0, 1),
                Number = int.Parse(move.Substring(1, move.Length - 1)),
                Result = result
            };
            LastResult = moveResult;

            if (moveResult.Result == "HIT" && !HitProximity.Any())
            {
                var letter = Convert.ToChar(moveResult.Letter);
                var number = moveResult.Number;

                for (int i = 1; i < 5; i++)
                {
                    number++;

                    if (number <= GridLength)
                    {
                        HitProximity.Add(moveResult.Letter + number);
                    }
                }


                var maxLetter = Convert.ToChar(GridLetter);
                maxLetter++;

                while (letter != maxLetter)
                {
                    letter++;
                    HitProximity.Add(letter.ToString() + moveResult.Number);
                }
            }
        }

        public ShipPosition PlaceShip()
        {
            if (_placedShips == MyPlacedShips.Count && GridLength > 8)
            {
                var random = new Random();

                int randomGridNumber = random.Next(1, GridLength - 4);
                _initialChar++;
                return new ShipPosition
                {
                    GridLetter = _initialChar.ToString(),
                    GridNumber = randomGridNumber,
                    Orientation = "vertical"
                };
            }

            ShipPosition ship = MyPlacedShips[_placedShips];
            ship.GridNumber += GridLengthDifference;
            _placedShips++;
            return ship;
        }

        public void SetupGrid(string gridSize, int maxTurns, string[] ships, int mineCount)
        {
            MinePositions = new List<string>
            {
                "B1",
                "C3",
                "G1",
                "G3",
                "D5"
            };
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
                for (int i = 1; i < GridLength + 1; i++)
                {
                    Positions.Add(c + i.ToString());
                }
            }
        }

        public Move NextMoveByType()
        {
            if (MinePositions.Any())
            {
                string minePosition = MinePositions[0];
                MinePositions.Remove(minePosition);
                return new Move { Type = "MINE", GridReference = minePosition };
            }

            string move = string.Empty;
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

            if (HitProximity.Any())
            {
                move = HitProximity[0];
                HitProximity.Remove(move);
            }
            else
            {
                move = Positions[0];
            }

            Positions.Remove(move);

            return move;
        }
    }

    public class MoveResult
    {
        public string Result { get; set; }
        public string Letter { get; set; }
        public int Number { get; set; }
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
