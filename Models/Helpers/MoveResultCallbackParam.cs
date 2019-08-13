namespace Uno.Models.Helpers
{
    public class MoveResultCallbackParam
    {
      
        public MoveResultCallbackParam(string command, string connectionId, object obj)
        {
            this.Command = command;
            this.ConnectionId = connectionId;
            this.Object = obj;

        }
        public string Command { get; set; }
        public string ConnectionId { get; set; }
        public object Object { get; set; }
    }
}