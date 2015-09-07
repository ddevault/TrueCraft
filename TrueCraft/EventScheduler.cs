using System;
using TrueCraft.API.Server;
using System.Collections.Generic;
using TrueCraft.API;
using System.Diagnostics;
using TrueCraft.Profiling;

namespace TrueCraft
{
    public class EventScheduler : IEventScheduler
    {
        private IList<ScheduledEvent> Events { get; set; } // Sorted
        private readonly object EventLock = new object();
        private IMultiplayerServer Server { get; set; }
        private HashSet<IEventSubject> Subjects { get; set; }
        private Stopwatch Stopwatch { get; set; }

        public EventScheduler(IMultiplayerServer server)
        {
            Events = new List<ScheduledEvent>();
            Server = server;
            Subjects = new HashSet<IEventSubject>();
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public void ScheduleEvent(string name, IEventSubject subject, TimeSpan when, Action<IMultiplayerServer> action)
        {
            lock (EventLock)
            {
                long _when = Stopwatch.ElapsedTicks + when.Ticks;
                if (!Subjects.Contains(subject))
                {
                    Subjects.Add(subject);
                    subject.Disposed += Subject_Disposed;
                }
                int i;
                for (i = 0; i < Events.Count; i++)
                {
                    if (Events[i].When > _when)
                        break;
                }
                Events.Insert(i, new ScheduledEvent
                {
                    Name = name,
                    Subject = subject,
                    When = _when,
                    Action = action
                });
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
            Profiler.Start("scheduler");
            lock (EventLock)
            {
                var start = Stopwatch.ElapsedTicks;
                for (int i = 0; i < Events.Count; i++)
                {
                    var e = Events[i];
                    if (e.When <= start)
                    {
                        Profiler.Start("scheduler." + e.Name);
                        e.Action(Server);
                        Events.RemoveAt(i);
                        i--;
                        Profiler.Done();
                    }
                    if (e.When > start)
                        break; // List is sorted, we can exit early
                    if (start > Stopwatch.ElapsedTicks + 200000)
                        break; // We're falling behind
                }
            }
            Profiler.Done(20);
        }

        private struct ScheduledEvent
        {
            public long When;
            public Action<IMultiplayerServer> Action;
            public IEventSubject Subject;
            public string Name;
        }
    }
}