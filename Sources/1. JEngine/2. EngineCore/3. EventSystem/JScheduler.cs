using J2y.Network;
using System;
using System.Collections.Generic;


namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JScheduler
    //		1. 특정 함수(Action)를 n초 후에 호출
    //      2. 주기적 업데이트 함수
    //		3. 시간 제어 (Time Scale), 잠시 멈춤 (Puase / Continue)
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public class JScheduler
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // static
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [static] Instance
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static JScheduler _instance;
        private static object _syncobj = new object();
        public static JScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    //var con = JActorManager.root.NewChild();
                    //con.PropInstance.Name = "JScheduler";
                    //con.AddNewComponent<TransformGroup>();
                    //con.AddNewComponent<ScriptComponent>()
                    //.LoadScript(/**/);
                    _instance = new JScheduler();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Commands
        internal readonly JNetQueue<Action> _commandsInMainThread = new JNetQueue<Action>(8);
        #endregion

        #region [변수] ScheduledEvents
        private List<JScheduledEvent> _active_events = new List<JScheduledEvent>();
        private List<JScheduledEvent> _remove_events = new List<JScheduledEvent>();
        private List<Action> _updateObjects = new List<Action>();
        private List<Action> _removeObjects = new List<Action>();
        private float _delta_time = 0f;
        private float _global_time_scale = 1f;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] ScheduledEvents
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<JScheduledEvent> ActiveEvents { get { return _active_events; } }
        public List<JScheduledEvent> RemoveEvents { get { return _remove_events; } }
        public float DeltaTime { get { return _delta_time; } }
        public float TimeScale { get { return _global_time_scale; } set { _global_time_scale = value; } }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Danuri] 엔진 스크립트 컴포넌트 수정 시 반영
        //#region [초기화] OnCreate
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void OnCreate()
        //{
        //	Instance = Instance ?? this;
        //}
        //#endregion

        //#region [정리] OnDestroy
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public void OnDestroy()
        //{
        //	Instance = Instance == this ? null : Instance;
        //}
        //#endregion
        #endregion

        #region [업데이트] Update
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Update()
        {
            _delta_time = JTimer.DeltaTime;

            //----------------------------------------------------------------------------------------------
            // 1. Scheduled Events
            // 
            if (_remove_events.Count > 0)
            {
                foreach (var remove_obj in _remove_events)
                    _active_events.Remove(remove_obj);
                _remove_events.Clear();
            }

            for (int i = _active_events.Count - 1; i >= 0; --i)
                _active_events[i].Update(_delta_time);


            //----------------------------------------------------------------------------------------------
            // 2. Updatable Objects
            //
            if (_removeObjects.Count > 0)
            {
                foreach (var remove_obj in _removeObjects)
                    _updateObjects.Remove(remove_obj);
                _removeObjects.Clear();
            }

            for (int i = _updateObjects.Count - 1; i >= 0; --i)
                _updateObjects[i]();


            //----------------------------------------------------------------------------------------------
            // 3. Main Thread Commands
            //
            execute_mainThreadCommands();
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Schedule
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Schedule] 등록 (Schedule)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule(float delay, Action callback)
        {
            if (delay <= 0f) { callback(); return null; }
            var scheduledEvent = new JScheduledEventInst(this, delay, callback);
            scheduledEvent.Play();
            return scheduledEvent;
        }

        #region [Schedule] Overloading Methods
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1>(float delay, Action<T1> callback, T1 arg1)
        {
            if (delay <= 0f) { callback(arg1); return null; }
            var scheduledEvent = new JScheduledEventInst<T1>(this, delay, callback, arg1);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1, T2>(float delay, Action<T1, T2> callback, T1 arg1, T2 arg2)
        {
            if (delay <= 0f) { callback(arg1, arg2); return null; }
            var scheduledEvent = new JScheduledEventInst<T1, T2>(this, delay, callback, arg1, arg2);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1, T2, T3>(float delay, Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            if (delay <= 0f) { callback(arg1, arg2, arg3); return null; }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3>(this, delay, callback, arg1, arg2, arg3);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1, T2, T3, T4>(float delay, Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (delay <= 0f) { callback(arg1, arg2, arg3, arg4); return null; }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4>(this, delay, callback, arg1, arg2, arg3, arg4);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1, T2, T3, T4, T5>(float delay, Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (delay <= 0f) { callback(arg1, arg2, arg3, arg4, arg5); return null; }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4, T5>(this, delay, callback, arg1, arg2, arg3, arg4, arg5);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1, T2, T3, T4, T5, T6>(float delay, Action<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (delay <= 0f) { callback(arg1, arg2, arg3, arg4, arg5, arg6); return null; }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4, T5, T6>(this, delay, callback, arg1, arg2, arg3, arg4, arg5, arg6);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Schedule<T1, T2, T3, T4, T5, T6, T7>(float delay, Action<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (delay <= 0f) { callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7); return null; }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4, T5, T6, T7>(this, delay, callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            scheduledEvent.Play();
            return scheduledEvent;
        }
        #endregion

        #endregion

        #region [Schedule] 등록 (Repeat)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat(float delay, Action callback)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(); return null;
            }
            var scheduledEvent = new JScheduledEventInst(this, delay, callback);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }

        #region [Schedule] Overloading Methods
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1>(float delay, Action<T1> callback, T1 arg1)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1>(this, delay, callback, arg1);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1, T2>(float delay, Action<T1, T2> callback, T1 arg1, T2 arg2)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1, arg2); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1, T2>(this, delay, callback, arg1, arg2);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1, T2, T3>(float delay, Action<T1, T2, T3> callback, T1 arg1, T2 arg2, T3 arg3)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1, arg2, arg3); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3>(this, delay, callback, arg1, arg2, arg3);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1, T2, T3, T4>(float delay, Action<T1, T2, T3, T4> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1, arg2, arg3, arg4); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4>(this, delay, callback, arg1, arg2, arg3, arg4);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1, T2, T3, T4, T5>(float delay, Action<T1, T2, T3, T4, T5> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1, arg2, arg3, arg4, arg5); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4, T5>(this, delay, callback, arg1, arg2, arg3, arg4, arg5);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1, T2, T3, T4, T5, T6>(float delay, Action<T1, T2, T3, T4, T5, T6> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1, arg2, arg3, arg4, arg5, arg6); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4, T5, T6>(this, delay, callback, arg1, arg2, arg3, arg4, arg5, arg6);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JScheduledEvent Repeat<T1, T2, T3, T4, T5, T6, T7>(float delay, Action<T1, T2, T3, T4, T5, T6, T7> callback, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (delay <= 0f)
            {
                JLogger.WriteWarning("[Warning] Repeat Delay is Zero. Function Call Just Once.");
                callback(arg1, arg2, arg3, arg4, arg5, arg6, arg7); return null;
            }
            var scheduledEvent = new JScheduledEventInst<T1, T2, T3, T4, T5, T6, T7>(this, delay, callback, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            scheduledEvent.Repeat();
            return scheduledEvent;
        }
        #endregion

        #endregion

        #region [Schedule] 등록 취소 (Cancle)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Cancel(ref JScheduledEvent scheduledEvent)
        {
            if (scheduledEvent != null && Instance.ActiveEvents.Contains(scheduledEvent))
            {
                Instance._remove_events.Add(scheduledEvent);
            }
            scheduledEvent = null;
        }
        #endregion

        #region [Schedule] Async wait (todo: 쓰레드풀에서 실행시 동기화 작업필요)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public WaitForMainThread Wait(float duration)
        {
            return new WaitForMainThread(duration);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Updatable
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Updatable] 등록
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddUpdatable(Action callback)
        {
            _updateObjects.Add(callback);
        }
        #endregion

        #region [Updatable] 취소
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CancelUpdatable(Action callback)
        {
            _removeObjects.Remove(callback);
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [메인쓰레드] Commands
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [메인쓰레드] [Commands] 추가
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddMainThreadCommand(Action fun)
        {
#if NET_SERVER
			_commandsInMainThread.Enqueue(fun);			
#else
            _commandsInMainThread.Enqueue(fun);
#endif
        }
        #endregion

        #region [메인쓰레드] [Commands] 메인 쓰레드로 복귀
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public WaitForMainThread WaitMainThread()
        {
            var instance = Instance;
            return new WaitForMainThread();
        }
        #endregion

        #region [메인쓰레드] [Commands] 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected void execute_mainThreadCommands()
        {
            while (_commandsInMainThread.Count > 0)
            {
                if (_commandsInMainThread.TryDequeue(out Action command))
                    command();
            }
        }
        #endregion

    }

}
