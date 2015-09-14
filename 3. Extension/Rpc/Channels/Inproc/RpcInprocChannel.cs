using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcInprocChannel: IRpcServerChannel, IRpcClientChannel
	{
		public bool Started
		{
			get { return true; }
		}

		public string Protocol
		{
			get { return "inproc"; }
		}

		public string ServerUri
		{
			get { return "inproc://"; }
		}

		public void Start()
		{	
		}

		public void Stop()
		{
		}

		public event Action<IRpcServerTransaction> TransactionStart;

		public int Timeout
		{
			get;
			set;
		}

		public static RpcInprocChannel Instance = new RpcInprocChannel();

		private RpcInprocChannel()
		{
		}

		public IRpcClientTransaction CreateTransaction(BaseUri serverUri)
		{
			IICAssert.Is<InprocUri>(serverUri, "");
			return new RpcInprocTransaction(this);
		}

		public void OnTransactionStart(RpcInprocTransaction trans)
		{
			TransactionStart.Invoke(trans);
		}
	}
}
