using System.IO;
using YamlDotNet.Serialization;

namespace TrueCraft.API
{
    public abstract class Configuration
    {
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
