using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Threading;
using Microsoft.AspNet.SignalR;
using RealTimeStatsTest.Hubs;
using RealTimeStatsTest.Models.Ext;

namespace RealTimeStatsTest.Models.Loop
{
    public class ServerListFetcher
    {
        private static readonly Lazy<ServerListFetcher> _instance =
          new Lazy<ServerListFetcher>(() => new ServerListFetcher());

        private IHubContext context;
        private readonly TimeSpan BroadcastInterval;
        private readonly TimeSpan DelayInterval;
        private Timer _broadcastLoop;
        private static CacheItemPolicy cip;
        public  static ServerListFetcher Instance 
        {
            get { return _instance.Value; }
        }
        public ServerListFetcher()
        {
            BroadcastInterval = TimeSpan.FromMilliseconds(30000);
            DelayInterval = TimeSpan.FromMilliseconds(0);
            cip = new CacheItemPolicy();
            cip.AbsoluteExpiration = DateTimeOffset.Now.AddHours(12);
            context = GlobalHost.ConnectionManager.GetHubContext<UpdaterHub>();
             _broadcastLoop = new Timer(
                ServerListUpdater,
                null,
                DelayInterval,
                BroadcastInterval);
            Debug.WriteLine("Server Updater loop started at interval: {0} seconds.", BroadcastInterval.Seconds);
        }

        public void ServerListUpdater(object state)
        {
            //get the server list
            AnnouncedListObj alo = ResponseHandler.GetALOWithoutServerInfo(1000);
            // get the one from memcache (or add if non existant)
            List<string> prevServerList = (List<string>)CacheHandler.AddToCache("servers", alo.result.servers,cip);
            //null means a previous list did not exist and has been added
            if (prevServerList == null)
            {
                //initialize a new if null, so we can do some operations
                prevServerList = new List<string>();
            }

            //diff contains removed servers, just remove em
            List<string> diff = prevServerList.Except(alo.result.servers).ToList();
            foreach (string server in diff)
            {
                CacheHandler.SetInvalidInCache(server);
                string id = Ext.Ext.CreateReadableID(server);
                context.Clients.All.removeServer(id);

            }
            
            //compare list server to the one in MemoryCache (or add it if its the first time)
            //ObjectCache oc = MemoryCache.Default;
            //lastServers = (List<string>)oc.AddOrGetExisting("servers", newestServers, cip);

            //if (lastServers == null)
            //{
            //    lastServers = new List<string>();
            //    Debug.WriteLine("New Server List was added");
            //}
            //diff = lastServers.Except(newestServers).ToList();
            //foreach (string server in diff)
            //{
            //    CacheHandler.SetInvalidInCache(server);
            //}

            //foreach (string server in newestServers)
            //{
            //    ServerInfo si = latestALO.result.serverInfos[server];
            //    if (si.players != null)
            //    {
            //        for (int i = si.players.Count - 1; i >= 0; i--)
            //        {
            //            PlayerInfo pi = si.players[i];
            //            if (IsNotValidPlayer(pi))
            //            {
            //                si.players.RemoveAt(i);
            //            }
            //        }

            //    }

            //    CacheHandler.AddToCache(server, si, cip);
            //}
            //oc.Set("servers", newestServers, cip);

        }
    }
}