using CLIInterface.Math3D;
using J2y.Interface;
using System.Collections.Generic;

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
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //
            // JPointer.EventSystem
            //
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            public static class EventSystem
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Enum
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Enum] InputEvent
                public enum InputEvent
                {
                    Pointer,
                    LeftPointerDown,
                    LeftPointerDoubleClick,
                    LeftPointerClick,
                    LeftPointerUp,
                    RightPointerDown,
                    RightPointerDoubleClick,
                    RightPointerClick,
                    RightPointerUp,
                    LeftBeginDrag,
                    LeftDrag,
                    LeftEndDrag,
                    RightBeginDrag,
                    RightDrag,
                    RightEndDrag,
                }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 변수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [변수] Event
                private static Dictionary<InputEvent, BaseEventSystem> _event;
                private static Queue<BaseEventSystem> _updater;
                private static Data _eventData;
                #endregion

                #region [변수] Select
                private static bool _selectLock;
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Property
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Property] Select
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static bool IsSelectLock { get { return _selectLock; } }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [초기화] 
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                static EventSystem()
                {
                    _updater = new Queue<BaseEventSystem>();
                    _event = new Dictionary<InputEvent, BaseEventSystem>();
                    _eventData = new Data();

                    _event.Add(InputEvent.Pointer, new Pointer());
                    _event.Add(InputEvent.LeftPointerDown, new LeftPointerDown());
                    _event.Add(InputEvent.LeftPointerDoubleClick, new LeftPointerDoubleClick());
                    _event.Add(InputEvent.LeftPointerClick, new LeftPointerClick());
                    _event.Add(InputEvent.LeftPointerUp, new LeftPointerUp());
                    _event.Add(InputEvent.LeftBeginDrag, new LeftBeginDrag());
                    _event.Add(InputEvent.LeftDrag, new LeftDrag());
                    _event.Add(InputEvent.LeftEndDrag, new LeftEndDrag());
                    _event.Add(InputEvent.RightPointerDown, new RightPointerDown());
                    _event.Add(InputEvent.RightPointerDoubleClick, new RightPointerDoubleClick());
                    _event.Add(InputEvent.RightPointerClick, new RightPointerClick());
                    _event.Add(InputEvent.RightPointerUp, new RightPointerUp());
                    _event.Add(InputEvent.RightBeginDrag, new RightBeginDrag());
                    _event.Add(InputEvent.RightDrag, new RightDrag());
                    _event.Add(InputEvent.RightEndDrag, new RightEndDrag());

                }
                #endregion

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static void Update(float deltaTime)
                {
                    Internal_CommonEventUpdate(deltaTime, _eventData);
                    Internal_StateEventUpdate(deltaTime, _eventData);
                }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Event
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Event] Enqueue
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static void Enqueue(InputEvent Event) { _updater.Enqueue(_event[Event]); }
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Select
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Select] SelectLock
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                internal static void SelectLock() { _selectLock = true; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                internal static void SelectUnLock() { _selectLock = false; }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Evnet
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Evnet] Internal Internal_CommonEventUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private static void Internal_CommonEventUpdate(float deltaTime, Data data)
                {

                    (data.selectedJActor as IUpdateSelectedHandler)?.OnUpdateSelected(data);

                    if (IsSelectLock)
                    {
                        data.clickTime += deltaTime;
                        ++data.clickCount;
                    }
                    else
                    {
                        data.pressEventCamera = null;
                        data.pointerDrag = null;
                        data.pointerPress = null;
                        data.pressPosition = Vector2.Zero();
                        data.clickCount = 0;
                        data.clickTime = 0f;
                        data.dragging = false;

                    }

                    data.delta = deltaPosition;
                    data.position = Position;

                    if (null != data.Pointered && IsSelectLock)
                    {
                        (data.Pointered as Interface.IPointerExitHandler)?.OnPointerExit(data);
                        data.Pointered = null;
                    }
                    if (0 != wheel)
                    {
                        data.scrollDelta = new Vector2(0f, wheel);
                        (JRaycast.EventSystem.FramePickingJActor as Interface.IScrollHandler)?.OnScroll(data);
                    }
                }
                #endregion

                #region [Evnet] Internal StateEventUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private static void Internal_StateEventUpdate(float deltaTime, Data data)
                {
                    if (0 == _updater.Count)
                        Enqueue(InputEvent.Pointer);
                    do
                    {
                        _updater.Dequeue().Update(deltaTime, _eventData);
                    }
                    while (1 < _updater.Count);

                }
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //
                // JPointer.EventSystem.Data
                //
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                public sealed class Data
                {

                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    // Data
                    //
                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    #region [Data] 
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public InputButton button { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public bool dragging { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public bool useDragThreshold { get; set; } = true;
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public int clickCount { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public float clickTime { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public float dragThreshold { get; set; } = 5f;
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public Vector2 pressPosition { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public Vector2 delta { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public Vector2 position { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public Vector2 scrollDelta { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JCamera pressEventCamera { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor Pointered { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor pointerDrag { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor pointerPress { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor rawPointerPress { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor lastPress { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor pointerEnter { get; set; }
                    //------------------------------------------------------------------------------------------------------------------------------------------------------
                    public JActor selectedJActor { get; set; }
                    #endregion
                }

            }

            #region [abstract] BaseEventSystem
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //
            // JPointer.BaseEventSystem
            //
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            internal abstract class BaseEventSystem
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 순수 가상 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public abstract void Update(float deltaTime, EventSystem.Data data);
                #endregion

            }
            #endregion

            #region [Pointer] 
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            internal class Pointer : BaseEventSystem
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    EventSystem.SelectUnLock();
                    Internal_PointerUpdate(deltaTime, data);

                    if (!IsCursorViewportArea) return;
                    if (Internal_ButtonUpdate(InputButton.Left, FrameState.DoubleClick, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Right, FrameState.DoubleClick, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Left, FrameState.Down, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Right, FrameState.Down, data)) return;
                    EventSystem.Enqueue(EventSystem.InputEvent.Pointer);
                }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // EventSystem
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [EventSystem] Internal_PointerUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private void Internal_PointerUpdate(float deltaTime, EventSystem.Data data)
                {
                    if (null == data.Pointered && null != JRaycast.EventSystem.FramePickingJActor)
                    {
                        (JRaycast.EventSystem.FramePickingJActor as Interface.IPointerEnterHandler)?.OnPointerEnter(data);
                        data.Pointered = JRaycast.EventSystem.FramePickingJActor;
                        data.pointerEnter = JRaycast.EventSystem.FramePickingJActor;
                    }

                    if (null != data.Pointered)
                        (data.Pointered as Interface.IPointerHandler)?.OnPointer(data);

                    if (null != data.Pointered && data.Pointered != JRaycast.EventSystem.FramePickingJActor)
                    {
                        (data.Pointered as Interface.IPointerExitHandler)?.OnPointerExit(data);
                        data.Pointered = null;
                    }
                }
                #endregion

                #region [EventSystem] ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_ButtonUpdate(InputButton input, FrameState state, EventSystem.Data data)
                {
                    if (Get(input, state))
                    {
                        EventSystem.SelectLock();

                        if (null != data.selectedJActor)
                            (data.selectedJActor as IDeselectHandler)?.OnDeselect(data);

                        data.button = input;
                        data.lastPress = data.selectedJActor;
                        data.pressPosition = Position;
                        data.pressEventCamera = JRaycast.EventSystem.RaycastCamera;
                        data.selectedJActor = JRaycast.EventSystem.FramePickingJActor;

                        (data.selectedJActor as ISelectHandler)?.OnSelect(data);

                        switch (data.button)
                        {
                            case InputButton.Left:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerDown);
                                        break;
                                    case FrameState.DoubleClick:
                                        EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerDoubleClick);
                                        break;
                                    case FrameState.Pressed:
                                        break;
                                    case FrameState.Up:
                                        break;
                                }
                                break;
                            case InputButton.Right:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        EventSystem.Enqueue(EventSystem.InputEvent.RightPointerDown);
                                        break;
                                    case FrameState.DoubleClick:
                                        EventSystem.Enqueue(EventSystem.InputEvent.RightPointerDoubleClick);
                                        break;
                                    case FrameState.Pressed:
                                        break;
                                    case FrameState.Up:
                                        break;
                                }
                                break;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                #endregion

            }
            #endregion

            #region [LeftPointerDown] 
            internal class LeftPointerDown : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    data.pointerPress = data.selectedJActor;

                    (data.pointerPress as Interface.IPointerDownHandler)?.OnPointerDown(data);
                    (data.pointerPress as Interface.IInitializePotentialDragHandler)?.OnInitializePotentialDrag(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerClick);
                }
                #endregion
            }
            #endregion

            #region [LeftPointerDoubleClick] 
            internal class LeftPointerDoubleClick : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    data.pointerPress = data.selectedJActor;

                    (data.pointerPress as Interface.IPointerDownHandler)?.OnPointerDown(data);
                    (data.pointerPress as Interface.IPointerDoubleClickHandler)?.OnPointerDoubleClick(data);
                    (data.pointerPress as Interface.IInitializePotentialDragHandler)?.OnInitializePotentialDrag(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerClick);
                }
                #endregion
            }
            #endregion

            #region [LeftPointerClick] 
            internal class LeftPointerClick : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    if (data.pointerPress is Interface.IPointerClickHandler)
                        data.rawPointerPress = data.pointerPress;
                    else
                        (data.pointerPress as Interface.IPointerClickHandler)?.OnPointerPress(data);

                    if (Internal_PointerUpdate(deltaTime, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Left, FrameState.Pressed, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Left, FrameState.Up, data)) return;
                    EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerUp);
                }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // EventSystem
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [EventSystem] Internal_PointerUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_PointerUpdate(float deltaTime, EventSystem.Data data)
                {
                    if (!IsCursorViewportArea)
                    {
                        EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerUp);
                        return true;
                    }

                    if (IsRepositionCursor)
                    {
                        data.useDragThreshold = true;

                        if (data.useDragThreshold)
                        {
                            if (5f < (data.position - data.pressPosition).Length())
                            {
                                EventSystem.Enqueue(EventSystem.InputEvent.LeftBeginDrag);
                                return true;
                            }
                        }
                        else
                        {
                            EventSystem.Enqueue(EventSystem.InputEvent.LeftBeginDrag);
                            return true;
                        }
                    }
                    return false;
                }
                #endregion

                #region [EventSystem] Internal_ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_ButtonUpdate(InputButton input, FrameState state, EventSystem.Data data)
                {
                    if (Get(input, state))
                    {
                        switch (input)
                        {
                            case InputButton.Left:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerClick);
                                        break;
                                    case FrameState.Up:
                                        EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerUp);
                                        break;
                                }
                                break;
                            case InputButton.Right:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        break;
                                    case FrameState.Up:
                                        break;
                                }
                                break;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                #endregion
            }
            #endregion

            #region [LeftPointerUp]
            internal class LeftPointerUp : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    (data.pointerPress as Interface.IPointerUpHandler)?.OnPointerUp(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.Pointer);
                    EventSystem.SelectUnLock();
                }
                #endregion
            }
            #endregion

            #region [LeftBeginDrag] 
            internal class LeftBeginDrag : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    data.dragging = true;
                    data.pointerDrag = data.selectedJActor;

                    (data.pointerDrag as Interface.IBeginDragHandler)?.OnBeginDrag(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.LeftDrag);
                }
                #endregion
            }
            #endregion

            #region [LeftDrag] 
            internal class LeftDrag : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    (data.pointerDrag as Interface.IDragHandler)?.OnDrag(data);

                    if (Internal_PointerUpdate(deltaTime, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Left, FrameState.Pressed, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Left, FrameState.Up, data)) return;
                    EventSystem.Enqueue(EventSystem.InputEvent.LeftEndDrag);
                }
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // EventSystem
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [EventSystem] Internal_PointerUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_PointerUpdate(float deltaTime, EventSystem.Data data)
                {
                    if (!IsCursorViewportArea)
                    {
                        EventSystem.Enqueue(EventSystem.InputEvent.LeftEndDrag);
                        return true;
                    }
                    return false;
                }
                #endregion

                #region [EventSystem] Internal_ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_ButtonUpdate(InputButton input, FrameState state, EventSystem.Data data)
                {
                    if (Get(input, state))
                    {
                        switch (input)
                        {
                            case InputButton.Left:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        EventSystem.Enqueue(EventSystem.InputEvent.LeftDrag);
                                        break;
                                    case FrameState.Up:
                                        EventSystem.Enqueue(EventSystem.InputEvent.LeftEndDrag);
                                        break;
                                }
                                break;
                            case InputButton.Right:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        break;
                                    case FrameState.Up:
                                        break;
                                }
                                break;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                #endregion
            }
            #endregion

            #region [LeftEndDrag] 
            internal class LeftEndDrag : BaseEventSystem
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    (data.pointerDrag as Interface.IEndDragHandler)?.OnEndDrag(data);
                    (JRaycast.EventSystem.FramePickingJActor as Interface.IDropHandler)?.OnDrop(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.Pointer);
                    EventSystem.SelectUnLock();
                }
                #endregion
            }
            #endregion

            #region [RightPointerDown] 
            internal class RightPointerDown : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    data.pointerPress = data.selectedJActor;

                    (data.pointerPress as Interface.IPointerDownHandler)?.OnPointerDown(data);
                    (data.pointerPress as Interface.IInitializePotentialDragHandler)?.OnInitializePotentialDrag(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.RightPointerClick);
                }
                #endregion
            }
            #endregion

            #region [RightPointerDoubleClick] 
            internal class RightPointerDoubleClick : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    data.pointerPress = data.selectedJActor;

                    (data.pointerPress as Interface.IPointerDownHandler)?.OnPointerDown(data);
                    (data.pointerPress as Interface.IPointerDoubleClickHandler)?.OnPointerDoubleClick(data);
                    (data.pointerPress as Interface.IInitializePotentialDragHandler)?.OnInitializePotentialDrag(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.RightPointerClick);
                }
                #endregion
            }
            #endregion

            #region [RightPointerClick] 
            internal class RightPointerClick : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    if (data.pointerPress is Interface.IPointerClickHandler)
                        data.rawPointerPress = data.pointerPress;
                    else
                        (data.pointerPress as Interface.IPointerClickHandler)?.OnPointerPress(data);

                    if (Internal_PointerUpdate(deltaTime, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Right, FrameState.Pressed, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Right, FrameState.Up, data)) return;
                    EventSystem.Enqueue(EventSystem.InputEvent.LeftPointerUp);
                }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // EventSystem
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [EventSystem] ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_PointerUpdate(float deltaTime, EventSystem.Data data)
                {
                    if (!IsCursorViewportArea)
                    {
                        EventSystem.Enqueue(EventSystem.InputEvent.RightPointerUp);
                        return true;
                    }

                    if (IsRepositionCursor)
                    {
                        if (data.useDragThreshold)
                        {
                            if (data.dragThreshold < (data.position - data.pressPosition).Length())
                            {
                                EventSystem.Enqueue(EventSystem.InputEvent.RightBeginDrag);
                                return true;
                            }
                        }
                        else
                        {
                            EventSystem.Enqueue(EventSystem.InputEvent.RightBeginDrag);
                            return true;
                        }
                    }
                    return false;
                }
                #endregion

                #region [EventSystem] ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_ButtonUpdate(InputButton input, FrameState state, EventSystem.Data data)
                {
                    if (Get(input, state))
                    {
                        switch (input)
                        {
                            case InputButton.Left:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        break;
                                    case FrameState.Up:
                                        break;
                                }
                                break;
                            case InputButton.Right:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        EventSystem.Enqueue(EventSystem.InputEvent.RightPointerClick);
                                        break;
                                    case FrameState.Up:
                                        EventSystem.Enqueue(EventSystem.InputEvent.RightPointerUp);
                                        break;
                                }
                                break;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                #endregion
            }
            #endregion

            #region [RightPointerUp]
            internal class RightPointerUp : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    (data.pointerPress as Interface.IPointerUpHandler)?.OnPointerUp(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.Pointer);
                    EventSystem.SelectUnLock();
                }
                #endregion
            }
            #endregion

            #region [RightBeginDrag] 
            internal class RightBeginDrag : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    data.dragging = true;
                    data.pointerDrag = data.selectedJActor;
                    (data.pointerDrag as Interface.IBeginDragHandler)?.OnBeginDrag(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.RightDrag);
                }
                #endregion
            }
            #endregion

            #region [RightDrag] 
            internal class RightDrag : BaseEventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    (data.pointerDrag as Interface.IDragHandler)?.OnDrag(data);

                    if (Internal_PointerUpdate(deltaTime, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Right, FrameState.Pressed, data)) return;
                    if (Internal_ButtonUpdate(InputButton.Right, FrameState.Up, data)) return;
                    EventSystem.Enqueue(EventSystem.InputEvent.RightEndDrag);
                }
                #endregion

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // EventSystem
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [EventSystem] ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_PointerUpdate(float deltaTime, EventSystem.Data data)
                {
                    if (!IsCursorViewportArea)
                    {
                        EventSystem.Enqueue(EventSystem.InputEvent.RightEndDrag);
                        return true;
                    }
                    return false;
                }
                #endregion

                #region [EventSystem] ButtonUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private bool Internal_ButtonUpdate(InputButton input, FrameState state, EventSystem.Data data)
                {
                    if (Get(input, state))
                    {
                        switch (input)
                        {
                            case InputButton.Left:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        break;
                                    case FrameState.Up:
                                        break;
                                }
                                break;
                            case InputButton.Right:
                                switch (state)
                                {
                                    case FrameState.Down:
                                        break;
                                    case FrameState.Pressed:
                                        EventSystem.Enqueue(EventSystem.InputEvent.RightDrag);
                                        break;
                                    case FrameState.Up:
                                        EventSystem.Enqueue(EventSystem.InputEvent.RightEndDrag);
                                        break;
                                }
                                break;
                        }
                        return true;
                    }
                    else
                        return false;
                }
                #endregion
            }
            #endregion

            #region [RightEndDrag] 
            internal class RightEndDrag : BaseEventSystem
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public override void Update(float deltaTime, EventSystem.Data data)
                {
                    (data.pointerDrag as Interface.IEndDragHandler)?.OnEndDrag(data);
                    (JRaycast.EventSystem.FramePickingJActor as Interface.IDropHandler)?.OnDrop(data);

                    EventSystem.Enqueue(EventSystem.InputEvent.Pointer);
                    EventSystem.SelectUnLock();
                }
                #endregion
            }
            #endregion

        }
    }
}
