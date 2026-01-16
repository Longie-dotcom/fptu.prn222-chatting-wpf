namespace Infrastructure.InfrastructureException
{
    public abstract class InfrastructureExceptionBase : Exception
    {
        protected InfrastructureExceptionBase(string message) : base(message) { }
    }

    public class MessagingConnectionException : InfrastructureExceptionBase
    {
        public MessagingConnectionException(string message) : base(message) { }
    }

    public class GrpcCommunicationException : InfrastructureExceptionBase
    {
        public GrpcCommunicationException(string message) : base(message) { }
    }
}
