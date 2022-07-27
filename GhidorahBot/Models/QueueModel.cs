using Discord.WebSocket;

namespace GhidorahBot.Models
{
    public class QueueModel
    {
        public SocketUser User { get; set; }
        public string ActivisionId { get; set; }

        public QueueModel(SocketUser user, string activisionId)
        {
            User = user;
            ActivisionId = activisionId;
        }
    }
}
