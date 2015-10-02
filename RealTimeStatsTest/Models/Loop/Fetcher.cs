using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using RealTimeStatsTest.Models;
using RealTimeStatsTest.Models.Ext;

namespace RealTimeStatsTest.Models.Loop
{
    internal class Fetcher
    {
        private static readonly Lazy<Fetcher> _instance =
            new Lazy<Fetcher>(() => new Fetcher());

        private readonly TimeSpan BroadcastInterval;
        private Timer _broadcastLoop;
        private static CacheItemPolicy cip;
        private static List<string> lastServers;
        public Fetcher()
        {
            lastServers = new List<string>();
            BroadcastInterval = TimeSpan.FromMilliseconds(1000);
            cip = new CacheItemPolicy();
            cip.AbsoluteExpiration = DateTimeOffset.Now.AddHours(12);

            _broadcastLoop = new Timer(
                Updater,
                null,
                BroadcastInterval,
                BroadcastInterval);
            Debug.WriteLine("Broadcast loop started at interval: {0} seconds.", BroadcastInterval.Seconds);
        }

        private void Updater(object state)
        {
            List<string> latestServerList = (List<string>) CacheHandler.GetFromCache("servers");
            if (latestServerList != null)
            {
                foreach (string server in latestServerList)
                {
                    ServerInfo si = ResponseHandler.GetServerInfoResponse(server);
                    CacheHandler.AddServerToCache(server, si, cip);
                }
            }
            //get latest servers from memcache
            //foreach server get their 
            //declare some stuff 

            //List<string> newestServers = (List<string>) CacheHandler.GetFromCache("servers");
            //List<string> diff = new List<string>();
            //const double timeout = 1000;
            ////Get the fresh set
            ////AnnouncedListObj latestALO = ResponseHandler.GetAnnounceObjectAuto(timeout);

            //if (newestServers != null &&
            //    newestServers.Count != 0)
            //{

            //    newestServers = latestALO.result.servers;

            //    //compare list server to the one in MemoryCache (or add it if its the first time)
            //    ObjectCache oc = MemoryCache.Default;
            //    lastServers = (List<string>)oc.AddOrGetExisting("servers", newestServers, cip);

            //    if (lastServers == null)
            //    {
            //        lastServers = new List<string>();
            //        Debug.WriteLine("New Server List was added");
            //    }
            //    diff = lastServers.Except(newestServers).ToList();
            //    foreach (string server in diff)
            //    {
            //        CacheHandler.SetInvalidInCache(server);
            //    }

            //    foreach (string server in newestServers)
            //    {
            //        ServerInfo si = latestALO.result.serverInfos[server];
            //        if (si.players != null)
            //        {
            //            for (int i = si.players.Count - 1; i >= 0; i--)
            //            {
            //                PlayerInfo pi = si.players[i];
            //                if (IsNotValidPlayer(pi))
            //                {
            //                    si.players.RemoveAt(i);
            //                }
            //            }

            //        }

            //        CacheHandler.AddToCache(server, si, cip);
            //    }
            //    oc.Set("servers", newestServers, cip);
            //}
        }

     
        private const string nonPlayerUID = "0000000000000000";
        //better to do this while the player names are set
        public static bool IsNotValidPlayer(PlayerInfo pi)
        {
            return  String.IsNullOrEmpty(pi.name) && pi.uid.Equals(nonPlayerUID);
        }

        public static Fetcher Instance
        {
            get
            {
                return _instance.Value;
            }
        }
    }
}
