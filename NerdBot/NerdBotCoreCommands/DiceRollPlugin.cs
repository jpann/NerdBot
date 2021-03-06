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
    public class DiceRollPlugin: PluginBase
    {
        private Random mRandom;

        public override string Name
        {
            get { return "roll command"; }
        }

        public override string Description
        {
            get { return "Rolls a die and returns the result.";  }
        }

        public override string ShortDescription
        {
            get { return "Rolls a die and returns the result."; }
        }

        public override string Command
        {
            get { return "roll"; }
        }

        public override string HelpCommand
        {
            get { return "help roll"; }
        }

        public override string HelpDescription
        {
            get { return string.Format("{0} example usage: 'roll' or 'roll 100' or 'roll 20 x5'", this.Command); }
        }

        public DiceRollPlugin(
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
            this.mRandom = new Random();
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

            int maxDieSides = 6;
            int dieCount = 1;

            // Roll 1 through argument value, if argument is an integer
            if (command.Arguments.Length == 1)
            {
                string argument = command.Arguments[0].ToLower();

                int n;
                bool isNumeric = int.TryParse(argument, out n);

                if (isNumeric)
                    maxDieSides = n;
                else
                {
                    // If it isn't a numeric, lets see if a die count was provided
                    if (argument.IndexOf('x') > 0)
                    {
                        // A die count was provided in the form of 'x#'

                        // Get the die sides as seen before the die count's 'x'
                        string tmpDieSides = argument.Substring(0, argument.IndexOf('x'));

                        // Get the die count as seen after the die count's 'x'
                        string tmpDieCount = argument.Substring(argument.IndexOf('x') + 1);

                        // Make sure we're not working with and empty die sides value
                        if (!string.IsNullOrEmpty(tmpDieSides))
                        {
                            int m;
                            bool isSidesNumeric = int.TryParse(tmpDieSides.Trim(), out m);

                            // If it is numerc, set maxSides to this value
                            if (isSidesNumeric)
                                maxDieSides = m;
                        }

                        // Make sure we're not working with an empty die count value
                        if (!string.IsNullOrEmpty(tmpDieCount))
                        {
                            int m;
                            bool isDieCountNumeric = int.TryParse(tmpDieCount.Trim(), out m);

                            // If it is numeric, set dieCount to this value
                            if (isDieCountNumeric)
                                dieCount = m;
                        }
                    }
                }
            }

            // Define rolls array
            int[] rolls = new int[dieCount];

            // Roll the dice
            for (int i = 0; i < dieCount; i++)
            {
                int roll = this.mRandom.Next(1, maxDieSides + 1);

                rolls[i] = roll;
            }

            messenger.SendMessage(string.Format("Roll 1-{0} x{1}: {2}",
                maxDieSides,
                dieCount,
                string.Join(", ", rolls)));

            return true;
        }
    }
}
