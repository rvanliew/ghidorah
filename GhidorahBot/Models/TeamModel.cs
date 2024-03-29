﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhidorahBot.Models
{
    public class TeamModel
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string Twitter { get; set; }
        public string Group { get; set; }
        public string Captain { get; set; }
        public string Manager { get; set; }
        public string LastUpdated { get; set; }
        public string DateCreated { get; set; }
        public string CreatedBy { get; set; }

        public TeamModel(string id, string teamName, string twitter, string group, string captain, string manager, string lastUpdated, string dateCreated, string createdBy)
        {
            Id = id;
            TeamName = teamName;
            Twitter = twitter;
            Group = group;
            Captain = captain;
            Manager = manager;
            LastUpdated = lastUpdated;
            DateCreated = dateCreated;
            CreatedBy = createdBy;
        }
    }
}
