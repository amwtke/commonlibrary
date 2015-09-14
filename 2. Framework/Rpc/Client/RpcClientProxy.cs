using System;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	//
	// New Every Time
	public sealed class RpcClientProxy
	{
		private static RpcClientPerfCounter _perfCounters = IICPerformanceCounterFactory.GetCounters<RpcClientPerfCounter>();

		#region Protected Fields
		private int _timeout = -1;
		private string _serviceName;
		private BaseUri _toUri;
		private RpcProxyNexus _nexus;
		#endregion

		#region Public Properties
		public string ServiceName
		{
			get { return _serviceName; }
		}

		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}
		#endregion

		internal RpcClientProxy(RpcProxyNexus nexus, string serviceName, BaseUri toUri)
		{
			_toUri = toUri;
			_nexus = nexus;
			_serviceName = serviceName;
			// p_serviceUrl = channel.FormatUrl(uri, p_serviceName);
			// p_from = ServiceEnviornment.ServiceName + "@" + ServiceEnviornment.ComputerName;
			// p_to = p_serviceUrl;
		}

		public void BeginInvoke(string methodName, Action<RpcClientContext> callback)
		{
			BeginInvoke<RpcNull>(methodName, null, callback);
		}

		public void BeginInvoke<TArgs>(string methodName, TArgs args, Action<RpcClientContext> callback)
		{
			if (callback == null)
				throw new NotSupportedException("callback *MUST NOT* be null!");

			_perfCounters.InvokeTotal.Increment();
			_perfCounters.InvokePerSec.Increment();
			_perfCounters.ConcurrentContext.Increment();

			RpcRequestHeader request = new RpcRequestHeader();
			request.FromComputer = ServiceEnviornment.ComputerName;
			request.FromService = ServiceEnviornment.ServiceName;
			request.Service = _serviceName;
			request.Method = methodName;
			request.ServerUri = _nexus.ServerUri.ToString();
			request.ToUri = ObjectHelper.ToString(_toUri);
			request.HasBody = typeof(TArgs) != typeof(RpcNull) && args != null;

			IRpcClientTransaction trans = _nexus.CreateTransaction();
			RpcClientContext ctx = new RpcClientContext(request, callback, trans, _nexus.ServiceRole);
			ctx.SendRequest<TArgs>(args, _timeout);
		}

		public TResults Invoke<TArgs, TResults>(string methodName, TArgs args)
		{
			return Invoke<TArgs, TResults>(methodName, args, _timeout);	// use Channel Default Timeout
		}

		public TResults Invoke<TArgs, TResults>(string methodName, TArgs args, int timeoutMs)
		{
			ManualResetEvent evt = new ManualResetEvent(false);
			RpcClientContext context = null;
			BeginInvoke<TArgs>(methodName, args,
				delegate(RpcClientContext c) {
					context = c;
					evt.Set();
				}
			);
			if (!evt.WaitOne(timeoutMs, false)) {
				throw new RpcException("SyncInvoke Timeout", "", RpcErrorCode.TransactionTimeout, null);
			} else {
				return context.EndInvoke<TResults>();
			}
		}
	}
}
