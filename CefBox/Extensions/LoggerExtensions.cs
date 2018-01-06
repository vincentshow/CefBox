using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace CefBox.Extensions
{
    public static class LoggerExtensions
    {
        private static string Template([CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            return $"[{callerMember}_{Path.GetFileName(callerPath)}({callerLine})]";
        }

        public static void Warn(this ILogger logger, string logInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            if (GlobalConfig.LogLevel <= LogLevel.Warn)
            {
                logger.LogWarning($"{Template(callerMember, callerPath, callerLine)} {logInfo}");
            }
        }

        public static void Warn(this ILogger logger, Func<string> getLogInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            Warn(logger, getLogInfo(), callerMember, callerPath, callerLine);
        }

        public static void Warn(this ILogger logger, Exception ex, string logInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            Warn(logger, logInfo, callerMember, callerPath, callerLine);
            Warn(logger, ex.ToString(), callerMember, callerPath, callerLine);
        }

        public static void Trace(this ILogger logger, string logInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            if (GlobalConfig.LogLevel <= LogLevel.Trace)
            {
                logger.LogTrace($"{Template(callerMember, callerPath, callerLine)} {logInfo}");
            }
        }

        public static void Trace(this ILogger logger, Func<string> getLogInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            Trace(logger, getLogInfo(), callerMember, callerPath, callerLine);
        }

        public static void Info(this ILogger logger, string logInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            if (GlobalConfig.LogLevel <= LogLevel.Info)
            {
                logger.LogInformation($"{Template(callerMember, callerPath, callerLine)} {logInfo}");
            }
        }

        public static void Info(this ILogger logger, Func<string> getLogInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            Info(logger, getLogInfo(), callerMember, callerPath, callerLine);
        }

        public static void Debug(this ILogger logger, string logInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            if (GlobalConfig.LogLevel <= LogLevel.Debug)
            {
                logger.LogDebug($"{Template(callerMember, callerPath, callerLine)} {logInfo}");
            }
        }

        public static void Debug(this ILogger logger, Func<string> getLogInfo, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            Debug(logger, getLogInfo(), callerMember, callerPath, callerLine);
        }

        public static void Error(this ILogger logger, string logInfo, Exception ex = null, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            if (GlobalConfig.LogLevel <= LogLevel.Error)
            {
                if (ex != null)
                {
                    logInfo += $": {ex.ToString()}";
                }
                logger.LogError($"{Template(callerMember, callerPath, callerLine)} {logInfo}");
            }
        }

        public static void Error(this ILogger logger, Func<string> getLogInfo, Exception ex = null, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerPath = null, [CallerLineNumber] int callerLine = 0)
        {
            Error(logger, getLogInfo(), ex, callerMember, callerPath, callerLine);
        }
    }
}
