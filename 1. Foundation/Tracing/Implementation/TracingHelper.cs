using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Imps.Services.CommonV4.Tracing
{
	class TracingHelper
	{
		public static string FormatMessage(string format, params object[] args)
		{
			try {
				return string.Format(format, args);
			} catch (Exception) {
				StringBuilder str = new StringBuilder();
				str.Append("FormatFailed: \"");
				str.Append(format);
				str.AppendFormat("\" Args({0}): ", args.Length);
				foreach (object obj in args) {
					str.Append(obj);
					str.Append(",");
				}
				return str.ToString();
			}
		}

		public static string FormatThreadInfo(Thread trd)
		{
			if (trd.IsThreadPoolThread) {
				return string.Format("P:{0}", trd.ManagedThreadId, trd.Name);
			} else if (trd.IsBackground) {
				if (string.IsNullOrEmpty(trd.Name))
					return string.Format("B:{0}", trd.ManagedThreadId, trd.Name);
				else
					return string.Format("B:{0} - {1}", trd.ManagedThreadId, trd.Name);
			} else {
				if (string.IsNullOrEmpty(trd.Name))
					return string.Format("{0}", trd.ManagedThreadId, trd.Name);
				else
					return string.Format("{0} - {1}", trd.ManagedThreadId, trd.Name);
			}
		}

		public static string FormatMethodInfo(MethodBase method)
		{
			StringBuilder sb = new StringBuilder();
			ParameterInfo[] pis = method.GetParameters();
			foreach (ParameterInfo pi in pis) {
				string pName = pi.Name;

			}
			MethodBody body = method.GetMethodBody();
			IList<LocalVariableInfo> lvis = body.LocalVariables;
			foreach (LocalVariableInfo lvi in lvis) {

			}
			return sb.ToString();
		}
	}
}
