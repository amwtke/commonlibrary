using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class RpcServiceDecorator<T>: RpcServiceBase
	{
		public RpcServiceDecorator(T serviceObj): base(string.Empty)
		{
			Type intf = typeof(T);
			if (!intf.IsInterface)
				throw new NotSupportedException();

			RpcServiceAttribute serviceAttr = AttributeHelper.GetAttribute<RpcServiceAttribute>(intf);
			p_serviceName = serviceAttr.ServiceName;
			_serviceObj = serviceObj;

			IICPerformanceCounterCategory category = new IICPerformanceCounterCategory("rpc:" + p_serviceName, PerformanceCounterCategoryType.MultiInstance);
			foreach (MethodInfo method in intf.GetMethods()) {
				RpcServiceMethodAttribute methodAttr = AttributeHelper.GetAttribute<RpcServiceMethodAttribute>(method);
				string methodName = method.Name;

				RpcServiceMethod m = new RpcServiceDecorator<T>.RpcServiceMethod();
				m.RatePerSecond = category.CreateCounter(methodName + " /sec.", PerformanceCounterType.RateOfCountsPerSecond32);
				m.TotalCount = category.CreateCounter(methodName + " Total.", PerformanceCounterType.NumberOfItems32);
				m.TotalFailed = category.CreateCounter(methodName + " Failed.", PerformanceCounterType.NumberOfItems32);
				m.Concurrent = category.CreateCounter(methodName + " Concurrent.", PerformanceCounterType.NumberOfItems32);
				m.Method = method;

				_methods.Add(methodName, m);
			}
			IICPerformanceCounterFactory.GetCounters(category);
		}

		public override void OnTransactionStart(RpcServerContext context)
		{
			RpcServiceMethod method;
			if (!_methods.TryGetValue(context.MethodName, out method)) {
				throw new RpcException("TransactionStart", "", RpcErrorCode.MethodNotFound, null);
			} else {
				try {
					method.RatePerSecond.Increment();
					method.TotalCount.Increment();
					method.Concurrent.Increment();
					method.Method.Invoke(_serviceObj, new object[] { context });
				} catch (Exception ex) {
					context.ReturnError(RpcErrorCode.ServerError, ex);
					method.TotalFailed.Increment();
				} finally {
					method.Concurrent.Decrement();
				}
			}
		}

		private object _serviceObj;
		private Dictionary<string, RpcServiceMethod> _methods = new Dictionary<string, RpcServiceMethod>();
	
		private class RpcServiceMethod
		{
			public IICPerformanceCounter RatePerSecond;
			public IICPerformanceCounter TotalCount;
			public IICPerformanceCounter TotalFailed;
			public IICPerformanceCounter Concurrent;
			public MethodInfo Method;
		}
	}
}
