#region License
/*The MIT License (MIT)

One Window

OneWindow_Message - Storage class for screen message information

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
using OneWindow_Unity.Interfaces;
using OneWindow_Unity.Unity;
using OneWindow_Unity;
using UnityEngine;

namespace OneWindow
{
	public class OneWindow_Message : MessageInterface
	{
		private string _realTime;
		private string _kspTime;
		private string _message;
		private double _timeStamp;
		private Color _textColor;
		private ContentLocation _location;
		private MessageContent _content;

		ScreenMessage _screenMessage;
		
		public OneWindow_Message(ScreenMessage message)
		{
			_screenMessage = message;

			_message = message.textInstance.text.text;

			_textColor = message.color;
			
			switch (message.style)
			{
				case ScreenMessageStyle.LOWER_CENTER:
					_location = ContentLocation.LowerCenter;
					break;
				case ScreenMessageStyle.UPPER_CENTER:
					_location = ContentLocation.UpperCenter;
					break;
				case ScreenMessageStyle.UPPER_LEFT:
					_location = ContentLocation.UpperLeft;
					break;
				case ScreenMessageStyle.UPPER_RIGHT:
					_location = ContentLocation.UpperRight;
					break;
			}

			_timeStamp = DateTime.Now.TimeOfDay.TotalSeconds;

			_realTime = "<size=14>" + DateTime.Now.ToString(OneWindow_Settings.Instance.TimeFormat) + "</size>";

			_kspTime = "<size=14>" + KSPUtil.PrintDateCompact(Planetarium.GetUniversalTime(), true, true) + "</size>";
		}

		public void SetMessage(MessageContent content)
		{
			_content = content;
		}

		public void UpdateText(string text)
		{
			_message = text;

			if (_content != null)
				_content.UpdateText(text, OneWindow_Settings.Instance.ShowRealTime, OneWindow_Settings.Instance.ShowKSPTime);
		}

		public string RealTime
		{
			get { return _realTime; }
		}

		public string KSPTime
		{
			get { return _kspTime; }
		}

		public string Message
		{
			get { return _message; }
		}

		public double TimeStamp
		{
			get { return _timeStamp; }
		}

		public Color TextColor
		{
			get { return _textColor; }
		}

		public ContentLocation Location
		{
			get { return _location; }
		}
	}
}
