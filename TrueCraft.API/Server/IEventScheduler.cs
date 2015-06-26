using System;

namespace TrueCraft.API.Server
{
    public interface IEventScheduler
    {
        void ScheduleEvent(DateTime when, Action<IMultiplayerServer> action);
        void Start();
        void Stop();
    }
}