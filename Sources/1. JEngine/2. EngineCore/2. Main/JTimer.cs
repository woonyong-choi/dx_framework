using System;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JTimer
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JTimer
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [현재 Tick]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long GetCurrentTick(bool useRuntimeTimer = false)
        {
            if (useRuntimeTimer)
                return DateTime.Now.Ticks;
            return _updateTimer.Ticks;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static DateTime GetCurrentTime(bool useRuntimeTimer = false)
        {
            if (useRuntimeTimer)
                return DateTime.Now;
            return _updateTimer;
        }
        #endregion

        #region [시간 변환]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float TickToTime(long tick)
        {
            return (float)TimeSpan.FromTicks(tick).TotalSeconds;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long TimeToTick(float time)
        {
            return TimeSpan.FromSeconds(time).Ticks;
        }
        #endregion

        #region [누적 시간]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static long GetElaspedTick(long start_tick)
        {
            return GetCurrentTick() - start_tick;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float GetElaspedTime(long start_tick)
        {
            return TickToTime(GetCurrentTick() - start_tick);
        }
        #endregion

        #region [남은 시간]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static float GetRemainTime(long start_tick, float duration)
        {
            return Math.Max(0, duration - GetElaspedTime(start_tick));
        }
        #endregion

        #region [범위 검사]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool CheckRange(long start_tick, long end_tick, long cur_tick)
        {
            return ((cur_tick >= start_tick) && (cur_tick <= end_tick));
        }

        internal static float GetElaspedTime(object network_owner_timer)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region [일/주/월 검사]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSameDay(long tick1, long tick2)
        {
            var dt1 = new DateTime(tick1);
            var dt2 = new DateTime(tick2);
            return (dt1.DayOfYear == dt2.DayOfYear);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSameWeek(long tick1, long tick2)
        {
            var dt1 = new DateTime(tick1);
            var dt2 = new DateTime(tick2);
            return (dt1.DayOfWeek == dt2.DayOfWeek);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsSameMonth(long tick1, long tick2)
        {
            var dt1 = new DateTime(tick1);
            var dt2 = new DateTime(tick2);
            return (dt1.Month == dt2.Month);
        }

        public static long AddDay(long tick1, int day)
        {
            var dt1 = new DateTime(tick1);
            dt1 = dt1.AddDays(day);
            return dt1.Ticks;
        }
        #endregion

        #region [Sleep] Idle Time
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void SleepIdleTime(DateTime start_time, int max_sleep_time = 5)
        {
            var elapsedTime = (JTimer.GetCurrentTime(true) - start_time);
            var sleepTIme = Math.Min(max_sleep_time - elapsedTime.Milliseconds, max_sleep_time);
            if (sleepTIme > 0)
                System.Threading.Thread.Sleep(sleepTIme);
        }
        #endregion

        #region 문자열 변환
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string ToString(long tick)
        {
            var span = new TimeSpan(tick);
            return String.Format("{0}:{1}:{2}", span.Hours, span.Minutes, span.Seconds);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 업데이트 타이머
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [업데이트 타이머] CurrentUpdateTime/DeltaTime
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static DateTime _updateTimer;
        public static long CurrentUpdateTick { get; private set; }
        public static float CurrentUpdateTime { get { return TickToTime(CurrentUpdateTick); } }
        public static float DeltaTime { get; private set; }


        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Update()
        {
            //  (DateTime.Now가 속도가 느림. 프레임당 한번만 호출)
            _updateTimer = DateTime.Now;

            if (0 == CurrentUpdateTick)
                CurrentUpdateTick = GetCurrentTick(true);
            DeltaTime = GetElaspedTime(CurrentUpdateTick);
            CurrentUpdateTick = GetCurrentTick(true);
        }
        #endregion


    }

}
