using System;
using System.Collections.Generic;
using TrueCraft.API;
using TrueCraft.API.World;
using System.Diagnostics;

namespace TrueCraft.Core.AI
{
    public class AStarPathFinder
    {
        private readonly Coordinates3D[] Neighbors =
        {
            Coordinates3D.North,
            Coordinates3D.East,
            Coordinates3D.South,
            Coordinates3D.West
        };

        private readonly Coordinates3D[][] DiagonalNeighbors =
        {
            new[] { Coordinates3D.North, Coordinates3D.East },
            new[] { Coordinates3D.North, Coordinates3D.West },
            new[] { Coordinates3D.South, Coordinates3D.East },
            new[] { Coordinates3D.South, Coordinates3D.West },
        };

        private PathResult TracePath(Coordinates3D start, Coordinates3D goal, Dictionary<Coordinates3D, Coordinates3D> parents)
        {
            var list = new List<Coordinates3D>();
            var current = goal;
            while (current != start)
            {
                current = parents[current];
                list.Insert(0, current);
            }
            list.Add(goal);
            return new PathResult { Waypoints = list };
        }

        private bool CanOccupyVoxel(IWorld world, BoundingBox box, Coordinates3D voxel)
        {
            var id = world.GetBlockID(voxel);
            // TODO: Make this more sophisticated
            return id == 0;
        }

        private IEnumerable<Coordinates3D> GetNeighbors(IWorld world, BoundingBox subject, Coordinates3D current)
        {
            for (int i = 0; i < Neighbors.Length; i++)
            {
                var next = Neighbors[i] + current;
                if (CanOccupyVoxel(world, subject, next))
                    yield return next;
            }
            for (int i = 0; i < DiagonalNeighbors.Length; i++)
            {
                var pair = DiagonalNeighbors[i];
                var next = pair[0] + pair[1] + current;

                if (CanOccupyVoxel(world, subject, next)
                    && CanOccupyVoxel(world, subject, pair[0] + current)
                    && CanOccupyVoxel(world, subject, pair[1] + current))
                    yield return next;
            }
        }

        public PathResult FindPath(IWorld world, BoundingBox subject, Coordinates3D start, Coordinates3D goal)
        {
            // Thanks to www.redblobgames.com/pathfinding/a-star/implementation.html
            var parents = new Dictionary<Coordinates3D, Coordinates3D>();
            var costs = new Dictionary<Coordinates3D, double>();
            var openset = new PriorityQueue<Coordinates3D>();
            var closedset = new HashSet<Coordinates3D>();

            openset.Enqueue(start, 0);
            parents[start] = start;
            costs[start] = start.DistanceTo(goal);

            while (openset.Count > 0)
            {
                var current = openset.Dequeue();
                if (current == goal)
                    return TracePath(start, goal, parents);

                closedset.Add(current);

                foreach (var next in GetNeighbors(world, subject, current))
                {
                    if (closedset.Contains(next))
                        continue;
                    var cost = (int)(costs[current] + current.DistanceTo(next));
                    if (!costs.ContainsKey(next) || cost < costs[next])
                    {
                        costs[next] = cost;
                        var priority = cost + next.DistanceTo(goal);
                        openset.Enqueue(next, priority);
                        parents[next] = current;
                    }
                }
            }

            return null;
        }
    }
}