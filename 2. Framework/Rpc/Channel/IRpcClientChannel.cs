using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public interface IRpcClientChannel
	{
		/// <summary> 事务超时时间 </summary>
		int Timeout { get; set; }

		/// <summary> 协议 </summary>
		string Protocol { get; }

		/// <summary> 创建Transaction </summary>
		IRpcClientTransaction CreateTransaction(BaseUri serverUri);

		// IRpcBatchClientTransaction CreateBatchTransaction(BaseUri serverUri);
	}
}
