#region License
/*The MIT License (MIT)

One Window

OneWindow_Settings - Persistent settings file script

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
using System.IO;
using System.Reflection;
using UnityEngine;
using OneWindow_Unity;

namespace OneWindow
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class OneWindow_Settings : MonoBehaviour
    {
		[Persistent]
		public byte ContentLocation = 15;
		[Persistent]
		public byte GameSceneStatus = 15;
		[Persistent]
		public bool ShowLocationIcon = false;
		[Persistent]
		public bool ShowRealTime = false;
		[Persistent]
		public bool ShowKSPTime = true;
		[Persistent]
		public bool ShowStandardMessages = true;
		[Persistent]
		public string TimeFormat = "H:mm:ss";
		[Persistent]
		public Vector2 SCPosition = new Vector2(60, -100);
		[Persistent]
		public Vector2 EditorPosition = new Vector2(60, -100);
		[Persistent]
		public Vector2 TrackingPosition = new Vector2(60, -100);
		[Persistent]
		public Vector2 FlightPosition = new Vector2(60, -100);
		[Persistent]
		public Vector2 Size = new Vector2(560, 300);

		private static bool loaded;
		private static OneWindow_Settings instance;

		private const string fileName = "PluginData/Settings.cfg";
		private string fullPath;

		public static OneWindow_Settings Instance
		{
			get { return instance; }
		}

		private void Awake()
		{
			if (loaded)
			{
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);

			loaded = true;

			instance = this;

			fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName).Replace("\\", "/");

			if (Load())
				OneWindow.Logging("Settings file loaded");
			else
			{
				if (Save())
					OneWindow.Logging("New Settings files generated at:\n{0}", fullPath);
			}
		}

		public bool Load()
		{
			bool b = false;

			try
			{
				if (File.Exists(fullPath))
				{
					ConfigNode node = ConfigNode.Load(fullPath);
					ConfigNode unwrapped = node.GetNode(GetType().Name);
					ConfigNode.LoadObjectFromConfig(this, unwrapped);
					b = true;
				}
				else
				{
					OneWindow.Logging("Settings file could not be found [{0}]", fullPath);
					b = false;
				}
			}
			catch (Exception e)
			{
				OneWindow.Logging("Error while loading settings file from [{0}]\n{1}", fullPath, e);
				b = false;
			}

			return b;
		}

		public bool Save()
		{
			bool b = false;

			try
			{
				ConfigNode node = AsConfigNode();
				ConfigNode wrapper = new ConfigNode(GetType().Name);
				wrapper.AddNode(node);
				wrapper.Save(fullPath);
				b = true;
			}
			catch (Exception e)
			{
				OneWindow.Logging("Error while saving settings file from [{0}]\n{1}", fullPath, e);
				b = false;
			}

			return b;
		}

		private ConfigNode AsConfigNode()
		{
			try
			{
				ConfigNode node = new ConfigNode(GetType().Name);

				node = ConfigNode.CreateConfigFromObject(this, node);
				return node;
			}
			catch (Exception e)
			{
				OneWindow.Logging("Failed to generate settings file node...\n{0}", e);
				return new ConfigNode(GetType().Name);
			}
		}

    }
}
