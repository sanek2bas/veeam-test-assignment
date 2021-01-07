using NLog;

namespace GZipTest.Logs
{
    public class LoggerProvider
    {
        public static Logger Logger()
        {
            return LogManager.GetCurrentClassLogger();
        }
    }
}
