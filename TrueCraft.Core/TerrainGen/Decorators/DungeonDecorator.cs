using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Decorations;

namespace TrueCraft.Core.TerrainGen.Decorators
{
    public class DungeonDecorator : IChunkDecorator
    {
        private int BaseLevel;
        public DungeonDecorator(int GroundLevel)
        {
            this.BaseLevel = GroundLevel;
        }

        public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
        {
            for (int Attemps = 0; Attemps < 8; Attemps++)
            {
                Perlin Noise = new Perlin();
                Noise.Seed = world.Seed - (chunk.Coordinates.X + chunk.Coordinates.Z);
                ClampNoise OffsetNoise = new ClampNoise(Noise);
                OffsetNoise.MaxValue = 3;
                var X = 0;
                var Z = 0;
                double Offset = 0;
                Offset += OffsetNoise.Value2D(X, Z);
                int FinalX = (int)Math.Floor(X + Offset);
                int FinalZ = (int)Math.Floor(Z + Offset);
                var Y = (int)(10 + Offset);

                var BlockX = MathHelper.ChunkToBlockX(FinalX, chunk.Coordinates.X);
                var BlockZ = MathHelper.ChunkToBlockZ(FinalZ, chunk.Coordinates.Z);
                var SpawnValue = OffsetNoise.Value2D(BlockX, BlockZ);
                if (SpawnValue > 1.95 && SpawnValue < 2.09)
                {
                    var Generated = new Dungeon().GenerateAt(world, chunk, new Coordinates3D(BlockX, Y, BlockZ));
                    if (Generated)
                        break;
                }
            }
        }
    }
}