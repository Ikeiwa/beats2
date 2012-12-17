using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

/*
 * DONE
 */
namespace Beats2.Common {
	
	/// <summary>
	/// Logger. Wraps Unity's Debug class.
	/// </summary>
	public static class Logger {
		private const string TAG = "Logger";
		
		public static bool debug { get; set; }
		public static string msg { get; set; }
		
		public static void Init() {
			msg = "";
			Reset();
			Logger.Debug(TAG, "Initialized...");
		}
		
		public static void Reset() {
			Logger.Debug(TAG, "Reset...");
		}

		public static void Debug(string tag, object obj) {
			if (debug) {
				msg = String.Format("D: {0}: {1}", tag, obj);
				UnityEngine.Debug.Log(msg);
			}
		}

		public static void Log(string tag, object obj) {
			msg = String.Format("L: {0}: {1}", tag, obj);
			UnityEngine.Debug.Log(msg);
		}
		
		public static void Warning(string tag, object obj) {
			msg = String.Format("W: {0}: {1}", tag, obj);
			UnityEngine.Debug.LogWarning(msg);
		}
		
		public static void Error(string tag, object obj) {
			msg = String.Format("E: {0}: {1}", tag, obj);
			UnityEngine.Debug.LogError(msg);
		}
		
		public static void Exception(string tag, object obj, Exception e) {
			msg = String.Format("X: {0}: {1}", tag, obj);
			UnityEngine.Debug.LogError(msg);
			UnityEngine.Debug.LogException(e);
		}
	}
}
