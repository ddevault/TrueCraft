using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;
using TrueCraft.Core.World;
using MonoGame.Utilities;
using TrueCraft.Client.Linux.Events;

namespace TrueCraft.Client.Linux.Handlers
{
    internal static class ChunkHandler
    {
        public static void HandleChunkPreamble(IPacket _packet, MultiplayerClient client)
        {
            var packet = (ChunkPreamblePacket)_packet;
            var coords = new Coordinates2D(packet.X, packet.Z);
            client.World.SetChunk(coords, new Chunk(coords));
        }

        public static void HandleChunkData(IPacket _packet, MultiplayerClient client)
        {
            var packet = (ChunkDataPacket)_packet;
            // TODO: Handle chunk data packets that only include a partial chunk
            // This only works with TrueCraft servers unless we fix that
            var data = ZlibStream.UncompressBuffer(packet.CompressedData);
            var chunk = client.World.FindChunk(new Coordinates2D(packet.X, packet.Z));

            Array.Copy(data, 0, chunk.Blocks, 0, chunk.Blocks.Length);
            Array.Copy(data, chunk.Blocks.Length, chunk.Metadata.Data, 0, chunk.Metadata.Data.Length);
            // TODO: Light

            client.OnChunkLoaded(new ChunkEventArgs(new ReadOnlyChunk(chunk)));
        }
    }
}