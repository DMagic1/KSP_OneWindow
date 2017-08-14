#region License
/*The MIT License (MIT)

One Window

OneWindow_Loader - Script to load and process UI elements

Copyright (C) 2017 DMagic
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OneWindow_Unity;

namespace OneWindow
{
	[KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
	public class OneWindow_Loader : MonoBehaviour
	{
		private const string bundleName = "/one_window_prefabs";
		private const string bundlePath = "GameData/OneWindow/Resources";

		private static bool loaded;
		private static bool TMPLoaded;
		private static bool UILoaded;
		private static bool screenMessageLoaded;

		private static TMP_FontAsset _tmpFont;
		private static Material _tmpMaterial;

		private static Texture2D _toolbarIcon;

		private static GameObject[] loadedPrefabs;

		private static GameObject windowPrefab;

		public static GameObject WindowPrefab
		{
			get { return windowPrefab; }
		}

		public static Texture2D ToolbarIcon
		{
			get { return _toolbarIcon; }
		}

		private void Awake()
		{
			if (loaded)
			{
				Destroy(gameObject);
				return;
			}

			StartCoroutine(Process());
		}

		private IEnumerator Process()
		{
			while (ScreenMessages.Instance == null)
				yield return null;

			if (!screenMessageLoaded)
			{
				_tmpFont = ScreenMessages.Instance.textPrefab.text.font;
				_tmpMaterial = ScreenMessages.Instance.textPrefab.text.fontSharedMaterial;

				screenMessageLoaded = true;
			}

			if (!TMPLoaded && !UILoaded)
			{
				if (loadedPrefabs == null)
				{
					string path = KSPUtil.ApplicationRootPath + bundlePath;

					AssetBundle prefabs = AssetBundle.LoadFromFile(path + bundleName);

					if (prefabs != null)
						loadedPrefabs = prefabs.LoadAllAssets<GameObject>();
				}

				if (loadedPrefabs != null)
				{
					if (!TMPLoaded)
						processTMPPrefabs();

					if (UISkinManager.defaultSkin != null && !UILoaded)
						processUIPrefabs();
				}
			}

			if (_toolbarIcon == null)
				_toolbarIcon = GameDatabase.Instance.GetTexture("OneWindow/Resources/AppIcon", false);

			if (TMPLoaded && UILoaded)
				OneWindow.Logging("UI Loaded and processed");

			if (TMPLoaded && UILoaded)
				loaded = true;

			Destroy(gameObject);
		}

		private void processTMPPrefabs()
		{
			for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
			{
				GameObject o = loadedPrefabs[i];

				if (o.name == "OneWindow")
					windowPrefab = o;

				if (o != null)
					processTMP(o);
			}

			TMPLoaded = true;
		}

		private void processTMP(GameObject obj)
		{
			TextHandler[] handlers = obj.GetComponentsInChildren<TextHandler>(true);

			if (handlers == null)
				return;

			for (int i = 0; i < handlers.Length; i++)
				TMProFromText(handlers[i]);
		}

		private void TMProFromText(TextHandler handler)
		{
			if (handler == null)
				return;

			Text text = handler.GetComponent<Text>();

			if (text == null)
				return;

			string t = text.text;
			Color c = text.color;
			int i = text.fontSize;
			bool r = text.raycastTarget;
			FontStyles sty = TMPProUtil.FontStyle(text.fontStyle);
			TextAlignmentOptions align = TMPProUtil.TextAlignment(text.alignment);
			float spacing = text.lineSpacing;
			GameObject obj = text.gameObject;

			DestroyImmediate(text);

			OneWindow_TextMeshPro tmp = obj.AddComponent<OneWindow_TextMeshPro>();

			tmp.text = t;
			tmp.color = c;
			tmp.fontSize = i;
			tmp.raycastTarget = r;
			tmp.alignment = align;
			tmp.fontStyle = sty;
			tmp.lineSpacing = spacing;

			if (handler.MessageText)
			{
				tmp.font = _tmpFont;
				tmp.fontSharedMaterial = _tmpMaterial;
			}
			else
			{
				tmp.font = UISkinManager.TMPFont;
				tmp.fontSharedMaterial = Resources.Load("Fonts/Materials/Calibri Dropshadow", typeof(Material)) as Material;
			}

			tmp.enableWordWrapping = true;
			tmp.isOverlay = false;
			tmp.richText = true;
		}

		private void processUIPrefabs()
		{
			for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
			{
				GameObject o = loadedPrefabs[i];

				if (o != null)
					processUIComponents(o);
			}

			UILoaded = true;
		}

		private void processUIComponents(GameObject obj)
		{
			StyleHandler[] styles = obj.GetComponentsInChildren<StyleHandler>(true);

			if (styles == null)
				return;

			for (int i = 0; i < styles.Length; i++)
				processComponents(styles[i]);
		}

		private void processComponents(StyleHandler style)
		{
			if (style == null)
				return;

			UISkinDef skin = UISkinManager.defaultSkin;

			if (skin == null)
				return;

			switch (style.StlyeType)
			{
				case StyleHandler.StyleTypes.Box:
					style.setImage(skin.box.normal.background);
					break;
				case StyleHandler.StyleTypes.Window:
					style.setImage(skin.window.normal.background);
					break;
				case StyleHandler.StyleTypes.Button:
					style.setButton(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.active.background);
					break;
				case StyleHandler.StyleTypes.Toggle:
					style.setToggle(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.active.background);
					break;
				case StyleHandler.StyleTypes.VerticalScrollbar:
					style.setScrollbar(skin.verticalScrollbar.normal.background, skin.verticalScrollbarThumb.normal.background);
					break;
				default:
					break;
			}
		}
	}
}
