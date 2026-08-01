using System;
using System.IO.MemoryMappedFiles;
//using System.IO.MemoryMappedFiles;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSharedMemory
    //
    //  1. 초기화
    //      _memory_command = JSharedMemory.Create("SharedMemory_command", 1024);
    //
    //  2. 삭제
    //      _memory_command.Dispose();
    //
    //  3. 데이터 쓰기
    //      using (var stream = new MemoryStream(_memory_command.Buffer))
    //      {
    //      	using (var writer = new BinaryWriter(stream))
    //      	{
    //      		writer.Write(some_data1);
    //      		writer.Write(some_data2);
    //      	}
    //      }
    //      _memory_command.WriteToSharedMemory();		
    //
    //  4. 데이터 읽기
    //      _memory_base.ReadFromSharedMemory();
    //      using (var stream = new MemoryStream(_memory_base.Buffer))
    //      {
    //      	using (var reader = new BinaryReader(stream))
    //      	{
    //      		var some_data1 = reader.ReadInt32();
    //      		var some_data2 = reader.ReadInt32();
    //          }
    //      }
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JSharedMemory : IDisposable
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Base
        public MemoryMappedFile _map_file;
        public byte[] _buffer;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] Property
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int Size
        {
            get { return (_buffer != null) ? _buffer.Length : 0; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public byte[] Buffer
        {
            get { return _buffer; }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 메인 함수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [생성] Create (Name, Size)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JSharedMemory Create(string name, int size)
        {
            var sm = new JSharedMemory();

            try
            {
                var map_file = MemoryMappedFile.CreateNew(name, size, MemoryMappedFileAccess.ReadWrite);
                sm._map_file = map_file;
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error]" + e.Message);
            }

            sm._buffer = new byte[size];
            return sm;
        }
        #endregion

        #region [생성] Open (Name, Size)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JSharedMemory Open(string name, int size)
        {
            try
            {
                var map_file = MemoryMappedFile.OpenExisting(name, MemoryMappedFileRights.ReadWrite);
                var sm = new JSharedMemory();
                sm._buffer = new byte[size];

                sm._map_file = map_file;
                return sm;
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error]" + e.Message);
            }
            return null;
        }
        #endregion

        #region [종료] Dispose
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            if (_map_file != null)
                _map_file.Dispose();
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 버퍼복사
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [쓰기] [버퍼] Buffer -> SharedMemory
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void WriteToSharedMemory()
        {
            using (var accessor = _map_file.CreateViewAccessor())
            {
                accessor.WriteArray(0, _buffer, 0, _buffer.Length);
            }
        }
        #endregion

        #region [읽기] [버퍼] SharedMemory -> Buffer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ReadFromSharedMemory(int length = 0)
        {
            using (var view = _map_file.CreateViewStream())
            {
                view.Position = 0;
                view.Read(_buffer, 0, (length > 0) ? length : _buffer.Length);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void ReadFromSharedMemory(int startIndex, int length)
        {
            using (var view = _map_file.CreateViewStream())
            {
                view.Position = startIndex;
                view.Read(_buffer, startIndex, length);
            }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // IO
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [쓰기] Write (int)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void WriteTo(int startIndex, int value)
        {
            using (var accessor = _map_file.CreateViewAccessor())
            {
                accessor.Write(startIndex, value);
            }
        }
        #endregion

        #region [읽기] Read (int)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int ReadInt32(int startIndex)
        {
            ReadFromSharedMemory(startIndex, 4);          // 여러번 하면 앞에서 부터 지워지는 경우가 있다.
            int result = BitConverter.ToInt32(_buffer, startIndex);
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int ReadInt32(byte[] bytes, int startIndex)
        {
            int result = 0;
            result |= (int)((bytes[startIndex + 0] << 0) & 0xFF);
            result |= (int)((bytes[startIndex + 1] << 8) & 0xFF);
            result |= (int)((bytes[startIndex + 2] << 16) & 0xFF);
            result |= (int)((bytes[startIndex + 3] << 24) & 0xFF);
            return result;
        }
        #endregion

        #region [읽기] Read (int[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int[] ReadInt32s(int count)
        {
            int size = sizeof(int) * count;
            ReadFromSharedMemory(0, size);

            return ReadInt32s(_buffer, 0, count);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public int[] ReadInt32s(byte[] buffer, int start, int count)
        {
            int[] results = new int[count];

            for (int i = 0; i < count; ++i)
            {
                int startIndex = start + i * sizeof(int);
                //results[i] = readInt(buffer, startIndex);
                results[i] = BitConverter.ToInt32(buffer, startIndex);
            }
            return results;
        }
        #endregion

        #region [읽기] Read (float[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float[] ReadFloats(int count)
        {
            int size = sizeof(float) * count;
            ReadFromSharedMemory(0, size);

            return ReadFloats(_buffer, 0, count);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float[] ReadFloats(byte[] buffer, int start, int count)
        {
            float[] results = new float[count];

            for (int i = 0; i < count; ++i)
            {
                int startIndex = start + i * sizeof(float);
                results[i] = BitConverter.ToSingle(buffer, startIndex);
            }
            return results;
        }
        #endregion

        #region [읽기] Read (double[])
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public double[] ReadDoubles(byte[] buffer, int start, int count)
        {
            double[] results = new double[count];

            for (int i = 0; i < count; ++i)
            {
                int startIndex = start + i * sizeof(double);
                results[i] = BitConverter.ToDouble(buffer, startIndex);
            }
            return results;
        }
        #endregion        

    }

}
