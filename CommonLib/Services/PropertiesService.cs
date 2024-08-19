using CommonLib.Data;
using CommonLib.Data.Databases;

namespace CommonLib.Services
{
    public class PropertiesService
    {
        private readonly SilentstormDatabase _db;
        private Dictionary<string, Property> _properties;

        public PropertiesService(SilentstormDatabase database)
        {
            _db = database;
            _properties = new Dictionary<string, Property>();
            ReloadPropertiesAsync().GetAwaiter().GetResult();
        }

        public async Task ReloadPropertiesAsync()
        {
            _properties = new Dictionary<string, Property>();
            foreach (var property in await _db.Properties.GetAllAsync())
                _properties.Add(property.Id, property);
        }

        public string? GetProperty(string key)
        {
            _properties.TryGetValue(key, out var property);
            return property?.Value;
        }

        public string GetPropertyOrDefault(string key, string defaultValue)
        {
            return GetProperty(key) ?? defaultValue;
        }

        public int? GetPropertyAsInt(string key)
        {
            return int.TryParse(GetProperty(key), out var value) ? value : null;
        }

        public int GetPropertyAsIntOrDefault(string key, int defaultValue)
        {
            return GetPropertyAsInt(key) ?? defaultValue;
        }

        public List<string>? GetPropertyAsList(string key, string separator = ",")
        {
            return GetProperty(key)
                ?.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        public List<string> GetPropertyAsListOrDefault(string key, List<string> defaultValue, string separator = ",")
        {
            return GetPropertyAsList(key, separator) ?? defaultValue;
        }
    }
}
