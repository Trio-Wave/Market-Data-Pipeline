namespace Triowave.Services
{
    public class AppSettingsService
    {
        private readonly IConfiguration _configuration;

        public AppSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T GetSection<T>(string sectionName) where T : class, new()
        {
            var section = new T();
            _configuration.GetSection(sectionName).Bind(section);
            return section;
        }

        public string? GetValue(string key)
        {
            return _configuration[key];
        }

        public T? GetValue<T>(string key)
        {
            return _configuration.GetValue<T>(key);
        }
    }
}
