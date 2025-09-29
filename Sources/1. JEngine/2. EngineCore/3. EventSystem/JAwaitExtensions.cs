
using System;
using System.Runtime.CompilerServices;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JAwaitExtensions 
    //		async/await extensions
    //
    //	
    //	@Features
    //		1. WaitForMainThread
    //
    //
    //	@Example (WaitForMainThread)
    //		public async Task TestAsync()
    //		{
    //			// Othre Thread
    //			await new WaitForMainThread();	
    //			// Main Tread
    //		}
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class WaitForMainThread
    {
        public float Delay { get; private set; }
        public WaitForMainThread() { }
        public WaitForMainThread(float delay) { Delay = delay; }
    }

    public class WaitForNetMessage
    {
        //public JNetMessageDispatcher_base NetMessageDispatcher { get; private set; }
        public int MessageID { get; private set; }
        public string SimpleMessageID { get; private set; }
        public bool OverwriteDispatcher { get; private set; }

        //public WaitForNetMessage(JNetMessageDispatcher_base dispatcher, int msgId, bool overwrite) { NetMessageDispatcher = dispatcher; MessageID = msgId; OverwriteDispatcher = overwrite;  }
        //public WaitForNetMessage(JNetMessageDispatcher_base dispatcher, string msgId, bool overwrite) { NetMessageDispatcher = dispatcher; SimpleMessageID = msgId; OverwriteDispatcher = overwrite; }
    }

    // todo: RPC Wait
    // todo: RPC GameObject Wait


    public static class JAwaitExtensions
    {
        #region [Awaiter] WaitForMainThread
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JMainThreadAwaiter GetAwaiter(this WaitForMainThread instruction)
        {
            var awaiter = new JMainThreadAwaiter();
            awaiter.Input = instruction;
            return awaiter;
        }
        #endregion

        #region [Awaiter] WaitForNetMessage
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JNetMessageAwaiter GetAwaiter(this WaitForNetMessage instruction)
        {
            var awaiter = new JNetMessageAwaiter();
            awaiter.Input = instruction;
            return awaiter;
        }
        #endregion
    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JMainThreadAwaiter 
    //
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JMainThreadAwaiter : INotifyCompletion
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Setting
        public WaitForMainThread Input;
        #endregion

        #region [변수] Internals
        private bool _done;
        private Action _continuation;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. await 필수 속성 및 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Awaiter] IsCompleted
        public bool IsCompleted => _done;
        #endregion

        #region [Awaiter] GetResult
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void GetResult() { }
        #endregion

        #region [INotifyCompletion] OnCompleted
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;

            if (Input.Delay <= 0f)
            {
                JScheduler.Instance.AddMainThreadCommand(() =>
                {
                    _done = true;
                    _continuation();
                });
            }
            else
            {
                JScheduler.Instance.Schedule(Input.Delay, () =>
                {
                    _done = true;
                    _continuation();
                });
            }
        }
        #endregion


    }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNetMessageAwaiter 
    //
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JNetMessageAwaiter : INotifyCompletion
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Setting
        public WaitForNetMessage Input;
        #endregion

        #region [변수] Internals
        //private bool _done;
        //private Action _continuation;
        #endregion

        #region [변수] Result
        //private JNetPeer _resultPeer;
        //private BinaryReader _resultReader;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 1. await 필수 속성 및 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Awaiter] IsCompleted
        //public bool IsCompleted => _done;
        #endregion

        #region [Awaiter] GetResult
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //public (JNetPeer, BinaryReader) GetResult() { return (_resultPeer, _resultReader); }
        #endregion

        #region [INotifyCompletion] OnCompleted
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void OnCompleted(Action continuation)
        {
            //_continuation = continuation;

            //if (Input.MessageID == 0)
            //{
            //	Input.NetMessageDispatcher.RegisterMessageHandler(Input.MessageID, (peer, reader) =>
            //	{
            //		_resultPeer = peer;
            //		_resultReader = reader;
            //		_done = true;
            //		_continuation();
            //	}, Input.OverwriteDispatcher);
            //}
            //else
            //{
            //	Input.NetMessageDispatcher.RegisterSimpleMessageHandler(Input.SimpleMessageID, (peer, reader) =>
            //	{
            //		_resultPeer = peer;
            //		_resultReader = reader;
            //		_done = true;
            //		_continuation();
            //	}, Input.OverwriteDispatcher);				
            //}
        }
        #endregion


    }


}
