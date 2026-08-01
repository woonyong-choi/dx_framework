using System;
using System.Collections;
using System.Collections.Generic;

namespace J2y
{
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // JSubScene.Handler
    //
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public abstract partial class JSubScene
    {
        public sealed class Handler
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 이벤트
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [변수] JScene
            private Action<JScene> OnBegineChangeScene;
            private Action<JScene> OnEndChangeScene;
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 변수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [변수] JScene
            private Dictionary<string, JSubScene> _scenes;
            private JSubScene _curruntscene;
            #endregion

            #region [변수] Coroutines
            private JCoroutines _coroutine;
            #endregion

            #region [변수] 속성
            private bool _Onchangescenelock;
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Property
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Property] JScene
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public JSubScene Currunt
            {
                get { return _curruntscene; }
            }
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 기본 함수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [초기화] 생성자
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Handler()
            {
                _scenes = new Dictionary<string, JSubScene>();
                _coroutine = new JCoroutines();

                OnBegineChangeScene = (jscene) => { _Onchangescenelock = true; };
                OnEndChangeScene = (jscene) => { _Onchangescenelock = false; };

                JEventHandler.RegisterEvent(this, Evnet.OnBegineChangeSubScene, OnBegineChangeScene);
                JEventHandler.RegisterEvent(this, Evnet.OnEndChangeSubScene, OnEndChangeScene);

            }
            #endregion

            #region [정리] 소멸자, 다누리 종료 시 동기화 전까지 소멸자에 대신 구현
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            ~Handler()
            {
                _scenes.Clear();
                _coroutine.StopAll();

                JEventHandler.UnregisterEvent(this, Evnet.OnBegineChangeSubScene, OnBegineChangeScene);
                JEventHandler.UnregisterEvent(this, Evnet.OnEndChangeSubScene, OnEndChangeScene);

            }
            #endregion

            #region [업데이트] Update
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public void Update(float deltaTime)
            {
                _coroutine?.Update(deltaTime);

                if (!_Onchangescenelock)
                    _curruntscene?.Update(deltaTime);
            }
            #endregion

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // JSubScene
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [JSubScene] 추가
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public bool Add(string name, JSubScene scene)
            {
                if (null == scene) return false;
                if (_scenes.ContainsKey(name)) return false;

                scene.Name = name;
                _scenes.Add(name, scene);

                return true;
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public bool Add<T>(string name) where T : JSubScene, new() { return Add(name, new T()); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public bool Add<T>() where T : JSubScene, new() { return Add<T>(typeof(T).Name); }
            #endregion

            #region [JSubScene] 제거
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public bool Remove(string name)
            {
                if (!_scenes.ContainsKey(name)) return false;

                _scenes.Remove(name);

                return true;
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public bool Remove(JSubScene scene) { return null != scene ? Remove(scene.Name) : false; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public bool Remove<T>() where T : JSubScene, new() { return Remove(typeof(T).Name); }
            #endregion

            #region [JSubScene] ChangeScene
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public IEnumerator OnChangeSubScene(string name)
            {
                if (_curruntscene != null && _curruntscene.Name == name)
                    return null;
                if (!_Onchangescenelock)
                    return OnChangeJSubSceneUpdate(name);
                return null;
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public IEnumerator OnChangeSubScene(JSubScene scene) { return OnChangeSubScene(scene.Name); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public IEnumerator OnChangeSubScene<T>() where T : JSubScene, new() { return OnChangeSubScene(typeof(T).Name); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            private IEnumerator OnChangeJSubSceneUpdate(string name)
            {
                if (!_scenes.ContainsKey(name)) yield break;

                JEventHandler.ExecuteEvent(this, Evnet.OnBegineChangeSubScene, _curruntscene);

                yield return _curruntscene?.End();

                _curruntscene = _scenes[name];

                yield return _curruntscene?.Begine();

                JEventHandler.ExecuteEvent(this, Evnet.OnEndChangeSubScene, _curruntscene);
            }
            #endregion

        }
    }

}

