using System.IO;
using System.Net;

namespace J2y.Network
{


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetBuffer: 네트워크 스트림 버퍼
    //
    //		NetMessage(사용자 메시지)를 Serialization(+ Header)한 후 버퍼로 저장해서 네트워크 큐로 관리한다.
    //      NetworkStream으로 바로 쓰지 않는 이유는 쓰레드가 다르기 때문에 메인 쓰레드가 블락 되는 것을 방지한다.
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JNetBuffer
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 상수, Static
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [상수] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // Number of bytes to overallocate for each message to avoid resizing
        protected const int c_overAllocateAmount = 4;
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] MemoryStream
        public MemoryStream _stream;
        public int _size;                       // MemoryStream의 버퍼 사이즈보다 작거나 같다. (버퍼에서 읽은 후 증가) (_stream.Length를 사용하지 않는 이유는 NetworkStream에서 읽을 경우 Buffer[]를 직접 넘기기 때문에 Length가 증가하지 않는다.) 
        public bool AutoDisposeStream = true;   // 비동기 디스패처의 경우 나중에 수동 삭제해야함.
        #endregion

        #region [변수] Working 
        public bool _immediate_send;          // 사용 X
        public IPEndPoint _send_end_point;    // UDP
        #endregion

        #region [변수] Header 
        private int _message_id;
        private INetMessageHeader _header; // 해더 타입이 다른 경우에만 유효함
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] MemoryStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public MemoryStream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte[] Buffer
        {
            get { return _stream.GetBuffer(); }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int position
        {
            get { return (int)_stream.Position; }
            set { _stream.Position = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int BufferCapacity
        {
            get { return _stream.Capacity; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int RemainBufferCapacity
        {
            get { return BufferCapacity - _size; }
        }
        #endregion

        #region [Property] Header
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public INetMessageHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int MessageID
        {
            get { return _message_id; }
            set { _message_id = value; }
        }
        #endregion

        #region [Property] UDP
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public IPEndPoint SendEndPoint
        {
            get { return _send_end_point; }
            set { _send_end_point = value; }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 기본 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public JNetBuffer() { }
        public JNetBuffer(int capacity)
        {
            // JNetTcpReceiveChannel 삭제시 Dispose 호출이 복잡해서 디폴트 메모리스트림 사용
            _stream = new MemoryStream(capacity); // JMemoryPool.GetRecyclableMemoryStream(); 
        }
        public JNetBuffer(byte[] buffer)
        {
            _stream = JMemoryPool.GetRecyclableMemoryStream();  // new MemoryStream(buffer);
        }
        #endregion

        #region [복제] Copy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Clone(JNetBuffer src)
        {
            if (src._size > 0)
                System.Buffer.BlockCopy(src.Buffer, 0, Buffer, 0, src._size);

            _size = src._size;
            _immediate_send = src._immediate_send;
            _send_end_point = src._send_end_point;
            Header = src.Header;
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 버퍼
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [버퍼] 데이터 읽기 (+포지션 이동)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Read(MemoryStream out_stream, int data_size)
        {
            out_stream.Write(Buffer, position, data_size);
            position += data_size;
        }
        #endregion

        #region [버퍼] [Compact] 데이터 앞으로 이동
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Compact()
        {
            var remain_size = _size - position;
            if (remain_size > 0)
                System.Buffer.BlockCopy(Buffer, position, Buffer, 0, remain_size);
            _size = remain_size;
            position = 0;
        }
        #endregion

        #region [버퍼] [Reset] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset()
        {
            _size = 0;
            position = 0;
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 유틸
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [유틸] 해더와 데이터를 합친 메모리 스트림을 리턴한다.
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MemoryStream CombinePacketData(INetMessageHeader header, MemoryStream data_stream, int data_size)
        {
            var new_stream = new MemoryStream();
            var writer = new BinaryWriter(new_stream);
            header.Write(writer);
            writer.Write(data_stream.GetBuffer(), 0, data_size);
            new_stream.Position = 0;
            return new_stream;
        }
        #endregion

    }


}
