using System.IO;
using YamlDotNet.Serialization;

namespace TrueCraft.API
{
    /// <summary>
    /// Abstract base class for configurations read from YAML files.
    /// </summary>
    public abstract class Configuration
    {
        /// <summary>
        /// Creates and returns a new configuration read from a YAML file.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="configFileName">The path to the YAML file.</param>
        /// <returns></returns>
        public static T LoadConfiguration<T>(string configFileName) where T : new()
        {
            T config;

            if (File.Exists(configFileName))
            {
                var deserializer = new Deserializer(ignoreUnmatched: true);
                using (var file = File.OpenText(configFileName))
                    config = deserializer.Deserialize<T>(file);
            }
            else
            {
                config = new T();
            }

            var serializer = new Serializer();
            using (var writer = new StreamWriter(configFileName))
                serializer.Serialize(writer, config);

            return config;
        }
    }
}
