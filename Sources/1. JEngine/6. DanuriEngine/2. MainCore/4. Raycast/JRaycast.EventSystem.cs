using CLIInterface;
using CLIInterface.Component;
using CLIInterface.Math3D;

namespace J2y
{
    namespace Danuri
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // JRaycast
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public partial class JRaycast
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //
            // EventSystem
            //
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            public static class EventSystem
            {

                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Property
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Property] Use
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static bool Use { get; set; } = true;
                #endregion

                #region [Property] Root
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static Container Root { get; set; }
                #endregion

                #region [Property] Picking
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static float FramePickingDistance { get; private set; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JActor FramePickingJActor { get; private set; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static Vector3 FramePickingPosition { get; private set; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static float PreviousFramePickingDistance { get; private set; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JActor PreviousFramePickingJActor { get; private set; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static Vector3 PreviousFramePickingPosition { get; private set; }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static JCamera RaycastCamera { get; private set; }
                #endregion


                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // 기본 함수
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [업데이트] Update
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static void Update(float deltaTime)
                {
                    if (Use)
                        Internal_PointerRaycastUpdate();
                }
                #endregion

                #region [업데이트] Internal_PointerRaycastUpdate
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                private static void Internal_PointerRaycastUpdate()
                {
                    var point = JPointer.Position;
                    var size = EGuiTools.GetScreenSize();

                    FramePickingDistance = 0f;
                    FramePickingJActor = null;
                    FramePickingPosition = new Vector3();

                    PreviousFramePickingDistance    = FramePickingDistance;
                    PreviousFramePickingJActor      = FramePickingJActor;
                    PreviousFramePickingPosition    = FramePickingPosition;

                    if(Root != null)
                    {
                        if (JCamera.Manager.ActiveUICamera?.RayCast(point, size, Root) ?? false)
                        {
                            FramePickingDistance = JCamera.Manager.ActiveUICamera.LastPickingDistance;
                            FramePickingJActor = JCamera.Manager.ActiveUICamera.LastPickingJActor;
                            FramePickingPosition = JCamera.Manager.ActiveUICamera.LastPickingPosition;
                            RaycastCamera = JCamera.Manager.ActiveUICamera;
                            return;
                        }
                        if (JCamera.Manager.ActiveCamera?.RayCast(point, size, Root) ?? false)
                        {
                            FramePickingDistance = JCamera.Manager.ActiveCamera.LastPickingDistance;
                            FramePickingJActor = JCamera.Manager.ActiveCamera.LastPickingJActor;
                            FramePickingPosition = JCamera.Manager.ActiveCamera.LastPickingPosition;
                            RaycastCamera = JCamera.Manager.ActiveCamera;
                            return;
                        }
                    }
                }
                #endregion
            }

        }
    }
}

