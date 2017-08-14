#region License
/*The MIT License (MIT)

One Window

OneWindow_AppLauncher - Script add toolbar button

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
using KSP.UI.Screens;
using UnityEngine;

namespace OneWindow
{
	[KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
	public class OneWindow_AppLauncher : MonoBehaviour
	{
		private ApplicationLauncherButton _button;
		private bool _ready;

		private static OneWindow_AppLauncher _instance;

		public static OneWindow_AppLauncher Instance
		{
			get { return _instance; }
		}

		public bool Ready
		{
			get { return _ready; }
		}

		private void Awake()
		{
			if (_instance != null)
			{
				Destroy(gameObject);
				return;
			}

			_instance = this;
		}

		private void Start()
		{
			StartCoroutine(AddButton());
		}

		private void OnDestroy()
		{
			GameEvents.onGUIApplicationLauncherUnreadifying.Remove(RemoveButton);

			_instance = null;
		}

		private IEnumerator AddButton()
		{
			while (!ApplicationLauncher.Ready)
				yield return null;

			while (ApplicationLauncher.Instance == null)
				yield return null;

			_button = ApplicationLauncher.Instance.AddModApplication(OnTrue, OnFalse, null, null, null, null, ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT, OneWindow_Loader.ToolbarIcon);
			_ready = true;

			GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);
		}

		private void RemoveButton(GameScenes scene)
		{
			if (_button == null)
				return;

			ApplicationLauncher.Instance.RemoveModApplication(_button);

			_button = null;
			_ready = false;
		}

		public void ToggleButtonState(bool isOn)
		{
			if (_button == null)
				return;

			if (isOn)
				_button.SetTrue(false);
			else
				_button.SetFalse(false);
		}

		private void OnTrue()
		{
			if (OneWindow.Instance != null)
				OneWindow.Instance.OpenWindow();
		}

		private void OnFalse()
		{
			if (OneWindow.Instance != null)
				OneWindow.Instance.CloseWindow();
		}
        
	}
}
