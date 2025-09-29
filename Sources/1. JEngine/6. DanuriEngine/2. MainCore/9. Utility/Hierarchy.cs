using CLIInterface.Component;
using CLIInterface.Scene;
using J2y.Interface;
using System.Collections.Generic;


namespace J2y
{
    namespace Danuri
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //
        // Hierarchy
        //
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public class Hierarchy
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Hierarchy
            //
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            #region [Hierarchy] [Component] [Find]
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> FindActiveComponents<T>() where T : ContainerComponent { return GetComponentsInChildren<T>(World.GetWorldContainer(), true); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> FindComponentAll<T>() where T : ContainerComponent { return GetComponentsInChildren<T>(World.GetWorldContainer(), true); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> GetComponentsInChildren<T>(Container con, bool includeInactive) where T : ContainerComponent
            {
                IList<T> outlist = new List<T>();

                Internal_GetJComponentsInChildren_Recursive(con, ref outlist, includeInactive);

                return outlist;
            }
            #endregion

            #region [Hierarchy] [Component] [Find] Internal
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            internal static void Internal_GetJComponentsInChildren_Recursive<T>(Container owner, ref IList<T> components, bool includeInactive) where T : ContainerComponent
            {
                if (owner.IsActive()) return;

                var comp = owner.GetComponent<T>();
                if (comp != null)
                    components.Add(comp);

                var size = owner.GetChildCount();
                for (int i = 0; i < size; i++)
                {
                    var child = owner.GetChild(i);
                    Internal_GetJComponentsInChildren_Recursive(child, ref components, includeInactive);
                }
            }
            #endregion

            #region [Hierarchy] [JActors] [Find]
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> FindActiveJActors<T>() where T : JActor { return GetJActorsInChildren<T>(World.GetWorldContainer(), true); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> FindJActorAll<T>() where T : JActor { return GetJActorsInChildren<T>(World.GetWorldContainer(), true); }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> GetJActorsInChildren<T>(Container con, bool includeInactive) where T : JActor
            {
                IList<T> outlist = new List<T>();

                Internal_GetJActorsInChildren_Recursive(con, ref outlist, includeInactive);

                return outlist;
            }
            #endregion

            #region [Hierarchy] [JActors] [Find] Internal
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            internal static void Internal_GetJActorsInChildren_Recursive<T>(Container owner, ref IList<T> actors, bool includeInactive) where T : JActor
            {
                if (owner.IsActive()) return;

                var actor = owner.GetActor<T>();
                if (actor != null)
                    actors.Add(actor);

                var size = owner.GetChildCount();
                for (int i = 0; i < size; i++)
                {
                    var child = owner.GetChild(i);
                    Internal_GetJActorsInChildren_Recursive(child, ref actors, includeInactive);
                }
            }
            #endregion

            #region [Hierarchy] [JActors] [Find]
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> FindActiveIEventSystemHandlers<T>() where T : class, IEventSystemHandler { return GetIEventSystemHandlersInChildren<T>(World.GetWorldContainer(), true) as IList<T>; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> FindIEventSystemHandlersAll<T>() where T : class, IEventSystemHandler { return GetIEventSystemHandlersInChildren<T>(World.GetWorldContainer(), true) as IList<T>; }
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public static IList<T> GetIEventSystemHandlersInChildren<T>(Container con, bool includeInactive) where T : class, IEventSystemHandler
            {
                IList<T> outlist = new List<T>();

                Internal_GetIEventSystemHandlersInChildren_Recursive(con, ref outlist, includeInactive);

                return outlist;
            }
            #endregion

            #region [Hierarchy] [JActors] [Find] Internal
            //------------------------------------------------------------------------------------------------------------------------------------------------------
            internal static void Internal_GetIEventSystemHandlersInChildren_Recursive<T>(Container owner, ref IList<T> interfaces, bool includeInactive) where T : class, IEventSystemHandler
            {
                if (!owner.IsActive()) return;

                var IEventSystemHandler = owner.GetActor<JActor>() as T;

                if (IEventSystemHandler != null)
                    interfaces.Add(IEventSystemHandler);

                var size    = owner.GetChildCount();
                var childs  = new Container[size];
                for (int i = 0; i < size; i++)
                    childs[i] = owner.GetChild(i);
                for (int i = 0; i < size; i++)
                    Internal_GetIEventSystemHandlersInChildren_Recursive(childs[i], ref interfaces, includeInactive);
            }
            #endregion


        }

    }
}
