using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeStatsTest.Models.Ext
{
    public static  class PlayersSort
    {
        public static int TeamSort(PlayerInfo lpi, PlayerInfo rpi)
        {
            if (lpi.team < rpi.team)
            {
                return -1;
            }
            else if (lpi.team > rpi.team)
            {
                return 1;
            }
            else
            {
                return ScoreSort(lpi, rpi);
            }
        }

        public static int ScoreSort(PlayerInfo lpi, PlayerInfo rpi)
        {
            if (lpi.score > rpi.score)
            {
                return -1;
            }
            else if (lpi.score < rpi.score)
            {
                return 1;
            }
            else
            {
                return KillSort(lpi, rpi);
            }
        }
        private static int KillSort(PlayerInfo lpi, PlayerInfo rpi)
        {
            if (lpi.kills > rpi.kills)
            {
                return -1;
            }
            else if (lpi.kills < rpi.kills)
            {
                return 1;
            }
            else
            {
                return DeathSort(lpi, rpi);
            }
        }

        private static int DeathSort(PlayerInfo lpi, PlayerInfo rpi)
        {
            if (lpi.deaths < rpi.deaths)
            {
                return -1;
            }
            else if (lpi.deaths > rpi.deaths)
            {
                return 1;
            }
            else
            {
                return AssistsSort(lpi, rpi);
            }
        }

        private static int AssistsSort(PlayerInfo lpi, PlayerInfo rpi)
        {
            if (lpi.assists > rpi.assists)
            {
                return -1;
            }
            else if (lpi.assists < rpi.assists)
            {
                return 1;
            }
            else
            {
                return 0;//screw name sort
            }
        }
    }
}