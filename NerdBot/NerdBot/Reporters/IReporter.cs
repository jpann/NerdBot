using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot.Reporters
{
    public interface IReporter
    {
        void Message(string message);
        void Error(string message, Exception er);
        void Warning(string message);
    }
}
