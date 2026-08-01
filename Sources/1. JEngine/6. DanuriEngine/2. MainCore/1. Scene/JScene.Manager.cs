using System;
using System.Collections;
using System.Collections.Generic;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JScene.Manager
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public abstract partial class JScene
    {
        public sealed class Manager
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 이벤트
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [변수] JScene
            private static Action<JScene> OnBegineChangeScene;
            private static Action<JScene> OnEndChangeScene;
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 변수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [변수] JScene
            private static Dictionary<string, JScene> _scenes;
            private static JScene _curruntscene;
            #endregion

            #region [변수] Coroutines
            private static JCoroutines _coroutine;
            #endregion

            #region [변수] 속성
            private static bool _Onchangescenelock;
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Property
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Property] JScene
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static Dictionary<string, JScene> JScenes => _scenes;
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static JScene Currunt => _curruntscene;
            #endregion

            #region [Property] JCoroutines
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static JCoroutines JCoroutines => _coroutine;
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 기본 함수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [초기화] 생성자
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            static Manager()
            {
                _scenes = new Dictionary<string, JScene>();
                _coroutine = new JCoroutines();

                OnBegineChangeScene = (jscene) => { _Onchangescenelock = true; };
                OnEndChangeScene = (jscene) => { _Onchangescenelock = false; };

                JEventHandler.RegisterEvent(Evnet.OnBegineChangeScene, OnBegineChangeScene);
                JEventHandler.RegisterEvent(Evnet.OnEndChangeScene, OnEndChangeScene);

            }
            #endregion

            #region [정리] 소멸자, 다누리 종료 시 동기화 전까지 소멸자에 대신 구현
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            ~Manager()
            {
                _scenes.Clear();
                _coroutine.StopAll();

                JEventHandler.UnregisterEvent(Evnet.OnBegineChangeScene, OnBegineChangeScene);
                JEventHandler.UnregisterEvent(Evnet.OnEndChangeScene, OnEndChangeScene);

            }
            #endregion

            #region [업데이트] Update
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static void Update(float deltaTime)
            {
                _coroutine?.Update(deltaTime);

                if (!_Onchangescenelock)
                    _curruntscene?.Update(deltaTime);
            }
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // JScene
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [JScene] 추가
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Add(string name, JScene scene)
            {
                if (null == scene) return false;
                if (_scenes.ContainsKey(name)) return false;

                scene.Name = name;
                _scenes.Add(name, scene);

                return true;
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Add<T>(string name) where T : JScene, new() { return Add(name, new T()); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Add<T>() where T : JScene, new() { return Add<T>(typeof(T).Name); }
            #endregion

            #region [JScene] 제거
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Remove(string name)
            {
                if (!_scenes.ContainsKey(name)) return false;

                _scenes.Remove(name);

                return true;
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Remove(JScene scene) { return null != scene ? Remove(scene.Name) : false; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Remove<T>() where T : JScene, new() { return Remove(typeof(T).Name); }
            #endregion

            #region [JScene] ChangeScene
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static void OnChangeScene(string name) 
            {
                if (_curruntscene != null && _curruntscene.Name == name)
                    return;
                if (!_Onchangescenelock) 
                    _coroutine.Start(OnChangeSceneUpdate(name)); 
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static void OnChangeScene(JScene scene) { OnChangeScene(scene.Name); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static void OnChangeScene<T>() where T : JScene, new() { OnChangeScene(typeof(T).Name); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            private static IEnumerator OnChangeSceneUpdate(string name)
            {
                if (!_scenes.ContainsKey(name)) yield break;

                JEventHandler.ExecuteEvent(Evnet.OnBegineChangeScene, _curruntscene);

                yield return _curruntscene?.End();

                _curruntscene = _scenes[name];

                yield return _curruntscene?.Begine();

                JEventHandler.ExecuteEvent(Evnet.OnEndChangeScene, _curruntscene);

            }
            #endregion

        }
    }
}

