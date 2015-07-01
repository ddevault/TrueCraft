using System;

namespace TrueCraft.API.Server
{
    public interface IEventScheduler
    {
        /// <summary>
        /// Schedules an event to occur some time in the future.
        /// </summary>
        /// <param name="subject">The subject of the event. If the subject is disposed, the event is cancelled.</param>
        /// <param name="when">When to trigger the event.</param>
        /// <param name="action">The event to trigger.</param>
        void ScheduleEvent(IEventSubject subject, DateTime when, Action<IMultiplayerServer> action);
        /// <summary>
        /// Triggers all pending scheduled events whose scheduled time has transpired.
        /// </summary>
        void Update();
    }
}