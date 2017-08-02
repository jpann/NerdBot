using System;
using System.IO;
using Nini.Config;

namespace NerdBot.TestsHelper
{
    public class ConfigReader
    {
        public TestConfiguration Read()
        {
            string configFile = Path.Combine(Environment.CurrentDirectory, "NerdBot_Tests.ini");

            IConfigSource source = new IniConfigSource(configFile);
            TestConfiguration config = new TestConfiguration();

            config.Url = source.Configs["Database"].Get("connectionString");
            if (string.IsNullOrEmpty(config.Url))
                throw new Exception("Configuration file is missing 'connectionString' setting in section 'Database'.");

            config.Database = source.Configs["Database"].Get("dbName");
            if (string.IsNullOrEmpty(config.Database))
                throw new Exception("Configuration file is missing 'dbName' setting in section 'Database'.");

            config.PriceDatabase = source.Configs["Database"].Get("priceDbName");
            if (string.IsNullOrEmpty(config.PriceDatabase))
                throw new Exception("Configuration file is missing 'priceDbName' setting in section 'Database'.");

            config.TestDb = source.Configs["Database"].Get("testDbName");
            if (string.IsNullOrEmpty(config.TestDb))
                throw new Exception("Configuration file is missing 'testDbName' setting in section 'Database'.");

            return config;
        }
    }

    public class TestConfiguration
    {
        public string Url { get; set; }
        public string Database { get; set; }
        public string PriceDatabase { get; set; }
        public string TestDb { get; set; }
    }
}
