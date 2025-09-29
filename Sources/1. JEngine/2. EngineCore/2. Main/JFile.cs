using System;
using System.IO;
using System.Threading.Tasks;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JFile
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public class JFile : IDisposable
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 변수
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [변수] File Property
        public string _fullname;
        #endregion

        #region [변수] [Text] Reader/Writer
        private StreamWriter _text_writer;
        private StreamReader _text_reader;
        #endregion

        #region [변수] [Binary] Reader/Writer
        public BinaryWriter _binary_writer;
        public BinaryReader _binary_reader;
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Property
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Property] [Text] Reader/Writer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual StreamWriter StreamWriter
        {
            get { return _text_writer; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual StreamReader StreamReader
        {
            get { return _text_reader; }
        }
        #endregion

        #region [Property] [Binary] Reader/Writer
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual BinaryWriter BinaryWriter
        {
            get { return _binary_writer; }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual BinaryReader BinaryReader
        {
            get { return _binary_reader; }
        }
        #endregion

        #region [Property] Stream
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Stream Stream
        {
            get
            {
                if (_binary_writer != null)
                    return _binary_writer.BaseStream;
                if (_binary_reader != null)
                    return _binary_reader.BaseStream;
                if (_text_writer != null)
                    return _text_writer.BaseStream;
                if (_text_reader != null)
                    return _text_reader.BaseStream;
                return null;
            }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 파일 
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [초기화] Private 생성자
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private JFile()
        { }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                Close();
                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~JFile() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region [파일] Exists
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool Exists(string filename)
        {
            return File.Exists(filename);
        }
        #endregion

        #region [파일] Close
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Close()
        {
            if (_text_writer != null)
            {
                _text_writer.Close();
                _text_writer = null;
            }

            if (_text_reader != null)
            {
                _text_reader.Close();
                _text_reader = null;
            }

            if (_binary_writer != null)
            {
                _binary_writer.Close();
                _binary_writer = null;
            }

            if (_binary_reader != null)
            {
                _binary_reader.Close();
                _binary_reader = null;
            }
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 텍스트파일
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [텍스트파일] Open
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JFile OpenText(string filename, bool read)
        {
            var file = new JFile();
            //file._fullname = Application.persistentDataPath + "/" + filename;
            file._fullname = filename;

#if UNITY_STANDALONE_WIN
            if (read)
            {
                if (!Exists(file._fullname))
                    return null;
                file._text_reader = File.OpenText(file._fullname);
            }
            else
            {
                file._text_writer = File.CreateText(file._fullname);
            }
#endif
            return file;
        }
        #endregion

        #region [텍스트파일] WriteLine/ReadLine
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void WriteLine(string text)
        {
            if (_text_writer != null)
                _text_writer.WriteLine(text);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public string ReadLine()
        {
            if (_text_reader != null)
                return _text_reader.ReadLine();
            return null;
        }
        #endregion

        #region [텍스트파일] [Async] WriteLine/ReadLine
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task WriteLineAsync(string text, bool main_thread = true)
        {
            if (_text_writer != null)
                await _text_writer.WriteLineAsync(text);

            if (main_thread)
                await JScheduler.Instance.WaitMainThread();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public async Task<string> ReadLineAsync(bool main_thread = true)
        {
            var result = "";
            if (_text_reader != null)
                result = _text_reader.ReadLine();
            if (main_thread)
                await JScheduler.Instance.WaitMainThread();
            return result;
        }
        #endregion

        #region [텍스트파일] [Sync/Async] All WriteLine/ReadLine
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static void WriteAllText(string filepath, string text) { File.WriteAllText(filepath, text); }
        internal static string ReadAllText(string filepath) { return File.ReadAllText(filepath); }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async Task WriteAllTextAsync(string filepath, string text, bool main_thread = true)
        {
            await Task.Run(() => File.WriteAllText(filepath, text));
            if (main_thread)
                await JScheduler.Instance.WaitMainThread();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async Task<string> ReadAllTextAsync(string filepath, bool main_thread = true)
        {
            if (!File.Exists(filepath))
                return null;
            var result = await Task.Run(() => File.ReadAllText(filepath));
            if (main_thread)
                await JScheduler.Instance.WaitMainThread();
            return result;
        }
        #endregion

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 바이너리파일
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [바이너리파일] Open
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static JFile OpenBinary(string filename, bool read)
        {
            JFile file = new JFile();

            //file._fullname = Application.persistentDataPath + "/" + filename;
            file._fullname = filename;
            if (read)
            {
                if (!Exists(file._fullname))
                    return null;
                file._binary_reader = new BinaryReader(File.Open(file._fullname, FileMode.Open));
            }
            else
            {
                file._binary_writer = new BinaryWriter(File.Open(file._fullname, FileMode.Create));
            }

            return file;
        }
        #endregion


        #region [텍스트파일] [sync/Async] All WriteLine/ReadLine

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static void WriteAllBytes(string filepath, byte[] bytes) { File.WriteAllBytes(filepath, bytes); }
        internal static byte[] ReadAllBytes(string filepath) { return File.ReadAllBytes(filepath); }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async Task WriteAllBytesAsync(string filepath, byte[] bytes, bool main_thread = true)
        {
            await Task.Run(() => File.WriteAllBytes(filepath, bytes));
            if (main_thread)
                await JScheduler.Instance.WaitMainThread();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static async Task<byte[]> ReadAllBytesAsync(string filepath, bool main_thread = true)
        {
            if (!File.Exists(filepath))
                return null;
            var bytes = await Task.Run(() => File.ReadAllBytes(filepath));
            if (main_thread)
                await JScheduler.Instance.WaitMainThread();
            return bytes;
        }
        #endregion



    }

}
