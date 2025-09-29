using CLIInterface;
using CLIInterface.Component;
using CLIInterface.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace J2y
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// JActor
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public partial class JActor : Actor
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 변수
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [변수] Base
		private static int s_instance_indexer = 1;
		private int _instanceId;
		#endregion

		#region [변수] Components
		protected IList<ContainerComponent> _components;
		protected IDictionary<Type, ContainerComponent> _cache_components;
		#endregion

		#region [변수] Coroutines
		protected JCoroutines _coroutine;
		protected Dictionary<string, IEnumerator> _coroutine_string_map = new Dictionary<string, IEnumerator>();
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Property
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Property] Base
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public string Name { get { return Container.PropInstance.Name ?? ""; } set { Container.PropInstance.Name = value; } }
		public Container Container => GetOwnerContainer();
		public TransformGroup Transform { get; private set; }
		#endregion

		#region [Property] Active
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public bool ActiveSelf { get { return Container?.PropInstance?.Enabled ?? false; } }
		public bool ActiveInHierarchy { get { return Container?.PropInstance?.Enabled ?? false; } }
		#endregion

		#region [Property] Components        
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public IList<ContainerComponent> Components
		{
			get
			{
				return null == _components ? _components = new List<ContainerComponent>() : _components;
			}
			set
			{
				_components = value;
			}
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		protected IDictionary<Type, ContainerComponent> CacheComponents
		{
			get
			{
				return null == _cache_components ? _cache_components = new Dictionary<Type, ContainerComponent>() : _cache_components;
			}
			set
			{
				_cache_components = value;
			}
		}
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

			_instanceId = s_instance_indexer++;

			for (int i = 0; i < Container.GetComponentsCount(); ++i)
				AddComponent(Container.GetComponent(i).GetType());

			Transform = GetComponent<TransformGroup>() ?? AddNewComponent<TransformGroup>() as TransformGroup;

			JActor.Manager.Add(this);
			JEventHandler.ExecuteEvent(Evnet.OnCreateJActor, this);

			return 0;
		}
		#endregion

		#region [정리] OnDestroy
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override int OnDestroy()
		{
			base.OnDestroy();

			Transform = null;

			JEventHandler.ExecuteEvent(Evnet.OnDestroyJActor, this);
			JActor.Manager.Remove(this);

			ClearComponents();

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
		public new virtual void Update()
		{
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void LateUpdate()
		{
		}
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Component
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Component] [Get]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent GetComponent(Type type)
		{
			if (null == type)
				return null;
			if (CacheComponents.ContainsKey(type))
				return CacheComponents[type];

			var com = Components.FirstOrDefault(c => (c.GetType() == type) || c.GetType().IsSubclassOf(type));
			if (com != null)
				CacheComponents[type] = com;
			return com;
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public T GetComponent<T>() where T : ContainerComponent { return GetComponent(typeof(T)) as T; }
		internal ContainerComponent GetComponentByName(string type_name) { return GetComponent(JUtil.NameToType(type_name)); }
		public ContainerComponent GetComponent(string type_name) { return GetComponentByName(type_name); }
		#endregion

		#region [Component] [Get] InChildren/InParent
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent GetComponentInChildren(System.Type type, bool includeInactive) { return Container.GetComponentInChildren(type, includeInactive); }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent GetComponentInChildren(System.Type type) { return GetComponentInChildren(type, false); }
		public T GetComponentInChildren<T>() where T : ContainerComponent { return GetComponentInChildren<T>(false); }
		public T GetComponentInChildren<T>(bool includeInactive) where T : ContainerComponent { return GetComponentInChildren(typeof(T), includeInactive) as T; }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent GetComponentInParent(System.Type type) { return Container.GetComponentInParent(type); }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public T GetComponentInParent<T>() where T : ContainerComponent { return GetComponentInParent(typeof(T)) as T; }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public T[] GetComponents<T>() { return (T[])Container.GetComponentsInternal(typeof(T), true, false, true, false, (object)null); }
		public void GetComponents(System.Type type, List<ContainerComponent> results) { Container.GetComponentsInternal(type, false, false, true, false, (object)results); }
		public void GetComponents<T>(List<T> results) { Container.GetComponentsInternal(typeof(T), false, false, true, false, (object)results); }
		public ContainerComponent[] GetComponents(System.Type type) { return (ContainerComponent[])Container.GetComponentsInternal(type, false, false, true, false, (object)null); }

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent[] GetComponentsInChildren(System.Type type) { return GetComponentsInChildren(type, false); }
		public ContainerComponent[] GetComponentsInChildren(System.Type type, bool includeInactive) { return (ContainerComponent[])Container.GetComponentsInternal(type, false, true, includeInactive, false, (object)null); }
		public T[] GetComponentsInChildren<T>(bool includeInactive) { return (T[])Container.GetComponentsInternal(typeof(T), true, true, includeInactive, false, (object)null); }
		public void GetComponentsInChildren<T>(bool includeInactive, List<T> results) { Container.GetComponentsInternal(typeof(T), true, true, includeInactive, false, (object)results); }
		public T[] GetComponentsInChildren<T>() { return GetComponentsInChildren<T>(false); }
		public void GetComponentsInChildren<T>(List<T> results) { GetComponentsInChildren<T>(false, results); }

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent[] GetComponentsInParent(System.Type type) { return GetComponentsInParent(type, false); }
		public ContainerComponent[] GetComponentsInParent(System.Type type, bool includeInactive) { return (ContainerComponent[])Container.GetComponentsInternal(type, false, true, includeInactive, true, (object)null); }
		public T[] GetComponentsInParent<T>(bool includeInactive) { return (T[])Container.GetComponentsInternal(typeof(T), true, true, includeInactive, true, (object)null); }
		public void GetComponentsInParent<T>(bool includeInactive, List<T> results) { Container.GetComponentsInternal(typeof(T), true, true, includeInactive, true, (object)results); }
		public T[] GetComponentsInParent<T>() { return GetComponentsInParent<T>(false); }
		#endregion

		#region [Component] [Add]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		private ContainerComponent Internal_AddComponentWithType(System.Type componentType)
		{
			var com = Container.FindComponentByType(componentType.Name) as ContainerComponent;
			if (com == null)
				return null;
			Components.Add(com);
			initComponentInternal(com);
			return com;
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent AddComponent(string className) { return Internal_AddComponentWithType(JUtil.NameToType(className)); }
		public ContainerComponent AddComponent(System.Type componentType) { return Internal_AddComponentWithType(componentType); }
		#endregion

		#region [Component] [Add] New
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		private ContainerComponent Internal_AddNewComponentWithType(System.Type componentType)
		{
			var com = Container.AddNewComponent(componentType.Name);
			if (com == null)
				return null;
			Components.Add(com);
			initComponentInternal(com);
			return com;
		}

		////------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent AddNewComponent(string className) { return Internal_AddNewComponentWithType(JUtil.NameToType(className)); }
		public ContainerComponent AddNewComponent(System.Type componentType) { return Internal_AddNewComponentWithType(componentType); }
		public T AddNewComponent<T>() where T : ContainerComponent { return AddNewComponent(typeof(T)) as T; }
		#endregion

		#region [Component] [Add] Internal
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		private static void initComponentInternal(ContainerComponent com)
		{
			//Danuri 
		}
		#endregion

		#region [Component] Clear
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void ClearComponents()
		{
			var keys = new Type[CacheComponents.Keys.Count];
			var list = CacheComponents.ToList();
			for (var i = 0; i < CacheComponents.Keys.Count; ++i)
				keys[i] = list[i].Key;

			foreach (var key in keys)
				CacheComponents.Remove(key);

			while (Components.Count > 0)
				Components.RemoveAt(0);
		}
		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Coroutine
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Coroutine] Start
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public JCoroutines StartCoroutine(IEnumerator routine)
		{
			if (null == _coroutine)
				_coroutine = new JCoroutines();
			_coroutine.Start(routine);
			return _coroutine;
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public JCoroutines StartCoroutine(string routine)
		{
			var methodinfo = GetType().GetMethod(routine);
			if ((methodinfo.ReturnType == typeof(IEnumerator)) == false)
				return null;

			if (null == _coroutine)
				_coroutine = new JCoroutines();

			_coroutine_string_map.Add(routine, methodinfo.Invoke(this, null) as IEnumerator);
			_coroutine.Start(_coroutine_string_map[routine]);
			return _coroutine;
		}
		#endregion

		#region [Coroutine] StopAll
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void StopAllCoroutines()
		{
			if (null == _coroutine)
				return;
			_coroutine.StopAll();
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void StopCoroutine(IEnumerator routine)
		{
			if (null == _coroutine)
				_coroutine = new JCoroutines();
			_coroutine.Stop(routine);
		}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void StopCoroutine(string routine)
		{
			if (!_coroutine_string_map.ContainsKey(routine))
				return;

			var methodinfo = GetType().GetMethod(routine);
			if ((methodinfo.ReturnType == typeof(IEnumerator)) == false)
				return;

			if (null == _coroutine)
				_coroutine = new JCoroutines();

			_coroutine.Stop(_coroutine_string_map[routine]);
			_coroutine_string_map.Remove(routine);
		}
		#endregion

		#region [Coroutine] Pause
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static IEnumerator Pause(float time)
		{
			yield return JCoroutines.Pause(time);
		}
		#endregion

		#region [Coroutine] Cancel (todo)
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void CancelInvoke(string methodName) { }
		public void CancelInvoke() { }
		#endregion

		#region [Coroutine] Invoke (todo)
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void Invoke(string methodName, float time) { }
		public void InvokeRepeating(string methodName, float time, float repeatRate) { }
		public bool IsInvoking(string methodName) { return false; }
		public bool IsInvoking() { return false; }
		#endregion

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// JActor
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [JActor] Active
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void SetActive(bool value) { Container.PropInstance.Enabled = value; }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public void SetActiveRecursively(bool state)
		{
			SetActive(state);
		}
		#endregion

		#region [JActor] [Find] Name
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static T Find<T>(string name) where T : JActor
		{
			var found = JActor.Manager.root.Find(name);
			return (found != null) ? found.GetActor<T>() : null;
		}
		public static JActor Find(string name) { return Find<JActor>(name); }
		public static T[] FindAll<T>(string name) where T : JActor
		{
			var founds = JActor.Manager.root.FindAll(name);
			var results = new List<T>();
			T jactor = null;

			foreach (var con in founds)
			{
				jactor = con.GetActor<T>();

				if (jactor is T)
					results.Add(jactor);
			}

			return results.ToArray();
		}
		public static JActor[] FindAll(string name) { return FindAll<JActor>(name); }
		#endregion

		#region [JActor] [Find] FindComponentOfType
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static T FindComponentOfType<T>() where T : ContainerComponent { return JActor.Manager.root.GetComponentInChildren(typeof(T), false) as T; }
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static T[] FindComponentsOfType<T>() where T : ContainerComponent { return JActor.Manager.root.GetComponentsInternal(typeof(T), false, true, false, false, (object)null) as T[]; }
		#endregion

		#region [JActor] [Find] FindActorOfComponentType
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public static U FindActorOfComponentType<T, U>() where T : ContainerComponent where U : JActor
		{
			var found = (FindComponentOfType<T>() as ContainerComponent)?.GetOwnerContainer();
			return (found != null) ? found.GetActor<U>() : null;
		}
		public static JActor FindActorOfComponentType<T>() where T : ContainerComponent
		{
			var found = (FindComponentOfType<T>() as ContainerComponent)?.GetOwnerContainer();
			return (found != null) ? found.GetActor<JActor>() : null;
		}
		public static U[] FindActorOfComponentsType<T, U>() where T : ContainerComponent where U : JActor
		{
			var found = (ContainerComponent[])FindComponentsOfType<T>();
			var results = new List<U>();
			U jactor = null;

			foreach (var con in found)
			{
				jactor = con.GetOwnerContainer()?.GetActor<U>();
				if (jactor is U)
					results.Add(jactor);
			}

			return results.ToArray();
		}
		public static JActor[] FindActorOfComponentsType<T>() where T : ContainerComponent { return FindActorOfComponentsType<T, JActor>(); }
		#endregion


	}
}
