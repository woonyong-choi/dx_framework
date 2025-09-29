using System;

//using Random = UnityEngine.Random;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [JUtil] Process
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static partial class JUtil
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Thread
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Thread] CurrentThreadID
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int CurrentThreadID
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId; }
        }
        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Process
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Process] 실행
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Process ExecuteProcess(string path, string program_name, bool check_duplicate_run = true, string arguments = "", ProcessWindowStyle window_style = ProcessWindowStyle.Normal) // ProcessWindowStyle.Hidden, Normal
        {
            if (check_duplicate_run && IsRunningProcess(program_name))
                return null;

            var process_fullname = string.Format("{0}\\{1}.exe", path, program_name);

            var info = new ProcessStartInfo(process_fullname);                           //, argument);
            info.WorkingDirectory = path;
            info.WindowStyle = window_style;
            info.Arguments = arguments;
            var process = Process.Start(info);
            return process;
        }
        #endregion

        #region [Process] 실행중인가
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsRunningProcess(string program_name, bool duplicate_running = false)
        {
            var process_list = Process.GetProcessesByName(program_name);
            if (process_list.Length > 0)
            {
                if (duplicate_running)                                                 // 디버깅을 위해 실행되어 있으면 실행하는 부분을 건너 띈다.
                    return true;
                else
                {
                    foreach (var process in process_list)
                        process.Kill();
                }
            }
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsRunningProcess(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return process != null;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
        #endregion

        #region [Process] 종료
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void KillProcess(string program_name)
        {
            var process_list = Process.GetProcessesByName(program_name);
            if (process_list.Length > 0)
            {
                foreach (var process in process_list)
                    process.Kill();
            }
        }
        #endregion

        #region [Process] WindowPos

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref WinRect rectangle);

        [DllImport("user32.dll")]
        public extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);



        public struct WinRect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }



        public static WinRect GetWindowRect(int processId)
        {
            var winHandle = GetWinHandle((uint)processId);
            WinRect NotepadRect = new WinRect();
            GetWindowRect(winHandle, ref NotepadRect);
            return NotepadRect;
        }

        public static void SetWindowRect(int processId, int X, int Y, int cx, int cy)
        {
            var winHandle = GetWinHandle((uint)processId);
            SetWindowPos(winHandle, IntPtr.Zero, X, Y, cx, cy, 0);
        }
        #endregion

        #region [Process] ProcessID -> WindowHandle
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        static IntPtr SelectedProcessWindowHandle;


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);//함수를 호출하는데 그 함수가 false를 리턴할 때 까지 계속하여 window 콜렉션을 첫번째 인자로 그리고 두번째 인자로는 lparam을 인자로 하여 호출하는 놈이다.
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static EnumWindowsProc EnumProcessWindowsProc = (hwnd, lParam) =>
        {
            if (!IsWindowVisible(hwnd))
                return true;

            uint processId;
            //IntPtr parentWindow;
            uint threadID;

            threadID = GetWindowThreadProcessId(hwnd, out processId);

            //StringBuilder buf255Space = new StringBuilder();
            //for (int i = 0; i < 255; i++)
            //{
            //    buf255Space.Append(" ");
            //}

            //StringBuilder sWindowText = new StringBuilder(buf255Space.ToString());
            //int             r = GetWindowText(hwnd, sWindowText, 255);
            //sWindowText = new StringBuilder(sWindowText.ToString().Substring(0, r));
            //Debug.Print("hwnd:"+hwnd +":"+sWindowText);

            if (processId == (uint)lParam)
            {
                SelectedProcessWindowHandle = hwnd;
                return false;
            }

            return true;
        };
        /// <summary>
        /// HwndHelper 와 유사한놈 이 방법으로 프로세스 아이디로 부터 윈도우 핸들을 얻을 수 있다.잘되는 놈이다.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static IntPtr WindowHandleByProcessID(IntPtr pid)
        {
            SelectedProcessWindowHandle = IntPtr.Zero;

            for (int i = 0; i < 1000; ++i)
            {
                EnumWindows(EnumProcessWindowsProc, pid);
                System.Threading.Thread.Sleep(10);
                if (SelectedProcessWindowHandle != IntPtr.Zero)
                    break;
            }


            if (SelectedProcessWindowHandle != IntPtr.Zero)
            {
                return SelectedProcessWindowHandle;
            }
            return SelectedProcessWindowHandle;
        }

        /// <summary>
        /// HwndHelper 와 유사함.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static IntPtr GetWinHandle(uint pid)
        {
            return WindowHandleByProcessID((IntPtr)pid);
        }
        #endregion

    }



}
