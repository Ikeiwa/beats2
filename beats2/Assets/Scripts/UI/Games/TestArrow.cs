using UnityEngine;
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;
using Beats2.Graphic;

namespace Beats2.UI {
	
	public class TestArrow : BeatsObject<Sprite> {

		private static SpriteData _data;

		public static void Init() {
			// Create SpriteData
			float width = Screens.min * 0.10f;
			Texture2D texture = SpriteLoader.GetTexture(Sprites.SANDBOX_ARROW);
			_data = new SpriteData(texture, width);
		}

		public static TestArrow Instantiate() {
			// Create GameObject
			GameObject obj = new GameObject();
			obj.name = "TestArrow";
			obj.tag = Tags.SANDBOX_TEST_ARROW;

			// Add TestArrow Component
			TestArrow beatsObj = obj.AddComponent<TestArrow>();

			// Add Sprite Component
			Sprite sprite = obj.AddComponent<Sprite>();
			sprite.Setup(_data);
			beatsObj._sprite = sprite;

			// Add BoxCollider
			BoxCollider collider = obj.AddComponent<BoxCollider>();
			collider.size = sprite.dimensions;
			beatsObj._collider = collider;

			// Return instantiated BeatsObject
			return beatsObj;
		}

		public static void Cleanup() {
			_data.Destroy();
		}
	}
}
