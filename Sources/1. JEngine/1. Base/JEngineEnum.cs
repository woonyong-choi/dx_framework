namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // [enum] NetPeerType
    //
    //	@TODO
    //		- 2바이트-Type, 2바이트-SubType
    //      
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region [enum] NetPeerType
    public enum eNetPeerType
    {
        None = 0x00000000,
        Client = 0x00000001,
        Server = 0x00000002,    // [C->S] 서버 함수 호출 (default: MainServer)
        Others = 0x00000004,    // [C->S->C] 주변 클라이언트에 전파 (본인도 호출)
        All = 0x00000008,   // [C->S->C] 주변 클라이언트에 전파 (본인 포함)
        Unknown = 0x4fffffff,
        //	todo: AI, Chat	
    }
    #endregion



    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Base
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    #region [enum] SceneSettingType
    public enum eSceneSettingType
    {
        GameObject,
        Prefab,
        UnityScene,
        Max,
    }
    #endregion


    #region [enum] SceneType
    public enum eSceneType
    {
        InstanceGame,
        InstanceField,
        TileField,
        Max,
    }
    #endregion

    #region [enum] NetworkArchitecture
    public enum eNetworkArchitectureType
    {
        Single,
        Multi_TcpOneServer,
        Multi_TcpTwoServer,
        Multi_WebContent_TcpFieldServer,
        Max,
    }
    #endregion


    #region [enum] Template
    #endregion


    #region [enum] LoadingState
    public enum eLoadingState
    {
        None,
        Loading,
        Complete,
    }
    #endregion


    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // 검토중
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    #region [enum] PopupType
    // Background 생성 유무
    public enum ePopupType
    {
        Normal,
        Layer
    }
    #endregion


    #region [enum] [Avatar] 프레임이벤트
    public enum eFrameEventType
    {
        Effect, Sound, Camera, Collision, Movement, MotionTrail, ExecuteEvent,
        Max
    }
    #endregion


}
