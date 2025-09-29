using System;
using System.Collections.Generic;

namespace J2y
{
    namespace Danuri
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // JCamera
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public partial class JCamera : JActor
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //
            // Camera.Manager
            //
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            public static new class Manager
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Event
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Event] Name
                public const string Evnet_Name_OnEnable = "Evnet_Name_OnEnable_JCamera";
                public const string Evnet_Name_OnDisable = "Evnet_Name_OnDisable_JCamera";
                #endregion

                #region [Event] Action
                private static Action<JCamera> Evnet_Action_OnEnable;
                private static Action<JCamera> Evnet_Action_OnDisable;
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 변수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [변수]
                private static IDictionary<ulong, JCamera> _cameras;
                private static IList<JCamera> _activeCameras;
                private static IList<JCamera> _activeUICameras;
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Property
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Property] Cameras
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IDictionary<ulong, JCamera> Cameras
                {
                    get
                    {
                        if (null == _cameras)
                            _cameras = new Dictionary<ulong, JCamera>();
                        return _cameras;
                    }
                    private set
                    {
                        if (null == value)
                            _cameras = value;
                    }
                }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JCamera ActiveCamera => _activeCameras.Count == 0 ? null : _activeCameras[0];
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JUICamera ActiveUICamera => _activeUICameras.Count == 0 ? null : _activeUICameras[0] as JUICamera;             
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [초기화] 생성자
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                static Manager()
                {
                    _activeCameras = new List<JCamera>();
                    _activeUICameras = new List<JCamera>();

                    Evnet_Action_OnEnable +=
                        (camera) =>
                        {
                            if (camera is JUICamera)
                            {
                                if (!_activeUICameras.Contains(camera))
                                    _activeUICameras.Add(camera);
                            }
                            else
                            { 
                                if (!_activeCameras.Contains(camera))
                                    _activeCameras.Add(camera);
                            }
                        };

                    Evnet_Action_OnDisable +=
                        (camera) =>
                        {
                            if (camera is JUICamera)
                            {
                                if (_activeUICameras.Contains(camera))
                                    _activeUICameras.Remove(camera);
                            }
                            else
                            {
                                if (_activeCameras.Contains(camera))
                                    _activeCameras.Remove(camera);
                            }
                        };

                    JEventHandler.RegisterEvent(Evnet_Name_OnEnable, Evnet_Action_OnEnable);
                    JEventHandler.RegisterEvent(Evnet_Name_OnDisable, Evnet_Action_OnDisable);
                }
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Camera
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Camera] [추가]
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                internal static bool Add(JCamera camera)
                {
                    if (Exist(camera.Container.UID)) return false;
                    else { Cameras[camera.Container.UID] = camera; }
                    return true;

                }
                #endregion

                #region [Camera] [제거]
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                internal static bool Remove(JCamera camera)
                {
                    if (!Exist(camera.Container.UID)) return false;
                    else { Cameras.Remove(camera.Container.UID); }
                    return true;
                }
                #endregion

                #region [JActor] [찾기]
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JCamera Find(ulong uid) { return Exist(uid) ? Cameras[uid] : null; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JCamera Find(string name)
                {
                    foreach(var camera in Cameras.Values)
                        if (camera.Name == name)
                            return camera;

                    return null;
                }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static bool Exist(ulong uid) { return Cameras.ContainsKey(uid); }
                #endregion

            }

        }
    }
}
