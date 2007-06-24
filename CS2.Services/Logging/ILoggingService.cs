using Castle.Core.Logging;

namespace CS2.Services.Logging
{
    public interface ILoggingService
    {
        ILogger Logger { get; set; }
    }
}