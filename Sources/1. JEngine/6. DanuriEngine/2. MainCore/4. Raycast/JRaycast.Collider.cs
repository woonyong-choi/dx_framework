using CLIInterface;
using CLIInterface.Scene;
using CLIInterface.Component;
using CLIInterface.Math3D;
using J2y.Interface;
using System.Collections.Generic;

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
            // JRaycast.Collider
            //
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            public sealed class Collider
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Raycast
                //
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                #region [Raycast] RaycastAll
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IList<Hit<T>> RaycastAll<T>(int x, int y, Container root = null) where T : class, IColliderHandler
                {
                    return RaycastAll<T>(JCamera.Manager.ActiveCamera.Camera, x, y, root);
                }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IList<Hit<T>> RaycastAll<T>(Camera cam, int x, int y, Container root = null) where T : class, IColliderHandler
                {
                    var results = new List<Hit<T>>();
                    var colliders = Hierarchy.GetIEventSystemHandlersInChildren<T>((root is null ? World.GetWorldContainer() : root), true);
                    var destpoint = Vector3.Zero();

                    foreach (var collider in colliders)
                    {
                        if (!collider.UseCollider) continue;
                        if (collider.Container.IsInactiveAnyParent()) continue;
                        
                        if (cam.GetPickedObject(collider.Container, x, y, true, true))
                            results.Add(new Hit<T>(collider, cam.GetLastPickingDistance(), cam.GetLastPickingPosition()));
                    }
                    return results;
                }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IList<Hit<T>> RaycastAll<T>(Vector3 org, Vector3 dir, Container root = null) where T : class, IColliderHandler
                {
                    return RaycastAll<T>(JCamera.Manager.ActiveCamera.Camera, org, dir, root);
                }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IList<Hit<T>> RaycastAll<T>(Camera cam, Vector3 org, Vector3 dir, Container root = null) where T : class, IColliderHandler
                {
                    var results = new List<Hit<T>>();
                    var colliders = Hierarchy.GetIEventSystemHandlersInChildren<T>((root is null ? World.GetWorldContainer() : root), true);
                    var destpoint = Vector3.Zero();

                    foreach (var collider in colliders)
                    {
                        if (!collider.UseCollider) continue;
                        if (collider.Container.IsInactiveAnyParent()) continue;

                        if (cam.GetPickedObject(collider.Container, org, dir, true, true))
                            results.Add(new Hit<T>(collider, cam.GetLastPickingDistance(), cam.GetLastPickingPosition()));
                    }
                    return results;
                }
                #endregion

                #region [Raycast] LineIntersectAll
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IList<Hit<T>> LineIntersectAll<T>(Ray ray) where T : class, IColliderHandler
                {
                    var maxDistance = 10000;
                    var layerMask = int.MaxValue;
                    return LineIntersectAll<T>(ray, maxDistance, layerMask);

                }
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                public static IList<Hit<T>> LineIntersectAll<T>(Ray ray, float maxDistance, int layerMask, Container root = null) where T : class, IColliderHandler
                {
                    var results = new List<Hit<T>>();
                    var colliders = Hierarchy.GetIEventSystemHandlersInChildren<T>((root is null ? World.GetWorldContainer() : root), true);
                    var destpoint = Vector3.Zero();

                    foreach (var collider in colliders)
                    {
                        if(!collider.UseCollider) continue;
                        if (collider.Container.IsInactiveAnyParent()) continue;
                        if ((layerMask & collider.Container.PropInstance.Layer) == 0) continue;

                        destpoint = Vector3.Zero();

                        if (collider.Boundingbox.GetLineIntersect(ray.Origin, ray.GetPoint(maxDistance), ref destpoint))
                             results.Add(new Hit<T>(collider, DepthConverter((collider as JActor), destpoint, ray.Origin), destpoint));
                    }

                    return results;
                }
                #endregion

                #region [DepthConverter] 
                //------------------------------------------------------------------------------------------------------------------------------------------------------
                static internal float DepthConverter(JActor actor, Vector3 destpoint, Vector3 origin)
                {
                    return null != actor ? -EGuiTools.CalculateRaycastDepth(actor.Container) : (destpoint - origin).Length();
                }
                #endregion

            }

        }
    }
}

