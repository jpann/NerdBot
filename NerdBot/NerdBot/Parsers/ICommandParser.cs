using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBotCommon.Parsers
{
    public interface ICommandParser
    {
        Command Parse(string text);
    }
}
