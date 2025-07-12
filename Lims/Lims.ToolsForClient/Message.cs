namespace Lims.ToolsForClient
{
    public class Message
    {
        public Message(string content)
        {
            Content = content;
        }
        public Message()
        {

        }
        public string Content { get; private set; }
    }
}
