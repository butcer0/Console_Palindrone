using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using ConsoleApp_Sandbox.Puzzles.Models;

namespace ConsoleApp_Sandbox.Puzzles
{
    public class PathFindingPuzzle
    {
        private const char SPACE_CHAR = ' ';
        private const char TARGET_CHAR = 'B';
        private const char PLAYER_CHAR = '.';
        private const char FOUND_PATH_CHAR = '_';
        private const int DELAY_TIME_MS = 300;
        private const int DELAY_TIME_MS_TRACE_PATH = 200;

        static readonly string[] MAP_TEST = new string[]
        {
            "+------+",
            "|      |",
            "|A X   |",
            "|XXX   |",
            "|   X  |",
            "| B    |",
            "|      |",
            "+------+",
        };

        static readonly string[] MAP_LEVEL_0 = new string[]
        {
            "+----------------+",
            "|    XX          |",
            "|A X    XXXX XXXX|",
            "|XXX      X      |",
            "|   XXXXXXXXXX   |",
            "| B     XXXX   X |",
            "|XX           XX |",
            "+----------------+",
        };
        
        static readonly string[] MAP_LEVEL_1 = new string[]
        {
            "+--------------------------+",
            "|    XX                    |",
            "|A X    XXXX XXXX    XXXXX |",
            "|XXX      X      XXXX      |",
            "|   XXXXXXXXXX    X   XX XX|",
            "| B     XXXX  XXXXXXX      |",
            "|XX                        |",
            "+--------------------------+",
        };
        
        public static void Run()
        {
            Console.Title = "A* Pathfinding";
            findPathAStar();
        }

        private static void findPathAStar()
        {
            Location current = null;
            var map = MAP_LEVEL_1;
            var start = new Location {X = 1, Y = 2};
            var target = new Location {X = 2, Y = 5};
            var openList = new List<Location>();
            var closedList = new List<Location>();
            var g = 0;

            PrintMap(map);
            
            // start by adding the original position to the open list
            openList.Add(start);

            var lowest = 0;
            while (openList.Count > 0)
            {
                // algorithm's logic goes here
                // get the square with the lowest F score
                lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                Console.SetCursorPosition(current.X, current.Y);
                Console.Write(PLAYER_CHAR);
                Console.SetCursorPosition(current.X, current.Y);
                System.Threading.Thread.Sleep(DELAY_TIME_MS);

                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                {
                    break;
                }

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, map);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                                                       && l.Y == adjacentSquare.Y) != null)
                        continue;
                    
                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }
            
            // assume path was found; let's show it
            while (current != null)
            {
                Console.SetCursorPosition(current.X, current.Y);
                Console.Write(FOUND_PATH_CHAR);
                Console.SetCursorPosition(current.X, current.Y);
                current = current.Parent;
                System.Threading.Thread.Sleep(DELAY_TIME_MS_TRACE_PATH);
            }
        }

        private static void PrintMap(string[] map)
        {
            Array.ForEach(map, Console.WriteLine);
        }
        
        /// <summary>
        /// Compute the distance to the destination
        /// </summary>
        /// <param name="x">Current X</param>
        /// <param name="y">Current Y</param>
        /// <param name="targetX">Target X</param>
        /// <param name="targetY">Target Y</param>
        /// <returns>Destination distance</returns>
        private static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        private static List<Location> GetWalkableAdjacentSquares(int x, int y, string[] map)
        {
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };

            return proposedLocations.Where(l => map[l.Y][l.X] == SPACE_CHAR || map[l.Y][l.X] == TARGET_CHAR).ToList();
        }
    }
}