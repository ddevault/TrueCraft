using System;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.Core;
using Microsoft.Xna.Framework.Content;

namespace TrueCraft.Client
{
    public class AudioManager
    {
        private Dictionary<string, SoundEffect[]> AudioPacks { get; set; }

        public float EffectVolume { get; set; }
        public float MusicVolume { get; set; }

        public AudioManager()
        {
            AudioPacks = new Dictionary<string, SoundEffect[]>();
            EffectVolume = MusicVolume = 1;
        }

        public void LoadDefaultPacks(ContentManager content)
        {
            string[][] packs = new[]
            {
                // TODO: step sound effects for cloth, sand, gravel, snow, wood
                new[]
                {
                    "grass",
                    "footstep_dirt_0.wav",
                    "footstep_dirt_1.wav",
                    "footstep_dirt_2.wav",
                },
                new[]
                {
                    "stone",
                    "footstep_stone_0.wav",
                    "footstep_stone_1.wav",
                    "footstep_stone_2.wav",
                },
            };
            foreach (var pack in packs)
            {
                var name = pack[0];
                LoadAudioPack(name, pack.Skip(1).ToArray());
            }
        }

        public void LoadAudioPack(string pack, string[] filenames)
        {
            var effects = new SoundEffect[filenames.Length];
            for (int i = 0; i < filenames.Length; i++)
            {
                using (var f = File.OpenRead(Path.Combine("Content", "Audio", filenames[i])))
                    effects[i] = SoundEffect.FromStream(f);
            }
            AudioPacks[pack] = effects;
        }

        public void PlayPack(string pack)
        {
            var i = MathHelper.Random.Next(0, AudioPacks[pack].Length);
            i = 0;
            AudioPacks[pack][i].Play();
        }
    }
}