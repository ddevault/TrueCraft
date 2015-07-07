using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.Physics
{
    public interface IPhysicsEntity
    {
        Vector3 Position { get; set; }
        Vector3 Velocity { get; set; }
        /// <summary>
        /// Acceleration due to gravity in meters per second squared.
        /// </summary>
        float AccelerationDueToGravity { get; }
        /// <summary>
        /// Velocity *= (1 - Drag) each second
        /// </summary>
        float Drag { get; }
        /// <summary>
        /// Terminal velocity in meters per second.
        /// </summary>
        float TerminalVelocity { get; }

        bool BeginUpdate();
        void EndUpdate(Vector3 newPosition);
    }
}
