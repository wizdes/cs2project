using Castle.Core.Logging;

namespace CS2.Services
{
    public interface ILoggedService
    {
        ILogger Logger
        {
            get;
            set;
        }
    }
}