using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeStatsTest.Models
{
    public class PlayerInfo
    {
        public string name { get; set; }
        public int score { get; set; }
        public int kills { get; set; }
        public int assists { get; set; }
        public int deaths { get; set; }
        public bool isAlive { get; set; }
        public int team { get; set; }
        public string uid { get; set; }

    }
}
