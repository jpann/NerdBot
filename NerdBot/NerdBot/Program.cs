using System.IO;
using Nini.Config;

namespace NerdBotCommon
{
    using System;
    using Nancy.Hosting.Self;

    class Program
    {
        private static string mHostname = "localhost";
        private static int mPort = 3579;

        static void Main(string[] args)
        {
            LoadApplicationConfig();

            var uri = new Uri(string.Format("http://{0}:{1}", mHostname, mPort));

            using (var host = new NancyHost(uri))
            {
                host.Start();

                Console.WriteLine("Your application is running on " + uri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }

        static void LoadApplicationConfig()
        {
            try
            {
                // Get configuration data
                string configFile = Path.Combine(Environment.CurrentDirectory, "NerdBot.ini");
                if (!File.Exists(configFile))
                    throw new Exception("Configuration file 'NerdBot.ini' does not exist.");

                IConfigSource source = new IniConfigSource(configFile);

                string hostname = source.Configs["App"].Get("hostname");
                if (!string.IsNullOrEmpty(hostname))
                    mHostname = hostname;

                int port = source.Configs["App"].GetInt("port");
                mPort = port;
            }
            catch (Exception)
            {
                // Use defaults
            }
        }
    }
}
