#region license
/*The MIT License (MIT)

One Window

OneWindow - Main window script

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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OneWindow_Unity.Interfaces;

namespace OneWindow_Unity.Unity
{
	public class OneWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
	{
		[SerializeField]
		private Toggle m_UpperLeftToggle = null;
		[SerializeField]
		private Toggle m_UpperRightToggle = null;
		[SerializeField]
		private Toggle m_UpperCenterToggle = null;
		[SerializeField]
		private Toggle m_LowerCenterToggle = null;
		[SerializeField]
		private Toggle m_DuplicateToggle = null;
		[SerializeField]
		private Toggle m_RealTimeToggle = null;
		[SerializeField]
		private Toggle m_KSPTimeToggle = null;
		[SerializeField]
		private Toggle m_LocationToggle = null;
		[SerializeField]
		private Slider m_SliderScale = null;
		[SerializeField]
		private TextHandler m_SliderText = null;
		[SerializeField]
		private TextHandler m_TitleText = null;
		[SerializeField]
		private RectTransform m_Content = null;
		[SerializeField]
		private ScrollRect m_Scroll = null;
		[SerializeField]
		private GameObject m_MessagePrefab = null;
		[SerializeField]
		private GameObject m_SettingsHolder = null;
		[SerializeField]
		private Vector2 _widthLimit = new Vector2(400, 800);
		[SerializeField]
		private Vector2 _heightLimit = new Vector2(300, 700);

		private bool _loaded = false;

		private WindowInterface _winInterface;

		public WindowInterface WinInterface
		{
			get { return _winInterface; }
		}

		private Animator _anim;

		private RectTransform _rect;
		private Vector2 _mouseStart;
		private Vector3 _windowStart;
		private Vector3 _resizeStart;

		private List<MessageContent> _allMessages = new List<MessageContent>();
		private List<MessageContent> _upperLeftMessages = new List<MessageContent>();
		private List<MessageContent> _upperRightMessages = new List<MessageContent>();
		private List<MessageContent> _upperCenterMessages = new List<MessageContent>();
		private List<MessageContent> _lowerCenterMessages = new List<MessageContent>();
		private List<MessageContent> _currentMessages = new List<MessageContent>();

		private static OneWindow _instance;

		public static OneWindow Instance
		{
			get { return _instance; }
		}

		private void Awake()
		{
			_rect = GetComponent<RectTransform>();
			_anim = GetComponent<Animator>();

			_instance = this;
		}

		public void Initialize(WindowInterface win)
		{
			if (win == null)
				return;

			_winInterface = win;

			if (m_TitleText != null)
				m_TitleText.OnTextUpdate.Invoke(win.Title + "  <size=10>" + win.Version + "</size>");

			if (m_DuplicateToggle != null)
				m_DuplicateToggle.isOn = win.ShowStandardMessages;

			if (m_RealTimeToggle != null)
				m_RealTimeToggle.isOn = win.ShowRealTime;

			if (m_KSPTimeToggle != null)
				m_KSPTimeToggle.isOn = win.ShowKSPTime;

			if (m_LocationToggle != null)
				m_LocationToggle.isOn = win.ShowLocationIcon;

			if (m_SliderScale != null)
				m_SliderScale.value = win.UIScale * 10;

			if (m_SliderText != null)
				m_SliderText.OnTextUpdate.Invoke("Scale: " + win.UIScale.ToString("P0"));

			if (m_SettingsHolder != null)
				m_SettingsHolder.SetActive(false);

			SetToggles();
			
			SetPosition(win.Position);

			SetSize(win.Size);

			SetScale(win.UIScale);

			_loaded = true;
		}

		private void SetPosition(Vector2 pos)
		{
			if (_rect == null || _winInterface == null)
				return;

			_rect.anchoredPosition = new Vector3(pos.x, pos.y, 0);
			
			_winInterface.ClampToScreen(_rect);
		}

		private void SetSize(Vector2 size)
		{
			if (_rect == null)
				return;

			if (size.x < _widthLimit.x)
				size.x = _widthLimit.x;
			else if (size.x > _widthLimit.y)
				size.x = _widthLimit.y;

			if (size.y < _heightLimit.x)
				size.y = _heightLimit.x;
			else if (size.y > _heightLimit.y)
				size.y = _heightLimit.y;

			_rect.sizeDelta = new Vector2(size.x, size.y);
		}

		private void SetScale(float scale)
		{
			if (_rect == null)
				return;

			_rect.localScale = Vector3.one * scale;
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (_rect == null)
				return;

			_mouseStart = eventData.position;
			_windowStart = _rect.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_rect == null)
				return;

			_rect.position = _windowStart + (Vector3)(eventData.position - _mouseStart);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (_rect == null || _winInterface == null)
				return;

			_winInterface.Position = new Vector2(_rect.anchoredPosition.x, _rect.anchoredPosition.y);

			_winInterface.ClampToScreen(_rect);
		}

		public void onBeginResize(BaseEventData eventData)
		{
			if (_rect == null || _winInterface == null)
				return;

			if (!(eventData is PointerEventData))
				return;

			_mouseStart = ((PointerEventData)eventData).position;
			_resizeStart = new Vector2(_winInterface.Size.x, _winInterface.Size.y);

		}

		public void onResize(BaseEventData eventData)
		{
			if (_rect == null)
				return;

			if (!(eventData is PointerEventData))
				return;

			float width = _resizeStart.x + (((PointerEventData)eventData).position.x - _mouseStart.x);
			float height = _resizeStart.y - (((PointerEventData)eventData).position.y - _mouseStart.y);

			if (width < _widthLimit.x)
				width = _widthLimit.x;
			else if (width > _widthLimit.y)
				width = _widthLimit.y;

			if (height < _heightLimit.x)
				height = _heightLimit.x;
			else if (height > _heightLimit.y)
				height = _heightLimit.y;

			_rect.sizeDelta = new Vector2(width, height);
		}

		public void onEndResize(BaseEventData eventData)
		{
			if (!(eventData is PointerEventData))
				return;

			if (_rect == null || _winInterface == null)
				return;

			_winInterface.Size = new Vector3(_rect.sizeDelta.x, _rect.sizeDelta.y);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			transform.SetAsLastSibling();
		}

		public void AddMessage(MessageInterface message)
		{
			MessageContent content = Instantiate(m_MessagePrefab).GetComponent<MessageContent>();

			if (content == null)
				return;

			content.Initialize(message);

			_allMessages.Add(content);

			bool add = false;

			switch (message.Location)
			{
				case ContentLocation.UpperLeft:
					_upperLeftMessages.Add(content);

					if ((_winInterface.Location & ContentLocation.UpperLeft) != 0)
						add = true;

					break;
				case ContentLocation.UpperRight:
					_upperRightMessages.Add(content);

					if ((_winInterface.Location & ContentLocation.UpperRight) != 0)
						add = true;

					break;
				case ContentLocation.UpperCenter:
					_upperCenterMessages.Add(content);

					if ((_winInterface.Location & ContentLocation.UpperCenter) != 0)
						add = true;

					break;
				case ContentLocation.LowerCenter:
					_lowerCenterMessages.Add(content);

					if ((_winInterface.Location & ContentLocation.LowerCenter) != 0)
						add = true;

					break;
			}

			if (add)
			{
				_currentMessages.Add(content);
				content.transform.SetParent(m_Content, false);
				content.transform.SetAsFirstSibling();
				m_Scroll.Rebuild(CanvasUpdate.Prelayout);				
			}
		}

		private void SetContent()
		{
			for (int i = _currentMessages.Count - 1; i >= 0; i--)
				_currentMessages[i].transform.SetParent(null, false);

			_currentMessages.Clear();

			if ((_winInterface.Location & ContentLocation.UpperLeft) != 0)
				_currentMessages.AddRange(_upperLeftMessages);
			
			if ((_winInterface.Location & ContentLocation.UpperRight) != 0)
				_currentMessages.AddRange(_upperRightMessages);

			if ((_winInterface.Location & ContentLocation.UpperCenter) != 0)
				_currentMessages.AddRange(_upperCenterMessages);

			if ((_winInterface.Location & ContentLocation.LowerCenter) != 0)
				_currentMessages.AddRange(_lowerCenterMessages);

			_currentMessages.Sort((a, b) => a.MesInterface.TimeStamp.CompareTo(b.MesInterface.TimeStamp));

			_currentMessages.Reverse();

			for (int i = 0; i < _currentMessages.Count; i++)
				_currentMessages[i].transform.SetParent(m_Content, false);
		}

		public void ToggleSettings(bool isOn)
		{
			if (_anim == null)
				return;

			_anim.SetBool("settings", isOn);
		}

		public void PrintToLog()
		{
			Debug.Log("[One_Window] Preparing Screen Message Data Dump...");

			for (int i = 0; i < _currentMessages.Count; i++)
			{
				MessageContent content = _currentMessages[i];

				Debug.Log(string.Format("Real Time: [{0}] - KSP Date: [{1}] - Message Location: [{2}]\n{3}", content.MesInterface.RealTime, content.MesInterface.KSPTime, content.MesInterface.Location, content.MesInterface.Message));
			}

			Debug.Log(string.Format("[One_Window] {0} Messages written to log file", _currentMessages.Count));
		}

		public void CycleFont()
		{
			if (_winInterface == null)
				return;

			int size = _winInterface.FontSize + 1;

			if (size > 3)
				size = 0;

			_winInterface.FontSize = size;
			
			for (int i = _allMessages.Count - 1; i >= 0; i--)
			{
				MessageContent content = _allMessages[i];

				content.UpdateTimeStamps(_winInterface.ShowRealTime, _winInterface.ShowKSPTime);
			}
		}

		public void UpdateScale(float scale)
		{
			if (!_loaded || m_SliderText == null)
				return;

			m_SliderText.OnTextUpdate.Invoke("Scale: " + (scale / 10).ToString("P0"));
		}

		public void SetUIScale()
		{
			if (_winInterface == null || m_SliderScale == null)
				return;

			_winInterface.UIScale = m_SliderScale.value / 10;

			SetScale(_winInterface.UIScale);
		}

		private bool NoToggled()
		{
			if (m_UpperLeftToggle != null && m_UpperLeftToggle.isOn)
				return false;

			if (m_UpperRightToggle != null && m_UpperRightToggle.isOn)
				return false;

			if (m_UpperCenterToggle != null && m_UpperCenterToggle.isOn)
				return false;

			if (m_LowerCenterToggle != null && m_LowerCenterToggle.isOn)
				return false;

			return true;
		}

		private void SetToggles()
		{
			if (m_UpperLeftToggle != null)
				m_UpperLeftToggle.isOn = (_winInterface.Location & ContentLocation.UpperLeft) != 0;

			if (m_UpperRightToggle != null)
				m_UpperRightToggle.isOn = (_winInterface.Location & ContentLocation.UpperRight) != 0;

			if (m_UpperCenterToggle != null)
				m_UpperCenterToggle.isOn = (_winInterface.Location & ContentLocation.UpperCenter) != 0;

			if (m_LowerCenterToggle != null)
				m_LowerCenterToggle.isOn = (_winInterface.Location & ContentLocation.LowerCenter) != 0;
		}

		public void ULToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			if (isOn)
				_winInterface.Location |= ContentLocation.UpperLeft;
			else
				_winInterface.Location ^= ContentLocation.UpperLeft;

			_loaded = false;
			
			SetToggles();

			if (NoToggled())
			{
				if (m_UpperLeftToggle != null)
					m_UpperLeftToggle.isOn = true;

				_winInterface.Location = ContentLocation.UpperLeft;

				_loaded = true;
				return;
			}

			_loaded = true;

			SetContent();
		}

		public void URToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			if (isOn)
				_winInterface.Location |= ContentLocation.UpperRight;
			else
				_winInterface.Location ^= ContentLocation.UpperRight;

			_loaded = false;

			SetToggles();

			if (NoToggled())
			{
				if (m_UpperRightToggle != null)
					m_UpperRightToggle.isOn = true;

				_winInterface.Location = ContentLocation.UpperRight;

				_loaded = true;
				return;
			}

			_loaded = true;

			SetContent();
		}

		public void UCToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			if (isOn)
				_winInterface.Location |= ContentLocation.UpperCenter;
			else
				_winInterface.Location ^= ContentLocation.UpperCenter;

			_loaded = false;

			SetToggles();

			if (NoToggled())
			{
				if (m_UpperCenterToggle != null)
					m_UpperCenterToggle.isOn = true;

				_winInterface.Location = ContentLocation.UpperCenter;

				_loaded = true;
				return;
			}

			_loaded = true;

			SetContent();
		}

		public void LCToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			if (isOn)
				_winInterface.Location |= ContentLocation.LowerCenter;
			else
				_winInterface.Location ^= ContentLocation.LowerCenter;

			_loaded = false;

			SetToggles();

			if (NoToggled())
			{
				if (m_LowerCenterToggle != null)
					m_LowerCenterToggle.isOn = true;

				_winInterface.Location = ContentLocation.LowerCenter;

				_loaded = true;
				return;
			}

			_loaded = true;

			SetContent();
		}

		public void DuplicateToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			_winInterface.ShowStandardMessages = isOn;
		}

		public void RealTimeToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			_winInterface.ShowRealTime = isOn;

			for (int i = _allMessages.Count - 1; i >= 0; i--)
			{
				MessageContent content = _allMessages[i];

				content.UpdateTimeStamps(isOn, _winInterface.ShowKSPTime);
			}
		}

		public void KSPTimeToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			_winInterface.ShowKSPTime = isOn;

			for (int i = _allMessages.Count - 1; i >= 0; i--)
			{
				MessageContent content = _allMessages[i];

				content.UpdateTimeStamps(_winInterface.ShowRealTime, isOn);
			}
		}

		public void IconToggle(bool isOn)
		{
			if (!_loaded || _winInterface == null)
				return;

			_winInterface.ShowLocationIcon = isOn;

			for (int i = _allMessages.Count - 1; i >= 0; i--)
			{
				MessageContent content = _allMessages[i];

				content.UpdateLocation(isOn);
			}
		}
	}
}
