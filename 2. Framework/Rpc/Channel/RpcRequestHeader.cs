using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcRequestHeader
	{
		public string ServerUri = string.Empty;		// ServerUri
		public string FromService = string.Empty;	// Service
		public string FromComputer = string.Empty;	// Computer
		public string Service = string.Empty;		// Target Service
		public string Method = string.Empty;		// Target Method
		public string ToUri = string.Empty;			// RoutableUri
		public bool HasBody = false;				// Body is null

		public string ServiceAtComputer
		{
			get
			{
				return FromService + '@' + FromComputer;
			}
			set
			{
				if (!SplitTwo(value, '@', out FromService, out FromComputer))
					throw new FormatException("Unexcepted Service@Computer: " + value);
			}
		}

		public string ServiceDotMethod
		{
			get
			{
				return Service + '.' + Method;
			}
			set
			{
				if (!SplitTwo(value, '.', out Service, out Method))
					throw new FormatException("Unexcepted Service@Computer: " + value);
			}
		}

		public static bool SplitTwo(string str, char sperator, out string left, out string right)
		{
			int l = str.IndexOf(sperator);
			if (l < 0) {
				left = string.Empty;
				right = string.Empty;
				return false;
			}

			left = str.Substring(0, l);
			right = str.Substring(l + 1);
			return true;
		}
	}
}
