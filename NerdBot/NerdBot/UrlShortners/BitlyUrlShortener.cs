using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitlyDotNET.Implementations;
using BitlyDotNET.Interfaces;

namespace NerdBot.UrlShortners
{
    public class BitlyUrlShortener : IUrlShortener
    {
        private readonly string mUser;
        private readonly string mKey;
        private readonly IBitlyService mBitlyService;

        public string User
        {
            get { return this.mUser; }
        }

        public string Key
        {
            get { return this.mKey;  }
        }

        public BitlyUrlShortener(string user, string key)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException("user");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");

            this.mUser = user;
            this.mKey = key;

            this.mBitlyService = new BitlyService(this.mUser, this.mKey);
        }

        public string ShortenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            try
            {
                string shortened;
                if (this.mBitlyService.Shorten(url, out shortened) == StatusCode.OK)
                {
                    return shortened;
                }

                return url;
            }
            catch (Exception er)
            {
                return url;
            }
        }
    }
}
