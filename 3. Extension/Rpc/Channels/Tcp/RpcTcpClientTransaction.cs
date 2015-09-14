//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc.Tcp
//{
//    class RpcTcpClientTransaction : IRpcClientTransaction
//    {
//        private RpcTcpClientConnection _connection;

//        private RpcTcpClientTransaction(RpcTcpClientConnection conn)
//        {
//            _connection = conn;
//        }

//        public void SendRequest<T>(RpcRequestHeader request, T args, Action<RpcResponseHeader> callback, int timeout)
//        {
//            _connection.BeginGetRequqest();
//            lock (_connection.SyncRoot) {
//            }
//            _connection.Queue
//            throw new NotImplementedException();
//        }

//        public T ReceiveResponse<T>()
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }
//}
