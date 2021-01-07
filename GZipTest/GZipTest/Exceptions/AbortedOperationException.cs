using System;

namespace GZipTest.Exceptions
{
    public class AbortedOperationException : Exception
    {
        private const string OperationWasInterrupted = "The operation was interrupted";

        public AbortedOperationException() : base($"{OperationWasInterrupted}.")
        {
        }

        public AbortedOperationException(string message) : base($"{OperationWasInterrupted}: " + message)
        {
        }

        public AbortedOperationException(string message, Exception inneException) : base($"{OperationWasInterrupted}: " + message, inneException)
        {
        }
    }
}
