using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public static class IICAssert
	{
		public static void IsTrue(bool condition)
		{
			IsTrue(condition, "");
		}

		public static void IsTrue(bool condition, string message)
		{
			if (!condition)
				throw new Exception("Assert Failed:" + message);
		}

		public static void AreEqual(object expected, object rval)
		{
			if (rval != expected) {
				string s = string.Format("Assert Failed: {0} != {1}", expected, rval);
				throw new Exception(s);
			}
		}

		public static void Is<T>(object obj)
		{
		}

		public static void Is<T>(object obj, string message)
		{
		}
	}
}
