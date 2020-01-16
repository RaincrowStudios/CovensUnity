using Oktagon.Utils;

namespace Oktagon.Analytics
{
    public interface IOktAnalyticsService
    {
        bool Initialized { get; }
        void Initialize(IOktConfigFileReader configFileReader);
    }
}