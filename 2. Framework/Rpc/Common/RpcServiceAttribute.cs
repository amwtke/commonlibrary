using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
	public class RpcServiceAttribute: Attribute
	{
		public string ServiceName
		{
			get { return _serviceName; }
		}

		public RpcServiceAttribute(string serviceName)
		{
			_serviceName = serviceName;
		}

		[Obsolete("NextVersion NotSure", true)]
		public bool IsTransparent
		{
			get;
			set;
		}

		private string _serviceName;
	}
}
