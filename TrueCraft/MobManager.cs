using System;
using TrueCraft.API.World;
using TrueCraft.API;
using System.Collections.Generic;
using TrueCraft.Core.Entities;
using TrueCraft.Core;
using TrueCraft.API.AI;

namespace TrueCraft
{
    public class MobManager
    {
        public EntityManager EntityManager { get; set; }

        private Dictionary<Dimension, List<ISpawnRule>> SpawnRules { get; set; }

        public MobManager(EntityManager manager)
        {
            EntityManager = manager;
            SpawnRules = new Dictionary<Dimension, List<ISpawnRule>>();
        }

        public void AddRules(Dimension dimension, ISpawnRule rules)
        {
            if (!SpawnRules.ContainsKey(dimension))
                SpawnRules[dimension] = new List<ISpawnRule>();
            SpawnRules[dimension].Add(rules);
        }

        public void SpawnInitialMobs(IChunk chunk, Dimension dimension)
        {
            if (!SpawnRules.ContainsKey(dimension))
                return;
            var rules = SpawnRules[dimension];
            foreach (var rule in rules)
            {
                if (MathHelper.Random.Next(rule.ChunkSpawnChance) == 0)
                    rule.GenerateMobs(chunk, EntityManager);
            }
        }

        /// <summary>
        /// Call at dusk and it'll spawn baddies.
        /// </summary>
        public void DayCycleSpawn(IChunk chunk, Dimension dimension)
        {
            // TODO
        }
    }
}