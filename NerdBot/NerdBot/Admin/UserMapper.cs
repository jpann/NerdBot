using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;

namespace NerdBotCommon.Admin
{
    public class UserMapper : IUserMapper
    {
        private static List<Tuple<string, string, Guid>> users = new List<Tuple<string, string, Guid>>();

        public UserMapper(BotConfig config)
        {
            users.Add(new Tuple<string, string, Guid>(config.AdminUser, config.AdminPassword, Guid.NewGuid()));
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var userRecord = users.FirstOrDefault(u => u.Item3 == identifier);

            return userRecord == null
                       ? null
                       : new AuthenticatedUser()
                       {
                           UserName = userRecord.Item1,
                           Claims = new[]
                            {
                                "Admin"
                            }
                       };
        }

        public static Guid? ValidateUser(string username, string password)
        {
            var userRecord = users.FirstOrDefault(u => u.Item1 == username && u.Item2 == password);

            if (userRecord == null)
            {
                return null;
            }

            return userRecord.Item3;
        }

    }
}
