using CLIInterface;
using CLIInterface.Component;
using CLIInterface.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace J2y
{
	public partial class JActor : Actor
	{

		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//
		// JActor.Manager
		//		
		//      1. All JActors (모든 객체 자동 등록)
		//      2. 유틸리티
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		public static class Manager
		{
			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
			// 변수
			//
			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			#region [변수]
			private static IDictionary<ulong, JActor> _jactors;
			private static Queue<JActor> _onstarts;
			private static JActor[] _frameUpdateJActors;
			#endregion


			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
			// Property
			// 
			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			#region [Property] Container Root
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static Container root { set; get; }
			#endregion

			#region [Property] Scheduler
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static JScheduler scheduler { set; get; }
			#endregion

			#region [Property] Jactors
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static IDictionary<ulong, JActor> JActors
			{
				get
				{
					if (null == _jactors)
						_jactors = new Dictionary<ulong, JActor>();
					return _jactors;
				}
				private set
				{
					if (null == value)
						_jactors = value;
				}
			}
			#endregion

			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
			// 기본 함수
			//
			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			#region [업데이트] Update
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static void Update(float deltaTime)
			{
				OnStart();
				Update();
				scheduler.Update();
				CoroutineUpdate(deltaTime);
				LateUpdate();
			}
			#endregion


			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
			// JActor
			//
			//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

			#region [JActor.Manager] 정적 생성자/소멸자
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			static Manager()
			{
				_jactors = new Dictionary<ulong, JActor>();
				_onstarts = new Queue<JActor>();
				scheduler = JScheduler.Instance;
				root = World.GetWorldContainer();
			}
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			static void Destructor(object sender, EventArgs e)
			{
			}
			#endregion

			#region [JActor] [추가]
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			internal static bool Add(JActor jactor)
			{
				if (Exist(jactor.Container.UID)) return false;
				else
				{
					JActors[jactor.Container.UID] = jactor;

					if (!_onstarts.Contains(jactor) && null != jactor.GetType().GetMethod("OnStart", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
						_onstarts.Enqueue(jactor);

					return true;
				}
			}
			#endregion

			#region [JActor] [제거]
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			internal static bool Remove(JActor jactor)
			{
				if (!Exist(jactor.Container.UID)) return false;
				else
				{
					JActors.Remove(jactor.Container.UID);
					if (_onstarts.Contains(jactor))
					{
						var list = _onstarts.ToList();
						list.Remove(jactor);
						_onstarts = new Queue<JActor>(list);
					}

					return true;
				}
			}
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static bool Remove(ulong uid) { return Remove(Find(uid)); }
			#endregion

			#region [JActor] [찾기]
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static JActor Find(ulong uid) { return Exist(uid) ? JActors[uid] : null; }
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			public static bool Exist(ulong uid) { return JActors.ContainsKey(uid); }
			#endregion

			#region [JActor] [업데이트]
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			internal static void OnStart()
			{
				while (0 < _onstarts.Count)
					JReflection.InvokeMethod(_onstarts.Dequeue(), "OnStart");

				_frameUpdateJActors = new JActor[JActors.Values.Count];
				JActors.Values.ToList().CopyTo(0, _frameUpdateJActors, 0, _frameUpdateJActors.Count());
			}
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			internal static void Update()
			{
				foreach (var jActor in _frameUpdateJActors)
					if(jActor.ActiveSelf) jActor.Update(); 
			}
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			internal static void CoroutineUpdate(float deltaTime)
			{
				foreach (var jActor in _frameUpdateJActors)
					if (jActor.ActiveSelf) jActor._coroutine?.Update(deltaTime);
			}
			//------------------------------------------------------------------------------------------------------------------------------------------------------
			internal static void LateUpdate()
			{
				foreach (var jActor in _frameUpdateJActors)
					if (jActor.ActiveSelf) jActor.LateUpdate();

				_frameUpdateJActors = null;
			}
			#endregion

		}
	}
}

