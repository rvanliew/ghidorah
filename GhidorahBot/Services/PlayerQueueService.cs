using Discord.Interactions;
using Discord.WebSocket;
using GhidorahBot.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Services
{
    public class PlayerQueueService
    {
        public string Notification = string.Empty;
        private List<QueueModel> _playerQue = new List<QueueModel>();
        private List<QueueModel> _teamOne = new List<QueueModel>();
        private List<QueueModel> _teamTwo = new List<QueueModel>();
        private List<QueueModel> _host = new List<QueueModel>();

        private List<ScrimModel> _scrimQue = new List<ScrimModel>();
        private int _scrimId = 0;

        public PlayerQueueService()
        {
            //
        }

        public void JoinQueue(SocketUser discordName, string activisionId)
        {
            bool matchResult = false;
            matchResult = _playerQue.Any(p => p.User.Equals(discordName));

            if (matchResult && _playerQue.Count < 8)
            {
                Notification = "You are already queue'd for random 8s.\r" +
                    "Please wait while we gather more players.\r" +
                    $"Players currently in queue: {_playerQue.Count}/8";
                return;
            }

            if (_playerQue.Count == 7)
            {
                //Last User
                _playerQue.Add(new QueueModel(discordName, activisionId));
                GetQueueStatus();
                return;
            }

            if(!matchResult)
            {
                _playerQue.Add(new QueueModel(discordName, activisionId));
                Notification = $"You have joined the que!\r" +
                    $"There are currently {_playerQue.Count}/8 player(s) waiting.";
            }
        }

        public void LeaveQueue(SocketUser discordName)
        {
            var playerToRemove = _playerQue.Single(p => p.User.Equals(discordName));
            _playerQue.Remove(playerToRemove);
            Notification = "You have left the queue.";
        }

        public void GetQueueStatus()
        {
            if (_playerQue.Count == 8)
            {
                var random = new Random();
                _host = _playerQue.OrderBy(x => random.Next()).Take(1).ToList();
                Notification = String.Empty;
                Notification = $"Players are ready!\r" +
                    $"Randomly suggested host: {_host[0].User.Mention}\r" +
                    $"Note: If this user does NOT want to host please talk amongst yourselves to determine a host.\r\r";

                Notification += $"\r";
                _teamOne = _playerQue.OrderBy(x => random.Next()).Take(4).ToList();

                foreach(var player in _teamOne)
                {
                    _playerQue.Remove(player);
                }

                _teamTwo = _playerQue.OrderBy(x => random.Next()).Take(4).ToList();
                Notification += $"TEAM 1:\r";

                foreach(var player in _teamOne)
                {
                    Notification += $"{player.User.Mention} // Activision Id: {player.ActivisionId}\r";
                }

                Notification += $"TEAM 2:\r";
                foreach (var player in _teamTwo)
                {
                    Notification += $"{player.User.Mention} // Activision Id: {player.ActivisionId}\r";
                }

                _teamOne.Clear();
                _teamTwo.Clear();
                _playerQue.Clear();
            }
            else
            {
                Notification = $"There are currently: {_playerQue.Count}/8 player(s) queue'd for random 8s.\r\r" +
                    $"You will be mentioned when all 8 players are queue'd.";
            }
        }

        public void JoinScrimQueue(SocketUser user, string activisionId, string scrimTime, string notes)
        {
            _scrimId++;
            _scrimQue.Add(new ScrimModel(_scrimId, user, activisionId, scrimTime, notes));

            Notification = $"You have joined the team scrim queue.\r" +
                $"Scrim Id: {_scrimId}\r" +
                $"Hosts Activision Id: {activisionId}\r" +
                $"Discord: {user.Mention}\r" +
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
                Notification = $"{user.Mention} You have accepted the scrim.\r" +
                    $"Please message {removeScrim.DiscordUser} or add Activision Id: {removeScrim.ActivisonId}";
                _scrimQue.Remove(removeScrim);
            }
        }

        public void LeaveScrimQueue()
        {

        }

        public void GetScrimQueueStatus()
        {
            Notification = $"There are currently {_scrimQue.Count} team scrims in queue.\r\r";

            foreach(var scrim in _scrimQue)
            {
                Notification += $"Scrim Id: {scrim.Id}\r" +
                    $"Hosts Discord: {scrim.DiscordUser}\r" +
                    $"Scrim Time: {scrim.ScrimTime}\r" +
                    $"Notes: {scrim.Notes}";
            }
        }

        //League Staff Only
        public void ClearScrimQueue()
        {
            _scrimQue.Clear();
            Notification = $"You have cleared the scrim queue.\r" +
                $"Scrim queue count = {_scrimQue.Count}";
        }
    }
}
