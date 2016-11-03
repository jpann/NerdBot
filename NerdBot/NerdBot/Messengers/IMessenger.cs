using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBot.Messengers
{
    public interface IMessenger
    {
        string BotId { get; }
        string BotName { get; }
        string[] IgnoreNames { get; }

        bool SendMessage(string message);
        bool SendMessageWithMention(string message, string mentionId, int start, int end);
    }
}
