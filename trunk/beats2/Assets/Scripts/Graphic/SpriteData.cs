using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

/*
 * DONE
 */
namespace Beats2.Graphic {

	public enum ScaleType {
		NONE,
		SCALED,
		SCALED_WIDTH,
		SCALED_HEIGHT
	}

	/// <summary>
	/// SpriteData. Wraps tk2d's tk2dSpriteCollectionDatas
	/// </summary>
	public class SpriteData {
		public float width, height, regionWidth, regionHeight;
		public tk2dSpriteCollectionData data;

		public SpriteData(Texture2D texture) : this(texture, 0f, 0f, ScaleType.NONE) {}
		public SpriteData(Texture2D texture, float width) : this(texture, width, 0f, ScaleType.SCALED_WIDTH) {}
		public SpriteData(Texture2D texture, float width, float height) : this(texture, width, height, ScaleType.SCALED) {}
		public SpriteData(Texture2D texture, float width, float height, ScaleType scaleType) {
			switch (scaleType) {
				case ScaleType.NONE:
					this.width = texture.width;
					this.height = texture.height;
					break;
				case ScaleType.SCALED_WIDTH:
					this.width = width;
					this.height = width * texture.height / texture.width;
					break;
				case ScaleType.SCALED_HEIGHT:
					this.width = height * texture.width / texture.height;
					this.height = height;
					break;
				case ScaleType.SCALED:
				default:
					this.width = width;
					this.height = height;
					break;
			}
			this.regionWidth = texture.width;
			this.regionHeight =
				texture.wrapMode == TextureWrapMode.Repeat ?
				this.height * texture.width / this.width :
				texture.height
			;

			Rect region = new Rect(0, 0, this.regionWidth, this.regionHeight);
			Vector2 anchor = new Vector2(this.regionWidth / 2, this.regionHeight / 2);
			tk2dRuntime.SpriteCollectionSize size = tk2dRuntime.SpriteCollectionSize.ForTk2dCamera();
			this.data = tk2dRuntime.SpriteCollectionGenerator.CreateFromTexture(texture, size, region, anchor);
		}

		/// <summary>
		/// Destroys the spriteCollection parent GameObject
		/// </summary>
		public void Destroy() {
			UnityEngine.Object.Destroy(data.gameObject);
		}
	}
}