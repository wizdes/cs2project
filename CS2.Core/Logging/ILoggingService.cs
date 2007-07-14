using Castle.Core.Logging;

namespace CS2.Core.Logging
{
    public interface ILoggingService
    {
        ILogger Logger { get; set; }
    }
}