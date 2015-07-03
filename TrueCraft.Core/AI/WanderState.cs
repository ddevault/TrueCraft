using System;
using TrueCraft.API.AI;
using TrueCraft.API.Entities;
using TrueCraft.API.Server;
using TrueCraft.API;
using TrueCraft.API.World;
using System.Threading.Tasks;

namespace TrueCraft.Core.AI
{
    public class WanderState : IMobState
    {
        /// <summary>
        /// Chance that mob will decide to move during an update when idle.
        /// Chance is equal to 1 / IdleChance.
        /// </summary>
        public int IdleChance { get; set; }
        /// <summary>
        /// The maximum distance the mob will move in an iteration.
        /// </summary>
        /// <value>The distance.</value>
        public int Distance { get; set; }
        public AStarPathFinder PathFinder { get; set; }

        public WanderState()
        {
            IdleChance = 20;
            Distance = 25;
            PathFinder = new AStarPathFinder();
        }

        public void Update(IMobEntity entity, IEntityManager manager)
        {
            var cast = entity as IEntity;
            if (entity.CurrentPath != null)
                entity.AdvancePath(manager.TimeSinceLastUpdate);
            else
            {
                if (MathHelper.Random.Next(IdleChance) == 0)
                {
                    var target = new Coordinates3D(
                        (int)(cast.Position.X + (MathHelper.Random.Next(Distance) - Distance / 2)),
                        0,
                        (int)(cast.Position.Z + (MathHelper.Random.Next(Distance) - Distance / 2))
                    );
                    IChunk chunk;
                    var adjusted = entity.World.FindBlockPosition(target, out chunk, generate: false);
                    target.Y = chunk.GetHeight((byte)adjusted.X, (byte)adjusted.Z) + 1;
                    Task.Factory.StartNew(() =>
                    {
                            entity.CurrentPath = PathFinder.FindPath(entity.World, entity.BoundingBox,
                                (Coordinates3D)cast.Position, target);
                    });
                }
            }
        }
    }
}