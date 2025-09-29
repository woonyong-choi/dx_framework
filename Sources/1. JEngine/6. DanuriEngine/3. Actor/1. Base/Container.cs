using CLIInterface;
using CLIInterface.Component;
using CLIInterface.Script;
using System;
using System.Collections.Generic;
using System.Linq;

namespace J2y
{

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //
    // ContainerEx
    //
    //      요약:
    //          Container클래스에 대한 확장 메소드입니다.
    //
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public static class ContainerEx
    {
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // 확장 메소드
        //
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region [Container] IsActive
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsActive(this Container container) { return (null != container) && container.PropInstance.Enabled; }
        #endregion

        #region [Container] IsInactiveAnyParent
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsInactiveAnyParent(this Container container)
        {
            while (null != container)
            {
                if (container.IsActive()) return false;

                container = container.GetParent();
            }

            return true;
        }
        #endregion

        #region [Container] [Find]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Container Find(this Container container, string name)
        {
            var containers = new List<Container>();

            container.Internal_Find(container, name, ref containers);

            return 0 == containers.Count ? null : containers[0];
        }
        public static Container Find(this Container container, Enum name) { return container.Find(name.ToString()); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Container FindInParent(this Container container, string name)
        {
            var containers = new List<Container>();

            container.Internal_FindInParent(container, name, ref containers);

            return 0 == containers.Count ? null : containers[0];
        }
        public static Container FindInParent(this Container container, Enum name) { return container.Find(name.ToString()); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Container[] FindAll(this Container container, string name)
        {
            var containers = new List<Container>();

            container.Internal_Find(container, name, ref containers);

            return 0 == containers.Count ? null : containers.ToArray();
        }
        public static Container[] FindAll(this Container container, Enum name) { return container.FindAll(name.ToString()); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Container[] FindAllInParent(this Container container, string name)
        {
            var containers = new List<Container>();

            container.Internal_FindInParent(container, name, ref containers);

            return 0 == containers.Count ? null : containers.ToArray();
        }
        public static Container[] FindAllInParent(this Container container, Enum name) { return container.FindAll(name.ToString()); }
        #endregion

        #region [Container] [Find] Internal
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Internal_Find(this Container container, Container onwer, string name, ref List<Container> containers)
        {
            if (!onwer.IsActive()) return;

            if (name == onwer.PropInstance.Name)
                containers.Add(onwer);

            if (0 == onwer.GetChildCount())
                return;

            for (int i = 0; i < onwer.GetChildCount(); ++i)
                onwer.GetChild(i).Internal_Find(onwer.GetChild(i), name, ref containers);

        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Internal_FindInParent(this Container container, Container onwer, string name, ref List<Container> containers)
        {
            if (!onwer.IsActive()) return;

            if (name == onwer.PropInstance.Name)
                containers.Add(onwer);

            onwer.GetParent()?.Internal_Find(onwer.GetParent(), name, ref containers);
        }
        #endregion

        #region [Container] 
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static ContainerComponent AddNewComponent(this Container container, System.Type type) { return container.AddNewComponent(type.Name); }
        public static T AddNewComponent<T>(this Container container) where T : ContainerComponent { return container.AddNewComponent(typeof(T)) as T; }
        #endregion

        #region [Container] GetActor
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static T GetActor<T>(this Container container) where T : Actor { return (container.FindComponentByType("ScriptComponent") as ScriptComponent)?.GetActor() as T ?? null; }
        public static Actor GetActor(this Container container) { return container.GetActor<Actor>(); }
        public static JActor GetJActor(this Container container) { return container.GetActor<JActor>(); }
        #endregion

        #region [Container] Destroy, DestroyThis, DestroyAll, DestroyChildAll
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Destroy(this Container container, string name)
        {
            var destroyContainer = container?.Find(name) ?? null;

            if (null != (destroyContainer?.PropInstance ?? null))
                destroyContainer.GetParent().DeleteContainer(destroyContainer);
        }
        public static void Destroy(this Container container, Enum name) { container.Destroy(name.ToString()); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DestroyThis(this Container container) { container.GetParent().DeleteContainer(container); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DestroyAll(this Container container, string name)
        {
            var destroyContainer = container?.FindAll(name) ?? null;

            if (null == destroyContainer || 0 == destroyContainer.Length)
                return;

            foreach (var destroy in destroyContainer)
            {
                if (null != (destroy?.PropInstance ?? null))
                    destroy.GetParent().DeleteContainer(destroy);
            }
        }
        public static void DestroyAll(this Container container, Enum name) { container.DestroyAll(name.ToString()); }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DestroyChildAll(this Container container) { while (0 != container.GetChildCount()) container.DeleteContainer(container.GetChild(0)); }
        #endregion

        #region [Component] [Get]
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static ContainerComponent GetComponent(this Container container, string type) { return container.FindComponentByType(type); }
        public static ContainerComponent GetComponent(this Container container, System.Type type) { return container.GetComponent(type.Name); }
        public static T GetComponent<T>(this Container container) where T : ContainerComponent { return container.GetComponent(typeof(T)) as T; }
        public static ContainerComponent[] GetComponents(this Container container, System.Type type) { return (ContainerComponent[])container.GetComponentsInternal(type, false, false, true, false, (object)null); }
        public static void GetComponents(this Container container, System.Type type, List<ContainerComponent> results) { container.GetComponentsInternal(type, false, false, true, false, (object)results); }
        public static void GetComponents<T>(this Container container, List<T> results) { container.GetComponentsInternal(typeof(T), false, false, true, false, (object)results); }
        public static T[] GetComponents<T>(this Container container) { return (T[])container.GetComponentsInternal(typeof(T), true, false, true, false, (object)null); }
        #endregion

        #region [Component] [Get] Internal
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Array GetComponentsInternal(this Container container, System.Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList)
        {
            var components = (List<ContainerComponent>)resultList;
            if (components == null)
                components = new List<ContainerComponent>();

            var actor = container.GetActor<JActor>();

            if (null == actor)
            {
                var found_components = new List<ContainerComponent>();
                for (int i = 0; i < container.GetComponentsCount(); ++i)
                    found_components.Add(container.GetComponent(i));
                components.AddRange(found_components.Where(c => c.ActiveSelf()));
            }
            else
            {
                var found_components = actor.Components.Where(c => type.IsAssignableFrom(c.GetType()));
                if (!includeInactive)
                    found_components = found_components.Where(c => c.ActiveSelf());
                components.AddRange(found_components);
            }


            if (reverse)
                components.Reverse();

            if (recursive)
            {
                for (int i = 0; i < container.GetChildCount(); ++i)
                {
                    var child = container.GetChild(i);
                    var sub_coms = child.GetComponentsInternal(type, useSearchTypeAsArrayReturnType, recursive, includeInactive, reverse, resultList);
                    components.AddRange(sub_coms.Cast<ContainerComponent>());
                }
            }

            if (useSearchTypeAsArrayReturnType)
            {
                var new_array = Array.CreateInstance(type, components.Count);
                for (int i = 0; i < components.Count; ++i)
                    new_array.SetValue(components[i], i);
                return new_array;
            }
            return components.ToArray();
        }
        #endregion

        #region [Component] [Get] InChildren
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static ContainerComponent GetComponentInChildren(this Container container, System.Type t, bool includeInactive)
        {
            var com = container.GetComponent(t);
            if (com != null)
                return com;
            for (int i = 0; i < container.GetChildCount(); ++i)
            {
                com = container.GetChild(i).GetComponentInChildren(t, includeInactive);
                if (com != null)
                    return com;
            }
            return null;
        }
        #endregion

        #region [Component] [Get] InParent
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static ContainerComponent GetComponentInParent(this Container container, System.Type t)
        {
            var com = container.GetComponent(t);
            if (com != null)
                return com;
            if (null == container.GetParent())
                return null;

            return container.GetParent().GetComponentInParent(t);
        }
        #endregion

    }

}
