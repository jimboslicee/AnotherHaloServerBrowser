using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using RealTimeStatsTest.Models.Ext;

namespace RealTimeStatsTest.Models.Loop
{
    public class BroadcastLoop
    {
        private static readonly Lazy<BroadcastLoop> _instance = new Lazy<BroadcastLoop>(() => new BroadcastLoop());

        private readonly TimeSpan BroadcastInterval;
        private readonly TimeSpan BroadcastHeaderInterval;
        private static List<string> lastServerList;
        private static Timer _broadcastTimer;
        private static Timer _broadcastHeaderTimer;
        private IHubContext context;
        private static int killCount = 0;
            

        public BroadcastLoop()
        {
            BroadcastInterval = TimeSpan.FromMilliseconds(1000);
            BroadcastHeaderInterval = TimeSpan.FromMilliseconds(1500);
            lastServerList = new List<string>();
            context = GlobalHost.ConnectionManager.GetHubContext<Hubs.UpdaterHub>();
            BroadcastInterval = TimeSpan.FromMilliseconds(1000);
            _broadcastTimer = new Timer(Updater,
                null,
                BroadcastInterval,
                BroadcastInterval);
            lastServerList = new List<string>();

            _broadcastHeaderTimer = new Timer(HeaderUpdater,
                null,
                BroadcastHeaderInterval,
                BroadcastHeaderInterval
                );
        }

        public void Updater(object state)
        {
            List<string> currentCachedServerList = (List<string>)CacheHandler.GetFromCache("servers");
            List<string> diff = new List<string>();

            //int diffRes = Ext.DiffCompare(lastServerList,cachedServerList);

            /*
             * OK so you thought you were clever but notreally
             * Just change it back to the way it was
             * add another Updater that would send updates to clients (not add)
             * 
             * TODO:
             * 
             * get diff results of list (and its count)
             * and do accordingly:
             * AddServer to clients
             * RemoveServer to clients
             * 
             *btw what happens if a server ends up like this
             *
             * t1 = {1, 2, 3, 4, 5, 6} - 6
             * t2 == {2, 3, 4, 5, 6, 7, 8} - 7
             * 
             * diff result is 1, 7, 8 add these servers...but 1 is removed!
             * 
             * or 
             * 
             * t1 = {1, 2, 3, 4, 5, 6} 
             * t2 = {7, 8, 9, 10, 11}
             * 
             * diff = {7, 8, 9, 10, 11} gets removed...but they exist!
             */

            //check if anything has been removed
            if (currentCachedServerList != null)
            {
                //diff = lastServerList.Except(currentCachedServerList).ToList();
                //foreach (string server in diff)
                //{
                //    ServerInfo si = (ServerInfo)CacheHandler.GetFromCache(server);
                //    string id = Ext.Ext.CreateReadableID(server);
                //    context.Clients.All.removeServer(id);
                //}
                //then just do all the newest list
                foreach (string server in currentCachedServerList)
                {
                    ServerInfo si = (ServerInfo)CacheHandler.GetFromCache(server);
                    if (si != null)
                    {
                        string id = Ext.Ext.CreateReadableID(server);
                        context.Clients.All.addOrUpdateServer(new { id = id, server = server, serverInfo = si });

                    }
                }


                lastServerList = currentCachedServerList;
            }

        }


        public void HeaderUpdater(object state)
        {
            ObjectCache oc = MemoryCache.Default;
            List<string> servers = (List<string>)oc.Get("servers");
            int playerSum = 0;
            int openServers = 0;
            if (servers != null)
            {
                
                foreach (string server in servers)
                {
                    ServerInfo si = CacheHandler.GetServerFromCache(server);
                    if (si != null)
                    {
                        playerSum += si.numPlayers;
                    }
                    
                        openServers++;
                    
                }
                context.Clients.All.updateHeaders(playerSum, openServers);
            }
        }

        public static BroadcastLoop Instance
        {
            get { return _instance.Value; }
        }



    }
}
