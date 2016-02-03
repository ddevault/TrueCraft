using System;
using System.IO;

namespace TrueCraft.Core
{
    public static class Paths
    {
        public static string Base
        {
            get
            {
                string os;
                // FIXME: SDL_GetPlatform() is nicer! -flibit
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    os = "Windows";
                }
                else
                {
                    if (Environment.CurrentDirectory.StartsWith("/Users/"))
                    {
                        os = "Mac OS X";
                    }
                    else // FIXME: Assumption! -flibit
                    {
                        os = "Linux";
                    }
                }
                string result;
                if (os.Equals("Windows"))
                {
                    result = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "truecraft"
                    );
                }
                else if (os.Equals("Mac OS X"))
                {
                    result = Environment.GetEnvironmentVariable("HOME");
                    if (String.IsNullOrEmpty(result))
                    {
                        result = "./"; // Oh well.
                    }
                    else
                    {
                        result += "/Library/Application Support/truecraft";
                    }
                    result = Path.Combine(result, "truecraft");
                }
                else if (os.Equals("Linux"))
                {
                    // Assuming a non-OSX Unix platform will follow the XDG. Which it should.
                    result = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
                    if (String.IsNullOrEmpty(result))
                    {
                        result = Environment.GetEnvironmentVariable("HOME");
                        if (String.IsNullOrEmpty(result))
                        {
                            result = "./"; // Oh well.
                        }
                        else
                        {
                            result += "/.config/";
                        }
                    }
                    result = Path.Combine(result, "truecraft");
                }
                else
                {
                    throw new NotSupportedException("Unhandled SDL2 platform!");
                }
                if (!Directory.Exists(result))
                {
                    Directory.CreateDirectory(result);
                }
                return result;
            }
        }

        public static string Worlds
        {
            get
            {
                return Path.Combine(Base, "worlds");
            }
        }

        public static string Settings
        {
            get
            {
                return Path.Combine(Base, "settings.json");
            }
        }

        public static string Screenshots
        {
            get
            {
                return Path.Combine(Base, "screenshots");
            }
        }

        public static string TexturePacks
        {
            get
            {
                return Path.Combine(Base, "texturepacks");
            }
        }
    }
}