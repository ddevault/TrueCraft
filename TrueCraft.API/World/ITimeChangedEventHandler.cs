using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.World
{
    public delegate void TimeChangedEventHandler(object sender, EventArgs e);
    public class TimeChangedEventArgs : EventArgs
    {
        public long Time { get; set; }
    }
}
