using Microsoft.Extensions.Configuration;

namespace Whispersteppe.XUnit
{

    /// <summary>
    /// collection fixture.  things that are shared between all unit tests without reloading
    /// </summary>
    public class XUnitCollectionFixture : IDisposable
    {
        IConfigurationRoot _config;

        public XUnitCollectionFixture()
        {
            _config = SetupConfig();

            //  refresh the database


        }

        /// <summary>
        /// set up the configuration
        /// </summary>
        /// <returns></returns>
        private IConfigurationRoot SetupConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("TestSettings.json")
                //                .AddEnvironmentVariables()
                .Build();

            _config = config;

            return config;

        }



        /// <summary>
        /// get a config and bind it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configName"></param>
        /// <returns></returns>
        public T GetConfig<T>(string configName) where T : new()
        {

            T config = new();

            _config.Bind(configName, config);

            return config;
        }

        /// <summary>
        /// dispose of anything that needs disposing
        /// </summary>
        public void Dispose()
        {
            //  nothing to dispose.  carry on
            GC.SuppressFinalize(this);
        }
    }
}
