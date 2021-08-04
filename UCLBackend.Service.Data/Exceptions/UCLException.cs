using System;

namespace UCLBackend.Service.Data.Exceptions
{
    public class UCLException : Exception
    {
        public UCLException(string message) : base(message)
        {

        }
    }
}