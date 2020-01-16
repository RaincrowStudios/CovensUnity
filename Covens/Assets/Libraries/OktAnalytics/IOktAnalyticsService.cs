using Oktagon.Utils;
using System.Collections.Generic;

namespace Oktagon.Analytics
{
    public interface IOktAnalyticsService
    {
        bool Initialized { get; }
        void Initialize(IOktConfigFileReader configFileReader);
        void PushEvent(string eventName, Dictionary<string, object> eventParams);
        string GetName();
    }
}