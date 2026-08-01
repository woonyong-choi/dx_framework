
using System;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JScheduledEvent
    //		Used by the JScheduler, the JScheduledEvent contains the delegate and arguments for the event that should execute at a time in the future.
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public abstract class JScheduledEvent : IPlayable
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [변수]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Playable
        protected JScheduler _scheduler;
        protected bool _pause = true;
        #endregion

        #region [변수] Timer
        protected float _duration = 0f;
        protected float _currentTime = 0f;
        protected float _deltaTime = 0f;
        protected float _timeScale = 1f;
        protected bool _repeat = false;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Property]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] Playable
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool IsPlay { get { return !_pause; } }
        public bool IsPause { get { return _pause; } }
        #endregion

        #region [Property] Timer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float Duration { get { return _duration; } set { _duration = value; } }
        public float CurrentTime { get { return _currentTime; } }
        public float RemainingTime { get { return _pause ? 0f : _duration - _currentTime; } }
        public float DeltaTime { get { return _deltaTime; } }
        public float TimeScale { get { return _timeScale; } }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Reset
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
            _pause = true;
            _repeat = false;
            _currentTime = 0f;
            _deltaTime = 0f;
            _timeScale = 1f;
        }
        public void Reset(float length)
        {
            Reset();
            _duration = length;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ResetTime()
        {
            _currentTime = 0f;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetTime(float time)
        {
            _currentTime = Mathf.Min(_duration, time);
        }
        #endregion

        #region [업데이트] 메인
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Update(float add_time)
        {
            if (_pause)
                return;

            _deltaTime = add_time * _timeScale;
            _currentTime += _deltaTime;
            if (_repeat)
            {
                if (_duration <= 0f)
                {
                    Execute();
                }
                else
                {
                    while (_currentTime >= _duration)
                    {
                        _currentTime -= _duration;
                        Execute();
                    }
                }
            }
            else if (_currentTime >= _duration)
            {
                Finish();
            }
        }
        #endregion

        #region [abstract] Execute
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract void Execute();
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // IPlayable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IPlayable] Play/Pause/Resume/Stop/Finish
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Play()
        {
            _repeat = false;

            if (_duration <= 0f)
                Finish();
            else
                updateScheduler(false);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Pause()
        {
            updateScheduler(true);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Resume()
        {
            updateScheduler(false);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Stop()
        {
            updateScheduler(true);
            _currentTime = 0f;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Finish()
        {
            updateScheduler(true);
            _deltaTime = _currentTime = 0f;
            Execute();
        }
        #endregion

        #region [Playable] Repeat
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Repeat()
        {
            _repeat = true;
            updateScheduler(false);
        }
        #endregion

        #region [Playable] Speed
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetPlaySpeed(float speed)
        {
            _timeScale = speed;
        }
        #endregion

        #region [구현] 스케줄러에 이벤트 등록 또는 삭제
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void updateScheduler(bool pause)
        {
            if (_pause == pause)
                return;

            _pause = pause;
            if (_pause)
            {
                _scheduler.RemoveEvents.Add(this);
            }
            else
            {
                if (_scheduler.RemoveEvents.Contains(this))
                    _scheduler.RemoveEvents.Remove(this);
                if (!_scheduler.ActiveEvents.Contains(this))
                    _scheduler.ActiveEvents.Add(this);
            }
        }
        #endregion
    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JScheduledEvent
    //		Used by the JScheduler, the JScheduledEvent contains the delegate and arguments for the event that should execute at a time in the future.
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JScheduledEventInst : JScheduledEvent
    {
        #region [JScheduledEvent] 구현
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Action Action { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEventInst(JScheduler parent, float delay, Action action)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f;
            Action = action;
        }
        public JScheduledEventInst(float delay, Action action)
            : this(JScheduler.Instance, delay, action)
        { }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void Execute() { Action?.Invoke(); }
        #endregion
    }

    #region [JScheduledEventInst] Overloading Class
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1> : JScheduledEvent
    {
        public Action<T1> Action { get; }
        public T1 Arg1 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1> action, T1 arg1)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1;
        }
        public JScheduledEventInst(float delay, Action<T1> action, T1 arg1)
            : this(JScheduler.Instance, delay, action, arg1)
        { }
        public override void Execute() { Action?.Invoke(Arg1); }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1, T2> : JScheduledEvent
    {
        public Action<T1, T2> Action { get; }
        public T1 Arg1 { get; }
        public T2 Arg2 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1; Arg2 = arg2;
        }
        public JScheduledEventInst(float delay, Action<T1, T2> action, T1 arg1, T2 arg2)
            : this(JScheduler.Instance, delay, action, arg1, arg2)
        { }
        public override void Execute() { Action?.Invoke(Arg1, Arg2); }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1, T2, T3> : JScheduledEvent
    {
        public Action<T1, T2, T3> Action { get; }
        public T1 Arg1 { get; }
        public T2 Arg2 { get; }
        public T3 Arg3 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1; Arg2 = arg2; Arg3 = arg3;
        }
        public JScheduledEventInst(float delay, Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
            : this(JScheduler.Instance, delay, action, arg1, arg2, arg3)
        { }
        public override void Execute() { Action?.Invoke(Arg1, Arg2, Arg3); }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1, T2, T3, T4> : JScheduledEvent
    {
        public Action<T1, T2, T3, T4> Action { get; }
        public T1 Arg1 { get; }
        public T2 Arg2 { get; }
        public T3 Arg3 { get; }
        public T4 Arg4 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1; Arg2 = arg2; Arg3 = arg3; Arg4 = arg4;
        }
        public JScheduledEventInst(float delay, Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            : this(JScheduler.Instance, delay, action, arg1, arg2, arg3, arg4)
        { }
        public override void Execute() { Action?.Invoke(Arg1, Arg2, Arg3, Arg4); }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1, T2, T3, T4, T5> : JScheduledEvent
    {
        public Action<T1, T2, T3, T4, T5> Action { get; }
        public T1 Arg1 { get; }
        public T2 Arg2 { get; }
        public T3 Arg3 { get; }
        public T4 Arg4 { get; }
        public T5 Arg5 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1; Arg2 = arg2; Arg3 = arg3; Arg4 = arg4; Arg5 = arg5;
        }
        public JScheduledEventInst(float delay, Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
            : this(JScheduler.Instance, delay, action, arg1, arg2, arg3, arg4, arg5)
        { }
        public override void Execute() { Action?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5); }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1, T2, T3, T4, T5, T6> : JScheduledEvent
    {
        public Action<T1, T2, T3, T4, T5, T6> Action { get; }
        public T1 Arg1 { get; }
        public T2 Arg2 { get; }
        public T3 Arg3 { get; }
        public T4 Arg4 { get; }
        public T5 Arg5 { get; }
        public T6 Arg6 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1; Arg2 = arg2; Arg3 = arg3; Arg4 = arg4; Arg5 = arg5; Arg6 = arg6;
        }
        public JScheduledEventInst(float delay, Action<T1, T2, T3, T4, T5, T6> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
            : this(JScheduler.Instance, delay, action, arg1, arg2, arg3, arg4, arg5, arg6)
        { }
        public override void Execute() { Action?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6); }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public class JScheduledEventInst<T1, T2, T3, T4, T5, T6, T7> : JScheduledEvent
    {
        public Action<T1, T2, T3, T4, T5, T6, T7> Action { get; }
        public T1 Arg1 { get; }
        public T2 Arg2 { get; }
        public T3 Arg3 { get; }
        public T4 Arg4 { get; }
        public T5 Arg5 { get; }
        public T6 Arg6 { get; }
        public T7 Arg7 { get; }
        public JScheduledEventInst(JScheduler parent, float delay, Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            _scheduler = parent; _duration = delay; _currentTime = 0f; _duration = delay;
            Action = action; Arg1 = arg1; Arg2 = arg2; Arg3 = arg3; Arg4 = arg4; Arg5 = arg5; Arg6 = arg6; Arg7 = Arg7;
        }
        public JScheduledEventInst(float delay, Action<T1, T2, T3, T4, T5, T6, T7> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
            : this(JScheduler.Instance, delay, action, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
        { }
        public override void Execute() { Action?.Invoke(Arg1, Arg2, Arg3, Arg4, Arg5, Arg6, Arg7); }
    }
    #endregion



}
