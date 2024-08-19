namespace CommonLib.Utils
{
    public sealed class ConfigurationFileUtils
    {
        private ConfigurationFileUtils() { }

        public static string ReadConfigurationFile(string configName)
        {
            return File.ReadAllText(configName);
        }

        public static T LoadConfiguration<T>(string configName = $@"config.json")
        {
            var configFile = ReadConfigurationFile(configName);
            return configFile.Deserialize<T>() ?? throw new NullReferenceException();
        }
    }
}
