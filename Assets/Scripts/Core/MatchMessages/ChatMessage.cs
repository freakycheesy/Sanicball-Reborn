using Mirror;

namespace SanicballCore.MatchMessages
{
    public enum ChatMessageType
    {
        System,
        User
    }

    public struct ChatMessage : NetworkMessage
    {
        public string From;
        public ChatMessageType Type;
        public string Text;

        public ChatMessage(string from, ChatMessageType type, string text)
        {
            From = from;
            Type = type;
            Text = text;
        }
    }
}