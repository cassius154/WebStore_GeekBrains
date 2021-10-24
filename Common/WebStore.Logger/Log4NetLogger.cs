using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using log4net;
using Microsoft.Extensions.Logging;

namespace WebStore.Logger
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        public Log4NetLogger(string category, XmlElement configuration)
        {
            var logger_repository = LogManager
               .CreateRepository(
                    Assembly.GetEntryAssembly(),
                    typeof(log4net.Repository.Hierarchy.Hierarchy));

            _log = LogManager.GetLogger(logger_repository.Name, category);

            log4net.Config.XmlConfigurator.Configure(configuration);
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        //public bool IsEnabled(LogLevel level)
        //{
        //    switch (level)
        //    {
        //        default:
        //            throw new InvalidEnumArgumentException(nameof(level), (int)level, typeof(LogLevel));

        //        case LogLevel.None:
        //            return false;

        //        case LogLevel.Trace:
        //            return _Log.IsDebugEnabled;

        //        case LogLevel.Debug:
        //            return _Log.IsDebugEnabled;

        //        case LogLevel.Information:
        //            return _Log.IsInfoEnabled;

        //        case LogLevel.Warning:
        //            return _Log.IsWarnEnabled;

        //        case LogLevel.Error:
        //            return _Log.IsErrorEnabled;

        //        case LogLevel.Critical:
        //            return _Log.IsFatalEnabled;
        //    }
        //}

        public bool IsEnabled(LogLevel level) =>
            level switch
            {
                LogLevel.None => false,
                LogLevel.Trace => _log.IsDebugEnabled,
                LogLevel.Debug => _log.IsDebugEnabled,
                LogLevel.Information => _log.IsInfoEnabled,
                LogLevel.Warning => _log.IsWarnEnabled,
                LogLevel.Error => _log.IsErrorEnabled,
                LogLevel.Critical => _log.IsFatalEnabled,
                _ => throw new InvalidEnumArgumentException(nameof(level), (int)level, typeof(LogLevel))
            };

        public void Log<TState>(
            LogLevel Level,
            EventId Id,
            TState State,
            Exception Error,
            Func<TState, Exception, string> Formatter)
        {
            if (Formatter is null)
                throw new ArgumentNullException(nameof(Formatter));

            if (!IsEnabled(Level))
                return;

            var log_string = Formatter(State, Error);
            if (string.IsNullOrEmpty(log_string) && Error is null)
                return;

            switch (Level)
            {
                default:
                    throw new InvalidEnumArgumentException(nameof(Level), (int)Level, typeof(LogLevel));

                case LogLevel.None:
                    break;

                case LogLevel.Trace:
                case LogLevel.Debug:
                    _log.Debug(log_string);
                    break;

                case LogLevel.Information:
                    _log.Info(log_string);
                    break;

                case LogLevel.Warning:
                    _log.Warn(log_string);
                    break;

                case LogLevel.Error:
                    _log.Error(log_string, Error);
                    break;

                case LogLevel.Critical:
                    _log.Fatal(log_string, Error);
                    break;
            }
        }
    }
}
