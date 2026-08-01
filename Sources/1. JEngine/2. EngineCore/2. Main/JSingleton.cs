//using System;
//using System.Collections.Generic;


//namespace J2y
//{
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//	//
//	// JSingleton
//	//
//	//
//	// @Usase
//	//		public class Manager : JSingleton<Manager> 
//	//		{ }	
//	//
//	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

//	public abstract class JSingleton<T> : JActor where T : JActor
//	{
//		#region [변수] Internal
//		private static T _instance = null;
//		private static object _syncobj = new object();
//		private static bool _appIsClosing = false;
//		#endregion

//		#region [Singleton] Instance
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		public static T Instance
//		{
//			get
//			{
//				if (_appIsClosing)
//					return null;

//				lock (_syncobj)
//				{
//					if (_instance == null)
//					{
//						T[] objs = GameObject.FindObjectsOfType<T>();

//						if (objs.Length > 0)
//							_instance = objs[0];

//						if (objs.Length > 1)
//							JLogger.WriteError("There is more than one " + typeof(T).Name + " in the scene.");

//						if (_instance == null)
//						{
//							string goName = typeof(T).ToString();
//							GameObject go = GameObject.Find(goName);
//							if (go == null)
//								go = new GameObject(goName);
//							GameObject.DontDestroyOnLoad(go);
//							_instance = go.AddComponent<T>();
//						}
//					}
//					return _instance;
//				}
//			}
//		}
//		#endregion

//		#region [종료] OnApplicationQuit
//		//------------------------------------------------------------------------------------------------------------------------------------------------------
//		protected virtual void OnApplicationQuit()
//		{
//			_appIsClosing = true;
//		}
//		#endregion

//	}

//}
