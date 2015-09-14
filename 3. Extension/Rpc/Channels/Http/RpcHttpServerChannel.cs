using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcHttpServerChannel: IRpcServerChannel
	{
		#region Private Member
		private bool _started;
		private string _serverUri;
		private object _syncRoot = new object();
		private HttpListener _listener;
		#endregion

		#region IRpcServerChannel Members
		public string Protocol
		{
			get { return "http"; }
		}

		public bool Started
		{
			get { return _started; }
		}

		public string ServerUri
		{
			get { return _serverUri; }
		}

		public event Action<IRpcServerTransaction> TransactionStart;

		public RpcHttpServerChannel(string prefix)
		{
			_listener = new HttpListener();
			_listener.Prefixes.Add(prefix);
			_serverUri = prefix;
		}

		public RpcHttpServerChannel(int port)
		{
			_listener = new HttpListener();
			_serverUri = string.Format("http://*:{0}/", port);
			_listener.Prefixes.Add(_serverUri);
		}

		public void Start()
		{
			lock (_syncRoot) {
				if (!_started) {
					try {
						_listener.Start();
						_listener.BeginGetContext(new AsyncCallback(ListenerCallback), this);
						_started = true;
					} catch (Exception ex) {
						throw new Exception("Http RpcServer Start Failed:" + _serverUri, ex);
					}
				}
			}
		}

		public void Stop()
		{
			lock (_syncRoot) {
				if (_started) {
					_listener.Stop();
					_started = false;
				}
			}
		}

		public static void ListenerCallback(IAsyncResult result)
		{
			// Console.WriteLine("Receive Callback");
			RpcHttpServerChannel channel = (RpcHttpServerChannel)result.AsyncState;

			//
			// Call EndGetContext to complete the asynchronous operation.
			HttpListenerContext context = channel._listener.EndGetContext(result);
			channel._listener.BeginGetContext(new AsyncCallback(ListenerCallback), channel);
			ProcessRequest(channel, context);
		}

		public static void ProcessRequest(RpcHttpServerChannel channel, HttpListenerContext httpContext)
		{
			try {
				RpcHttpServerTransaction trans = new RpcHttpServerTransaction(httpContext);
				channel.TransactionStart(trans);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "RpcServiceStartFailed");
			}
		}

		#endregion

	}
}