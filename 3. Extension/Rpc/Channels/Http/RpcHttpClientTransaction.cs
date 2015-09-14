using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpClientTransaction: IRpcClientTransaction, IDisposable
	{
		private RpcHttpClientChannel _channel;

		private string _serviceUrl = null;
		private Action<RpcResponseHeader> _callback;

		private WebRequest _webRequest = null;
		private WebResponse _webResponse = null;
		private ManualResetEvent _waitHandle;

		public RpcHttpClientTransaction(RpcHttpClientChannel channel, BaseUri serverUri)
		{
			_channel = channel;
		}
		
		~RpcHttpClientTransaction()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
			}

			if (_webRequest != null) {
				try {
					_webRequest.Abort();
				} catch (Exception ex) {
					SystemLog.Unexcepted(ex);
				}
			}
			if (_waitHandle != null) {
				_waitHandle.Close();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void IRpcClientTransaction.SendRequest<T>(RpcRequestHeader request, T args, Action<RpcResponseHeader> callback, int timeout)
		{
			_callback = callback;
            _serviceUrl = string.Format("{0}/{1}.{2}", request.ServerUri, request.Service, request.Method);

            _webRequest = HttpWebRequest.Create(_serviceUrl);
            _webRequest.Method = "POST";
			_webRequest.Proxy = null;
            _webRequest.ContentType = "multipart/byteranges";
            _webRequest.Headers.Add(HttpRequestHeader.From, request.ServiceAtComputer);
			_webRequest.Headers.Add(HttpRequestHeader.Pragma, _serviceUrl);
			_webRequest.Headers.Add(HttpRequestHeader.Cookie, "to=" + ObjectHelper.ToString(request.ToUri));

			byte[] buffer = null;
			if (!request.HasBody) {
				_webRequest.Headers.Add("Null", "true");
				_webRequest.ContentLength = 0;
			} else {
				buffer = ProtoBufSerializer.ToByteArray<T>(args);
				_webRequest.ContentLength = buffer.Length;
			}

            timeout = timeout > 0 ? timeout : _channel.Timeout;

			if (timeout > 0) {
				_waitHandle = new ManualResetEvent(false);
				ThreadPool.RegisterWaitForSingleObject(_waitHandle, new WaitOrTimerCallback(TimeoutCallback), this, timeout, true);
			}
			if (_webRequest.ContentLength == 0) {
				_webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
			} else {
				_webRequest.BeginGetRequestStream(
					delegate(IAsyncResult asyncResult) {
						try {
							Stream stream = _webRequest.EndGetRequestStream(asyncResult);
							stream.Write(buffer, 0, buffer.Length);
							stream.Close();
							_webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
						} catch (Exception ex) {
							var resp = RpcResponseHeader.CreateError(RpcErrorCode.SendFailed, ex);
							_callback(resp);
						}
					}, 
					this
				);
			}
		}

		public T ReceiveResponse<T>()
		{
			Stream stream = _webResponse.GetResponseStream();
			return ProtoBufSerializer.Deserialize<T>(stream);
		}

		private static void ResponseCallback(IAsyncResult asyncResult)
		{
			RpcHttpClientTransaction trans = (RpcHttpClientTransaction)asyncResult.AsyncState;
			RpcResponseHeader header = null;

			try {
				var response = trans._webRequest.EndGetResponse(asyncResult);
				trans._webResponse = response;

				string warn = response.Headers.Get("Warning");

				if (!string.IsNullOrEmpty(warn)) {
					RpcErrorCode errCode = (RpcErrorCode)Enum.Parse(typeof(RpcErrorCode), warn);
					if (errCode != RpcErrorCode.OK) {
						Exception ex = null;
						if (response.ContentLength > 0) {
							Stream stream = response.GetResponseStream();
							ex = BinarySerializer.Deserialize<Exception>(stream);
						}
						header = RpcResponseHeader.CreateError(errCode, ex);
					} else {
						SystemLog.Error(LogEventID.RpcFailed, "Unexcepted Message");
						header = RpcResponseHeader.CreateError(RpcErrorCode.Unknown, null);
					}
				} else {
					bool hasBody = (response.Headers["Null"] != "true");
					header = RpcResponseHeader.CreateSuccess(hasBody);
				}
			} catch (WebException ex) {
				if (ex.Status == WebExceptionStatus.Timeout) {
					header = RpcResponseHeader.CreateError(RpcErrorCode.TransactionTimeout, ex);
				} else {
					header = RpcResponseHeader.CreateError(RpcErrorCode.SendFailed, ex);
				}
			} catch (Exception ex) {
				header = RpcResponseHeader.CreateError(RpcErrorCode.SendFailed, ex);
			}
			trans._callback(header);
			trans._waitHandle.Set();
		}

		private static void TimeoutCallback(object state, bool setted)
		{
			RpcHttpClientTransaction trans = null;
			try {
				if (setted) { // Timeout
					trans = (RpcHttpClientTransaction)state;
					var resp = RpcResponseHeader.CreateError(RpcErrorCode.TransactionTimeout, null);
					trans._callback(resp);
				}
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "TimeoutCallback");
			} finally {
				if (trans != null) {
					trans.Dispose();
				}
			}
		}
	}
}
