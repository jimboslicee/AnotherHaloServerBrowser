using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RealTimeStatsTest.Models;
namespace RealTimeStatsTest.Models.Ext
{
    public static class CacheHandler
    {
        private static ObjectCache oc = MemoryCache.Default;

        public static object AddToCache(string key, object value, CacheItemPolicy cip)
        {
            return oc.AddOrGetExisting(key, value, cip);
        }
        public static void AddServerToCache(string server, ServerInfo si, CacheItemPolicy cip)
        {
            if (si != null && si.port != 0)
            {
               
                //ServerInfo si = alo.result.serverInfos[server];
               
                if (si.players != null)
                {
                    if (si.players.Count > 2)
                    {
                        si.normalTeamGame = checkIfTeamGame(si);
                    }
                    
                    if (si.normalTeamGame)
                    {
                        si.players.Sort(PlayersSort.TeamSort);

                    }
                    else
                    {
                        si.players.Sort(PlayersSort.ScoreSort);
                    }
                    
                }
                oc.Set(server, si, cip);
            } //or maxplayers == -1 or numPlayers, etc
            else
            {
                Debug.WriteLine("[{0}] {1} \t No response", DateTime.Now, server);
            }

            //string id = Ext.CreateReadableID(server, si.hostPlayer);
            //context.Clients.All.addServer(id, server, si);
        }

        public static object GetFromCache(string key)
        {
            return oc.Get(key);
        }
        public static void SetInvalidInCache(string server)
        {
            ObjectCache oc = MemoryCache.Default;
            ServerInfo si = (ServerInfo)oc.Get(server);
            if (si != null)
            {
                Debug.WriteLine("[{0}] {1} \t has been off'd", DateTime.Now, server);
                si.isActive = false;
            }
            // string id = Ext.CreateReadableID(server, si.hostPlayer);
            //context.Clients.All.removeServer(id);

        }

        public static ServerInfo GetServerFromCache(string server)
        {
            ObjectCache oc = MemoryCache.Default;
            return (ServerInfo)oc.Get(server);
        }
        private static bool checkIfTeamGame(ServerInfo si)
        {
            foreach (PlayerInfo pi in si.players)
            {
                if (pi.team > 1)
                {
                    return false;
                }
            }
            return true;
        }
       
    }
}
