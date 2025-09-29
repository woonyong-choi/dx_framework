//using System;
//using System.Collections.Generic;
//using CLIInterface;
//using CLIInterface.Component;
//using CLIInterface.Math3D;
//using CLIInterface.Scene;
//using CLIInterface.Script;

//namespace J2y
//{
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//	//
//	// JActor
//	//
//	//
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//	public class JActor : Actor
//	{
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 변수
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [변수] Base

//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// delegate
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [delegate] Declaration
//		public delegate void JActorDelegate(JActor jActor);
//		#endregion


//		#region [delegate] JActorOnCreateEvnet, JActorOnDestroyEvent, JActorEnableEvnet, JActorDisableEvent
//		public JActorDelegate JActorOnCreateEvnet { get; set; }
//		public JActorDelegate JActorOnDestroyEvent { get; set; }
//		public JActorDelegate JActorEnableEvnet { get; set; }
//		public JActorDelegate JActorDisableEvent { get; set; }
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// Property
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [Property]  Container, TransformGroup, Name
//		public Container Container { get; private set; }
//		public TransformGroup TransformGroup { get; private set; }
//		public string Name { get { return Container.PropInstance.Name; } }
//		#endregion


//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//		// 기본 함수
//		//
//		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//		#region [초기화] Static Constructor, Constructor, OnCreate
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		static JActor()
//		{ 
//		}
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public JActor()
//		{
//		}
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public override int OnCreate()
//		{
//			base.OnCreate();

//			Container		= GetOwnerContainer();
//			TransformGroup  = Container.GetComponent<TransformGroup>() ?? Container.AddNewComponent<TransformGroup>();

//			JActorOnCreateEvnet?.Invoke(this);

//			return 0;
//		}
//		#endregion


//		#region [정리] Destructor, OnDestroy
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		~JActor()
//		{
//		}
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public override int OnDestroy()
//		{
//			base.OnDestroy();

//			JActorOnDestroyEvent?.Invoke(this);

//			return 0;
//		}
//		#endregion


//		#region [활성화] OnEnable
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public override int OnEnable()
//		{
//			JActorEnableEvnet?.Invoke(this);

//			return 0;
//		}
//		#endregion


//		#region [비활성화] OnDisable
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public override int OnDisable()
//		{
//			JActorDisableEvent?.Invoke(this);

//			return 0;
//		}
//		#endregion


//		#region [업데이트] Update, LateUpdate
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public new virtual void Update()
//		{
//		}
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public virtual void LateUpdate()
//		{
//		}
//		#endregion


//	}
//}
