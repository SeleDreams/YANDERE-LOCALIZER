using System;
using UnityEngine;
using MoonSharp.Interpreter;
using YandereNext.Debugging;
using UnityEngine.SceneManagement;

namespace YandereNext.LUA
{
	public static class LUAScriptLinking
	{
		public static void SetGlobals<T>(string key, T table)
		{
			LUAScriptManager.ScriptInstance.Globals[key] = table;
		}

		public static object GetGlobals(string key)
		{
			return LUAScriptManager.ScriptInstance.Globals[key];
		}

		public static void CallFunction(string name, params object[] args)
		{
			try
			{
				LUAScriptManager.ScriptInstance.Call(GetGlobals(name), args);
			}
			catch (Exception ex)
			{
				if (!(ex is ArgumentException))
				{
					Debug.Log(ex.GetType().ToString() + " : " + ex.Message);
				}
			}
		}

		public static void StartLinking()
		{
			Debug.Log("Registering types");
			RegisterTypes();
			Debug.Log("Linking Data");
			LinkData();
		}

		public static void SendToLUA<T>(Func<T> method = null)
		{
			UserData.RegisterType<T>();
			Type t = typeof(T);
			SetGlobals(t.Name, t);
			if (method != null)
			{
				SetGlobals(method.Method.Name, method);
			}
		}

		public static void RegisterTypes()
		{
			//UserData.RegistrationPolicy = InteropRegistrationPolicy.;
			SendToLUA<Debug>();
			SendToLUA<Input>();
			SendToLUA<SceneManager>();
			SendToLUA<LogConsole>();
			SendToLUA<Localizer>();
			SendToLUA<TextExtractor>();
		}

		// Links properties returning informations to LUA
		public static void LinkData()
		{
			SetGlobals("RootDir", YandereNextManager.RootDir);
			SetGlobals("PluginsDir", YandereNextManager.PluginsDir);
			SetGlobals("ModDir", YandereNextManager.ModDir);
			SetGlobals("BundlesDir", YandereNextManager.BundlesDir);
		}
	}
}
