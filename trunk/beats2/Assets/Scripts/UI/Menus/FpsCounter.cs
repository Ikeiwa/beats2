using UnityEngine;
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;
using Beats2.Graphic;

namespace Beats2.UI {

	public class FpsCounter : BeatsObject<Text> {

		private float _updateDiff;
		private float _timeDiff;
		private int _frameDiff;
		private float FPS_COUNTER_UPDATE_INTERVAL;

		public static FpsCounter Instantiate(TextData data, float fontHeight) {
			// Create GameObject
			GameObject obj = new GameObject();
			obj.name = "FPS Counter";
			obj.tag = Tags.UNTAGGED;

			// Add FpsCounter Component
			FpsCounter beatsObj = obj.AddComponent<FpsCounter>();
			beatsObj.FPS_COUNTER_UPDATE_INTERVAL = SettingsManager.GetValueFloat(Settings.FPS_COUNTER_UPDATE_INTERVAL);
			beatsObj.ResetCounter();

			// Add Sprite Component
			Text sprite = obj.AddComponent<Text>();
			sprite.Setup(data, data.width * fontHeight / data.height, fontHeight, TextAnchor.UpperRight);
			sprite.text = "XXX.XX FPS";
			beatsObj._sprite = sprite;

			// Return instantiated BeatsObject
			return beatsObj;
		}

		// Reset counters
		public void ResetCounter() {
			_updateDiff = FPS_COUNTER_UPDATE_INTERVAL;
			_timeDiff = 0f;
			_frameDiff = 0;
		}

		// Update FPS text
		public void OnUpdate() {
			_updateDiff -= Time.deltaTime;
			_timeDiff += Time.deltaTime / Time.timeScale; // More accurate than just Time.deltaTime
			_frameDiff++;
			
			if (_updateDiff <= 0f) {
				float fps = _frameDiff / _timeDiff;
				string text = System.String.Format(
					"{0:f2} FPS",
					fps
					);
				_sprite.text = text;
				if (fps < 30f) {
					_sprite.color = Color.red;
				} else if (fps < 45f) {
					_sprite.color = Color.yellow;
				} else {
					_sprite.color = Color.white;
				}
				ResetCounter();
			}
		}
	}
}