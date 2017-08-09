using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdBotCommon.Extensions
{
    public static class TypeExtensions
    {
        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Contains(interfaceType);
        }
    }
}
