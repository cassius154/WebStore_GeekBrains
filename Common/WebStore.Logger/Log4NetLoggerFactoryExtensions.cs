using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;


namespace WebStore.Logger
{
    public static class Log4NetLoggerFactoryExtensions
    {
        private static string _checkFilePath(string filePath)
        {
            //if(FilePath is null || FilePath.Length == 0)
            if (filePath is not { Length: > 0 })
                throw new ArgumentException("Не указан путь к файлу");

            if (Path.IsPathRooted(filePath))
                return filePath;

            var assembly = Assembly.GetEntryAssembly();
            var dir = Path.GetDirectoryName(assembly!.Location);
            return Path.Combine(dir!, filePath);
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string configurationFile = "log4net.config")
        {
            factory.AddProvider(new Log4NetLoggerProvider(_checkFilePath(configurationFile)));
            return factory;
        }
    }
}
