using System.Net.WebSockets;

namespace Hub.Runtime
{
    public class UserSession
    {
        public Guid UserID { get; } = Guid.NewGuid();
        public string? Name { get; set; }
        public WebSocket Socket { get; }

        public DateTime ConnectedAt { get; } = DateTime.UtcNow;

        public UserSession(WebSocket socket, string? name)
        {
            Socket = socket;
            Name = name;
        }
    }
}
