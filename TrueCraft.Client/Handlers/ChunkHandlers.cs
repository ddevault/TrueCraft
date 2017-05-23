using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;
using TrueCraft.Core.World;
using MonoGame.Utilities;
using TrueCraft.Client.Events;
using TrueCraft.API.World;

namespace TrueCraft.Client.Handlers
{
    internal static class ChunkHandlers
    {
        public static void HandleBlockChange(IPacket _packet, MultiplayerClient client)
        {
            var packet = (BlockChangePacket)_packet;
            var coordinates = new Coordinates3D(packet.X, packet.Y, packet.Z);
            Coordinates3D adjusted;
            IChunk chunk;
            try
            {
                adjusted = client.World.World.FindBlockPosition(coordinates, out chunk);
            }
            catch (ArgumentException)
            {
                // Relevant chunk is not loaded - ignore packet
                return;
            }
            chunk.SetBlockID(adjusted, (byte)packet.BlockID);
            chunk.SetMetadata(adjusted, (byte)packet.Metadata);
            client.OnBlockChanged(new BlockChangeEventArgs(coordinates, new TrueCraft.API.Logic.BlockDescriptor(),
                new TrueCraft.API.Logic.BlockDescriptor()));
            client.OnChunkModified(new ChunkEventArgs(new ReadOnlyChunk(chunk)));
        }

        public static void HandleChunkPreamble(IPacket _packet, MultiplayerClient client)
        {
            var packet = (ChunkPreamblePacket)_packet;
            var coords = new Coordinates2D(packet.X, packet.Z);
            client.World.SetChunk(coords, new Chunk(coords));
        }

        public static void HandleChunkData(IPacket _packet, MultiplayerClient client)
        {
            var packet = (ChunkDataPacket)_packet;
            var coords = new Coordinates3D(packet.X, packet.Y, packet.Z);
            var data = ZlibStream.UncompressBuffer(packet.CompressedData);
            IChunk chunk;
            var adjustedCoords = client.World.World.FindBlockPosition(coords, out chunk);

            if (packet.Width == Chunk.Width
                && packet.Height == Chunk.Height
                && packet.Depth == Chunk.Depth) // Fast path
            {
                // Block IDs
                Buffer.BlockCopy(data, 0, chunk.Data, 0, chunk.Data.Length);
                // Block metadata
                Buffer.BlockCopy(data, chunk.Data.Length, chunk.Metadata.Data, 0, chunk.Metadata.Data.Length);
                // Block light
                Buffer.BlockCopy(data, chunk.Data.Length + chunk.Metadata.Data.Length,
                    chunk.BlockLight.Data, 0, chunk.BlockLight.Data.Length);
                // Sky light
                Buffer.BlockCopy(data, chunk.Data.Length + chunk.Metadata.Data.Length + chunk.BlockLight.Data.Length,
                    chunk.SkyLight.Data, 0, chunk.SkyLight.Data.Length);
            }
            else // Slow path
            {
                int x = adjustedCoords.X, y = adjustedCoords.Y, z = adjustedCoords.Z;
                int fullLength = packet.Width * packet.Height * packet.Depth; // Length of full sized byte section
                int nibbleLength = fullLength / 2; // Length of nibble sections
                for (int i = 0; i < fullLength; i++) // Iterate through block IDs
                {
                    chunk.SetBlockID(new Coordinates3D(x, y, z), data[i]);
                    y++;
                    if (y >= packet.Height)
                    {
                        y = 0;
                        z++;
                        if (z >= packet.Depth)
                        {
                            z = 0;
                            x++;
                            if (x >= packet.Width)
                            {
                                x = 0;
                            }
                        }
                    }
                }
                x = adjustedCoords.X; y = adjustedCoords.Y; z = adjustedCoords.Z;
                for (int i = fullLength; i < nibbleLength; i++) // Iterate through metadata
                {
                    byte m = data[i];
                    chunk.SetMetadata(new Coordinates3D(x, y, z), (byte)(m & 0xF));
                    chunk.SetMetadata(new Coordinates3D(x, y + 1, z), (byte)(m & 0xF0 << 8));
                    y += 2;
                    if (y >= packet.Height)
                    {
                        y = 0;
                        z++;
                        if (z >= packet.Depth)
                        {
                            z = 0;
                            x++;
                            if (x >= packet.Width)
                            {
                                x = 0;
                            }
                        }
                    }
                }
                // TODO: Lighting
            }
            chunk.UpdateHeightMap();
            chunk.TerrainPopulated = true;
            client.OnChunkLoaded(new ChunkEventArgs(new ReadOnlyChunk(chunk)));
        }
    }
}