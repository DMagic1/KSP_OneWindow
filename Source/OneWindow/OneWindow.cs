#region License
/*The MIT License (MIT)

One Window

OneWindow - Main window controller

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

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using OneWindow_Unity.Interfaces;
using OneWindow_Unity;
using OneWindow_Unity.Unity;
using KSP.UI;

namespace OneWindow
{
	[KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
	public class OneWindow : MonoBehaviour, WindowInterface
	{
		public class OnCreateScreenMessage : UnityEvent<ScreenMessagesText> { }
		public class ScreenMessageTextAwake : UnityEvent<ScreenMessagesText> { }
		public class ScreenMessageTextDestroy : UnityEvent<ScreenMessagesText> { }

		public static ScreenMessageTextAwake OnScreenMessageAwake = new ScreenMessageTextAwake();
		public static ScreenMessageTextDestroy OnScreenMessageDestroy = new ScreenMessageTextDestroy();
		public static OnCreateScreenMessage UpdateMessage = new OnCreateScreenMessage();

		private static OneWindow instance;

		public static OneWindow Instance
		{
			get { return instance; }
		}

		private string _title = "One Window";
		private string _version;
		private bool _processed;

		private RectTransform _oldUpperLeft;
		private RectTransform _oldUpperRight;
		private RectTransform _oldUpperCenter;
		private RectTransform _oldLowerCenter;

		private OneWindow_Rect _upperLeftWatcher;
		private OneWindow_Rect _upperRightWatcher;
		private OneWindow_Rect _upperCenterWatcher;
		private OneWindow_Rect _lowerCenterWatcher;

		private ScreenMessages _screenMessages;

		private bool _isVisible;

		private Dictionary<ScreenMessagesText, OneWindow_Message> _messages = new Dictionary<ScreenMessagesText, OneWindow_Message>();

		private OneWindow_Unity.Unity.OneWindow _uiElement;

		public bool IsVisible
		{
			get { return _isVisible; }
		}

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;

			UpdateMessage.AddListener(new UnityAction<ScreenMessagesText>(MessageUpdate));
			OnScreenMessageAwake.AddListener(new UnityAction<ScreenMessagesText>(NewMessageText));
			OnScreenMessageDestroy.AddListener(new UnityAction<ScreenMessagesText>(DestroyMessageText));
			GameEvents.onGameSceneLoadRequested.Add(SceneLoad);
		}

		private void Start()
		{
			Assembly assembly = AssemblyLoader.loadedAssemblies.GetByAssembly(Assembly.GetExecutingAssembly()).assembly;
			var ainfoV = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
			switch (ainfoV == null)
			{
				case true: _version = ""; break;
				default: _version = ainfoV.InformationalVersion; break;
			}

			StartCoroutine(WaitForScreenMessages());
		}

		private void OnDestroy()
		{
			instance = null;

			UpdateMessage.RemoveListener(new UnityAction<ScreenMessagesText>(MessageUpdate));
			OnScreenMessageAwake.RemoveListener(new UnityAction<ScreenMessagesText>(NewMessageText));
			OnScreenMessageDestroy.RemoveListener(new UnityAction<ScreenMessagesText>(DestroyMessageText));
			GameEvents.onGameSceneLoadRequested.Remove(SceneLoad);

			if (OneWindow_Settings.Instance != null)
				OneWindow_Settings.Instance.Save();

			if (_uiElement != null)
				Destroy(_uiElement.gameObject);

			_uiElement = null;
		}

		private void SceneLoad(GameScenes scene)
		{
			if (ScreenMessages.Instance == null)
				return;

			ScreenMessages.Instance.upperLeft = _oldUpperLeft;
			ScreenMessages.Instance.upperRight = _oldUpperRight;
			ScreenMessages.Instance.upperCenter = _oldUpperCenter;
			ScreenMessages.Instance.lowerCenter = _oldLowerCenter;
		}

		private IEnumerator WaitForScreenMessages()
		{
			while (ScreenMessages.Instance == null)
				yield return null;

			Logging("Processing ScreenMessages");

			_screenMessages = ScreenMessages.Instance;

			_oldUpperLeft = _screenMessages.upperLeft;
			_oldUpperRight = _screenMessages.upperRight;
			_oldUpperCenter = _screenMessages.upperCenter;
			_oldLowerCenter = _screenMessages.lowerCenter;

			_upperLeftWatcher = new GameObject("ULWatcher", typeof(RectTransform), typeof(OneWindow_Rect), typeof(VerticalLayoutGroup)).GetComponent<OneWindow_Rect>();
			_upperRightWatcher = new GameObject("URWatcher", typeof(RectTransform), typeof(OneWindow_Rect), typeof(VerticalLayoutGroup)).GetComponent<OneWindow_Rect>();
			_upperCenterWatcher = new GameObject("UCWatcher", typeof(RectTransform), typeof(OneWindow_Rect), typeof(VerticalLayoutGroup)).GetComponent<OneWindow_Rect>();
			_lowerCenterWatcher = new GameObject("LCWatcher", typeof(RectTransform), typeof(OneWindow_Rect), typeof(VerticalLayoutGroup)).GetComponent<OneWindow_Rect>();

			_screenMessages.upperLeft = (RectTransform)_upperLeftWatcher.transform;
			_screenMessages.upperRight = (RectTransform)_upperRightWatcher.transform;
			_screenMessages.upperCenter = (RectTransform)_upperCenterWatcher.transform;
			_screenMessages.lowerCenter = (RectTransform)_lowerCenterWatcher.transform;

			_upperLeftWatcher.Rect = _oldUpperLeft;
			_upperLeftWatcher.Style = ScreenMessageStyle.UPPER_LEFT;

			_upperRightWatcher.Rect = _oldUpperRight;
			_upperRightWatcher.Style = ScreenMessageStyle.UPPER_RIGHT;

			_upperCenterWatcher.Rect = _oldUpperCenter;
			_upperCenterWatcher.Style = ScreenMessageStyle.UPPER_CENTER;

			_lowerCenterWatcher.Rect = _oldLowerCenter;
			_lowerCenterWatcher.Style = ScreenMessageStyle.LOWER_CENTER;

			ScreenMessages.Instance.textPrefab.gameObject.AddOrGetComponent<OneWindow_ScreenMessageListener>();

			_processed = true;

			Logging("ScreenMessages Transforms Processed");

			InitializeWindow();
		}

		private void InitializeWindow()
		{
			_uiElement = GameObject.Instantiate(OneWindow_Loader.WindowPrefab).GetComponent<OneWindow_Unity.Unity.OneWindow>();

			if (_uiElement == null)
				return;

			_uiElement.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform, false);

			_uiElement.Initialize(this);

			byte g = OneWindow_Settings.Instance.GameSceneStatus;
			bool active = false;

			switch(HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					active = (g & 1) != 0;
					break;
				case GameScenes.EDITOR:
					active = (g & 2) != 0;
					break;
				case GameScenes.SPACECENTER:
					active = (g & 4) != 0;
					break;
				case GameScenes.TRACKSTATION:
					active = (g & 8) != 0;
					break;
			}

			_isVisible = active;

			_uiElement.gameObject.SetActive(active);

			StartCoroutine(WaitForApp(active));
		}

		private IEnumerator WaitForApp(bool active)
		{
			while (OneWindow_AppLauncher.Instance == null || !OneWindow_AppLauncher.Instance.Ready)
				yield return null;

			OneWindow_AppLauncher.Instance.ToggleButtonState(active);
		}

		public void OpenWindow()
		{
			switch (HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					OneWindow_Settings.Instance.GameSceneStatus |= 1;
					break;
				case GameScenes.EDITOR:
					OneWindow_Settings.Instance.GameSceneStatus |= 2;
					break;
				case GameScenes.SPACECENTER:
					OneWindow_Settings.Instance.GameSceneStatus |= 4;
					break;
				case GameScenes.TRACKSTATION:
					OneWindow_Settings.Instance.GameSceneStatus |= 8;
					break;
			}

			_isVisible = true;

			if (_uiElement != null)
				_uiElement.gameObject.SetActive(true);
		}

		public void CloseWindow()
		{
			switch (HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					OneWindow_Settings.Instance.GameSceneStatus ^= 1;
					break;
				case GameScenes.EDITOR:
					OneWindow_Settings.Instance.GameSceneStatus ^= 2;
					break;
				case GameScenes.SPACECENTER:
					OneWindow_Settings.Instance.GameSceneStatus ^= 4;
					break;
				case GameScenes.TRACKSTATION:
					OneWindow_Settings.Instance.GameSceneStatus ^= 8;
					break;
			}

			_isVisible = false;

			if (_uiElement != null)
				_uiElement.gameObject.SetActive(false);
		}

		private void MessageUpdate(ScreenMessagesText message)
		{
			if (!_processed)
				return;

			var enumerator = _messages.GetEnumerator();

			//Logging("Checking for text update: {0}", message.text.text);

			while (enumerator.MoveNext())
			{
				var pair = enumerator.Current;

				if (pair.Key == message && pair.Value.Message != message.text.text)
				{
					//Logging("Updating message text: {0}", message.text.text);
					pair.Value.UpdateText(message.text.text);
					break;
				}
			}
		}

		private void NewMessageText(ScreenMessagesText message)
		{
			if (!_processed)
				return;

			//Logging("Message Created: {0}", message.text.text);

			ScreenMessage screenMessage = null;

			for (int i = ScreenMessages.Instance.ActiveMessages.Count - 1; i >= 0; i--)
			{
				ScreenMessage sm = ScreenMessages.Instance.ActiveMessages[i];

				if (sm.textInstance != message)
					continue;

				screenMessage = sm;
				break;
			}

			if (screenMessage == null)
				return;

			//Logging("Generating Screen Message");

			OneWindow_Message oneMessage = new OneWindow_Message(screenMessage);

			if (!_messages.ContainsKey(message))
				_messages.Add(message, oneMessage);

			_uiElement.AddMessage(oneMessage);
		}

		private void DestroyMessageText(ScreenMessagesText message)
		{
			if (!_processed)
				return;

			//Logging("Message Destroyed: {0}", message.text.text);

			var enumerator = _messages.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var pair = enumerator.Current;
				
				if (pair.Key == message && pair.Value.Message != message.text.text)
				{
					//Logging("Updating message text: {0}", message.text.text);
					pair.Value.UpdateText(message.text.text);
					break;
				}

				if (pair.Key == message)
				{
					//Logging("Removing Message");
					_messages.Remove(message);
					break;
				}
			}
		}

		public void ClampToScreen(RectTransform rect)
		{
			UIMasterController.ClampToScreen(rect, Vector2.zero);
		}

		public string Title
		{
			get { return _title; }
		}

		public string Version
		{
			get { return _version; }
		}

		public bool ShowLocationIcon
		{
			get { return OneWindow_Settings.Instance.ShowLocationIcon; }
			set { OneWindow_Settings.Instance.ShowLocationIcon = value; }
		}

		public bool ShowRealTime
		{
			get { return OneWindow_Settings.Instance.ShowRealTime; }
			set { OneWindow_Settings.Instance.ShowRealTime = value; }
		}

		public bool ShowKSPTime
		{
			get { return OneWindow_Settings.Instance.ShowKSPTime; }
			set { OneWindow_Settings.Instance.ShowKSPTime = value; }
		}

		public bool ShowStandardMessages
		{
			get { return OneWindow_Settings.Instance.ShowStandardMessages; }
			set { OneWindow_Settings.Instance.ShowStandardMessages = value; }
		}

		public Vector2 Position
		{
			get
			{
				switch(HighLogic.LoadedScene)
				{
					case GameScenes.SPACECENTER:
						return OneWindow_Settings.Instance.SCPosition;
					case GameScenes.TRACKSTATION:
						return OneWindow_Settings.Instance.TrackingPosition;
					case GameScenes.EDITOR:
						return OneWindow_Settings.Instance.EditorPosition;
					case GameScenes.FLIGHT:
						return OneWindow_Settings.Instance.FlightPosition;
					default:
						return new Vector2();
				}
			}
			set
			{
				switch(HighLogic.LoadedScene)
				{
					case GameScenes.SPACECENTER:
						OneWindow_Settings.Instance.SCPosition = value;
						break;
					case GameScenes.TRACKSTATION:
						OneWindow_Settings.Instance.TrackingPosition = value;
						break;
					case GameScenes.EDITOR:
						OneWindow_Settings.Instance.EditorPosition = value;
						break;
					case GameScenes.FLIGHT:
						OneWindow_Settings.Instance.FlightPosition = value;
						break;
					default:
						break;
				}
			}
		}

		public Vector2 Size
		{
			get { return OneWindow_Settings.Instance.Size; }
			set { OneWindow_Settings.Instance.Size = value; }
		}

		public ContentLocation Location
		{
			get { return (ContentLocation)OneWindow_Settings.Instance.ContentLocation; }
			set { OneWindow_Settings.Instance.ContentLocation = (byte)value; }
		}

		public static void Logging(string s, params object[] m)
		{
			Debug.Log(string.Format("[One_Window] " + s, m));
		}
		
	}
}
