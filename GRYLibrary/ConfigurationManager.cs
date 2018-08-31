
namespace GRYLibrary
{
    public sealed class ConfigurationManager<T> where T: new()
    {
        public string ConfigurationFile = null;
        public ConfigurationManager(string configurationFile)
        {
            this.ConfigurationFile = configurationFile;
        }
        public T LoadConfiguration()
        {
            throw new System.NotImplementedException();
        }
        public void SaveConfiguration(T configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}
