using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcPipeServerChannel: IRpcServerChannel
	{
		private bool _started;
		private string _pipeName;
		private NamedPipeServerStream[] _pipeServers;

		public bool Started
		{
			get { return _started; }
		}

		public string Protocol
		{
			get { return "pipe"; }
		}

		public string ServerUri
		{
			get { return "pipe://.:" + _pipeName + "/"; }
		}

		public event Action<IRpcServerTransaction> TransactionStart;

		public RpcPipeServerChannel(string pipeName, int instanceCount)
		{
			_pipeName = pipeName;
			_pipeServers = new NamedPipeServerStream[instanceCount];
			for (int i = 0; i < instanceCount; i++) {
				//
				// TODO: Add PipeSecurity
				PipeSecurity security = new PipeSecurity();
				// new PipeAccessRule(.

				security.AddAccessRule(new PipeAccessRule(new NTAccount("Everyone"), PipeAccessRights.FullControl, AccessControlType.Allow));

				_pipeServers[i] = new NamedPipeServerStream(pipeName, PipeDirection.InOut,
					instanceCount, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0,
					security);
					
			}
		}

		public void Start()
		{
			if (!_started) {
				foreach (NamedPipeServerStream stream in _pipeServers) {
					stream.BeginWaitForConnection(new AsyncCallback(ConnectionCallback), stream);
				}
				_started = true;
			}
		}

		public void Stop()
		{
			return;

			//
			// Stop Service的时候, 不能简单的Stop掉PipeService

			//if (_started) {
			//    foreach (NamedPipeServerStream stream in _pipeServers) {
			//        stream.Dispose();
			//    }
			//    _started = false;
			//}
			//			 
		}

		private void ConnectionCallback(IAsyncResult ar)
		{
			try {
				NamedPipeServerStream stream = (NamedPipeServerStream)ar.AsyncState;
				stream.EndWaitForConnection(ar);

				RpcPipeServerTransaction trans = new RpcPipeServerTransaction(stream, this);
				TransactionStart(trans);
			} catch (Exception ex) {
				SystemLog.Error(LogEventID.RpcFailed, ex, "RpcPipeServerChannel.ConnectionCallback Failed");
			}
		}

		internal void RecycleServerStream(NamedPipeServerStream stream)
		{
			stream.BeginWaitForConnection(new AsyncCallback(ConnectionCallback), stream);
		}
	}
}
