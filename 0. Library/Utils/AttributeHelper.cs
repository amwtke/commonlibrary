using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class AttributeHelper
	{
		public static T GetAttribute<T>(Type t) where T: Attribute
		{
			return GetAttribute<T>(t.GetCustomAttributes(typeof(T), false), t.Name, false);
		}

		public static T GetAttribute<T>(MethodInfo method) where T: Attribute
		{
			return GetAttribute<T>(method.GetCustomAttributes(typeof(T), false), method.Name, false);
		}

		public static T GetAttribute<T>(FieldInfo field) where T: Attribute
		{
			return GetAttribute<T>(field.GetCustomAttributes(typeof(T), false), field.Name, false);
		}

		public static T TryGetAttribute<T>(FieldInfo field) where T: Attribute
		{
			return GetAttribute<T>(field.GetCustomAttributes(typeof(T), false), field.Name, true);
		}

		public static T TryGetAttribute<T>(Type type) where T: Attribute
		{
			return GetAttribute<T>(type.GetCustomAttributes(typeof(T), false), type.Name, true);
		}

		private static T GetAttribute<T>(object[] attrs, string hostName, bool isTry)
		{
			if (attrs.Length == 0) {
				if (isTry)
					return default(T);
				else
					throw new Exception(string.Format("Attribute{0} not found in {1}", typeof(T).Name, hostName));
			}

			if (attrs.Length > 1)
				throw new Exception(string.Format("More than 1 Attribute{0} found in {1}", typeof(T).Name, hostName));

			if (attrs[0] is T)
				return (T)attrs[0];
			else
				throw new Exception("Unknown Type:" + typeof(T).Name);
		}
	}
}
