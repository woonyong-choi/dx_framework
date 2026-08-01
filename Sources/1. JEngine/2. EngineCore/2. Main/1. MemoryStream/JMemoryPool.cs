using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;



namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JMemoryPool
    //
    //
    //	@Usages
    //		1. MemoryStream
    //			- var stream = JMemoryPool.GetMemoryStream(1024); 
    //			- ... stream use
    //			- ReturnMemoryStream(ms);
    //
    //		2. JMemoryPool.GetRecyclableMemoryStream();
    //			- Return 필요 없음
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JMemoryPool
    {
        private static Dictionary<int, Queue<byte[]>> _memory_size_pool = new Dictionary<int, Queue<byte[]>>();

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // byte[]
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


        #region [byte] GetBuffer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Queue<byte[]> GetMemoryPool(int bufferSize)
        {
            if (_memory_size_pool.ContainsKey(bufferSize))
                return _memory_size_pool[bufferSize];
            var pool = _memory_size_pool[bufferSize] = new Queue<byte[]>();
            return pool;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static byte[] GetBuffer(int bufferSize)
        {
            lock (_memory_size_pool)
            {
                var memory_pool = GetMemoryPool(bufferSize);
                if (memory_pool.Count <= 0)
                {
                    var buffer = new byte[bufferSize];
                    return buffer;
                }
                return memory_pool.Dequeue();
            }
        }
        #endregion

        #region [byte] ReturnBuffer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static void ReturnBuffer(byte[] buffer)
        {
            lock (_memory_size_pool)
            {
                var memory_pool = GetMemoryPool(buffer.Length);
                memory_pool.Enqueue(buffer);
            }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // MemoryStream
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [MemoryStream] GetMemoryPoolBufferSize
        private const int c_min_memory_stream_size = 128;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int GetMemoryPoolBufferSize(int bufferSize)
        {
            bufferSize = Math.Max(c_min_memory_stream_size, bufferSize);

            var c = Math.Ceiling(Math.Log(bufferSize, 2));
            var max_size = (int)Math.Pow(2, c);
            return max_size;
        }

        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static Queue<MemoryStream> GetMemoryStreamPool(int bufferSize)
        //{
        //	bufferSize = GetMemoryPoolBufferSize(bufferSize);

        //	if (_memory_stream_pool.ContainsKey(bufferSize))
        //		return _memory_stream_pool[bufferSize];
        //	var pool = _memory_stream_pool[bufferSize] = new Queue<MemoryStream>();
        //	return pool;
        //}
        #endregion

        #region [MemoryStream] GetMemoryStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MemoryStream GetMemoryStream(int bufferSize)
        {
            bufferSize = GetMemoryPoolBufferSize(bufferSize);
            var buffer = GetBuffer(bufferSize);

            // todo: MemoryPool에서 갖고 오기(데이터 사이즈를 모르니 메모리풀에서 갖고 올 수 없다. GC가 더 최적화를 잘할 듯 하다)
            var stream = new MemoryStream(buffer, 0, buffer.Length, true, true);
            return stream;
        }
        #endregion

        #region [MemoryStream] ReturnMemoryStream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void ReturnMemoryStream(MemoryStream stream)
        {
            if (stream.CanWrite)
                return;

            ReturnBuffer(stream.GetBuffer());
        }
        #endregion




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // RecyclableMemoryStream
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [RecyclableMemoryStream] Instance
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static RecyclableMemoryStreamManager _recyclableMemoryStreamManagerInstance;
        private static RecyclableMemoryStreamManager RecyclableMemoryStreamManagerInstance
        {
            get
            {
                if (null == _recyclableMemoryStreamManagerInstance)
                    _recyclableMemoryStreamManagerInstance = InitializeRecyclableMemoryStreamManager();
                return _recyclableMemoryStreamManagerInstance;
            }
        }
        #endregion

        #region [RecyclableMemoryStream] Initialize
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static RecyclableMemoryStreamManager InitializeRecyclableMemoryStreamManager()
        {
            int blockSize = 1024; // 128KB -> 1KB
            int largeBufferMultiple = 1024 * 1024;
            int maxBufferSize = 128 * largeBufferMultiple;

            var manager = new RecyclableMemoryStreamManager(blockSize,
                                                            largeBufferMultiple,
                                                            maxBufferSize);

            manager.GenerateCallStacks = true;
            // manager.AggressiveBufferReturn = true;
            //manager.MaximumFreeLargePoolBytes = maxBufferSize * 4;
            manager.MaximumFreeSmallPoolBytes = 100 * blockSize;
            return manager;
        }
        #endregion

        #region [RecyclableMemoryStream] Get
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static MemoryStream GetRecyclableMemoryStream()
        {
            return RecyclableMemoryStreamManagerInstance.GetStream();
        }
        #endregion



    }




}


