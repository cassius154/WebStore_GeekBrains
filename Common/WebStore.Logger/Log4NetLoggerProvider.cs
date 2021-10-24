using System.Collections.Concurrent;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace WebStore.Logger
{
    public class Log4NetLoggerProvider : ILoggerProvider
    {
        private readonly string _сonfigurationFile;
        private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers = new();

        public Log4NetLoggerProvider(string сonfigurationFile) => _сonfigurationFile = сonfigurationFile;

        public ILogger CreateLogger(string сategory) =>
            _loggers.GetOrAdd(сategory, ctgr =>
            {
                var xml = new XmlDocument();
                xml.Load(_сonfigurationFile);
                return new Log4NetLogger(ctgr, xml["log4net"]);
            });

        public void Dispose() => _loggers.Clear();
    }
}
