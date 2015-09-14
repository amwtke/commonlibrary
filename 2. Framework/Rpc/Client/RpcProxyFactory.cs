using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Collections.Generic;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	public static class RpcProxyFactory
	{
		private static object _syncRoot = new object();
		private static Dictionary<string, IRpcClientChannel> _channels = new Dictionary<string, IRpcClientChannel>();
		private static Dictionary<BaseUri, RpcProxyNexus> _nexuses = new Dictionary<BaseUri, RpcProxyNexus>();

		[Obsolete("Timeout需要在Channel或者RpcClientProxy上设置", true)]
		public static int Timeout
		{
			set { throw new NotSupportedException(); }
		}

		public static void RegisterClientChannel(IRpcClientChannel channel)
		{
			if (!_channels.ContainsKey(channel.Protocol)) {
				_channels.Add(channel.Protocol, channel);
			}
		}

		public static RpcClientProxy GetProxyDirect(string uri, string serviceName)
		{
			ServerUri u = ServerUri.Parse(uri);
			return GetProxyInner(u, null, serviceName);
		}

		public static RpcClientProxy GetProxy<T>(string uri)
		{
			ServerUri u = ServerUri.Parse(uri);
			return GetProxyInner<T>(u, null);
		}

		public static RpcClientProxy GetProxy<T>(ResolvableUri uri)
		{
			ServerUri solved = uri.Resolve(RouteMethod.Rpc);
			return GetProxyInner<T>(solved, uri);
		}

		private static RpcClientProxy GetProxyInner<T>(ServerUri solvedUri, ResolvableUri toUri)
		{
			var attr = AttributeHelper.GetAttribute<RpcServiceAttribute>(typeof(T));
			if (attr == null)
				throw new NotSupportedException("RpcInterface MUST markup with [RpcService] Attribute");

			return GetProxyInner(solvedUri, toUri, attr.ServiceName);
		}

		private static RpcClientProxy GetProxyInner(ServerUri solvedUri, ResolvableUri toUri, string serviceName)
		{
			if (_channels.Count == 0)
				throw new Exception("You *MUST* Register at least 1 client channel at first");

			IRpcClientChannel channel;
			if (!_channels.TryGetValue(solvedUri.Protocol, out channel)) {
				throw new Exception(string.Format("{0} protocol:'{1}' not found", solvedUri.ToString(), solvedUri.Protocol));
			}

			RpcProxyNexus nexus;
			lock (_syncRoot) {
				if (!_nexuses.TryGetValue(solvedUri, out nexus)) {
					ResolvableUri r = toUri as ResolvableUri;
					string serviceRole = r != null ? r.Service : "";
					nexus = new RpcProxyNexus(channel, solvedUri, serviceRole);
					_nexuses.Add(solvedUri, nexus);
				}
			}

			RpcClientProxy proxy = new RpcClientProxy(nexus, serviceName, toUri);
			return proxy;
		}
 	}
}
