﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Services
{
    public interface IPresenceHandler
    {
        Task InitializeAsync();
    }
}
