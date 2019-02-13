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
                string result;
                if (RuntimeInfo.IsWindows)
                {
                    result = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }
                else if (RuntimeInfo.IsMacOSX)
                {
                    result = Environment.GetEnvironmentVariable("HOME");
                    if (String.IsNullOrEmpty(result))
                    {
                        result = "./"; // Oh well.
                    }
                    else
                    {
                        result += "/Library/Application Support/";
                    }
                }
                else if (RuntimeInfo.IsLinux)
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
                }
                else
                {
                    System.Console.WriteLine("Unrecognized platform. Fall back to CWD.");
                    result = Environment.CurrentDirectory;
                }
                result = Path.Combine(result, "truecraft");
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