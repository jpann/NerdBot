using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.IO;

namespace NerdBot.Extensions
{
    public static class RequestBodyExtension
    {
        public static string ReadAsString(this RequestStream requestStream)
        {
            using (var reader = new StreamReader(requestStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
