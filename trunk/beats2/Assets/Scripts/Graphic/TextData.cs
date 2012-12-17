using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Beats2;
using Beats2.Common;

namespace Beats2.Graphic {

	/// <summary>
	/// TextData. Wraps tk2d's tk2dFontData
	/// </summary>
	public class TextData {
		public tk2dFontData data;
		public float width, height;

		public TextData(string imgUrl, string textUrl) : this(Loader.LoadTexture(imgUrl, false), Loader.LoadText(textUrl)) {}
		public TextData(Texture2D texture, string textInfo) {
			GameObject obj = new GameObject();
			obj.name = "TextData";
			this.data = obj.AddComponent<tk2dFontData>();

			FontInfo fontInfo = FontBuilder.ParseBMFont(textInfo);
			FontBuilder.BuildFont(fontInfo, data, 1, 0, false, false, null, 0);

			Material fontMaterial = new Material(Shader.Find("tk2d/BlendVertexColor"));
			fontMaterial.mainTexture = texture;
			this.data.material = fontMaterial;
			this.width = this.data.largestWidth;
			this.height = this.data.lineHeight;
		}

		public void Destroy() {
			UnityEngine.Object.Destroy(data.gameObject);
		}
	}
}
