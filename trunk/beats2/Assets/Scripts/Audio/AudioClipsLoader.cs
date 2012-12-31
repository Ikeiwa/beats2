using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

namespace Beats2.Audio {

	public class AudioInfo : Attribute {
		public string name, path;
		public bool stream, loop;
		public AudioInfo(string name, string path, bool stream, bool loop) {
			this.name = name;
			this.path = path;
			this.stream = stream;
			this.loop = loop;
		}
	}

	public enum AudioClips {
		[AudioInfo("Sandbox_Song", "Sandbox/Song.mp3", true, true)] SANDBOX_SONG,
	}

	public static class AudioLoader {
		private const string TAG = "AudioLoader";
		private static Dictionary<AudioClips, AudioClip> _audioCache;

		public static void Init() {
			Reset();
			Logger.Debug(TAG, "Initialized...");
		}

		public static void Reset() {
			PreloadAudio();
			Logger.Debug(TAG, "Reset...");
		}

		private static void PreloadAudio() {
			int numAudio = Enum.GetNames(typeof(AudioClips)).Length;
			_audioCache = new Dictionary<AudioClips, AudioClip>(numAudio);

			foreach (AudioClips audio in Enum.GetValues(typeof(AudioClips))) {
				// Reflection magic!
				MemberInfo memberInfo = typeof(AudioClips).GetMember(audio.ToString()).FirstOrDefault();
				AudioInfo audioInfo = (AudioInfo)Attribute.GetCustomAttribute(memberInfo, typeof(AudioInfo));

				string path = SysInfo.GetPath(audioInfo.path);
#if !UNITY_ANDROID && UNITY_EDITOR
				// Temporary fix til I use an FMOD plugin
				path = path.Replace(".mp3", ".ogg");
#endif
				AudioClip clip = LoadAudio(path, audioInfo.stream);
				_audioCache.Add(audio, clip);
			}
		}

		public static AudioClip LoadAudio(string url, bool stream) {
			AudioClip clip;
			WWW www = new WWW(SysInfo.GetWwwPath(url));
			while (!www.isDone); // FIXME: Blocks, thread this?
			clip = www.GetAudioClip(false, stream);
			www.Dispose();
			return clip;
		}

		public static AudioClip GetAudioClip(AudioClips audio) {
			AudioClip clip;
			if (!_audioCache.TryGetValue(audio, out clip)) {
				Logger.Error(TAG, String.Format("Unable to fetch audio clip \"{0}\"", audio));
			}
			return clip;
		}
	}
}
