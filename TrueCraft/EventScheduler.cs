using System;
using TrueCraft.API.Server;
using System.Collections.Generic;
using TrueCraft.API;

namespace TrueCraft
{
    public class EventScheduler : IEventScheduler
    {
        private IList<ScheduledEvent> Events { get; set; } // Sorted
        private readonly object EventLock = new object();
        private IMultiplayerServer Server { get; set; }
        private HashSet<IEventSubject> Subjects { get; set; }

        public EventScheduler(IMultiplayerServer server)
        {
            Events = new List<ScheduledEvent>();
            Server = server;
            Subjects = new HashSet<IEventSubject>();
        }

        public void ScheduleEvent(IEventSubject subject, DateTime when, Action<IMultiplayerServer> action)
        {
            lock (EventLock)
            {
                if (!Subjects.Contains(subject))
                {
                    Subjects.Add(subject);
                    subject.Disposed += Subject_Disposed;
                }
                int i;
                for (i = 0; i < Events.Count; i++)
                {
                    if (Events[i].When > when)
                        break;
                }
                Events.Insert(i, new ScheduledEvent { Subject = subject, When = when, Action = action });
            }
        }

        void Subject_Disposed(object sender, EventArgs e)
        {
            // Cancel all events with this subject
            lock (EventLock)
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    if (Events[i].Subject == sender)
                    {
                        Events.RemoveAt(i);
                        i--;
                    }
                }
                Subjects.Remove((IEventSubject)sender);
            }
        }

        public void Update()
        {
            lock (EventLock)
            {
                var start = DateTime.UtcNow;
                for (int i = 0; i < Events.Count; i++)
                {
                    var e = Events[i];
                    if (e.When <= start)
                    {
                        e.Action(Server);
                        Events.RemoveAt(i);
                        i--;
                    }
                    if (e.When > start)
                        break; // List is sorted, we can exit early
                }
            }
        }

        private struct ScheduledEvent
        {
            public DateTime When;
            public Action<IMultiplayerServer> Action;
            public IEventSubject Subject;
        }
    }
}