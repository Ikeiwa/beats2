using UnityEngine;
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

namespace Beats2.UI {
	
	public class AudioPlayer : MonoBehaviour {

		private AudioSource _audioSrc;
		private List<string> _audioUrls;
		private Dictionary<int, AudioClip> _audioClips;

		public static AudioPlayer Instantiate() {
			// Create GameObject
			GameObject obj = new GameObject();
			obj.name = "MusicPlayer";
			obj.tag = Tags.MENU_MUSIC_PLAYER;

			// Add AudioPlayer Component
			AudioPlayer audioPlayer = obj.AddComponent<AudioPlayer>();
			audioPlayer._audioUrls = new List<string>();
			audioPlayer._audioClips = new Dictionary<int, AudioClip>();

			// Add AudioSource Component
			AudioSource audioSrc = obj.AddComponent<AudioSource>();
			audioPlayer._audioSrc = audioSrc;

			return audioPlayer;
		}

		public int Load(string audioUrl) {
			int id = audioUrl.GetHashCode();
			if (_audioClips.ContainsKey(id)) {
				return id;
			}

			AudioClip audio = Loader.LoadAudio(audioUrl);
			_audioClips.Add(id, audio);
			_audioUrls.Add(audioUrl);
			_audioSrc.clip = audio;
			return id;
		}

		public void Set(int audioId) {
			AudioClip clip;
			_audioClips.TryGetValue(audioId, out clip);
			_audioSrc.clip = clip;
		}

		public void Play() {
			_audioSrc.Play();
		}

		public void Pause() {
			_audioSrc.Pause();
		}

		public void Stop() {
			_audioSrc.Stop();
		}

		public bool isPlaying {
			get { return _audioSrc.isPlaying; }
		}

		public float volume {
			get { return _audioSrc.volume; }
			set { _audioSrc.volume = value; }
		}

		public bool loop {
			get { return _audioSrc.loop; }
			set { _audioSrc.loop = value; }
		}

		public float time {
			get {
				// AudioSource.time is inaccurate for compressed audio data
				//return audioSrc.time;
				return (float)_audioSrc.timeSamples / (float)_audioSrc.clip.frequency;
			}
			set { _audioSrc.time = value; }
		}

		public void Destroy(bool unloadAudio) {
			if (unloadAudio) {
				foreach (string audioUrl in _audioUrls) {
					Loader.UnloadAudio(audioUrl);
				}
			}
			UnityEngine.Object.Destroy(gameObject);
		}

	}
}
