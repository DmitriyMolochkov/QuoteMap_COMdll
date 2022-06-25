using System;

namespace QuoteMap
{
    public class SessionNotOpenException: Exception
    {
        public SessionNotOpenException() : base("Session not open") {}
    }
}