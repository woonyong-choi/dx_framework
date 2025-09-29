using CLIInterface;
using CLIInterface.Component;
using CLIInterface.Math3D;
using CLIInterface.Scene;
using J2y.Interface;
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
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 변수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [변수]
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Property
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Property] Camera
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Camera Camera { get; private set; }
            #endregion

            #region [Property] Picking
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public float LastPickingDistance { get; protected set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public JActor LastPickingJActor { get; protected set; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Vector3 LastPickingPosition { get; protected set; }
            #endregion


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 기본 함수
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [초기화] OnCreate
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnCreate()
            {
                base.OnCreate();

                Camera = GetComponent<Camera>() ?? AddNewComponent<Camera>() as Camera;

                JCamera.Manager.Add(this);

                return 0;
            }
            #endregion

            #region [정리] OnDestroy
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnDestroy()
            {
                base.OnDestroy();

                Camera = null;

                JCamera.Manager.Remove(this);

                return 0;
            }
            #endregion

            #region [활성화] OnEnable
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnEnable()
            {
                base.OnEnable();

                JEventHandler.ExecuteEvent(Manager.Evnet_Name_OnEnable, this);

                return 0;
            }
            #endregion

            #region [비활성화] OnDisable
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnDisable()
            {
                base.OnDisable();

                JEventHandler.ExecuteEvent(Manager.Evnet_Name_OnDisable, this);

                return 0;
            }
            #endregion

            #region [업데이트] Update, LateUpdate
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override void Update()
            {
                base.Update();

            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override void LateUpdate()
            {
                base.LateUpdate();
            }
            #endregion


            ////++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            //// RayCast
            ////
            ////++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [RayCast] 
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public virtual bool RayCast(Vector2 pos, Vector2 screensize, Container root = null)
            {
                var inpos = new Vector3(pos.x, pos.y, 0f);

                if (!Container.IsActive()) return false;

                var checkpos = this.ScreenToViewportPoint(inpos);

                if (checkpos.x < -1.0f || checkpos.x > 1.0f || checkpos.y < -1.0f || checkpos.y > 1.0f) return false;

                var result = false;
                var ray = new JRaycast.Ray(Transform.GetPosition(), this.ScreenToWorldPoint(inpos, screensize));
                var mask = Camera.PropCamera.LayerFilter;
                var dist = EGuiTools.GetCameraFarView(Camera) - EGuiTools.GetCameraNearView(Camera);
                var hits = JRaycast.Collider.RaycastAll<IColliderHandler>(Camera, (int)pos.x, (int)pos.y, root) as List<JRaycast.Hit<IColliderHandler>>;
                hits.Sort((v1, v2) => v1.Distance.CompareTo(v2.Distance));

                if (0 != hits.Count)
                {
                    LastPickingDistance = hits[0].Distance;
                    LastPickingJActor   = hits[0].Collider.Container.GetJActor();
                    LastPickingPosition = hits[0].Point;
                    result = true;
                }

                while (hits.Count > 0)
                    hits.RemoveAt(0);

                return result;
            }
            #endregion


        }


        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // CameraEx
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static class JCameraEx
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Camera
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Camera] 
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static JRaycast.Ray ScreenPointToRay(this JCamera cam, Vector3 screenpos)
            {
                var worldpos = ScreenToWorldPoint(cam, screenpos);
                return new JRaycast.Ray(worldpos, EGuiTools.GetTransformGroup(cam.Container).GetDirection());
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static JRaycast.Ray ScreenPointToRay(this JCamera cam, Vector3 screenpos, Vector2 screensize)
            {
                var worldpos = ScreenToWorldPoint(cam, screenpos, screensize);
                return new JRaycast.Ray(worldpos, EGuiTools.GetTransformGroup(cam.Container).GetDirection());
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ScreenToViewportPoint(this JCamera cam, Vector3 screenpos)
            {
                var size = EGuiTools.GetScreenSize();
                var depth = EGuiTools.GetCameraZDistance(cam.Camera);
                return new Vector3((screenpos.x / size.x) * 2.0f - 1.0f, (1.0f - (screenpos.y / size.y)) * 2.0f - 1.0f, screenpos.z / depth);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ScreenToViewportPoint(this JCamera cam, Vector3 screenpos, Vector2 screensize)
            {
                var depth = EGuiTools.GetCameraZDistance(cam.Camera);
                return new Vector3((screenpos.x / screensize.x) * 2.0f - 1.0f, (1.0f - (screenpos.y / screensize.y)) * 2.0f - 1.0f, screenpos.z / depth);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ScreenToWorldPoint(this JCamera cam, Vector3 screenpos)
            {
                var viewpos = ScreenToViewportPoint(cam, screenpos);
                return ViewportToWorldPoint(cam, viewpos);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ScreenToWorldPoint(this JCamera cam, Vector3 screenpos, Vector2 screensize)
            {
                var viewpos = ScreenToViewportPoint(cam, screenpos, screensize);
                return ViewportToWorldPoint(cam, viewpos);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ViewportToScreenPoint(this JCamera cam, Vector3 viewpos)
            {
                var size = EGuiTools.GetScreenSize();
                var depth = EGuiTools.GetCameraZDistance(cam.Camera);
                return new Vector3((viewpos.x + 1.0f) * 0.5f * size.x * 2, (1.0f - (viewpos.y + 1.0f) * 0.5f) * size.y, viewpos.z * depth);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ViewportToScreenPoint(this JCamera cam, Vector3 viewpos, Vector2 screensize)
            {
                var depth = EGuiTools.GetCameraZDistance(cam.Camera);
                return new Vector3((viewpos.x + 1.0f) * 0.5f * screensize.x * 2, (1.0f - (viewpos.y + 1.0f) * 0.5f) * screensize.y, viewpos.z * depth);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 ViewportToWorldPoint(this JCamera cam, Vector3 viewpos)
            {
                var viewMInv = cam.Camera.GetViewProjectionInverse(0);
                return Vector3.TransformCoord(viewpos, viewMInv);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 WorldToViewportPoint(this JCamera cam, Vector3 viewpos)
            {
                var viewMInv = cam.Camera.GetViewProjectionInverse(0);
                return Vector3.TransformCoord(viewpos, viewMInv);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 WorldToScreenPoint(this JCamera cam, Vector3 position)
            {
                var viewpos = WorldToViewportPoint(cam, position);
                return ViewportToScreenPoint(cam, viewpos);
            }
            //------------------------------------------------------------------------------------------------------------------------------------------------------	
            public static Vector3 WorldToScreenPoint(this JCamera cam, Vector3 position, Vector2 screensize)
            {
                var viewpos = WorldToViewportPoint(cam, position);
                return ViewportToScreenPoint(cam, viewpos, screensize);
            }
            #endregion
        }
    }
}
