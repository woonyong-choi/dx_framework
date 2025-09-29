using System;
using System.IO;
using static System.Net.IPAddress;

namespace J2y.Network
{
    #region [Extension] BigEndian
    namespace BigEndianExtension
    {
        public static class BigEndian
        {
            public static short ToBigEndian(this short value) => HostToNetworkOrder(value);
            public static int ToBigEndian(this int value) => HostToNetworkOrder(value);
            public static long ToBigEndian(this long value) => HostToNetworkOrder(value);
            public static short FromBigEndian(this short value) => NetworkToHostOrder(value);
            public static int FromBigEndian(this int value) => NetworkToHostOrder(value);
            public static long FromBigEndian(this long value) => NetworkToHostOrder(value);
        }
    }
    #endregion


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [Interface] NetMessageHeader
    //		
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public interface INetMessageHeader
    {
        #region [Property] Header Data
        int HeaderSize { get; }
        int DataSize { get; }
        int PacketSize { get; }
        int MessageID { get; }
        #endregion

        #region [Property] Header Property (StartTag)
        bool HasStartTag { get; }
        byte StartTag { get; }
        #endregion

        #region [IO] Read/Write
        void Write(BinaryWriter writer);
        void Write(byte[] buffer, int ptr);
        void Read(BinaryReader reader);
        void Read(byte[] buffer, int ptr);
        #endregion
    }



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // NetMessageHeader
    //		
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JNetMessageHeader : INetMessageHeader
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Header
        protected int _data_size;
        protected int _message_id;
        protected int _sequence_number;
        protected int _checksum;
        #endregion

        #region [Property] Size
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual int HeaderSize { get { return JNetBase.HeaderByteSize; } }
        public virtual int DataSize { get { return _data_size; } set { _data_size = value; } }
        public virtual int PacketSize
        {
            get { return DataSize + HeaderSize; }
            set { _data_size = value - HeaderSize; }
        }
        #endregion

        #region [Property] Header Data
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual int MessageID { get { return _message_id; } set { _message_id = value; } }
        public virtual int SequenceNumber { get { return _sequence_number; } set { _sequence_number = value; } }
        public virtual int Checksum { get { return _checksum; } set { _checksum = value; } }
        public virtual bool HasStartTag { get { return false; } }
        public virtual byte StartTag { get { return 0; } }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // IO
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [IO] [Stream] Write/Read
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(_data_size);
            writer.Write(_message_id);
            writer.Write(_sequence_number);
            writer.Write(_checksum);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Read(BinaryReader reader)
        {
            _data_size = reader.ReadInt32();
            _message_id = reader.ReadInt32();
            _sequence_number = reader.ReadInt32();
            _checksum = reader.ReadInt32();
        }
        #endregion

        #region [IO] [Buffer] Write/Read
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Write(byte[] buffer, int ptr)
        {
            //ptr *= 8;
            //NetBitWriter.WriteInt32(_data_size, 32, buffer, ptr + 0);
            //NetBitWriter.WriteInt32(_message_id, 32, buffer, ptr + 32);
            //NetBitWriter.WriteInt32(_sequence_number, 32, buffer, ptr + 64);
            //NetBitWriter.WriteInt32(_checksum, 32, buffer, ptr + 96);
            CopyToByteArray(_data_size, buffer, ptr + 0);
            CopyToByteArray(_message_id, buffer, ptr + 4);
            CopyToByteArray(_sequence_number, buffer, ptr + 8);
            CopyToByteArray(_checksum, buffer, ptr + 12);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Read(byte[] buffer, int ptr)
        {
            //ptr *= 8;
            //_data_size = NetBitWriter.ReadInt32(buffer, 32, ptr + 0);
            //_message_id = NetBitWriter.ReadInt32(buffer, 32, ptr + 32);
            //_sequence_number = NetBitWriter.ReadInt32(buffer, 32, ptr + 64);
            //_checksum = NetBitWriter.ReadInt32(buffer, 32, ptr + 96);
            _data_size = BitConverter.ToInt32(buffer, ptr);
            _message_id = BitConverter.ToInt32(buffer, ptr + 4);
            _sequence_number = BitConverter.ToInt32(buffer, ptr + 8);
            _checksum = BitConverter.ToInt32(buffer, ptr + 12);
        }
        #endregion

        #region [IO] ShallowCopy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal JNetMessageHeader ShallowCopy()
        {
            return (JNetMessageHeader)this.MemberwiseClone();
        }
        #endregion        

        #region [유틸] CopyToByteArray (int32)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CopyToByteArray(int source, byte[] destination, int offset)
        {
            //// check if there is enough space for all the 4 bytes we will copy
            //if (destination.Length < offset + 4)
            //    throw new ArgumentException("Not enough room in the destination array");

            destination[offset] = (byte)(source >> 24); // fourth byte
            destination[offset + 1] = (byte)(source >> 16); // third byte
            destination[offset + 2] = (byte)(source >> 8); // second byte
            destination[offset + 3] = (byte)source; // last byte is already in proper position
        }
        #endregion


    }

}
