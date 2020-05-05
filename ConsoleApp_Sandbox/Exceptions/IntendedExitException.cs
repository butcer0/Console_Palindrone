using System;

namespace ConsoleApp_Sandbox.Exceptions
{
    [Serializable()]
    public class IntendedExitException : System.Exception
    {
        public IntendedExitException() : base() { }
        public IntendedExitException(string message) : base(message) { }
        public IntendedExitException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client.
        protected IntendedExitException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    } 
}