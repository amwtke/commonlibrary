using System;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4
{
	public enum RpcErrorCode
	{
		OK					= 200,
		SendFailed			= -1,			// 在客户端产生错误
		TransactionTimeout	= -2,			// Transaction超时, 在规定时间内未收到服务器应答

		ServiceNotFound		= 404,			// 服务未找到, Server端返回
		MethodNotFound		= 405,			// 方法未找到, Server端返回
		ServerError			= 500,			// 内部错误, Server端返回
		ServerBusy			= 503,			// 服务器忙, 保护性错误

		ServerTimeout		= 504,			// 中转超时, 中转Server返回
		ServerTransferFailed	= 600,		// 中转错误, 中转Server返回
		Unknown,							// 未知

		InvaildRequestArgs = 512,			
		InvaildResponseArgs = 513,
	}
}
