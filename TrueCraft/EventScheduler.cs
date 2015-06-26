using System;
using TrueCraft.API.Server;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrueCraft.API.Logging;

namespace TrueCraft
{
    public class EventScheduler : IEventScheduler
    {
        private List<ScheduledEvent> Events { get; set; }
        private object EventLock = new object();
        private IMultiplayerServer Server { get; set; }

        private SemaphoreSlim Sem = new SemaphoreSlim(0, 1);

        private CancellationTokenSource Cancel;

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

            if (Sem.CurrentCount == 0)
                Sem.Release();
        }

        public void Start()
        {
            Cancel = new CancellationTokenSource();

            Thread scheduleThread = new Thread(Update);
            scheduleThread.IsBackground = true;

            scheduleThread.Start();
        }

        public void Stop()
        {
            Cancel.Cancel();
        }

        private void Update()
        {
            while (true)
            {
                if (Cancel.IsCancellationRequested)
                    break;

                try
                {
                    ScheduledEvent? nextEvent = null;
                    lock (EventLock)
                    {
                        var evts = Events.ToList();
                        evts.Sort();

                        DateTime now = DateTime.Now;
                        for (int i = 0; i < evts.Count; i++)
                        {
                            ScheduledEvent evt = evts[i];

                            if (evt.When < now)
                                break;

                            evts.RemoveAt(i);
                            i--;

                            evt.Action(Server);
                        }

                        if (evts.Count > 0)
                            nextEvent = evts.First();

                        Events = evts;
                    }

                    var tasks = new List<Task> { Sem.WaitAsync(Cancel.Token) };
                    if (nextEvent != null)
                    {
                        TimeSpan ts = nextEvent.Value.When - DateTime.Now;

                        if (ts < TimeSpan.Zero)
                            continue;

                        tasks.Add(Task.Delay(ts, Cancel.Token));
                    }
                    
                    Task.WhenAny(tasks).Wait();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Server.Log(LogCategory.Error, "Scheduler Error", ex.ToString());
                }
            }
        }

        private struct ScheduledEvent : IComparable<ScheduledEvent>
        {
            public DateTime When;
            public Action<IMultiplayerServer> Action;

            public int CompareTo(ScheduledEvent other)
            {
                if (When > other.When)
                    return 1;
                if (When == other.When)
                    return 0;

                return -1;
            }
        }
    }
}