using System.IO;
using Mono.Unix;
using Mono.Unix.Native;
using NerdBotCommon.Utilities;
using Nini.Config;

namespace NerdBot
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
                Console.WriteLine("Your application is running on " + uri);

                host.Start();

                if (RuntimeUtility.IsRunningOnMono())
                {
                    var terminationSignals = RuntimeUtility.GetUnixTerminationSignals();
                    UnixSignal.WaitAny(terminationSignals);
                }
                else
                {
                    Console.WriteLine("Press any [Enter] to close the host.");
                    Console.ReadLine();
                }

                host.Stop();
            }
        }

        static void LoadApplicationConfig()
        {
            try
            {
                if (!RuntimeUtility.IsRunningOnMono())
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
                else
                {
                    mHostname = "localhost";
                    mPort = 3579;
                }

            }
            catch (Exception er)
            {
                // Use defaults
                Console.WriteLine(er.Message);
                Console.WriteLine(er.StackTrace);
            }
        }
    }
}
