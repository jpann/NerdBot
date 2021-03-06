﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;

namespace NerdBot.Reporters
{
    public class GroupMeReporter : IReporter
    {
        private readonly IMessenger mMessenger;

        public GroupMeReporter(IMessenger messenger)
        {
            if (messenger == null)
                throw new ArgumentNullException("messenger");

            this.mMessenger = messenger;
        }

        public void Message(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            this.mMessenger.SendMessage(message);
        }

        public void Error(string message, Exception er)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            if (er == null)
                throw new ArgumentNullException("er");

            this.mMessenger.SendMessage(string.Format("ERROR REPORT: {0}", message));
            this.mMessenger.SendMessage(string.Format("ERROR: {0}", er.Message));
        }

        public void Warning(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            this.mMessenger.SendMessage(string.Format("WARNING REPORT: {0}", message));
        }
    }
}
