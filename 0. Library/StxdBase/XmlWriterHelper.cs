///////////////////////////////////////////////////////////////////////////////////
/// XmlWriterHelper.cs
///
/// Leevi 2003-5-29
/// 
///////////////////////////////////////////////////////////////////////////////////
///
/// Copyright (C) 2004 Leevi
///
/// This library is free software; you can redistribute it and/or
/// modify it under the terms of the GNU Lesser General Public
/// License as published by the Free Software Foundation; either
/// version 2.1 of the License, or (at your option) any later version.
/// 
/// This library is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
/// Lesser General Public License for more details.
/// 
/// You should have received a copy of the GNU Lesser General Public
/// License along with this library; if not, write to the Free Software
/// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
/// 
///////////////////////////////////////////////////////////////////////////////////
using System;
using System.Xml;
using System.Text;

namespace Kumaraji.Xml.Stxd
{
	public class XmlWriterHelper
	{
		private XmlWriterHelper(){}

		public static void AddAttribute(XmlWriter writer, string name, string value)
		{
			writer.WriteAttributeString(name, MaskInvalidCharacters(value));
		}

		public static void AddAttribute(XmlWriter writer, string name, string value, string defaultValue)
		{
			if( value == defaultValue )
				return ;
			AddAttribute(writer, name, value);
		}

		// 
		// Mask Invalid Characters
		// Character not in {#x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD]} will be masked
		public static string MaskInvalidCharacters(string s)
		{
			if (string.IsNullOrEmpty(s))
				return s;

			StringBuilder ret = null;
			for (int i = 0; i < s.Length; i++) {
				char ch = s[i];

				bool invalidChar = !((ch >= 0x20 && ch <= 0xD7FF) ||
					(ch >= 0xe000 && ch <= 0xfffd)
					|| ch == '\t' || ch == '\r' || ch == '\n');

				if (ret == null && invalidChar) {
					ret = new StringBuilder();
					ret.Append(s.Substring(0, i));
				}

				if (!invalidChar && ret != null) {
					ret.Append(ch);
				}
			}

			if (ret == null)
				return s;
			else
				return ret.ToString();
		}
	}
}
