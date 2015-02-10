using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Networking;
using System.Collections.Generic;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WaterBlock : BlockProvider
    {
        private const byte MaxFlow = 7;

        public static readonly byte BlockID = 0x08;

        public override byte ID { get { return 0x08; } }
        
        public override double BlastResistance { get { return 500; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 3; } }
        
        public override string DisplayName { get { return "Water"; } }

        public override BoundingBox? BoundingBox
        {
            get
            {
                return null;
            }
        }

        private bool PlaceWater(IMultiplayerServer server, Coordinates3D coords, IWorld world, byte meta = 0)
        {
            var old = world.GetBlockID(coords);
            if (old == WaterBlock.BlockID || old == StationaryWaterBlock.BlockID)
                return false;
            world.SetBlockID(coords, BlockID);
            world.SetMetadata(coords, meta);
            server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(0.25), (s) =>
                AutomataUpdate(s, world, coords));
            return true;
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
                server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(0.25), (_server) =>
                    DoAutomata(_server, world, coords));
            }
        }

        internal bool CanFlow(IWorld world, Coordinates3D coords)
        {
            var down = world.BlockRepository.GetBlockProvider(world.GetBlockID(coords + Coordinates3D.Down));
            if (!down.Opaque)
                return true;
            const int maxDistance = 5;
            var extraLocations = new List<Coordinates3D>();
            var nearest = new Coordinates3D(maxDistance + 1, -1, maxDistance + 1);
            for (int x = -maxDistance; x < maxDistance; x++)
            {
                for (int z = -maxDistance; z < maxDistance; z++)
                {
                    if (Math.Abs(z) + Math.Abs(x) > maxDistance)
                        continue;
                    var check = new Coordinates3D(x, -1, z);
                    var c = world.GetBlockID(check + coords);
                    if (c == 0 || c == WaterBlock.BlockID || c == StationaryWaterBlock.BlockID)
                    {
                        if (!LineOfSight(world, check + coords, coords))
                            continue;
                        if (coords.DistanceTo(check + coords) == coords.DistanceTo(nearest + coords))
                            extraLocations.Add(check);
                        if (coords.DistanceTo(check + coords) < coords.DistanceTo(nearest + coords))
                        {
                            extraLocations.Clear();
                            nearest = check;
                        }
                    }
                }
            }
            if (nearest == new Coordinates3D(maxDistance + 1, -1, maxDistance + 1))
            {
                extraLocations.Add(new Coordinates3D(-maxDistance - 1, -1, maxDistance + 1));
                extraLocations.Add(new Coordinates3D(maxDistance + 1, -1, -maxDistance - 1));
                extraLocations.Add(new Coordinates3D(-maxDistance - 1, -1, -maxDistance - 1));
            }
            extraLocations.Add(nearest);
            bool spread = false;
            for (int i = 0; i < extraLocations.Count; i++)
            {
                var location = extraLocations[i];
                location.Clamp(1);
                var xPotential = world.GetBlockID(new Coordinates3D(location.X, 0, 0) + coords);
                if (xPotential == 0)
                {
                    var old = world.GetBlockID(coords);
                    return old != WaterBlock.BlockID && old != StationaryWaterBlock.BlockID;
                }

                var zPotential = world.GetBlockID(new Coordinates3D(0, 0, location.Z) + coords);
                if (zPotential == 0)
                {
                    var old = world.GetBlockID(coords);
                    return old != WaterBlock.BlockID && old != StationaryWaterBlock.BlockID;
                }
            }
            return spread;
        }

        public bool DoAutomata(IMultiplayerServer server, IWorld world, Coordinates3D coords)
        {
            var meta = world.GetMetadata(coords);        
            Coordinates3D[] neighbors =
                {
                    Coordinates3D.Left,
                    Coordinates3D.Right,
                    Coordinates3D.Forwards,
                    Coordinates3D.Backwards
                };
            var up = world.GetBlockID(coords + Coordinates3D.Up);
            var down = world.BlockRepository.GetBlockProvider(world.GetBlockID(coords + Coordinates3D.Down));

            if (!down.Opaque)
            {
                PlaceWater(server, coords + Coordinates3D.Down, world, 1);
                if (meta != 0)
                    return true;
            }

            // Check inward flow
            if (up == WaterBlock.BlockID || up == StationaryWaterBlock.BlockID)
                meta = 1;
            else
            {
                if (meta != 0)
                {
                    byte minMeta = 15;
                    int sources = 0;
                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        var nId = world.GetBlockID(coords + neighbors[i]);
                        if (nId == WaterBlock.BlockID || nId == StationaryWaterBlock.BlockID)
                        {
                            var _meta = world.GetMetadata(coords + neighbors[i]);
                            if (_meta < minMeta)
                                minMeta = _meta;
                            if (_meta == 0)
                                sources++;
                        }
                    }
                    if (sources >= 2)
                    {
                        world.SetMetadata(coords, 0);
                        return true;
                    }
                    if (minMeta > 0)
                    {
                        meta = (byte)(minMeta + 1);
                        if (meta >= MaxFlow + 1)
                        {
                            world.SetBlockID(coords, 0);
                            return true;
                        }
                    }
                }
            }
            world.SetMetadata(coords, meta);

            // Check outward flow
            if (meta < MaxFlow)
            {
                const int maxDistance = 5;
                var extraLocations = new List<Coordinates3D>();
                var nearest = new Coordinates3D(maxDistance + 1, -1, maxDistance + 1);
                for (int x = -maxDistance; x < maxDistance; x++)
                {
                    for (int z = -maxDistance; z < maxDistance; z++)
                    {
                        if (Math.Abs(z) + Math.Abs(x) > maxDistance)
                            continue;
                        var check = new Coordinates3D(x, -1, z);
                        var c = world.GetBlockID(check + coords);
                        if (c == 0 || c == WaterBlock.BlockID || c == StationaryWaterBlock.BlockID)
                        {
                            if (!LineOfSight(world, check + coords, coords))
                                continue;
                            if (coords.DistanceTo(check + coords) == coords.DistanceTo(nearest + coords))
                                extraLocations.Add(check);
                            if (coords.DistanceTo(check + coords) < coords.DistanceTo(nearest + coords))
                            {
                                extraLocations.Clear();
                                nearest = check;
                            }
                        }
                    }
                }
                if (nearest == new Coordinates3D(maxDistance + 1, -1, maxDistance + 1))
                {
                    extraLocations.Add(new Coordinates3D(-maxDistance - 1, -1, maxDistance + 1));
                    extraLocations.Add(new Coordinates3D(maxDistance + 1, -1, -maxDistance - 1));
                    extraLocations.Add(new Coordinates3D(-maxDistance - 1, -1, -maxDistance - 1));
                }
                extraLocations.Add(nearest);
                bool spread = false;
                for (int i = 0; i < extraLocations.Count; i++)
                {
                    var location = extraLocations[i];
                    location.Clamp(1);
                    var xPotential = world.GetBlockID(new Coordinates3D(location.X, 0, 0) + coords);
                    if (xPotential == 0)
                    {
                        if (PlaceWater(server, new Coordinates3D(location.X, 0, 0) + coords, world, (byte)(meta + 1)))
                            spread = true;
                    }

                    var zPotential = world.GetBlockID(new Coordinates3D(0, 0, location.Z) + coords);
                    if (zPotential == 0)
                    {
                        if (PlaceWater(server, new Coordinates3D(0, 0, location.Z) + coords, world, (byte)(meta + 1)))
                            spread = true;
                    }
                }
                if (!spread)
                {
                    world.SetBlockID(coords, StationaryWaterBlock.BlockID);
                    return false;
                }
            }
            return true;
        }

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

        public void ScheduleNextEvent(Coordinates3D coords, IWorld world, IMultiplayerServer server)
        {
            server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(0.25), (_server) =>
                AutomataUpdate(_server, world, coords));
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            ScheduleNextEvent(descriptor.Coordinates, world, user.Server);
        }
    }

    public class StationaryWaterBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x09;

        public override byte ID { get { return 0x09; } }

        public override string DisplayName { get { return "Water (stationary)"; } }

        public override double BlastResistance { get { return 500; } }

        public override double Hardness { get { return 100; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 3; } }

        public override BoundingBox? BoundingBox
        {
            get
            {
                return null;
            }
        }

        public override void BlockUpdate(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world)
        {
            var provider = server.BlockRepository.GetBlockProvider(WaterBlock.BlockID) as WaterBlock;
            if (provider.CanFlow(world, descriptor.Coordinates))
            {
                world.SetBlockID(descriptor.Coordinates, provider.ID);
                provider.ScheduleNextEvent(descriptor.Coordinates, world, server);
            }
        }
    }
}