using System;
using System.IO;

//using Random = UnityEngine.Random;
using System.Runtime.InteropServices;
using System.Text;
//using Newtonsoft.Json;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JUtil
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static partial class JUtil
    {

        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // FileSystem
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Path] Documents Path (Platform Indepedant)
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string pathForDocumentsFile(string filename)
        {
            //if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //    string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            //    path = path.Substring(0, path.LastIndexOf('/'));
            //    return Path.Combine(Path.Combine(path, "Documents"), filename);
            //}

            //else if (Application.platform == RuntimePlatform.Android)
            //{
            //    string path = Application.persistentDataPath;
            //    path = path.Substring(0, path.LastIndexOf('/'));
            //    return Path.Combine(path, filename);
            //}

            //else
            //{
            //    string path = Application.dataPath;
            //    path = path.Substring(0, path.LastIndexOf('/'));
            //    return Path.Combine(path, filename);
            //}
            return "";
        }
        #endregion

        #region [Path] Current
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string CurrentDirectory
        {
            get { return Environment.CurrentDirectory; }
            set { Environment.CurrentDirectory = value; }
        }
        #endregion

        #region [Path] Combine
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }
        #endregion

        #region [Folder] Crate/Delete
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CreateFolder(string folder)
        {
            var di = new DirectoryInfo(folder);
            if (di.Exists == false)
                di.Create();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool ExistsFolder(string folder)
        {
            var di = new DirectoryInfo(folder);
            return di.Exists;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DeleteFolder(string folder, bool recursive)
        {
            var di = new DirectoryInfo(folder);
            if (di.Exists)
                di.Delete(recursive);
        }
        #endregion

        #region [Folder] Copy
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        #endregion



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Ini File
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [INI] DllImport
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);
        #endregion

        #region [INI] Read/Write
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static void WriteIniString(string section, string key, string val, string filePath)
        {
            WritePrivateProfileString(section, key, val, filePath);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------        
        public static string ReadIniString(string section, string key, string def, string filePath)
        {
            var sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, def, sb, sb.Capacity, filePath);
            return sb.ToString();
        }
        #endregion


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Json
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Json] From/To
        //#if NET_SERVER
        ////------------------------------------------------------------------------------------------------------------------------------------------------------
        //public static T FromJson<T>(string json) { return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json); ; }
        //public static object FromJson(string json, Type type) { return JsonConvert.DeserializeObject(json, type); }
        //public static string ToJson(object obj) { return (obj != null) ? JsonConvert.SerializeObject(obj) : ""; }

        //#else
        //		//------------------------------------------------------------------------------------------------------------------------------------------------------
        //		public static T FromJson<T>(string json) { return JsonUtility.FromJson<T>(json); }
        //		public static object FromJson(string json, Type type) { return JsonUtility.FromJson(json, type); }
        //		public static string ToJson(object obj) { return JsonUtility.ToJson(obj); }
        //#endif
        #endregion


    }



}
