using System.Collections.Generic;
using UnityEngine;
using TurtlePath.Core;
using TurtlePath.Grid;

namespace TurtlePath.Path
{
    public static class PathValidator
    {
        /// <summary>
        /// BFS flood fill from the nest. Returns all cells connected to the nest.
        /// </summary>
        public static HashSet<Vector2Int> FloodFillFromNest(GridManager gridManager, Vector2Int nestPos)
        {
            HashSet<Vector2Int> connected = new HashSet<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            connected.Add(nestPos);
            queue.Enqueue(nestPos);

            // Exploration order: N, E, S, W
            Direction[] directions = { Direction.North, Direction.East, Direction.South, Direction.West };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                Cell currentCell = gridManager.GetCell(current.x, current.y);
                if (currentCell == null) continue;

                foreach (Direction dir in directions)
                {
                    // Current cell must have a port in this direction
                    if (!currentCell.HasPort(dir)) continue;

                    Cell neighbor = gridManager.GetNeighbor(currentCell, dir);
                    if (neighbor == null) continue;

                    // Neighbor must have a port in the opposite direction
                    if (!neighbor.HasPort(dir.Opposite())) continue;

                    Vector2Int neighborPos = neighbor.GridPosition;
                    if (connected.Contains(neighborPos)) continue;

                    connected.Add(neighborPos);
                    queue.Enqueue(neighborPos);
                }
            }

            return connected;
        }

        /// <summary>
        /// BFS shortest path from nest to sea. Returns ordered list of positions, or null if no path.
        /// </summary>
        public static List<Vector2Int> FindPath(GridManager gridManager, Vector2Int nestPos, Vector2Int seaPos)
        {
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();

            cameFrom[nestPos] = new Vector2Int(-1, -1); // sentinel
            queue.Enqueue(nestPos);

            Direction[] directions = { Direction.North, Direction.East, Direction.South, Direction.West };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == seaPos)
                {
                    // Reconstruct path
                    List<Vector2Int> path = new List<Vector2Int>();
                    Vector2Int step = seaPos;
                    Vector2Int sentinel = new Vector2Int(-1, -1);

                    while (step != sentinel)
                    {
                        path.Add(step);
                        step = cameFrom[step];
                    }

                    path.Reverse();
                    return path;
                }

                Cell currentCell = gridManager.GetCell(current.x, current.y);
                if (currentCell == null) continue;

                foreach (Direction dir in directions)
                {
                    if (!currentCell.HasPort(dir)) continue;

                    Cell neighbor = gridManager.GetNeighbor(currentCell, dir);
                    if (neighbor == null) continue;

                    if (!neighbor.HasPort(dir.Opposite())) continue;

                    Vector2Int neighborPos = neighbor.GridPosition;
                    if (cameFrom.ContainsKey(neighborPos)) continue;

                    cameFrom[neighborPos] = current;
                    queue.Enqueue(neighborPos);
                }
            }

            return null; // No path found
        }
    }
}
