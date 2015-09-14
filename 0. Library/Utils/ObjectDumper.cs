using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class ObjectDumper
	{
		public static bool DumpInner(StringBuilder str, object obj, string fieldName, int level)
		{
			if (str.Length > 8192) {
				str.Append("...TOO LONG...");
				return false;
			}

			if (obj == null) {
				DumpValue(str, obj, fieldName, level);
				return true;
			}

			Type type = obj.GetType();

			if (type.IsValueType || type == typeof(string)) {
				DumpValue(str, obj, fieldName, level);
				return true;
			}

			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (fields.Length == 0) {
				DumpValue(str, obj, fieldName, level);
				return true;
			} else {
				ApppendTabs(str, level);
				string typeName = ObjectHelper.GetTypeName(obj.GetType(), false);
				str.AppendFormat("{0}: {1} {{\r\n", fieldName, typeName, obj);
				foreach (FieldInfo field in fields) {
					object fieldObj = field.GetValue(obj);
					DumpInner(str, fieldObj, field.Name, level + 1);
				}
				ApppendTabs(str, level);
				str.Append("}");
			}
			return true;
		}

		public static bool DumpValue(StringBuilder str, object obj, string fieldName, int level)
		{
			ApppendTabs(str, level);
			if (obj == null) {
				str.AppendFormat("{0} = null\r\n", fieldName);
				return true;
			}

			IEnumerable e = obj as IEnumerable;
			string typeName = ObjectHelper.GetTypeName(obj.GetType(), false);

			if (e == null || obj.GetType() == typeof(string)) {
				str.AppendFormat("{0}: {1} = {2}\r\n", fieldName, typeName, obj);
			} else {
				int i = 0;
				str.AppendFormat("{0}: {1} = {{\r\n", fieldName, typeName);
				foreach (object o in e) {
					string s = string.Format("{0}[{1}]", fieldName, i + 1);
					if (!DumpInner(str, o, fieldName, level + 1))
						return false;
				}
				str.Append("}\r\n");
			}
			return true;
		}

		private static void ApppendTabs(StringBuilder str, int level)
		{
			for (int i = 0; i < level; i++) {
				str.Append("\t");
			}
		}
	}
}
