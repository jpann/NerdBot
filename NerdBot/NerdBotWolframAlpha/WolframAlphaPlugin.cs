using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NerdBot;
using NerdBot.Parsers;
using NerdBot.Plugin;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Mtg;
using NerdBotCommon.Mtg.Prices;
using NerdBotCommon.Parsers;
using NerdBotCommon.UrlShortners;
using Nini.Config;
using WolframAlphaNET;
using WolframAlphaNET.Misc;
using WolframAlphaNET.Objects;

namespace NerdBotWolframAlpha
{
    public class WolframAlphaPlugin : PluginBase
    {
        private const int cSubPodTakeCount = 3; // The number of sub pods to take
        private const float cScanTimeout = 3.0f; // How long should the query scan timeout be. The larger this value is, the longer it will take for a response

        private WolframAlpha mWolframAlpha;

        public override string Name
        {
            get { return "wf command"; }
        }

        public override string Description
        {
            get { return "Returns the result of a WolframAlpha query.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns the result of a WolframAlpha query."; }
        }

        public override string Command
        {
            get { return "wf"; }
        }

        public override string HelpCommand
        {
            get { return "help wf"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'wf price of electricity in Albuquerque' or 'wf robbery Phoenix, AZ' or 'wf do you know a dirty joke?", this.Command); }
        }

        public WolframAlphaPlugin(
                IBotServices services,
                BotConfig config)
            : base(
                services,
                config)
        {
            string configFile = Path.Combine(Environment.CurrentDirectory, "Wolfram.ini");

            string appId = LoadConfig(configFile);

            this.mWolframAlpha = new WolframAlpha(appId);
            this.mWolframAlpha.ScanTimeout = cScanTimeout;
        }

        public override void OnLoad()
        {

        }

        public override void OnUnload()
        {
        }

        public override async Task<bool> OnMessage(IMessage message, IMessenger messenger)
        {

            return false;
        }

        public override async Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            if (command.Arguments.Any())
            {
                string argument = command.Arguments.FirstOrDefault();

                this.mLoggingService.Trace("Using query: {0}", argument);

                try
                {
                    QueryResult result = this.mWolframAlpha.Query(argument);

                    if (result != null)
                    {
                        Pod primaryPod = result.GetPrimaryPod();

                        if (primaryPod != null)
                        {
                            if (primaryPod.SubPods.HasElements())
                            {
                                foreach (SubPod subPod in primaryPod.SubPods.Take(cSubPodTakeCount))
                                {
                                    if (subPod.Image != null && string.IsNullOrEmpty(subPod.Plaintext))
                                    {
                                        messenger.SendMessage(subPod.Image.Src);
                                    }
                                    else
                                    {
                                        messenger.SendMessage(subPod.Plaintext);
                                    }
                                    
                                }
                            }
                        }
                        else
                        {
                            // If there was no primary pod, get the 2nd pod which should have the main information.
                            if (result.Pods.Count() >= 2)
                            {
                                var nextPod = result.Pods[1];

                                if (nextPod != null)
                                {
                                    foreach (SubPod subPod in nextPod.SubPods.Take(cSubPodTakeCount))
                                    {
                                        messenger.SendMessage(subPod.Plaintext);
                                    }
                                }
                            }
                        }
                    }

                    if (result.DidYouMean.HasElements())
                    {
                        foreach (DidYouMean didYouMean in result.DidYouMean.Take(2))
                        {
                            messenger.SendMessage("Did you mean: " + didYouMean.Value);
                        }

                    }
                }
                catch (Exception er)
                {
                    this.mLoggingService.Error(er, "WolframAlpha query error");

                    throw;
                }
            }
            else
            {
                this.mLoggingService.Warning("No arguments provided.");
            }

            return false;
        }

        private string LoadConfig(string file)
        {
            IConfigSource source = new IniConfigSource(file);

            string appId = source.Configs["Wolfram"].Get("appId");
            if (string.IsNullOrEmpty(appId))
                throw new Exception("Configuration file is missing 'appId' setting in section 'Wolfram'.");

            return appId;
        }
    }
}
