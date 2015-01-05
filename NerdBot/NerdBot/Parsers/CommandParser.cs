using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NerdBot.Parsers
{
    public class CommandParser : ICommandParser
    {
        public Command Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("text");

            Match cmdMatch = Regex.Match(text, @"^(?<cmd>[A-Za-z0-9]+) (?:(?<args>[A-Za-z0-9!%&\-']+),?)+", RegexOptions.IgnoreCase);

            if (cmdMatch.Success)
            {
                Command cmd = new Command();
                string command = cmdMatch.Groups["cmd"].Value;

                List<string> arguments = new List<string>();

                foreach (Capture capture in cmdMatch.Groups["args"].Captures)
                {
                    arguments.Add(capture.Value);
                }

                cmd.Cmd = command;
                cmd.Arguments = arguments.ToArray();

                return cmd;
            }

            return null;
        }
    }
}
