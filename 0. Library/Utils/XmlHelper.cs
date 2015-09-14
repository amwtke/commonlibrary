/*
 * Amigo II XmlHelper
 * used in XmlEncode
 * 
 * gaolei@amigo.bjmcc.net
 * created:	2006-04-25
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Imps.Services.CommonV4
{
	/// <summary>
	///	    Xml Encode
	/// </summary>
	public static class XmlHelper
	{

		public static string Encode(string s)
		{
			return Encode(s, true);
		}

		//
		// Any occurrence of & must be replaced by &amp;
		// Any occurrence of < must be replaced by &lt;
		// Any occurrence of > must be replaced by &gt;
		// Any occurrence of " (double quote) must be replaced by &quot;
		// Any occurrence of ' (simple quote) must be replaced by &apos;
		// Any character not in {#x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD]} must be masked or replace with &#x...
		public static string Encode(string s, bool maskInvaildCharacter)
		{
			if (string.IsNullOrEmpty(s))
				return s;

			StringBuilder ret = null;
			for (int i = 0; i < s.Length; i++) {
				char ch = s[i];

				string ts = null;
				bool invalidChar = false;

				if ((ch >= 0x20 && ch <= 0xD7FF) || (ch >= 0xe000 && ch <= 0xfffd) || ch == '\t' || ch == '\r' || ch == '\n') {
					switch (ch) {
						case '<':
							ts = "&lt;";
							break;
						case '>':
							ts = "&gt;";
							break;
						case '\"':
							ts = "&quot;";
							break;
						case '\'':
							ts = "&apos;";
							break;
						case '&':
							ts = "&amp;";
							break;
						case '\u000a':
							ts = "&#xa;";
							break;
						case '\u000d':
							ts = "&#xd;";
							break;
						default:
							// null
							break;
					}
				} else {
					// invaild character
					invalidChar = true;
				}

				if (ret == null && (ts != null || invalidChar)) {
					ret = new StringBuilder();
					ret.Append(s.Substring(0, i));
				}

				if (ts != null) {
					ret.Append(ts);
				} else if (invalidChar) {
					if (!maskInvaildCharacter)
						ret.AppendFormat("&#x{0:x};", (ushort)ch);
				} else if (ret != null) {
					ret.Append(ch);
				}
			}

			if (ret == null)
				return s;
			else
				return ret.ToString();
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

				bool invalidChar = !(
					(ch >= 0x20 && ch <= 0xD7FF) || 
					(ch >= 0xe000 && ch <= 0xfffd) || 
					ch == '\t' || 
					ch == '\r' ||
					ch == '\n');

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

		/// <summary>
		/// 将html格式文本转换成纯文本格式
		/// </summary>
		/// <param name="htmlText">html格式文本</param>
		/// <returns>转换后的纯文本</returns>
		public static String HtmlToText(String htmlText)
		{
			StringBuilder text = new StringBuilder();
			using (XmlReader reader = XmlReader.Create(new StringReader("<r>" + htmlText + "</r>"))) {
				try {
					while (reader.Read()) {
						switch (reader.NodeType) {
							case XmlNodeType.Text:
								text.Append(reader.Value);
								break;
							default:
								break;
						}
					}
				} catch (XmlException ex) {
					throw new XmlParseException("Xml has syntax error, parse failed.", ex);
				}
			}
			return text.ToString();
		}
	}

	public class XmlParseException : Exception
	{
		public XmlParseException(string message, Exception inner)
			: base(message, inner)
		{ }
	}
}
