#region license
/*The MIT License (MIT)

One Window

MessageContent - Message content script

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

using UnityEngine;
using UnityEngine.UI;
using OneWindow_Unity.Interfaces;

namespace OneWindow_Unity.Unity
{
	public class MessageContent : MonoBehaviour
	{
		[SerializeField]
		private TextHandler m_Message = null;
		[SerializeField]
		private Image m_LocationIcon = null;
		[SerializeField]
		private Sprite m_ULIcon = null;
		[SerializeField]
		private Sprite m_URIcon = null;
		[SerializeField]
		private Sprite m_UCIcon = null;
		[SerializeField]
		private Sprite m_LCIcon = null;

		private MessageInterface mesInterface;
		private string _text;
		private Sprite _icon;

		public MessageInterface MesInterface
		{
			get { return mesInterface; }
		}

		public string Text
		{
			get {return _text;}
		}

		public void Initialize(MessageInterface mes)
		{
			if (mes == null)
				return;

			mesInterface = mes;
			_text = mes.Message;

			switch (mes.Location)
			{
				case ContentLocation.UpperLeft:
					_icon = m_ULIcon;
					break;
				case ContentLocation.UpperRight:
					_icon = m_URIcon;
					break;
				case ContentLocation.UpperCenter:
					_icon = m_UCIcon;
					break;
				case ContentLocation.LowerCenter:
					_icon = m_LCIcon;
					break;
			}

			if (m_LocationIcon != null)
			{
				if (_icon != null)
				{
					m_LocationIcon.sprite = _icon;
					m_LocationIcon.gameObject.SetActive(OneWindow.Instance.WinInterface.ShowLocationIcon);
				}
				else
					m_LocationIcon.gameObject.SetActive(false);
			}

			if (m_Message != null)
			{
				string text = string.Format("{0}{1}{2}", OneWindow.Instance.WinInterface.ShowRealTime ? mes.RealTime + "  " : "", OneWindow.Instance.WinInterface.ShowKSPTime ? mes.KSPTime + "  " : "",  mes.Message);

				m_Message.OnTextUpdate.Invoke(text);
				m_Message.OnColorUpdate.Invoke(mes.TextColor);
			}

			mes.SetMessage(this);
		}

		public void UpdateTimeStamps(bool realOn, bool kspOn)
		{
			string text = string.Format("{0}{1}{2}", realOn ? mesInterface.RealTime + "  " : "", kspOn ? mesInterface.KSPTime + "  " : "", _text);

			if (m_Message != null)
				m_Message.OnTextUpdate.Invoke(text);
		}

		public void UpdateLocation(bool isOn)
		{
			if (m_LocationIcon != null)
				m_LocationIcon.gameObject.SetActive(isOn);
		}

		public void UpdateText(string text, bool realOn, bool kspOn)
		{
			_text = text;

			string newText = string.Format("{0}{1}{2}", realOn ? mesInterface.RealTime + "  " : "", kspOn ? mesInterface.KSPTime + "  " : "", _text);
			
			if (m_Message != null)
				m_Message.OnTextUpdate.Invoke(newText);
		}

	}
}
