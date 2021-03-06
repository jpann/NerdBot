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
    public class RandomCardPlugin : PluginBase
    {
        public override string Name
        {
            get { return "randomcard command"; }
        }

        public override string Description
        {
            get { return "Returns a random card.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random card."; }
        }

        public override string Command
        {
            get { return "randomcard"; }
        }

        public override string HelpCommand
        {
            get { return "help randomcard"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'randomcard'", this.Command); }
        }

        public RandomCardPlugin(
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

            Card card = await this.Services.Store.GetRandomCard();

            if (card != null)
            {
                messenger.SendMessage(card.Img);

                return true;
            }

            return false;
        }
    }
}
