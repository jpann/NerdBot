using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdBot.Reporters
{
    public interface IReporter
    {
        void ReportError(string message, Exception er);
        void ReportWarning(string message);
    }
}
