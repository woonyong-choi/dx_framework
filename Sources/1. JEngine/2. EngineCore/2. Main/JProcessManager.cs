using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;



//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//
// JProcessManager
//
//
//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

public class JProcessManager
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // 변수
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    #region [DllImport] 
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className, string windowName);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

    [DllImport("User32.dll")]
    public static extern IntPtr GetParent(IntPtr hwnd);

    [DllImport("User32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("User32.dll")]
    public static extern IntPtr GetWindow(IntPtr hWnd, uint nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("User32.dll")]
    public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

    [DllImport("user32.dll")]
    public static extern int EnumWindows(CallBackPtr callPtr, int lPar);

    public delegate bool CallBackPtr(int hwnd, int lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    public const int SW_MINIMIZE = 0X6;
    public const int SW_RESTORE = 0X9;



    private const int MOUSEEVENTF_MOVE = 0x0001;
    private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const int MOUSEEVENTF_LEFTUP = 0x0004;
    private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const int MOUSEEVENTF_RIGHTUP = 0x0010;
    private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

    [DllImport("user32.dll")]
    static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);


    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;

    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    public static IntPtr _process_main = Process.GetCurrentProcess().Handle;

    #endregion

    public static void LeftClick(int x, int y)
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
    }


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // 외부 프로세스 실행
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    #region [외부프로세스] 실행
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static Process RunProgram(string programName, string arguments, bool hidden = false, bool waitForExit = false)
    {

        if (!System.IO.File.Exists(programName))
            return null;
        programName = programName.Substring(0, programName.LastIndexOf('.'));
        J2y.JLogger.Write("[RunProgrm]" + programName);

        ProcessStartInfo startInfo = new ProcessStartInfo(programName);

        //public static Process RunProgrm(string fullpath, string program_name, string arguments, bool hidden = false, bool waitForExit = false)
        //{
        //	if (!System.IO.File.Exists(fullpath + program_name))
        //		return null;
        //	UnityEngine.Debug.Log("[RunProgrm]" + program_name);

        //	ProcessStartInfo startInfo = new ProcessStartInfo();
        //	startInfo.Domain = fullpath;
        //	startInfo.FileName = fullpath + program_name;

        if (hidden)
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.Arguments = arguments;


        //try
        //{
        var process = Process.Start(startInfo);

        if (waitForExit)
        {
            if (process.WaitForExit(1200000))
            {
                return process;
            }
            else
            {
                return process;
            }
        }
        return process;
        //}
        //catch(Exception e)
        //{
        //    //var log = JFile.Create("error.log", false);
        //    //log.WriteLine(e.ToString());
        //    //StreamWriter writer = new StreamWriter("C:\\error.log", true);
        //    File.AppendAllText(@"C:\error.log", string.Format("\n{0} - {1}", DateTime.Now.ToString(), e.ToString()));

        //}

        //return null;
    }
    #endregion

    #region [외부프로세스] 종료
    //------------------------------------------------------------------------------------------------------------------------------------------------------    
    public static void ExitProgram(string programName)
    {
        var processList = Process.GetProcesses();
        var current = Process.GetCurrentProcess();

        foreach (var proc in processList)
        {
            try
            {
                //UnityEngine.Debug.Log("[프로그램]" + proc.ProcessName);
                if ((proc.ProcessName == programName) && (current.Id != proc.Id))
                    proc.Kill();
            }
            catch (Exception) { }
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static void ExitProgram(Process process)
    {
        try
        {
            process.Kill();
        }
        catch (Exception) { }
    }
    #endregion

    #region [외부프로세스] 실행중인지
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static bool IsRunningProgram(string programName)
    {
        J2y.JLogger.Write(programName);
        var processList = Process.GetProcesses();

        foreach (var proc in processList)
        {
            try
            {
                if (proc.ProcessName == programName)
                    return true;
            }
            catch (Exception) { }
        }

        return false;
    }
    #endregion

    #region [외부프로세스] 찾기
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static Process FindProgram(string programName)
    {
        J2y.JLogger.Write(programName);
        var processList = Process.GetProcesses();

        foreach (var proc in processList)
        {
            try
            {
                if (proc.ProcessName == programName)
                    return proc;
            }
            catch (Exception) { }
        }

        return null;
    }
    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static Process GetCurrentProgram()
    {
        return Process.GetCurrentProcess();
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------------
    public static IntPtr FindProgramByPath(string path)
    {

        //UnityEngine.Debug.Log(programName);
        var processList = Process.GetProcesses();

        foreach (var proc in processList)
        {
            try
            {
                if (proc.MainModule.ModuleName.ToLower().Contains(path.ToLower()))
                    return proc.MainWindowHandle;
            }
            catch (Exception) { }
        }

        return IntPtr.Zero;
    }
    #endregion

    #region [프로세스] ID로 윈도우 핸들 찾기

    public static IntPtr GetWindowHandle(Process process, string program_title = null)
    {
        return GetWindowHandle(process.Id);
    }

    public static IntPtr GetWindowHandle(IntPtr process_id, string program_title = null)
    {
        return GetWindowHandle(process_id.ToInt32());
    }

    public static IntPtr GetWindowHandle(int process_id)
    {
        var tempHwnd = FindWindow(null, null);

        while (true)
        {
            if (GetParent(tempHwnd) == IntPtr.Zero || GetParent(tempHwnd) == null)
            {
                uint pid;
                GetWindowThreadProcessId(tempHwnd, out pid);
                if (process_id == pid)
                    return tempHwnd;
            }

            tempHwnd = GetWindow(tempHwnd, 2);

            if (tempHwnd == null || tempHwnd == IntPtr.Zero)
            {
                break;
            }
        }

        return IntPtr.Zero;
    }

    public static bool EnumWindow(int hWnd, int lParam)
    {
        uint pid;

        IntPtr handle = new IntPtr(hWnd);
        GetWindowThreadProcessId(handle, out pid);

        if (lParam == pid)
        {
            //SpRoot._process_external_program[index] = handle;
            HandleList.values.Add(handle);
            J2y.JLogger.Write(string.Format("[SPEN] HANDLE : {0}, Param : {1}\n", handle, pid));
            return false;
        }
        return true;
    }

    public static class HandleList
    {
        public static List<IntPtr> values;

        static HandleList()
        {
            values = new List<IntPtr>();
            values.Clear();
        }

        public static void Clear()
        {
            values.Clear();
        }

        public static IntPtr FindHandle(int pid)
        {
            foreach (var value in values)
            {
                uint find_pid;
                GetWindowThreadProcessId(value, out find_pid);

                if (pid == find_pid)
                    return value;
            }

            return IntPtr.Zero;
        }
    }

    #endregion


}


