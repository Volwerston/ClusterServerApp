using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClusterServerApp
{
    public class AppHub : Hub
    {
        public void ShowProgress(int percent)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<AppHub>();
            context.Clients.All.showProgress(percent);
        }
    }
}