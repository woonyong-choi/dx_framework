#define USE_LOG4NET

using System;
#if USE_LOG4NET

using log4net.Appender;
using log4net.Core;
using log4net.Config;
using System.IO;
using log4net;
using log4net.Layout;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JLog
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class JLog
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Base

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Log
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static ILog _log;
        public static ILog Log
        {
            get
            {
                if (null == _log)
                {
                    ConfigureAllLogging();
                    _log = LogManager.GetLogger("JLogger");
                }
                return _log;
            }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Static 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static JLog()
        {
        }
        #endregion

        #region [Log] Static 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Debug(object message) { Log.Debug(message); }
        public static void DebugFormat(string format, params object[] args) { Log.DebugFormat(format, args); }
        public static void Error(object message) { Log.Error(message); }
        public static void ErrorFormat(string format, params object[] args) { Log.ErrorFormat(format, args); }
        public static void Fatal(object message) { Log.Fatal(message); }
        public static void FatalFormat(string format, params object[] args) { Log.FatalFormat(format, args); }
        public static void Info(object message) { Log.Info(message); }
        public static void InfoFormat(string format, params object[] args) { Log.InfoFormat(format, args); }
        public static void Warn(object message) { Log.Warn(message); }
        public static void WarnFormat(string format, params object[] args) { Log.WarnFormat(format, args); }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 내부 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] ConfigureAllLogging
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static void ConfigureAllLogging()
        {
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();

            // setup the appender that writes to Log\EventLog.txt
            var now = DateTime.Now;
            var fileAppender = new RollingFileAppender
            {
                AppendToFile = true,
                File = Path.Combine(Environment.CurrentDirectory, string.Format("Logs/{0}/{1}-{2}.log", now.Year, now.Month, now.Day)),
                Layout = patternLayout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = false
            };
            fileAppender.ActivateOptions();

            var unityLogger = new UnityAppender
            {
                Layout = new PatternLayout()
            };
            unityLogger.ActivateOptions();

            BasicConfigurator.Configure(unityLogger, fileAppender);
        }
        #endregion

    }

    #region [Class] UnityAppender
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class UnityAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            string message = RenderLoggingEvent(loggingEvent);

            if (Level.Compare(loggingEvent.Level, Level.Error) >= 0)
            {
                // everything above or equal to error is an error
                JLogger.WriteError(message);
            }
            else if (Level.Compare(loggingEvent.Level, Level.Warn) >= 0)
            {
                // everything that is a warning up to error is logged as warning
                JLogger.WriteError(message);
            }
            else
            {
                // everything else we'll just log normally
                JLogger.WriteError(message);
            }
        }
    }
    #endregion

}
#endif

