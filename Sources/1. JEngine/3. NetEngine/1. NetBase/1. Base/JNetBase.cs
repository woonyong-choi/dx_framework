using System;
using System.Collections.Generic;

namespace J2y.Network
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // enum
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [enum] NetSendResult
    public enum eNetSendResult
    {
        /// <summary>
        /// Message failed to enqueue because there is no connection
        /// </summary>
        FailedNotConnected = 0,

        /// <summary>
        /// Message was immediately sent
        /// </summary>
        Sent = 1,

        /// <summary>
        /// Message was queued for delivery
        /// </summary>
        Queued = 2,

        /// <summary>
        /// Message was dropped immediately since too many message were queued
        /// </summary>
        Dropped = 3,

        UnknownError = 4,
    }
    #endregion

    #region [enum] NetConnectionStatus
    public enum eNetConnectionStatus
    {
        /// <summary>
        /// No connection, or attempt, in place
        /// </summary>
        None,

        /// <summary>
        /// Connect has been sent; waiting for ConnectResponse
        /// </summary>
        InitiatedConnect,

        /// <summary>
        /// Connect was received, but ConnectResponse hasn't been sent yet
        /// </summary>
        ReceivedInitiation,

        /// <summary>
        /// Connect was received and ApprovalMessage released to the application; awaiting Approve() or Deny()
        /// </summary>
        RespondedAwaitingApproval, // We got Connect, released ApprovalMessage

        /// <summary>
        /// Connect was received and ConnectResponse has been sent; waiting for ConnectionEstablished
        /// </summary>
        RespondedConnect, // we got Connect, sent ConnectResponse

        /// <summary>
        /// Connected
        /// </summary>
        Connected,        // we received ConnectResponse (if initiator) or ConnectionEstablished (if passive)

        /// <summary>
        /// In the process of disconnecting
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Disconnected
        /// </summary>
        Disconnected
    }
    #endregion

    #region [enum] FileTransferState
    public enum eFileTransferState
    {
        Start, Chunks, End,
    }
    #endregion

    #region [enum] Net Server State
    public enum eNetServerState
    {
        Stop, Running,
    }
    #endregion



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // eNetMessageProtocol
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [Constant] eNetMessageProtocol

    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static class eNetMessageProtocol
    {
        public const byte Default = 0;                 // None
        public const byte SimpleMessage = 1;           // [S<->C] 자주 사용하는 간단한 메시지 (string)
        public const byte FileTransfer = 2;            // [S<->C] File Transfer
        public const byte RpcRequest = 3;              // [S<->C] RPC
        public const byte RpcResponse = 4;             // [S<->C] RPC Response
    }
    #endregion




    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JNetBase
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    internal static class JNetBase
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Constant
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [상수] Buffer Size
        public const int HeaderByteSize = 16;
        //public const int NetworkMTU = 1408;
        public const int NetworkMTU = 1024 * 1024;
        public const long SendMTUMaxTime = 500;                       // ms, 최대 누적시간
        public const int MaxReceiveBufferSize = 1024 * 1024 * 10;     // 10 MB (최대 전송 버퍼)
        public const int FileChunkSize = 573330;

        //internal const int DefaultSendBufferSize = 1024;              // 1 KB
        //internal const int MaxSendBufferSize = 1024 * 1024 * 10;      // 10 MB

        #endregion

        #region [백업] [상수] Constant
        internal const int NumTotalChannels = 99;
        internal const int NetChannelsPerDeliveryMethod = 32;
        internal const int NumSequenceNumbers = 1024;

        internal const int UnreliableWindowSize = 128;
        internal const int ReliableOrderedWindowSize = 64;
        internal const int ReliableSequencedWindowSize = 64;
        internal const int DefaultWindowSize = 64;

        internal const int MaxFragmentationGroups = ushort.MaxValue - 1;

        internal const int UnfragmentedMessageHeaderSize = 5;

        ///// <summary>
        ///// Number of channels which needs a sequence number to work
        ///// </summary>
        //internal const int NumSequencedChannels = ((int)NetMessageType.UserReliableOrdered1 + NetBase.NetChannelsPerDeliveryMethod) - (int)NetMessageType.UserSequenced1;

        ///// <summary>
        ///// Number of reliable channels
        ///// </summary>
        //internal const int NumReliableChannels = ((int)NetMessageType.UserReliableOrdered1 + NetBase.NetChannelsPerDeliveryMethod) - (int)NetMessageType.UserReliableUnordered;

        internal const string ConnResetMessage = "Connection was reset by remote host";



        // Maximum transmission unit
        // Ethernet can take 1500 bytes of payload, so lets stay below that.
        // The aim is for a max full packet to be 1440 bytes (30 x 48 bytes, lower than 1468)
        // -20 bytes IP header
        //  -8 bytes UDP header
        //  -4 bytes to be on the safe side and align to 8-byte boundary
        // Total 1408 bytes

        /// <summary>
        /// Default MTU value in bytes
        /// </summary>

        public static int MaxIOThreadCount = 10;
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // MessageHeader Make (프로젝트에 따라 MessageHeader 구조가 다른 경우가 있음)
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] MakeMessageHeaderDelegate
        public delegate INetMessageHeader MakeMessageHeaderDelegate(int data_size, int message_id);
        public static Dictionary<int, MakeMessageHeaderDelegate> MakeMessageHeaderDelegates = new Dictionary<int, MakeMessageHeaderDelegate>(); // [HeaderType, Delegate]
        #endregion

        #region [MakeMessageHeader] Default
        private static MakeMessageHeaderDelegate _defaultMakeMessageHeader;
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MakeMessageHeaderDelegate DefaultMakeMessageHeader
        {
            get
            {
                if (null == _defaultMakeMessageHeader)
                    _defaultMakeMessageHeader = defaultMakeMessageHeaderImpl;
                return _defaultMakeMessageHeader;
            }
            set { _defaultMakeMessageHeader = value; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static INetMessageHeader defaultMakeMessageHeaderImpl(int data_size, int message_id)
        {
            var header = new JNetMessageHeader();
            header.DataSize = data_size;
            header.MessageID = message_id;

            // todo:
            //_send_sequence_number = (_send_sequence_number + 1) % JNetBase.NumSequenceNumbers;
            //header._sequence_number = _send_sequence_number;
            //header._checksum = 0; // todo: mem_stream.GetBuffer()[start_pos]를 기준으로 계산
            return header;
        }
        #endregion

        #region [MakeMessageHeader] Add, Find
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool AddMakeMessageHeader(int header_type, MakeMessageHeaderDelegate fun)
        {
            if (!MakeMessageHeaderDelegates.ContainsKey(header_type))
            {
                MakeMessageHeaderDelegates[header_type] = fun;
                if (header_type == 0)
                    DefaultMakeMessageHeader = fun;
                return true;
            }
            return false;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MakeMessageHeaderDelegate GetMakeMessageHeader(int header_type)
        {
            if (0 == header_type)
                return DefaultMakeMessageHeader;
            if (MakeMessageHeaderDelegates.ContainsKey(header_type))
                return MakeMessageHeaderDelegates[header_type];
            return null;
        }
        #endregion

        #region [MessageHeader] Make
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static INetMessageHeader MakeMessageHeader(int header_type, int data_size, int message_id)
        {
            var make_fun = GetMakeMessageHeader(header_type);
            if (null == make_fun)
                throw new JNetException("Unknown MessageHeader Type:" + header_type);

            return make_fun(data_size, message_id);
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] 패킷 유효성 검사
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool ValidPacket(JNetMessageHeader header)
        {
            return true;
        }
        #endregion

        #region [유틸] Check Sum 계싼
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte MakeChecksum(byte[] send_buf)
        {
            byte checkSum = 0;
            foreach (var ch in send_buf)
                checkSum += (byte)ch;

            return checkSum;
        }
        #endregion


        #region [사용X] [유틸] 쓰레드 개수
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static List<int> _threadIds = new List<int>();
        static object _lock_threads = new object();
        public static void AddThreadID(int threadId)
        {
            lock (_lock_threads)
            {
                if (!_threadIds.Contains(threadId))
                {
                    _threadIds.Add(threadId);
                    Console.WriteLine("IO Thread Count : {0}", _threadIds.Count);
                }
            }
        }
        #endregion


    }
}
