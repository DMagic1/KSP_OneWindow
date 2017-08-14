using System;
using System.Collections.Generic;
using OneWindow_Unity.Interfaces;
using UnityEngine;

namespace OneWindow_Unity.Unity
{
	public class Fake : WindowInterface
	{
		private string title = "Title";
		private string version = "v";
		private ContentLocation location = ContentLocation.UpperCenter;
		private bool showL;
		private bool showR;
		private bool showK;
		private bool showS;
		private Vector2 size = new Vector2(560, 300);
		private Vector2 pos = new Vector2(40, -100);

		public Fake()
		{
			location = ContentLocation.UpperCenter;
		}

		public string Title { get { return title; } }

		public string Version { get { return version; } }

		public bool ShowLocationIcon
		{
			get { return showL; }
			set { showL = value; }
		}

		public bool ShowRealTime
		{
			get { return showR; }
			set { showR = value; }
		}

		public bool ShowKSPTime
		{
			get { return showK; }
			set { showK = value; }
		}
		public bool ShowStandardMessages
		{
			get { return showS; }
			set { showS = value; }
		}

		public Vector2 Position
		{
			get { return pos; }
			set { pos = value; }
		}

		public Vector2 Size
		{
			get { return size; }
			set { size = value; }
		}

		public void ClampToScreen(RectTransform rect)
		{

		}

		public ContentLocation Location { get { return location; } set { location = value; } }
	}
}
