
using System;
using System.Collections.Generic;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JEventHandler
    //		Adds a generic event system. The event system allows objects to register, unregister, and execute events on a particular object.
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JEventHandler
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] EventTable

        private static Dictionary<object, Dictionary<string, Delegate>> s_EventTable = new Dictionary<object, Dictionary<string, Delegate>>();
        private static Dictionary<string, Delegate> s_GlobalEventTable = new Dictionary<string, Delegate>();

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private JEventHandler()
        {
            ClearTable();
        }
        #endregion

        #region [초기화] ClearTable
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ClearTable()
        {
            s_EventTable.Clear();
            JEventHandler.ExecuteEvent("OnEventHandlerClear");
        }
        #endregion

        #region [Delegate] Get
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static Delegate GetDelegate(string eventName)
        {
            Delegate handler;
            if (s_GlobalEventTable.TryGetValue(eventName, out handler))
            {
                return handler;
            }
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static Delegate GetDelegate(object obj, string eventName)
        {
            if (obj == null)
                return null;

            Dictionary<string, Delegate> handlers;
            if (s_EventTable.TryGetValue(obj, out handlers))
            {
                Delegate handler;
                if (handlers.TryGetValue(eventName, out handler))
                {
                    return handler;
                }
            }
            return null;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Event] Register/Unregister
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Event] Register
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterEvent(string eventName, Delegate handler)
        {
            Delegate prevHandlers;
            if (s_GlobalEventTable.TryGetValue(eventName, out prevHandlers))
            {
                s_GlobalEventTable[eventName] = Delegate.Combine(prevHandlers, handler);
            }
            else
            {
                s_GlobalEventTable.Add(eventName, handler);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterEvent(object obj, string eventName, Delegate handler)
        {
            if (obj == null)
            {
                JLogger.Write("JEventHandler.RegisterEvent error: target object cannot be null.");
                return;
            }

            Dictionary<string, Delegate> handlers;
            if (!s_EventTable.TryGetValue(obj, out handlers))
            {
                handlers = new Dictionary<string, Delegate>();
                s_EventTable.Add(obj, handlers);
            }

            Delegate prevHandlers;
            if (handlers.TryGetValue(eventName, out prevHandlers))
            {
                handlers[eventName] = Delegate.Combine(prevHandlers, handler);
            }
            else
            {
                handlers.Add(eventName, handler);
            }
        }
        #endregion

        #region [Event] Register<T>
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void RegisterEvent(string eventName, Action handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent(object obj, string eventName, Action handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T>(string eventName, Action<T> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T>(object obj, string eventName, Action<T> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U>(string eventName, Action<T, U> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U>(object obj, string eventName, Action<T, U> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V>(string eventName, Action<T, U, V> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T1, T2, T3, T4, T5>(object obj, string eventName, Action<T1, T2, T3, T4, T5> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        public static void RegisterEvent<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> handler) { RegisterEvent(eventName, (Delegate)handler); }
        public static void RegisterEvent<T1, T2, T3, T4, T5, T6>(object obj, string eventName, Action<T1, T2, T3, T4, T5, T6> handler) { RegisterEvent(obj, eventName, (Delegate)handler); }
        #endregion

        #region [Event] Unregister
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void UnregisterEvent(object obj, string eventName)
        {
            if (obj == null)
            {
                JLogger.Write("JEventHandler.UnregisterEvent error: target object cannot be null.");
                return;
            }

            if (s_EventTable.TryGetValue(obj, out Dictionary<string, Delegate> handlers))
                handlers.Remove(eventName);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void UnregisterEvent(string eventName, Delegate handler)
        {
            Delegate prevHandlers;
            if (s_GlobalEventTable.TryGetValue(eventName, out prevHandlers))
            {
                s_GlobalEventTable[eventName] = Delegate.Remove(prevHandlers, handler);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void UnregisterEvent(object obj, string eventName, Delegate handler)
        {
            if (obj == null)
            {
                JLogger.Write("JEventHandler.UnregisterEvent error: target object cannot be null.");
                return;
            }

            Dictionary<string, Delegate> handlers;
            if (s_EventTable.TryGetValue(obj, out handlers))
            {
                Delegate prevHandlers;
                if (handlers.TryGetValue(eventName, out prevHandlers))
                {
                    handlers[eventName] = Delegate.Remove(prevHandlers, handler);
                }
            }
        }
        #endregion

        #region [Event] Unregister<T>
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void UnregisterEvent(string eventName, Action handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent(object obj, string eventName, Action handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T>(string eventName, Action<T> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T>(object obj, string eventName, Action<T> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U>(string eventName, Action<T, U> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U>(object obj, string eventName, Action<T, U> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V>(string eventName, Action<T, U, V> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V, W>(string eventName, Action<T, U, V, W> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T1, T2, T3, T4, T5>(object obj, string eventName, Action<T1, T2, T3, T4, T5> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        public static void UnregisterEvent<T1, T2, T3, T4, T5, T6>(string eventName, Action<T1, T2, T3, T4, T5, T6> handler) { UnregisterEvent(eventName, (Delegate)handler); }
        public static void UnregisterEvent<T1, T2, T3, T4, T5, T6>(object obj, string eventName, Action<T1, T2, T3, T4, T5, T6> handler) { UnregisterEvent(obj, eventName, (Delegate)handler); }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // [Event] Execute
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Event] Execute
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ExecuteEvent(string eventName) { (GetDelegate(eventName) as Action)?.Invoke(); }
        public static void ExecuteEvent(object obj, string eventName) { (GetDelegate(obj, eventName) as Action)?.Invoke(); }
        public static void ExecuteEvent<T>(string eventName, T arg1) { (GetDelegate(eventName) as Action<T>)?.Invoke(arg1); }
        public static void ExecuteEvent<T>(object obj, string eventName, T arg1) { (GetDelegate(obj, eventName) as Action<T>)?.Invoke(arg1); }
        public static void ExecuteEvent<T, U>(string eventName, T arg1, U arg2) { (GetDelegate(eventName) as Action<T, U>)?.Invoke(arg1, arg2); }
        public static void ExecuteEvent<T, U>(object obj, string eventName, T arg1, U arg2) { (GetDelegate(obj, eventName) as Action<T, U>)?.Invoke(arg1, arg2); }
        public static void ExecuteEvent<T, U, V>(string eventName, T arg1, U arg2, V arg3) { (GetDelegate(eventName) as Action<T, U, V>)?.Invoke(arg1, arg2, arg3); }
        public static void ExecuteEvent<T, U, V>(object obj, string eventName, T arg1, U arg2, V arg3) { (GetDelegate(obj, eventName) as Action<T, U, V>)?.Invoke(arg1, arg2, arg3); }
        public static void ExecuteEvent<T, U, V, W>(string eventName, T arg1, U arg2, V arg3, W arg4) { (GetDelegate(eventName) as Action<T, U, V, W>)?.Invoke(arg1, arg2, arg3, arg4); }
        public static void ExecuteEvent<T, U, V, W>(object obj, string eventName, T arg1, U arg2, V arg3, W arg4) { (GetDelegate(obj, eventName) as Action<T, U, V, W>)?.Invoke(arg1, arg2, arg3, arg4); }
        public static void ExecuteEvent<T1, T2, T3, T4, T5>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) { (GetDelegate(eventName) as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5); }
        public static void ExecuteEvent<T1, T2, T3, T4, T5>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) { (GetDelegate(obj, eventName) as Action<T1, T2, T3, T4, T5>)?.Invoke(arg1, arg2, arg3, arg4, arg5); }
        public static void ExecuteEvent<T1, T2, T3, T4, T5, T6>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) { (GetDelegate(eventName) as Action<T1, T2, T3, T4, T5, T6>)?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6); }
        public static void ExecuteEvent<T1, T2, T3, T4, T5, T6>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) { (GetDelegate(obj, eventName) as Action<T1, T2, T3, T4, T5, T6>)?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6); }
        #endregion

    }
}
