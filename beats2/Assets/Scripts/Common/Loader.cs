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

		private static string _root;
		private static Dictionary<int, Texture2D> _textureCache;
		private static Dictionary<int, AudioClip> _audioCache;
		private const string FILE_PROTOCOL = "file://";

		public static void Init() {
			_root = GetRoot();
			Reset();
			Logger.Debug(TAG, "Initialized...");
		}

		public static void Reset() {
			_textureCache = new Dictionary<int, Texture2D>();
			_audioCache = new Dictionary<int, AudioClip>();
			Logger.Debug(TAG, "Reset...");
		}

		private static string GetRoot() {
			string dataPath = UnityEngine.Application.dataPath;
			if (dataPath.IndexOf('/') != -1) {
				string parentPath = dataPath.Substring(0, dataPath.LastIndexOf('/'));
				if (Directory.Exists(parentPath)) {
					return parentPath;
				}
			}
			Logger.Error(TAG, String.Format("Unable to find parent of dataPath: {0}", dataPath));
			return dataPath;
		}

		public static string GetPath(string fileName) {
			string filePath = String.Format("{0}{1}{2}", _root, Path.AltDirectorySeparatorChar, fileName);
			if (File.Exists(filePath)) {
				return FILE_PROTOCOL + filePath;
			} else {
				Logger.Error(TAG, String.Format("Unable to find file: {0}", filePath));
				return null;
			}
		}

		/// <summary>
		/// Returns a Texture2D ready to be used for a Graphics object
		/// </summary>
		public static Texture2D LoadTexture(string url, bool repeat) {
			Texture2D texture;
			int id = url.GetHashCode();
			if (_textureCache.ContainsKey(id)) {
				if (_textureCache.TryGetValue(id, out texture)) {
					return texture;
				}
			}

			Logger.Log(TAG, String.Format("Loading texture: {0}", url));
			WWW www = new WWW(url);
			while (!www.isDone); // FIXME: Blocks, thread this?
			texture = www.texture; // Compare with www.LoadImageIntoTexture(texture)?
			texture.wrapMode = (repeat) ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
			texture.Compress(true);
			www.Dispose();
			_textureCache.Add(id, texture);
			return texture;
		}

		public static void UnloadTexture(string url) {
			int id = url.GetHashCode();
			_textureCache.Remove(id);
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
