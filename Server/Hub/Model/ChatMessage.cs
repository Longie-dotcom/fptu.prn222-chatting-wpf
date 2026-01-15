namespace Hub.Model
{
    public enum MessageType
    {
        Message, File, Image, Connected, Handshake
    }

    public class ChatMessage
    {
        public Guid ChatMessageID { get; set; }
        public Guid SenderID { get; set; }
        public string Message { get; set; } = string.Empty;
        public MessageType Type { get; set; } = MessageType.Message;
        public DateTime Time { get; set; } = DateTime.Now;
        public string SenderName { get; set; } = string.Empty;
    }
}
