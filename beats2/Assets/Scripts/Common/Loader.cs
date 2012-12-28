using UnityEngine; // Keep for various data formats
using System;
using System.IO;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

/*
 * DONE
 */
namespace Beats2.Common {

	/// <summary>
	/// Resource loader. Wraps Unity's WWW class.
	/// </summary>
	public static class Loader {
		private const string TAG = "Loader";

		private static Dictionary<int, AudioClip> _audioCache;

		public static void Init() {
			Reset();
			Logger.Debug(TAG, "Initialized...");
		}

		public static void Reset() {
			_audioCache = new Dictionary<int, AudioClip>();
			Logger.Debug(TAG, "Reset...");
		}

		public static AudioClip LoadAudio(string url) {
			AudioClip audio;
			int id = url.GetHashCode();
			if (_audioCache.ContainsKey(id)) {
				if (_audioCache.TryGetValue(id, out audio)) {
					return audio;
				}
			}

			Logger.Log(TAG, String.Format("Loading audio: {0}", url));
			WWW www = new WWW(url);
			while (!www.isDone); // FIXME: Blocks, thread this?
			audio = www.GetAudioClip(false, false);
			www.Dispose();
			_audioCache.Add(id, audio);
			return audio;
		}

		public static void UnloadAudio(string url) {
			int id = url.GetHashCode();
			_audioCache.Remove(id);
		}

		public static string LoadText(string url) {
			string text;
			Logger.Log(TAG, String.Format("Loading text: {0}", url));
			WWW www = new WWW(url);
			while (!www.isDone);
			text = www.text;
			www.Dispose();
			return text;
		}
	}
}
