using System;
using TrueCraft.API.Entities;
using TrueCraft.API;

namespace TrueCraft.Entities
{
    public class PlayerEntity : IEntity
    {
        public PlayerEntity()
        {
        }

        public int EntityID { get; set; }
        public Vector3 Position { get; set; }
    }
}