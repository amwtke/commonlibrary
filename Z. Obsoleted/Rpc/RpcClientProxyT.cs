using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	[Obsolete("用不带泛型RpcClientProxy的代替, RpcClientProxy不在计划支持泛型的封装")]
	public class RpcClientProxy<T>
	{
		private RpcClientProxy _proxy;

		public RpcClientProxy(RpcClientProxy proxy)
		{
			_proxy = proxy;
		}

		public static implicit operator RpcClientProxy<T>(RpcClientProxy proxy)
		{
			return new RpcClientProxy<T>(proxy);
		}

		public string ServiceName
		{
			get { return _proxy.ServiceName; }
		}

		public void BeginInvoke<TArgs>(string methodName, TArgs args, Action<RpcClientContext> endDelegate)
		{
			_proxy.BeginInvoke(methodName, args, endDelegate);
		}
	}
}
