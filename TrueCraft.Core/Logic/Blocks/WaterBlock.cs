using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Networking;
using System.Collections.Generic;
using System.Linq;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WaterBlock : BlockProvider
    {
        // Fluids in Minecraft propegate according to a set of rules as cellular automata.
        // Source blocks start at zero and each block progressively further from the source
        // is one greater than the largest value nearby. When they reach 7, the water stops
        // propgetating.

        private const double SecondsBetweenUpdates = 0.25;

        private const byte MaximumFluidDepletion = 7;

        private static readonly Coordinates3D[] Neighbors =
            {
                Coordinates3D.Left,
                Coordinates3D.Right,
                Coordinates3D.Forwards,
                Coordinates3D.Backwards
            };

        /// <summary>
        /// Represents a block that the currently updating water block is able to flow outwards into.
        /// </summary>
        protected struct LiquidFlow
        {
            public LiquidFlow(Coordinates3D targetBlock, byte level)
            {
                TargetBlock = targetBlock;
                Level = level;
            }

            /// <summary>
            /// The block to be filled with water.
            /// </summary>
            public Coordinates3D TargetBlock;
            /// <summary>
            /// The water level to fill the target block with.
            /// </summary>
            public byte Level;
        }

        public static readonly byte BlockID = 0x08;

        public override byte ID { get { return 0x08; } }
        
        public override double BlastResistance { get { return 500; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 3; } }
        
        public override string DisplayName { get { return "Water"; } }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            return new ItemStack[0];
        }

        public override BoundingBox? BoundingBox
        {
            get
            {
                return null;
            }
        }

        public void ScheduleNextEvent(Coordinates3D coords, IWorld world, IMultiplayerServer server)
        {
            server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(SecondsBetweenUpdates), (_server) =>
                AutomataUpdate(_server, world, coords));
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            ScheduleNextEvent(descriptor.Coordinates, world, user.Server);
        }

        private void AutomataUpdate(IMultiplayerServer server, IWorld world, Coordinates3D coords)
        {
            if (world.GetBlockID(coords) != BlockID)
                return;
            server.BlockUpdatesEnabled = false;
            var again = DoAutomata(server, world, coords);
            server.BlockUpdatesEnabled = true;
            if (again)
            {
                server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(SecondsBetweenUpdates), (_server) =>
                    DoAutomata(_server, world, coords));
            }
        }

        /// <summary>
        /// Returns true if the given candidate coordinate has a line-of-sight to the given target coordinate.
        /// </summary>
        private bool LineOfSight(IWorld world, Coordinates3D candidate, Coordinates3D target)
        {
            candidate += Coordinates3D.Up;
            var direction = target - candidate;
            direction.Clamp(1);
            do
            {
                int z = candidate.Z;
                do
                {
                    var p = world.BlockRepository.GetBlockProvider(world.GetBlockID(candidate));
                    if (p.Opaque)
                        return false;
                    candidate.Z += direction.Z;
                } while (target.Z != candidate.Z);
                candidate.Z = z;
                candidate.X += direction.X;
            } while (target.X != candidate.X);
            return true;
        }

        /// <summary>
        /// Examines neighboring blocks and determines the new water level that this block should adopt.
        /// </summary>
        protected byte DetermineInwardFlow(IWorld world, Coordinates3D coords)
        {
            var currentLevel = world.GetMetadata(coords);
            var up = world.GetBlockID(coords + Coordinates3D.Up);
            if (up == WaterBlock.BlockID || up == StationaryWaterBlock.BlockID) // Check for water above us
                return currentLevel;
            else
            {
                if (currentLevel != 0)
                {
                    byte highestNeighboringFluid = 15;
                    int neighboringSourceBlocks = 0;
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        var nId = world.GetBlockID(coords + Neighbors[i]);
                        if (nId == WaterBlock.BlockID || nId == StationaryWaterBlock.BlockID)
                        {
                            var neighborLevel = world.GetMetadata(coords + Neighbors[i]);
                            if (neighborLevel < highestNeighboringFluid)
                                highestNeighboringFluid = neighborLevel;
                            if (neighborLevel == 0)
                                neighboringSourceBlocks++;
                        }
                    }
                    if (neighboringSourceBlocks >= 2)
                        currentLevel = 0;
                    if (highestNeighboringFluid > 0)
                        currentLevel = (byte)(highestNeighboringFluid + 1);
                }
            }
            return currentLevel;
        }

        /// <summary>
        /// Produces a list of outward flow targets that this block may flow towards.
        /// </summary>
        protected LiquidFlow[] DetermineOutwardFlow(IWorld world, Coordinates3D coords)
        {
            // The maximum distance we will search for lower ground to flow towards
            const int DropCheckDistance = 5;

            var outwardFlow = new List<LiquidFlow>(5);

            var currentLevel = world.GetMetadata(coords);
            var blockBelow = world.BlockRepository.GetBlockProvider(world.GetBlockID(coords + Coordinates3D.Down));
            if (!blockBelow.Opaque)
            {
                outwardFlow.Add(new LiquidFlow(coords + Coordinates3D.Down, 1));
                if (currentLevel != 0)
                    return outwardFlow.ToArray();
            }

            if (currentLevel < MaximumFluidDepletion)
            {
                // This code is responsible for seeking out candidates for flowing towards.
                // Water in Minecraft will flow in the direction of the nearest drop-off where
                // there is at least one block removed on the Y axis.
                // It will flow towards several equally strong candidates at once.

                var candidateFlowPoints = new List<Coordinates3D>(4);
                var furthestPossibleCandidate = new Coordinates3D(x: DropCheckDistance + 1, z: DropCheckDistance + 1) + Coordinates3D.Down;

                var nearestCandidate = furthestPossibleCandidate;
                for (int x = -DropCheckDistance; x < DropCheckDistance; x++)
                {
                    for (int z = -DropCheckDistance; z < DropCheckDistance; z++)
                    {
                        if (Math.Abs(z) + Math.Abs(x) > DropCheckDistance)
                            continue;
                        var check = new Coordinates3D(x: x, z: z) + Coordinates3D.Down;
                        var c = world.BlockRepository.GetBlockProvider(world.GetBlockID(check + coords));
                        if (!c.Opaque)
                        {
                            if (!LineOfSight(world, check + coords, coords))
                                continue;
                            if (coords.DistanceTo(check + coords) == coords.DistanceTo(nearestCandidate + coords))
                                candidateFlowPoints.Add(check);
                            if (coords.DistanceTo(check + coords) < coords.DistanceTo(nearestCandidate + coords))
                            {
                                candidateFlowPoints.Clear();
                                nearestCandidate = check;
                            }
                        }
                    }
                }
                if (nearestCandidate == furthestPossibleCandidate)
                {
                    candidateFlowPoints.Add(new Coordinates3D(x: -DropCheckDistance - 1, z: DropCheckDistance + 1) + Coordinates3D.Down);
                    candidateFlowPoints.Add(new Coordinates3D(x: DropCheckDistance + 1, z: -DropCheckDistance - 1) + Coordinates3D.Down);
                    candidateFlowPoints.Add(new Coordinates3D(x: -DropCheckDistance - 1, z: -DropCheckDistance - 1) + Coordinates3D.Down);
                }
                candidateFlowPoints.Add(nearestCandidate);

                // For each candidate, determine if we are actually capable of flowing towards it.
                // We are able to flow through blocks with a hardness of zero, but no others. We are
                // not able to flow through established water blocks.
                for (int i = 0; i < candidateFlowPoints.Count; i++)
                {
                    var location = candidateFlowPoints[i];
                    location.Clamp(1);

                    var xCoordinateCheck = new Coordinates3D(x: location.X) + coords;
                    var zCoordinateCheck = new Coordinates3D(z: location.Z) + coords;

                    var xID = world.BlockRepository.GetBlockProvider(world.GetBlockID(xCoordinateCheck));
                    var zID = world.BlockRepository.GetBlockProvider(world.GetBlockID(zCoordinateCheck));

                    if (xID.Hardness == 0 && xID.ID != WaterBlock.BlockID && xID.ID != StationaryWaterBlock.BlockID)
                    {
                        if (outwardFlow.All(f => f.TargetBlock != xCoordinateCheck))
                            outwardFlow.Add(new LiquidFlow(xCoordinateCheck, (byte)(currentLevel + 1)));
                    }

                    if (zID.Hardness == 0 && zID.ID != WaterBlock.BlockID && zID.ID != StationaryWaterBlock.BlockID)
                    {
                        if (outwardFlow.All(f => f.TargetBlock != zCoordinateCheck))
                            outwardFlow.Add(new LiquidFlow(zCoordinateCheck, (byte)(currentLevel + 1)));
                    }
                }
            }
            return outwardFlow.ToArray();
        }

        public bool DoAutomata(IMultiplayerServer server, IWorld world, Coordinates3D coords)
        {
            var previousLevel = world.GetMetadata(coords);

            var inward = DetermineInwardFlow(world, coords);
            var outward = DetermineOutwardFlow(world, coords);

            if (outward.Length == 1 && outward[0].TargetBlock == coords + Coordinates3D.Down)
            {
                // Exit early if we have placed a water block beneath us (and we aren't a source block)
                if (previousLevel != 0)
                    return true;
            }

            // Process inward flow
            if (inward > MaximumFluidDepletion)
            {
                world.SetBlockID(coords, 0);
                return true;
            }
            world.SetMetadata(coords, inward);
            if (inward == 0 && previousLevel != 0)
            {
                // Exit early if we have become a source block
                return true;
            }

            // Process outward flow
            for (int i = 0; i < outward.Length; i++)
            {
                var target = outward[i].TargetBlock;
                // For each block we can flow into, generate an item entity if appropriate
                var provider = world.BlockRepository.GetBlockProvider(world.GetBlockID(target));
                provider.GenerateDropEntity(new BlockDescriptor { Coordinates = target, ID = provider.ID }, world, server);
                // And overwrite the block with a new water block.
                world.SetBlockID(target, WaterBlock.BlockID);
                world.SetMetadata(target, outward[i].Level);
                server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(SecondsBetweenUpdates), s => AutomataUpdate(s, world, target));
            }
            // Set our block to still water if we are done spreading.
            if (outward.Length == 0)
            {
                world.SetBlockID(coords, StationaryWaterBlock.BlockID);
                return false;
            }
            return true;
        }
    }

    public class StationaryWaterBlock : WaterBlock
    {
        public static readonly new byte BlockID = 0x09;

        public override byte ID { get { return 0x09; } }

        public override string DisplayName { get { return "Water (stationary)"; } }

        public override double BlastResistance { get { return 500; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 3; } }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            return new ItemStack[0];
        }

        public override BoundingBox? BoundingBox
        {
            get
            {
                return null;
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            // This space intentionally left blank
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (source.ID == StationaryWaterBlock.BlockID || source.ID == WaterBlock.BlockID)
                return;
            var outward = DetermineOutwardFlow(world, descriptor.Coordinates);
            if (outward.Length != 0)
            {
                world.SetBlockID(descriptor.Coordinates, WaterBlock.BlockID);
                ScheduleNextEvent(descriptor.Coordinates, world, server);
            }
        }
    }
}