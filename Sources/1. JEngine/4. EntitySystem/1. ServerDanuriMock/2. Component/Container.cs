

#if NET_SERVER

namespace J2y
{

	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// Container
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class Container : ComponentArchivable
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 변수
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#region [변수] Parent
		protected Container _parentContainer;
#endregion

#region [변수] Components
		//protected IList<ContainerComponent> _components;
		//protected IDictionary<Type, ContainerComponent> _cache_components;
#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Property
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#region [Property] Components        
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		//public IList<ContainerComponent> Components
		//{
		//	get
		//	{
		//		return null == _components ? _components = new List<ContainerComponent>() : _components;
		//	}
		//	set
		//	{
		//		_components = value;
		//	}
		//}
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		//protected IDictionary<Type, ContainerComponent> CacheComponents
		//{
		//	get
		//	{
		//		return null == _cache_components ? _cache_components = new Dictionary<Type, ContainerComponent>() : _cache_components;
		//	}
		//	set
		//	{
		//		_cache_components = value;
		//	}
		//}
#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 기본 함수
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#region [초기화]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public Container()
			: base("Container")
		{
		}
#endregion

#region [정리]
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		~Container()
		{
			//Components.Clear();
			//CacheComponents.Clear();
		}
#endregion

#region [활성화] OnEnable
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void OnEnable()
		{
		}
#endregion

#region [비활성화] OnDisable
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public virtual void OnDisable()
		{
		}
#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Container
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

#region [Container][선언만 .. 작업은 안함] 
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public ContainerComponent AddNewComponent(string componentContainerTypeName)
		{
			//Debug

			return null;
		}
		public void ChangeParent(Container parentContainer)
		{
			//Debug
		}
		public bool DeleteContainer(Container deleteContainer)
		{
			//Debug

			return false;
		}
		public Container DetachChild(int idx)
		{
			//Debug

			return null;
		}
		public ContainerComponent FindComponentByType(string containerComponentTypeName)
		{
			//Debug

			return null;
		}
		public List<ContainerComponent> FindComponentsByType(string containerComponentTypeName)
		{
			//Debug

			return null;
		}
		public Container FindContainer(string containerName)
		{
			//Debug

			return null;
		}
		public int FindIndex(Container child)
		{
			//Debug

			return 0;
		}
		public Container GetChild(int idx)
		{
			//Debug

			return null;
		}
		public int GetChildCount()
		{
			//Debug

			return 0;
		}
		public ContainerComponent GetComponent(int index)
		{
			//Debug

			return null;
		}
		public int GetComponentsCount()
		{
			//Debug

			return 0;
		}
		public Container GetParent()
		{
			//Debug

			return null;
		}
		public void Insert(Container child)
		{
		}
		public Container LoadPrefab(string prefabPath)
		{
			//Debug

			return null;
		}
		public Container NewChild()
		{
			//Debug

			return null;
		}
		public void RemoveComponent(int index)
		{
			//Debug
		}
		public void RemoveComponent(string className)
		{
			//Debug
		}
		public void RemoveComponent(ContainerComponent component)
		{
			//Debug
		}
#endregion

	}

}

#endif
