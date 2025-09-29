using System;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JLogger
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JLogger
    {
        public static void Write(string log, int log_level = 0, params object[] args)
        {
            Console.WriteLine(string.Format(log, args));
        }
        public static void WriteWarning(string log, params object[] args)
        {
            Console.WriteLine(string.Format(log, args));
        }
        public static void WriteError(string log, params object[] args)
        {
            Console.WriteLine(string.Format(log, args));
        }
        public static void WriteFormat(string log, params object[] args)
        {
            Console.WriteLine(string.Format(log, args));
        }
    }
}
