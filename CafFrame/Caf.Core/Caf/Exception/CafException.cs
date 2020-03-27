using System;
using System.Runtime.Serialization;
namespace Caf.Core
{
    public class CafException: Exception
    {
        public CafException()
        {

        }

        public CafException(string message)
            : base(message)
        {

        }

        public CafException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

    }
}
