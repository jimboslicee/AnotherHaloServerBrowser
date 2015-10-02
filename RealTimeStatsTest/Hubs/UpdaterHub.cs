using System;
using Microsoft.AspNet.SignalR;
using RealTimeStatsTest.Models.Loop;
using System.Diagnostics;

namespace RealTimeStatsTest.Hubs
{
    public class UpdaterHub: Hub
    {
        private static BroadcastLoop _broadcastLoop;

        public override System.Threading.Tasks.Task OnConnected()
        {

            return base.OnConnected();
        }
        public UpdaterHub()
        {
            Debug.WriteLine("[{0}] 2nd UpdaterHub starting...", DateTime.Now);
           
            _broadcastLoop = new BroadcastLoop();
            
        }

        public void Hello()
        {
            Debug.WriteLine("hallo mate");
        }
    }
}
