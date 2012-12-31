using UnityEngine;
using System;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;
using Beats2.Graphic;

namespace Beats2.UI {
	
	public class TestMine : BeatsObject<SpriteAnim> {

		private static SpriteAnimData _data;

		public static void Init() {
			// Create SpriteData
			float width = Screens.min * 0.20f;
			Texture2D texture = SpriteLoader.GetTexture(Sprites.SANDBOX_MINE);
			_data = new SpriteAnimData("TestMine", texture, width, 8);
		}

		public static TestMine Instantiate() {
			// Create GameObject
			GameObject obj = new GameObject();
			obj.name = "TestMine";
			obj.tag = Tags.SANDBOX_TEST_MINE;

			// Add TestMine Component
			TestMine beatsObj = obj.AddComponent<TestMine>();

			// Add Sprite Component
			SpriteAnim sprite = obj.AddComponent<SpriteAnim>();
			sprite.Setup(_data);
			beatsObj._sprite = sprite;

			// Add BoxCollider
			BoxCollider collider = obj.AddComponent<BoxCollider>();
			collider.size = sprite.dimensions;
			beatsObj._collider = collider;

			// Return instantiated BeatsObject
			return beatsObj;
		}

		public void Play() {
			_sprite.Play();
		}

		public static void Cleanup() {
			_data.Destroy();
		}
	}
}
