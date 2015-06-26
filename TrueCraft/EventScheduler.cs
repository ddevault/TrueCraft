using System;
using TrueCraft.API.Server;
using System.Collections.Generic;

namespace TrueCraft
{
    public class EventScheduler : IEventScheduler
    {
        // TODO: This could be done more efficiently if the list were kept sorted
        
        private IList<ScheduledEvent> Events { get; set; }
        private object EventLock = new object();
        private IMultiplayerServer Server { get; set; }

        public EventScheduler(IMultiplayerServer server)
        {
            Events = new List<ScheduledEvent>();
            Server = server;
        }

        public void ScheduleEvent(DateTime when, Action<IMultiplayerServer> action)
        {
            lock (EventLock)
            {
                Events.Add(new ScheduledEvent { When = when, Action = action });
            }
        }

        public void Update()
        {
            lock (EventLock)
            {
                var start = DateTime.Now;
                for (int i = 0; i < Events.Count; i++)
                {
                    var e = Events[i];
                    if (e.When <= start)
                    {
                        e.Action(Server);
                        Events.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private struct ScheduledEvent
        {
            public DateTime When;
            public Action<IMultiplayerServer> Action;
        }
    }
}