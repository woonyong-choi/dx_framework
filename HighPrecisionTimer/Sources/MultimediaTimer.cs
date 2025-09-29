using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HighPrecisionTimer
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // MultimediaTimer
    //
    // A timer based on the multimedia timer API with 1ms precision.
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class MultimediaTimer : IDisposable
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Setting
        private const int EventTypeSingle   = 0;
        private const int EventTypePeriodic = 1;
        #endregion

        #region [변수] TaskDone
        private static readonly Task TaskDone = Task.FromResult<object>(null);
        #endregion

        #region [변수] Base
        private bool _disposed           = false;
        private int _interval            = 0;
        private int _resolution          = 0;
        private volatile uint _timerId   = 0;
        #endregion

        #region [변수] Callback
        // Hold the timer callback to prevent garbage collection.
        private readonly MultimediaTimerCallback Callback;
        #endregion

        #region [변수] EventHandler
        public event EventHandler Elapsed;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // The period of the timer in milliseconds.
        public int Interval
        {
            get { return _interval; }
            set
            {
                CheckDisposed();

                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                _interval = value;
                if (Resolution > Interval)
                    Resolution = value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// The resolution of the timer in milliseconds. The minimum resolution is 0, meaning highest possible resolution.
        public int Resolution
        {
            get { return _resolution; }
            set
            {
                CheckDisposed();

                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                _resolution = value;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        /// Gets whether the timer has been started yet.
        public bool IsRunning
        {
            get { return _timerId != 0; }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public MultimediaTimer()
        {
            Callback = new MultimediaTimerCallback(TimerCallbackMethod);
            Resolution = 5;
            Interval = 10;
        }
        #endregion

        #region [정리] 소멸자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        ~MultimediaTimer()
        {
            Dispose(false);
        }
        #endregion

        #region [정리] Dispose
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            Dispose(true);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
            if (IsRunning)
            {
                Internal_Stop();
            }

            if (disposing)
            {
                Elapsed = null;
                GC.SuppressFinalize(this);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("MultimediaTimer");
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Timer
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Timer] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Task Delay(int millisecondsDelay, CancellationToken token = default(CancellationToken))
        {
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay", millisecondsDelay, "The value cannot be less than 0.");
            }

            if (millisecondsDelay == 0)
            {
                return TaskDone;
            }

            token.ThrowIfCancellationRequested();

            // allocate an object to hold the callback in the async state.
            object[] state = new object[1];
            var completionSource = new TaskCompletionSource<object>(state);
            MultimediaTimerCallback callback = (uint id, uint msg, ref uint uCtx, uint rsv1, uint rsv2) =>
            {
                // Note we don't need to kill the timer for one-off events.
                completionSource.TrySetResult(null);
            };

            state[0] = callback;
            UInt32 userCtx = 0;
            var timerId = NativeMethods.TimeSetEvent((uint)millisecondsDelay, (uint)0, callback, ref userCtx, EventTypeSingle);
            if (timerId == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return completionSource.Task;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void TimerCallbackMethod(uint id, uint msg, ref uint userCtx, uint rsv1, uint rsv2)
        {
            var handler = Elapsed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        #endregion

        #region [Timer] Start
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Start()
        {
            CheckDisposed();

            if (IsRunning)
                throw new InvalidOperationException("Timer is already running");

            // Event type = 0, one off event
            // Event type = 1, periodic event
            UInt32 userCtx = 0;
            _timerId = NativeMethods.TimeSetEvent((uint)Interval, (uint)Resolution, Callback, ref userCtx, EventTypePeriodic);
            if (_timerId == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }
        #endregion

        #region [Timer] Stop
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            CheckDisposed();

            if (!IsRunning)
                throw new InvalidOperationException("Timer has not been started");

            Internal_Stop();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Internal_Stop()
        {
            NativeMethods.TimeKillEvent(_timerId);
            _timerId = 0;
        }
        #endregion

    }

    internal delegate void MultimediaTimerCallback(UInt32 id, UInt32 msg, ref UInt32 userCtx, UInt32 rsv1, UInt32 rsv2);

    internal static class NativeMethods
    {
        [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeSetEvent")]
        internal static extern UInt32 TimeSetEvent(UInt32 msDelay, UInt32 msResolution, MultimediaTimerCallback callback, ref UInt32 userCtx, UInt32 eventType);

        [DllImport("winmm.dll", SetLastError = true, EntryPoint = "timeKillEvent")]
        internal static extern void TimeKillEvent(UInt32 uTimerId);
    }
}
