using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeStatsTest.Models
{
    public class AnnouncedListObj
    {
        [JsonProperty("listVersion")]
        public int listVersion { get; set; }

        [JsonProperty("result")]
        public Result result { get; set; }
    }
}
