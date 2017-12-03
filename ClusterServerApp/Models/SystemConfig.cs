using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClusterServerApp.Models
{
    public class SystemConfig
    {
        public int MaxProcesses { get; set; }
        public int ProcessesRunning { get; set; }
    }
}