using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	public static class RpcServiceManager
	{
		private static object _syncRoot = new object();
		private static RpcServerPerfCounter PerfCounter = IICPerformanceCounterFactory.GetCounters<RpcServerPerfCounter>();
		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcServiceManager));

		private static Dictionary<string, RpcServiceBase> _services = new Dictionary<string, RpcServiceBase>();
		private static List<IRpcServerChannel> _channels = new List<IRpcServerChannel>();

		static RpcServiceManager()
		{
			var detectSrv = new RpcServiceDelegate("__DETECT__", DetectProc);
			_services.Add(detectSrv.ServiceName, detectSrv);
		}

		public static void RegisterServerChannel(IRpcServerChannel channel)
		{
			lock (_syncRoot) {
				channel.TransactionStart += new Action<IRpcServerTransaction>(TransactionStartCallback);
				_channels.Add(channel);
			}
		}

		public static void RegisterRawService(RpcServiceBase service)
		{
			lock (_syncRoot) {
				_services.Add(service.ServiceName, service);
			}
		}

		public static void RegisterService<T>(T service)
		{
			RpcServiceAttribute serviceAttr = AttributeHelper.GetAttribute<RpcServiceAttribute>(typeof(T));
			RegisterService<T>(serviceAttr.ServiceName, service);
		}

		public static void RegisterService<T>(string serviceName, T service)
		{
			lock (_syncRoot) {
				RpcServiceDecorator<T> realService = new RpcServiceDecorator<T>(service);
				_services.Add(serviceName, realService);
			}
		}

		public static void Start()
		{
			foreach (IRpcServerChannel channel in _channels) {
				channel.Start();
				// _tracing.WarnFmt("RpcChannel<{0}> Started on {1}", channel.Protocol, channel.UrlPrefix);
			}
		}

		public static void Stop()
		{
			foreach (IRpcServerChannel channel in _channels) {
				channel.Stop();
			}
		}

		// NextVersion
		//public static void Start(string[] dlls)
		//{
		//    foreach (IRpcServerChannel channel in _channels) {
		//        channel.Start();
		//        _tracing.WarnFmt("RpcChannel<{0}> Started on {1}", channel.Protocol, channel.UrlPrefix);
		//    }
		//    if (_hasTransparent)
		//        RpcGetArgsHelper.Build(dlls);
		//}

		private static void TransactionStartCallback(IRpcServerTransaction trans)
		{
			RpcServerContext context = null;
			try {
				context = new RpcServerContext(trans);

				RpcServiceBase serviceBase;
				PerfCounter.InvokePerSec.Increment();
				PerfCounter.InvokeTotal.Increment();

				if (_services.TryGetValue(context.ServiceName, out serviceBase)) {
					serviceBase.OnTransactionStart(context);
				} else {
					context.ReturnError(RpcErrorCode.ServiceNotFound, new Exception(context.ServiceName + " NotFound"));
				}
			} catch (RpcException ex) {
				context.ReturnError(ex.RpcCode, ex);
			} catch (Exception ex) {
				context.ReturnError(RpcErrorCode.ServerError, ex);
			}
		}

		private static void DetectProc(RpcServerContext context)
		{
			throw new NotImplementedException();
			//switch (context.MethodName) {

			//}
		}
	}
}
