using UnityEngine;
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;
using Beats2.Graphic;

namespace Beats2.UI {
	
	public class TestText : BeatsObject<Text> {

		public static TestText Instantiate(TextData data, string text, float fontWidth, float fontHeight, TextAnchor anchor) {
			// Create GameObject
			GameObject obj = new GameObject();
			obj.name = "TestText";
			obj.tag = Tags.UNTAGGED;

			// Add TestText Component
			TestText beatsObj = obj.AddComponent<TestText>();

			// Add Sprite Component
			Text sprite = obj.AddComponent<Text>();
			sprite.Setup(data, fontWidth, fontHeight, anchor);
			sprite.text = text;
			beatsObj._sprite = sprite;

			// Return instantiated BeatsObject
			return beatsObj;
		}

		public string text {
			get { return _sprite.text; }
			set { _sprite.text = value; }
		}
	}
}

