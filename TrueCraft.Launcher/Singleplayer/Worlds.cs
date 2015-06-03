using System;
using System.IO;
using TrueCraft.Core.World;
using System.Collections.Generic;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core;
using TrueCraft.Core.Logic;
using System.Linq;

namespace TrueCraft.Launcher.Singleplayer
{
    public class Worlds
    {
        public static string WorldsPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".truecraft", "worlds");
            }
        }

        public static Worlds Local { get; set; }

        public BlockRepository BlockRepository { get; set; }
        public World[] Saves { get; set; }

        public void Load()
        {
            if (!Directory.Exists(WorldsPath))
                Directory.CreateDirectory(WorldsPath);
            BlockRepository = new BlockRepository();
            BlockRepository.DiscoverBlockProviders();
            var directories = Directory.GetDirectories(WorldsPath);
            var saves = new List<World>();
            foreach (var d in directories)
            {
                try
                {
                    var w = World.LoadWorld(d);
                    saves.Add(w);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    /* Who cares */
                }
            }
            Saves = saves.ToArray();
        }

        public World CreateNewWorld(string name, string seed)
        {
            int s;
            if (!int.TryParse(seed, out s))
            {
                // TODO: Hash seed string
                s = MathHelper.Random.Next();
            }
            var world = new World(name, s, new StandardGenerator());
            world.BlockRepository = BlockRepository;
            var safeName = name;
            foreach (var c in Path.GetInvalidFileNameChars())
                safeName = safeName.Replace(c.ToString(), "");
            world.Name = name;
            world.Save(Path.Combine(WorldsPath, safeName));
            Saves = Saves.Concat(new[] { world }).ToArray();
            return world;
        }
    }
}