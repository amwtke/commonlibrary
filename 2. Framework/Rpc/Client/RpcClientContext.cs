using System;
using System.Diagnostics;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	public sealed class RpcClientContext
	{
		private object _args;
		private string _serviceUrl;
		private string _serviceRole;
		private RpcRequestHeader _request;
		private RpcResponseHeader _response;
		private IRpcClientTransaction _trans;
		private Action<RpcClientContext> _callback;
		private RpcClientObserverItem _observer;

		internal RpcClientContext(RpcRequestHeader request, Action<RpcClientContext> callback, IRpcClientTransaction trans, string serviceRole)
		{
			_trans = trans;
			_request = request;
			_callback = callback;
			_serviceUrl = string.Format("{0}/{1}.{2}", request.ServerUri, request.Service, request.Method);
			_serviceRole = serviceRole;
		}

		internal void SendRequest<TArgs>(TArgs args, int timeout)
		{
			_observer = RpcObserverManager.GetClientItem(_request.ServerUri, _request.Service, _request.Method, _serviceRole); 

			Stopwatch watch = new Stopwatch();
			watch.Start();

			try {
				_args = args;
				_trans.SendRequest<TArgs>(
					_request,
					args,
					response => ProcessResponse(response, watch.ElapsedTicks),
					timeout
				);

				TracingManager.Info(
					delegate() {
						_observer.RequestTracer.InfoFmt2(
							_request.FromService,
							_request.ToUri,
							"Args = {0}",
							ObjectHelper.DumpObject(_args)
						);
					}
				);
			} catch (Exception ex) {
				_observer.RequestTracer.ErrorFmt2(
					ex,
					_request.FromService,
					_request.ToUri.ToString(),
					"Args = {0}",
					ObjectHelper.DumpObject(_args)
				);
				throw;
			}
		}

		public void EndInvoke()
		{
			EndInvoke<RpcNull>();
		}

		public T EndInvoke<T>()
		{
			T retValue = default(T);
			Exception error = null;

			try {
				if (_response.ErrorCode != RpcErrorCode.OK) {
					throw new RpcException("EndInvoke Failed", _serviceUrl, _response.ErrorCode, _response.Error);
				}

				if (typeof(T) == typeof(RpcNull) || !_response.HasBody) {
					retValue = default(T);
				} else {
					retValue = _trans.ReceiveResponse<T>();
				}
				
				TracingManager.Info(
					delegate() {
						_observer.ResponseTracer.InfoFmt2(
							_request.FromService,
							_request.ToUri,
							"Args={0}\r\nResults={1}",
							ObjectHelper.DumpObject(_args),
							ObjectHelper.DumpObject((object)retValue)
						);
					}
				);

				return retValue;
			} catch (Exception ex) {
				_observer.ResponseTracer.ErrorFmt2(
					ex,
					_request.FromService,
					_request.ToUri,
					"Args={0}\r\n",
					ObjectHelper.DumpObject(_args)
				);
				error = ex;
				throw;
			}
		}

		private void ProcessResponse(RpcResponseHeader response, long costTicks)
		{
			_response = response;
			_observer.Track(response.ErrorCode == RpcErrorCode.OK, response.Error, costTicks);
			_callback(this);
		}
	}
}

