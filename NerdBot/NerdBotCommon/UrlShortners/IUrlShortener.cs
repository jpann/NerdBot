using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdBotCommon.UrlShortners
{
    public interface IUrlShortener
    {
        string User { get; }
        string Key { get; }

        string ShortenUrl(string url);
    }
}
