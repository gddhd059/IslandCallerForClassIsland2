using ClassIsland.Shared.Enums;
using IslandCallerForClassIsland2.Views.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IslandCallerForClassIsland2.Models
{
    public class Status
    {
        public static Status Instance { get; set; } = new Status();
        public Window fluenthover = new HoverFluent();
        public TimeState lessonstatu;
    }
}
