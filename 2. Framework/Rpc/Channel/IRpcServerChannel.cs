using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public interface IRpcServerChannel
	{
		bool Started { get; }

		void Start();

		void Stop();

		string Protocol { get; }

		string ServerUri { get; }

		event Action<IRpcServerTransaction> TransactionStart;

		// event Action<IRpcBatchServerTransaction> BatchTransactionStart;
	}
}
