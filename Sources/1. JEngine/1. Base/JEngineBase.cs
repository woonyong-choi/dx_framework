namespace J2y
{


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // [공통] 
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public partial class JEngineBase
    {
        public static bool _final_release = false;
        public const float _current_engine_version = 1.0f;



        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 게임 설정
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static bool _sound = true;
        public static float _game_distance_ratio = 0.01f;
        public static string DataPath = "Assets/JOne/Data"; // temp

#if UNITY_EDITOR
#else    // Release
#endif


#if UNITY_SERVER
		public static bool IsUnityServer = true;
#else
        public static bool IsUnityServer = false;
#endif





#if NET_SERVER
		public static bool IsNetServer = true;	
#else
        public static bool IsNetServer = false;
#endif
        public static bool IsNetClient => !IsNetServer;




        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // GameFramework
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static string ContentServerUrl = "http://127.0.0.1:5000";
        public static string ContentServerApiUrl => ContentServerUrl + "/api";
    }


}


