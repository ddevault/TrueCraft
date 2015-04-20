using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class FallingGravelEntity : FallingSandEntity
    {
        public FallingGravelEntity(Vector3 position) : base(position)
        {
        }

        public override byte EntityType { get { return 71; } }
    }
}