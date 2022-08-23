using Discord.Interactions;
using Discord.WebSocket;
using GhidorahBot.Models;
using GhidorahBot.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;
using Discord.Commands;

namespace GhidorahBot.Services
{
    public class PlayerQueueService
    {
        private DiscordSocketClient _discordClient { get; set; }

        public string LocalNotification = string.Empty;
        public string GlobalNotification = string.Empty;
        public string DirectMessage = string.Empty;
        public ulong GuildId;
        public ulong ChannelId;
        public List<PlayerQueueGuildModel> GuildIdList = new List<PlayerQueueGuildModel>();

        private List<QueueModel> _playerQue = new List<QueueModel>();
        private List<QueueModel> _teamOne = new List<QueueModel>();
        private List<QueueModel> _teamTwo = new List<QueueModel>();
        private List<QueueModel> _host = new List<QueueModel>();

        private List<ScrimModel> _scrimQue = new List<ScrimModel>();
        private int _scrimId = 0;

        private Timer timer_exit = null;
        private DateTime time_started = DateTime.MinValue;
        private bool _clearQueue;

        public PlayerQueueService(DiscordSocketClient client)
        {
            _discordClient = client;
        }

        public void JoinQueue(SocketUser discordName, string activisionId, ulong guildId, ulong channelId)
        {
            bool matchResult = false;
            matchResult = _playerQue.Any(p => p.User.Equals(discordName));

            if (matchResult && _playerQue.Count < 8)
            {
                DirectMessage = "You are already queue'd for random 8s.\r" +
                    "Please wait while we gather more players.\r" +
                    $"Players currently in queue: {_playerQue.Count}/8";
                return;
            }

            if (_playerQue.Count == 7)
            {
                try
                {
                    PlayerQueueGuildModel uniqueGuild = GuildIdList.Single(x => x.GuildId.Equals(guildId));
                }
                catch
                {
                    GuildIdList.Add(new PlayerQueueGuildModel(guildId, channelId));
                }
                            
                _playerQue.Add(new QueueModel(discordName, activisionId));
                GetQueueStatus();
                return;
            }

            if(!matchResult)
            {
                try
                {
                    PlayerQueueGuildModel uniqueGuild = GuildIdList.Single(x => x.GuildId.Equals(guildId));
                }
                catch
                {
                    GuildIdList.Add(new PlayerQueueGuildModel(guildId, channelId));
                }

                _playerQue.Add(new QueueModel(discordName, activisionId));
                LocalNotification = $"joined the queue!\r" +
                    $"There are currently {_playerQue.Count}/8 player(s) waiting.";
            }
        }

        public void LeaveQueue(SocketUser discordName)
        {
            DirectMessage = string.Empty;
            QueueModel playerToRemove = null;

            try
            {
                playerToRemove = _playerQue.Single(p => p.User.Equals(discordName));
            }
            catch
            {
                DirectMessage = "You are not currently queue'd for 8s.";
            }
            finally
            {
                _playerQue.Remove(playerToRemove);
                DirectMessage = "You have left the queue.";
            }           
        }

        public void GetQueueStatus()
        {
            //TODO: Change this back to 8
            if (_playerQue.Count == 3)
            {
                GuildIdList = GuildIdList.Distinct().ToList();

                var random = new Random();
                _host = _playerQue.OrderBy(x => random.Next()).Take(1).ToList();
                GlobalNotification = String.Empty;
                GlobalNotification = $"Players are ready!\r" +
                    $"Randomly suggested host: {_host[0].User.Mention}\r" +
                    $"Note: If this user does NOT want to host please talk amongst yourselves to determine a host.\r\r";

                GlobalNotification += $"\r";
                _teamOne = _playerQue.OrderBy(x => random.Next()).Take(4).ToList();

                foreach(var player in _teamOne)
                {
                    _playerQue.Remove(player);
                }

                _teamTwo = _playerQue.OrderBy(x => random.Next()).Take(4).ToList();
                GlobalNotification += $"TEAM 1:\r";

                foreach(var player in _teamOne)
                {
                    GlobalNotification += $"{player.User.Mention} | Activision Id: {player.ActivisionId}\r";
                }

                GlobalNotification += $"TEAM 2:\r";
                foreach (var player in _teamTwo)
                {
                    GlobalNotification += $"{player.User.Mention} | Activision Id: {player.ActivisionId}\r";
                }

                _teamOne.Clear();
                _teamTwo.Clear();
                _playerQue.Clear();
            }
            else
            {
                LocalNotification = $"There are currently: {_playerQue.Count}/8 player(s) queue'd for random 8s.\r\r" +
                    $"You will be mentioned when all 8 players are queue'd.";
            }
        }

        public void JoinScrimQueue(SocketUser user, string activisionId, string scrimTime, string notes,
            ulong guildId, ulong channelId)
        {
            if(string.IsNullOrWhiteSpace(scrimTime))
            {
                scrimTime = $"{DateTime.Now}";
            }

            _scrimId++;
            _scrimQue.Add(new ScrimModel(_scrimId, user, activisionId, scrimTime, notes));

            LocalNotification = $"Team scrim added to queue.\r" +
                $"Scrim Id: {_scrimId}\r" +
                $"Hosts Activision Id: {activisionId}\r" +
                $"Discord Username: {user.Username}#{user.Discriminator}\r" +
                $"Scrim Time: {scrimTime}\r" +
                $"Notes: {notes}\r\r" +
                "If a team would like to accept this scrim please use the command:\r" +
                "!accept {scrimId} (please do not include brackets)";
        }

        public void AcceptScrim(SocketUser user, string index)
        {
            int id;
            bool validInt;
            validInt = int.TryParse(index, out id);

            if(validInt)
            {
                var removeScrim = _scrimQue.Single(s => s.Id == id);
                LocalNotification = $"{user.Mention} You have accepted the scrim.\r" +
                    $"Please message {removeScrim.DiscordUser} or add Activision Id: {removeScrim.ActivisonId}";
                _scrimQue.Remove(removeScrim);
            }
        }

        public void GetScrimQueueStatus()
        {
            LocalNotification = $"There are currently {_scrimQue.Count} team scrims in queue.\r\r";

            foreach(var scrim in _scrimQue)
            {
                LocalNotification += $"Scrim Id: {scrim.Id}\r" +
                    $"Hosts Discord: {scrim.DiscordUser}\r" +
                    $"Scrim Time: {scrim.ScrimTime}\r" +
                    $"Notes: {scrim.Notes}";
            }
        }

        //League Staff Only
        public void ClearScrimQueue()
        {
            _scrimQue.Clear();
            LocalNotification = $"Scrim queue cleared | Queue count = {_scrimQue.Count}";
        }

        public async void ClearQueueHandler()
        {
            await Logger.Log(Discord.LogSeverity.Info, $"{nameof(Program)}", $"Time has started");

            try
            {
                time_started = DateTime.Now;
                timer_exit = new Timer(10000); //Check every 10 seconds
                timer_exit.Elapsed += new ElapsedEventHandler(OnTimer);
                timer_exit.AutoReset = false;
                timer_exit.Enabled = true;
            }
            catch (Exception ex)
            {
                await Logger.Log(Discord.LogSeverity.Info, $"ClearQueueHandler", $"Error: {ex}");
            }
        }

        private async void OnTimer(object? sender, ElapsedEventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            
            //Reset time for boolean
            DateTime dtResetQueueBoolean = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 3, 0, 0);
            //Cutoff time for Today
            DateTime dtTodayCutoff = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 3, 1, 0);
            DateTime dtCutoffDelay = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 3, 1, 30);


            if (time_started < dtResetQueueBoolean && dtNow >= dtResetQueueBoolean)
            {
                _clearQueue = true;
            }

            //Compare times against cutoff for today
            if(_clearQueue)
            {
                if (time_started < dtTodayCutoff && dtNow >= dtTodayCutoff && dtNow <= dtCutoffDelay)
                {
                    ClearScrimQueue();
                    await Logger.Log(Discord.LogSeverity.Info, $"Method | OnTimer", $"{LocalNotification}");
                    timer_exit.Enabled = true;
                    _clearQueue = false;
                }
                else
                {
                    timer_exit.Enabled = true;
                }
            }
            else
            {
                timer_exit.Enabled = true;
            }
        }
    }
}
