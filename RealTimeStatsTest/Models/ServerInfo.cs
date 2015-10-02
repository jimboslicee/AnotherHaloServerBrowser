using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeStatsTest.Models
{
    public class ServerInfo
    {
        public string name { get; set; }
        public int port { get; set; }
        public string hostPlayer { get; set; }
        public string map { get; set; }
        public string mapFile { get; set; }
        public string variant { get; set; }
        public string variantType { get; set; }
        public string status { get; set; }
        public int numPlayers { get; set; }
        public int maxPlayers { get; set; }
        public string xnkid { get; set; }   
        public string xnadr { get; set; }
        public bool passworded { get; set; }
        public string gameVersion { get; set; }
        public string eldewritoVersion { get; set; }

        public List<PlayerInfo> players { get; set; }

        [JsonIgnore]
        public bool isActive { get; set; }
        [JsonIgnore]
        public DateTime lastUpdated { get; set; }


        /*Because there are some odd gametypes that are teams I dont know*/
        public bool normalTeamGame { get; set; }

        public ServerInfo()
        {
            //port can never be 0 sooo we'll use this as unresponsive server!
            port = 0;
            numPlayers = -1;
            maxPlayers = -1;
            normalTeamGame = false;
            isActive = true;
        }

        public void Update(ServerInfo newSI)
        {

            if (newSI.players != null)
            {
                players = newSI.players;
            }
            //this will change more frequently
            this.numPlayers = newSI.numPlayers;
            //this will change often
            map = newSI.map;
            this.mapFile = newSI.mapFile;
            this.normalTeamGame = newSI.normalTeamGame;
            this.variant = newSI.variant;
            this.variantType = newSI.variantType;
            this.status = newSI.status;
            //these will probably not change often between the same servers, unless it becomes passworded
            this.maxPlayers = newSI.maxPlayers;
            this.xnkid = newSI.xnkid;
            this.xnadr = newSI.xnadr;
            this.passworded = newSI.passworded;
            name = newSI.name;
            port = newSI.port;
            hostPlayer = newSI.hostPlayer;
            this.eldewritoVersion = newSI.eldewritoVersion;
            this.gameVersion = newSI.gameVersion;
            this.isActive = newSI.isActive;
        }
    }
}
