#region License
/*The MIT License (MIT)

One Window

WindowInterface - Interface for main window

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
using UnityEngine;

namespace OneWindow_Unity.Interfaces
{
	public interface WindowInterface
	{
		string Title { get; }

		string Version { get; }

		bool ShowLocationIcon { get; set; }

		bool ShowRealTime { get; set; }

		bool ShowKSPTime { get; set; }

		bool ShowStandardMessages { get; set; }

		Vector2 Position { get; set; }

		Vector2 Size { get; set; }

		ContentLocation Location { get; set; }

		void ClampToScreen(RectTransform rect);
	}
}
