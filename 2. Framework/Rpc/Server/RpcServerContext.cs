using System;
using System.IO;
using System.Threading;
using System.Diagnostics;

using Google.ProtoBuf;

using Imps.Services.CommonV4.Rpc;

namespace Imps.Services.CommonV4
{
	public class RpcServerContext
	{
		private object _args;
		private int _hasReturned;
		private RpcRequestHeader _request;
		private RpcServerObserverItem _observer;
		private IRpcServerTransaction _trans;
		private Stopwatch _watch;
		private int _returnIfCount = 0;

		public string From
		{
			get { return _request.ServiceAtComputer; }
		}

		public string To
		{
			get { return _request.ServerUri; }
		}

		public string FromService
		{
			get { return _request.FromService; }
		}

		public string FromComputer
		{
			get { return _request.FromComputer; }
		}

		public string ToUri
		{
			get { return _request.ServerUri; }
		}

		public ResolvableUri ContextUri
		{
			get
			{
				if (!string.IsNullOrEmpty(_request.ToUri)) {
					return ResolvableUri.Parse(_request.ToUri);
				} else {
					return null;
				}
			}
		}
		public string ServiceName
		{
			get { return _request.Service; }
		}

		public string MethodName
		{
			get { return _request.Method; }
		}

		public RpcServerContext(IRpcServerTransaction trans)
		{
			_request = trans.ReceiveRequestHeader();
			_trans = trans;
			_hasReturned = 0;

			_observer = RpcObserverManager.GetServerItem(_request.Service, _request.Method, _request.FromService, _request.FromComputer);
			_watch = new Stopwatch();
			_watch.Start();
			// _perfCounters.ConcurrentContext.Increment();
		}

		public T GetArgs<T>()
		{
			try {
				if (!_request.HasBody) {
					object a = null;
					return (T)a;
				}

				T ret = _trans.ReceiceRequest<T>();
				_args = ret;
				TracingManager.Info(
					delegate() {
						_observer.RequestTracer.InfoFmt2(
							_request.ServiceAtComputer,
							_request.ToUri,
							"Args={0}",
							ObjectHelper.DumpObject(_args)
						);
					}
				);
				return ret;
			} catch (Exception ex) {
				_observer.RequestTracer.ErrorFmt2(
					ex,
					_request.ServiceAtComputer,
					_request.ToUri,
					"Args={0}",
					ObjectHelper.DumpObject(_args)
				); 
				throw new RpcException("RpcServerContext.GetArgs", "", RpcErrorCode.InvaildRequestArgs, ex);
			}
		}

		public void Return()
		{
			var response = RpcResponseHeader.CreateSuccess(false);
			ReturnInner<RpcNull>(response, null);
		}

		public void Return<T>(T results)
		{
			var response = RpcResponseHeader.CreateSuccess(results != null);
			ReturnInner<T>(response, results);
		}

		public void ReturnError(RpcErrorCode errCode, Exception ex)
		{
			var response = RpcResponseHeader.CreateError(errCode, ex);
			ReturnError(response);
		}

		public void ThrowException(Exception ex)
		{
			var response = RpcResponseHeader.CreateError(RpcErrorCode.ServerError, ex);
			ReturnError(response);
		}

		private void ReturnInner<T>(RpcResponseHeader response, T results)
		{
			try {
				if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) != 0) {
					string msg = string.Format("Return more than once <{0}.{1}>", this.ServiceName, this.MethodName);
					throw new NotSupportedException(msg);
				}

				TracingManager.Info(
					delegate() {
						_observer.ResponseTracer.InfoFmt2(
							_request.FromService + "@" + _request.FromComputer,
							_request.ToUri,
							"Args={0}\r\nResults={1}",
							ObjectHelper.DumpObject(_args),
							ObjectHelper.DumpObject(results)
						);
					}
				);

				_trans.SendResponse(response, results);
				_observer.Track(true, response.Error, (int)_watch.ElapsedMilliseconds);
			} catch (Exception innerEx) {
				SystemLog.Error(LogEventID.RpcFailed, innerEx, "RpcServerContext.ReturnError Failed");
			} finally {
				_perfCounters.ConcurrentContext.Decrement();
				_perfCounters.InvokeFailed.Increment();
			}
		}

		private void ReturnError(RpcResponseHeader response)
		{
			try {
				if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) != 0) {
					string msg = string.Format("Return more than once <{0}.{1}> ex={2}", this.ServiceName, this.MethodName, response.Error);
					throw new NotSupportedException(msg);
				}

				_observer.ResponseTracer.ErrorFmt2(
					response.Error,
					_request.FromService + "@" + _request.FromComputer,
					_request.ToUri,
					"Args={0}",
					ObjectHelper.DumpObject(_args)
				);

				_trans.SendError(response);

				_observer.Track(false, response.Error, _watch.ElapsedTicks);
			} catch (Exception innerEx) {
				SystemLog.Error(LogEventID.RpcFailed, innerEx, "RpcServerContext.ReturnError Failed");
			} finally {
				_perfCounters.ConcurrentContext.Decrement();
				_perfCounters.InvokeFailed.Increment();
			}
		}
			//}
		
			//try {
			//    if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) == 0) {
			//        TracingManager.Info(
			//            delegate() {
			//                string argStr = ObjectHelper.DumpObject(_args, "Args");
			//                string retStr = ObjectHelper.DumpObject(result, "Results");
			//                string loggerName = string.Format("RpcServiceResponse.{0}.{1}", _serviceName, _methodName);
								
			//                _observer.ResponseTracer.InfoFmt2(
			//                    _request.FromService + "@" + _request.FromComputer,
			//                    _request.ToUri,
			//                    "Args={0}\r\nResults={1}",
			//                    _args,
			//                    results);
			//                }
			//            );
			//            _trans.SendResponse<T>(result);
			//            if (TransactionEnd != null)
			//                TransactionEnd(true);
			//        } else {
			//            string msg = string.Format("Can't return more than once {0}.{1}", _serviceName, _methodName);
			//            throw new NotSupportedException(msg);
			//        }
			//    } catch (Exception ex) {
			//        _trans.SendResponse(RpcErrorCode.ServerError, ex);
			//    }
			//    _perfCounters.ConcurrentContext.Decrement();

			//}
		//}

			//if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) == 0) {
			//    TracingManager.Info(
			//        delegate() {
			//            string argStr = ObjectHelper.DumpObject(_args, "Args");
			//            string loggerName = string.Format("RpcServiceResponse.{0}.{1}", _serviceName, _methodName);

			//            ITracing tracing = TracingManager.GetTracing(loggerName);
			//            tracing.InfoFmt2(_from, _to, "Success Args={0}\r\n Result=NULL\r\n", argStr);
			//        }
			//    );
			//    _trans.SendResponse();
			//    if (TransactionEnd != null)
			//        TransactionEnd(true);
			//} else {
			//    throw new NotSupportedException("Return more than once");
			//} 
			//_perfCounters.ConcurrentContext.Decrement();

		public void ReturnIf<TResults>(TResults results, Func<int, bool> ifProc)
		{
			lock (_trans) {
				_returnIfCount++;
				if (ifProc(_returnIfCount)) {
					Return<TResults>(results);
				}
			}
		}

			//try {
			//    if (Interlocked.CompareExchange(ref _hasReturned, 1, 0) == 0) {
			//        TracingManager.Error(
			//            delegate() {
			//                string argStr = ObjectHelper.DumpObject(_args, "Args");
			//                string loggerName = string.Format("RpcServiceResponse.{0}.{1}", _serviceName, _methodName);
			//                ITracing tracing = TracingManager.GetTracing(loggerName);
			//                tracing.InfoFmt2(_from, _to, "Failed Args={0}\r\n Result=NULL\r\n", argStr);
			//            }
			//        );
			//        _trans.SendResponse(errCode, ex);
			//        if (TransactionEnd != null)
			//            TransactionEnd(false);
			//    } else {
			//        string msg = string.Format("Return more than once <{0}> \r\n{1}", errCode, ex);
			//        throw new NotSupportedException(msg);
			//    }
			//} catch (Exception innerEx) {
			//    SystemLog.Error(LogEventID.RpcFailed, innerEx, "RpcServerContext.ReturnError Failed");
			//} finally {
			//    _perfCounters.ConcurrentContext.Decrement();
			//    _perfCounters.InvokeFailed.Increment();
			//}

		public bool HasReturned
		{
			get { return _hasReturned != 0; }
		}

		private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcServerContext));
		private static RpcServerPerfCounter _perfCounters = IICPerformanceCounterFactory.GetCounters<RpcServerPerfCounter>();
	}
}
