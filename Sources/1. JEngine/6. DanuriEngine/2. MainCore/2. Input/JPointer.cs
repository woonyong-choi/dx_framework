using CLIInterface.DeviceCenter;
using CLIInterface.Math3D;

namespace J2y
{
    namespace Danuri
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // JPointer
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public partial class JPointer
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Enum
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Enum] 
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public enum InputButton
            {
                Left = 0,
                Middle = 1,
                Right = 2
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public enum FrameState
            {
                Down = 0,
                DoubleClick = 1,
                Pressed = 2,
                Up = 3
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public enum FramePressState
            {
                //이 프레임에서 버튼을 눌렀습니다.
                Pressed = 0,
                // 이 프레임에서 버튼이 해제되었습니다.
                Released = 1,
                //버튼을 눌렀다가이 프레임을 놓습니다.
                PressedAndReleased = 2,
                //마지막 프레임과 동일합니다.
                NotChanged = 3
            }
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 변수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [변수]
            private static bool[,] _buttondata;
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Property
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Property] Mouse
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static Vector2 deltaPosition { get; private set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static Vector2 Position { get; private set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static Vector2 peviousPsition { get; private set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static Vector2 psitionOnScreen { get; private set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static int wheel { get; private set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool LButtonDown { get { return _buttondata[(int)InputButton.Left, (int)FrameState.Down]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool LButton { get { return _buttondata[(int)InputButton.Left, (int)FrameState.Pressed]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool LButtonUp { get { return _buttondata[(int)InputButton.Left, (int)FrameState.Up]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool MButtonDown { get { return _buttondata[(int)InputButton.Middle, (int)FrameState.Down]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool MButton { get { return _buttondata[(int)InputButton.Middle, (int)FrameState.Pressed]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool MButtonUp { get { return _buttondata[(int)InputButton.Middle, (int)FrameState.Up]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool RButtonDown { get { return _buttondata[(int)InputButton.Right, (int)FrameState.Down]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool RButton { get { return _buttondata[(int)InputButton.Right, (int)FrameState.Pressed]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool RButtonUp { get { return _buttondata[(int)InputButton.Right, (int)FrameState.Up]; } }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool IsRepositionCursor { get; private set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool IsCursorViewportArea { get; private set; }
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 기본 함수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [초기화]
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            static JPointer()
            {
                _buttondata = new bool[(int)InputButton.Right + 1, (int)FrameState.Up + 1];
            }
            #endregion

            #region [업데이트] Update
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static void Update(float deltaTime)
            {
                Internal_PointerUpdate();
            }
            #endregion

            #region [업데이트] Update
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            private static void Internal_PointerUpdate()
            {
                deltaPosition = Mouse.GetDeltaPosition();
                Position = Mouse.GetPosition();
                peviousPsition = Mouse.GetPreviousPosition();
                psitionOnScreen = Mouse.GetPositionOnScreen();
                wheel = Mouse.GetWheel();

                _buttondata[(int)InputButton.Left, (int)FrameState.Down] = Mouse.GetLButtonDown();
                _buttondata[(int)InputButton.Left, (int)FrameState.DoubleClick] = Mouse.GetLButtonDblClk();
                _buttondata[(int)InputButton.Left, (int)FrameState.Pressed] = Mouse.GetLButton();
                _buttondata[(int)InputButton.Left, (int)FrameState.Up] = Mouse.GetLButtonUp();
                _buttondata[(int)InputButton.Middle, (int)FrameState.Down] = Mouse.GetMButtonDown();
                _buttondata[(int)InputButton.Middle, (int)FrameState.DoubleClick] = Mouse.GetMButtonDblClk();
                _buttondata[(int)InputButton.Middle, (int)FrameState.Pressed] = Mouse.GetMButton();
                _buttondata[(int)InputButton.Middle, (int)FrameState.Up] = Mouse.GetMButtonUp();
                _buttondata[(int)InputButton.Right, (int)FrameState.Down] = Mouse.GetRButtonDown();
                _buttondata[(int)InputButton.Right, (int)FrameState.DoubleClick] = Mouse.GetRButtonDblClk();
                _buttondata[(int)InputButton.Right, (int)FrameState.Pressed] = Mouse.GetRButton();
                _buttondata[(int)InputButton.Right, (int)FrameState.Up] = Mouse.GetRButtonUp();

                if (0 != deltaPosition.x || 0 != deltaPosition.y) IsRepositionCursor = true;
                else IsRepositionCursor = false;

                var viewportpoint = JCamera.Manager.ActiveUICamera?.ScreenToViewportPoint(Position.ToV3()) ?? Vector3.Zero();
                if (viewportpoint.x < -1.0f || viewportpoint.x > 1.0f || viewportpoint.y < -1.0f || viewportpoint.y > 1.0f)
                    IsCursorViewportArea = false;
                else IsCursorViewportArea = true;
            }
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Mouse
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Mouse] Button
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool GetButtonDown(InputButton input) { return _buttondata[(int)input, (int)FrameState.Down]; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool GetButton(InputButton input) { return _buttondata[(int)input, (int)FrameState.Pressed]; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool GetButtonUp(InputButton input) { return _buttondata[(int)input, (int)FrameState.Up]; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static bool Get(InputButton input, FrameState state) { return _buttondata[(int)input, (int)state]; }
            #endregion

        }
    }
}
