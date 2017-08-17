using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NerdBotCommon.Parsers;

namespace NerdBot.Parsers
{
    public interface ICommandParser
    {
        Command Parse(string text);
    }
}
