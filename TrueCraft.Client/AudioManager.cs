using System;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.Core;
using Microsoft.Xna.Framework.Content;
using NVorbis;

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
                new[]
                {
                    "footstep.cloth",
                    "default_sand_footstep.1.ogg", "default_sand_footstep.2.ogg" // TODO: Cloth sound effects
                },
                new[]
                {
                    "footstep.grass",
                    "default_grass_footstep.1.ogg", "default_grass_footstep.2.ogg", "default_grass_footstep.3.ogg"
                },
                new[]
                {
                    "footstep.gravel",
                    "default_gravel_footstep.1.ogg", "default_gravel_footstep.2.ogg", "default_gravel_footstep.3.ogg",
                    "default_gravel_footstep.4.ogg"
                },
                new[]
                {
                    "footstep.sand",
                    "default_sand_footstep.1.ogg", "default_sand_footstep.2.ogg"
                },
                new[]
                {
                    "footstep.snow",
                    "default_snow_footstep.1.ogg", "default_snow_footstep.2.ogg", "default_snow_footstep.3.ogg"
                },
                new[]
                {
                    "footstep.stone",
                    "default_hard_footstep.1.ogg", "default_hard_footstep.2.ogg", "default_hard_footstep.3.ogg"
                },
                new[]
                {
                    "footstep.wood",
                    "default_wood_footstep.1.ogg", "default_wood_footstep.2.ogg"
                },
                new[]
                {
                    "footstep.glass",
                    "default_glass_footstep.ogg"
                },
                new[]
                {
                    "hurt",
                    "default_hurt.wav"
                }
            };
            foreach (var pack in packs)
            {
                var name = pack[0];
                LoadAudioPack(name, pack.Skip(1).ToArray());
            }
        }

        private SoundEffect LoadOgg(Stream stream)
        {
            using (var reader = new VorbisReader(stream, false))
            {
                float[] _buffer = new float[reader.TotalSamples];
                byte[] buffer = new byte[reader.TotalSamples * 2];
                reader.ReadSamples(_buffer, 0, _buffer.Length);
                for (int i = 0; i < _buffer.Length; i++)
                {
                    short val = (short)Math.Max(Math.Min(short.MaxValue * _buffer[i], short.MaxValue), short.MinValue);
                    var decoded = BitConverter.GetBytes(val);
                    buffer[i * 2] = decoded[0];
                    buffer[i * 2 + 1] = decoded[1];
                }
                return new SoundEffect(buffer, reader.SampleRate, reader.Channels == 1 ? AudioChannels.Mono : AudioChannels.Stereo);
            }
        }

        public void LoadAudioPack(string pack, string[] filenames)
        {
            var effects = new SoundEffect[filenames.Length];
            for (int i = 0; i < filenames.Length; i++)
            {
                using (var f = File.OpenRead(Path.Combine("Content", "Audio", filenames[i])))
                {
                    if (filenames[i].EndsWith(".wav"))
                        effects[i] = SoundEffect.FromStream(f);
                    else if (filenames[i].EndsWith(".ogg"))
                        effects[i] = LoadOgg(f);
                }
            }
            AudioPacks[pack] = effects;
        }

        public void PlayPack(string pack, float volume = 1.0f)
        {
            var i = MathHelper.Random.Next(0, AudioPacks[pack].Length);
            AudioPacks[pack][i].Play(volume * EffectVolume, 1.0f, 0.0f);
        }
    }
}
