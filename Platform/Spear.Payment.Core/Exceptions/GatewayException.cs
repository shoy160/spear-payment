using System;

namespace Spear.Payment.Core.Exceptions
{
    public class GatewayException : Exception
    {
        public GatewayException(string message)
            : base(message)
        {
        }
    }
}
