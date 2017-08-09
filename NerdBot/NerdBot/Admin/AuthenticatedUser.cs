using System.Collections.Generic;
using Nancy.Security;

namespace NerdBotCommon.Admin
{
    public class AuthenticatedUser : IUserIdentity
    {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}
