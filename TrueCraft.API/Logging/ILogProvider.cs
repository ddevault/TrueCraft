using System;

namespace TrueCraft.API.Logging
{
    public interface ILogProvider
    {
        void Log(LogCategory category, string text, params object[] parameters);
    }
}