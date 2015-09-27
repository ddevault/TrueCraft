using System;
using TrueCraft.API;
using TrueCraft.Core.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client
{
    /// <summary>
    /// Efficient ray caster that can cast a ray into a voxel map
    /// and return the voxel that it intersects with.
    /// </summary>
    public static class VoxelCast
    {
        // Thanks to http://gamedev.stackexchange.com/questions/47362/cast-ray-to-select-block-in-voxel-game

        public static Tuple<Coordinates3D, BlockFace> Cast(ReadOnlyWorld world,
            Ray ray, IBlockRepository repository, int posmax, int negmax)
        {
            // TODO: There are more efficient ways of doing this, fwiw

            double min = negmax * 2;
            var pick = -Coordinates3D.One;
            for (int x = -posmax; x <= posmax; x++)
            {
                for (int y = -negmax; y <= posmax; y++)
                {
                    for (int z = -posmax; z <= posmax; z++)
                    {
                        var coords = (Coordinates3D)(new Vector3(x, y, z) + ray.Position).Round();
                        if (!world.IsValidPosition(coords))
                            continue;
                        var id = world.GetBlockID(coords);
                        if (id != 0)
                        {
                            var provider = repository.GetBlockProvider(id);
                            var box = provider.BoundingBox;
                            if (box != null)
                            {
                                var distance = ray.Intersects(box.Value.OffsetBy(coords));
                                if (distance != null && distance.Value < min)
                                {
                                    min = distance.Value;
                                    pick = coords;
                                }
                            }
                        }
                    }
                }
            }
            if (pick == -Coordinates3D.One)
                return null;
            return new Tuple<Coordinates3D, BlockFace>(pick, BlockFace.PositiveY);
        }
    }
}
