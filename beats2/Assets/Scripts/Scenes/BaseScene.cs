using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;
using Beats2.Data;
using Beats2.Graphic;
using Beats2.UI;

namespace Beats2.Scenes {
	public class BaseScene : MonoBehaviour {

		// Use this for initialization
		public virtual void Start() {}

		protected void InitAll() {
			// Order matters
			Logger.Init();
			SettingsManager.Init();
			StringsManager.Init();
			Screens.Init();
			SysInfo.Init();

			// Order doesn't matter
			Inputs.Init();
			Rand.Init();
			Score.Init();
			Vibrator.Init();
			Tracker.Init();
			SpriteLoader.Init();
			AudioLoader.Init();
		}

		public virtual void Update() {}

	}
}
