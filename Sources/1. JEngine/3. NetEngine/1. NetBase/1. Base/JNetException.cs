using System;
using System.Diagnostics;

namespace J2y.Network
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetException
    //		Exception thrown in the Network Library
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public sealed class JNetException : Exception
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// NetException constructor
        /// </summary>
        public JNetException()
            : base()
        {
        }

        /// <summary>
        /// NetException constructor
        /// </summary>
        public JNetException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// NetException constructor
        /// </summary>
        public JNetException(string message, Exception inner)
            : base(message, inner)
        {
        }
        #endregion

        #region [DEBUG] [Static] Assert
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Throws an exception, in DEBUG only, if first parameter is false
        /// </summary>
        [Conditional("DEBUG")]
        public static void Assert(bool isOk, string message)
        {
            if (!isOk)
                throw new JNetException(message);
        }

        /// <summary>
        /// Throws an exception, in DEBUG only, if first parameter is false
        /// </summary>
        [Conditional("DEBUG")]
        public static void Assert(bool isOk)
        {
            if (!isOk)
                throw new JNetException();
        }
        #endregion

    }
}
