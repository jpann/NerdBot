using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBot.Http
{
    public interface IHttpClient
    {
        string Post(string url, string json);
    }
}
