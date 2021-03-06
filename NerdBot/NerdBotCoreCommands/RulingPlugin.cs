﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace NerdBotCoreCommands
{
    public class RulingPlugin : PluginBase
    {
        private const string cRulingUrl = "{0}/ruling/{1}";

        public override string Name
        {
            get { return "ruling command"; }
        }

        public override string Description
        {
            get { return "Returns a link that displays the specified card's rulings."; }
        }

        public override string ShortDescription
        {
            get { return "Returns a link that displays the specified card's rulings."; }
        }

        public override string Command
        {
            get { return "ruling"; }
        }

        public override string HelpCommand
        {
            get { return "help ruling"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'ruling spore clou%' or 'ruling fem;spore clou%' or 'ruling 'fallen empires;spore %loud'", this.Command); }
        }

        public RulingPlugin(
                IBotServices services,
                BotConfig config
            )
            : base(
                services,
                config)
        {
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
                Card card = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    // Get card using only name
                    card = await this.Services.Store.GetCard(name);
                }
                else if (command.Arguments.Length == 2)
                {
                    string name = command.Arguments[1];
                    string set = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    if (string.IsNullOrEmpty(set))
                        return false;

                    // Get card using only name
                    card = await this.Services.Store.GetCard(name, set);
                }

                if (card != null)
                {
                    if (card.Rulings.Any())
                    {
                        string url = string.Format(cRulingUrl, this.Config.HostUrl, card.MultiverseId);

                        messenger.SendMessage(string.Format("Card ruling: {0}", url));
                    }
                    else
                    {
                        messenger.SendMessage(string.Format("Card '{0}' in set '{1}' has no rulings available.", card.Name, card.SetId));
                    }
                }
            }

            return false;
        }
    }
}
