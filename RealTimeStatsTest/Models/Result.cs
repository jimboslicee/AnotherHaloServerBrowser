using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeStatsTest.Models
{
    public class Result
    {
        public int code { get; set; }
        public string msg { get; set; }
        [JsonIgnore]
        public Dictionary<string, ServerInfo> serverInfos { get; set; }
        public List<string> servers { get; set; }
    }
}
