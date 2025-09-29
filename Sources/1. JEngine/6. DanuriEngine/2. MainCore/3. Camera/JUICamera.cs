using CLIInterface;
using CLIInterface.Component;
using CLIInterface.EGui;
using CLIInterface.Math3D;
using J2y.Interface;
using System.Collections.Generic;


namespace J2y
{
    namespace Danuri
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // JUICamera
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public class JUICamera : JCamera
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

            #region [Property] EGuiCamera
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public EGuiCamera eguicamera { get; private set; }
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

                eguicamera = GetComponent<EGuiCamera>() ?? AddNewComponent<EGuiCamera>() as EGuiCamera;

                return 0;
            }
            #endregion

            #region [정리] OnDestroy
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnDestroy()
            {
                base.OnDestroy();

                eguicamera = null;

                return 0;
            }
            #endregion

            #region [활성화] OnEnable
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnEnable()
            {
                base.OnEnable();

                return 0;
            }
            #endregion

            #region [비활성화] OnDisable
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override int OnDisable()
            {
                base.OnDisable();

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


            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // RayCast
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [RayCast] [RayCast]
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public override bool RayCast(Vector2 pos, Vector2 screensize, Container root = null)
            {
                var inpos = new Vector3(pos.x, pos.y, 0f);

                if (!Container.IsActive()) return false;

                var checkpos = this.ScreenToViewportPoint(inpos);

                if (checkpos.x < -1.0f || checkpos.x > 1.0f || checkpos.y < -1.0f || checkpos.y > 1.0f) return false;

                var result = false;
                var ray = this.ScreenPointToRay(inpos, screensize);
                var mask = Camera.PropCamera.LayerFilter;
                var dist = EGuiTools.GetCameraFarView(Camera) - EGuiTools.GetCameraNearView(Camera);
                var hits = JRaycast.Collider.LineIntersectAll<IWidgetColliderHandler>(ray, dist, mask, root) as List<JRaycast.Hit<IWidgetColliderHandler>>;
                    hits.Sort((v1, v2) => v1.Distance.CompareTo(v2.Distance));

                if (0 != hits.Count)
                {
                    LastPickingDistance     = hits[0].Distance;
                    LastPickingJActor       = hits[0].Collider.Container.GetJActor();
                    LastPickingPosition     = hits[0].Point;
                    result = true;
                }

                while (hits.Count > 0)
                    hits.RemoveAt(0);

                return result;
            }
            #endregion
        }
    }
}
