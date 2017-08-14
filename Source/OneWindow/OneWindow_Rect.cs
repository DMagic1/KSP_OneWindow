#region License
/*The MIT License (MIT)

One Window

OneWindow_Rect - Script to handle new screen messages

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
using OneWindow_Unity;

namespace OneWindow
{
	public class OneWindow_Rect : MonoBehaviour
	{
		private RectTransform _rect;
		private ScreenMessageStyle _style;
		private ContentLocation _location;

		public RectTransform Rect
		{
			get { return _rect; }
			set	{_rect = value;	}
		}

		public ScreenMessageStyle Style
		{
			set
			{
				_style = value;

				switch(value)
				{
					case ScreenMessageStyle.UPPER_LEFT:
						_location = ContentLocation.UpperLeft;
						break;
					case ScreenMessageStyle.UPPER_RIGHT:
						_location = ContentLocation.UpperRight;
						break;
					case ScreenMessageStyle.UPPER_CENTER:
						_location = ContentLocation.UpperCenter;
						break;
					case ScreenMessageStyle.LOWER_CENTER:
						_location = ContentLocation.LowerCenter;
						break;
				}
			}
		}

		private IEnumerator GetScreenMessageText(Transform t)
		{
			ScreenMessagesText text = t.GetComponent<ScreenMessagesText>();

			if (text == null)
				yield break;

			if (text.text.text.StartsWith("New TextNew"))
				yield break;

			OneWindow.UpdateMessage.Invoke(text);

			if (OneWindow_Settings.Instance.ShowStandardMessages || !OneWindow.Instance.IsVisible || ((ContentLocation)OneWindow_Settings.Instance.ContentLocation & _location) == 0)
			{
				t.SetParent(_rect, false);
				//OneWindow.Logging("Setting parent to old transform");
			}
			else
			{
				yield return new WaitForEndOfFrame();
				t.SetParent(null);
				//OneWindow.Logging("Setting parent null");
			}
		}

		private void OnTransformChildrenChanged()
		{
			if (transform.childCount == 0)
			{
				//OneWindow.Logging("No children...");
				return;
			}

			//OneWindow.Logging("Child change detected: {0}", transform.childCount);

			StartCoroutine(GetScreenMessageText(transform.GetChild(transform.childCount - 1)));
		}

	}
}
