using System;
using System.Runtime.Serialization;

namespace AspNetCore.Security.Jwt
{
    [Serializable]
    public class SecurityException : Exception
    {
        public SecurityException(string msg) : base(msg)
        {

        }

        protected SecurityException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }        
    }
}
