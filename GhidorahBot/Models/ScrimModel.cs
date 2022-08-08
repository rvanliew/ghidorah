using Discord.WebSocket;

namespace GhidorahBot.Models
{
    public class ScrimModel
    {
        public int Id { get; set; }
        public SocketUser DiscordUser { get; set; }
        public string ActivisonId { get; set; }
        public string ScrimTime { get; set; }
        public string Notes { get; set; }

        public ScrimModel(int id, SocketUser discordUser, string activisionId,string scrimTime, string notes)
        {
            Id = id;
            DiscordUser = discordUser;
            ActivisonId = activisionId;
            ScrimTime = scrimTime;
            Notes = notes;
        }
    }
}
