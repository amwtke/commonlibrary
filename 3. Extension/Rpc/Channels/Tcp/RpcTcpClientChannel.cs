//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc.Tcp
//{
//    class RpcTcpClientChannel : IRpcClientChannel
//    {
//        private int _timeout = 120;
//        private object _syncRoot;
//        private Dictionary<BaseUri, Socket> _connections;

//        public RpcTcpClientChannel()
//        {
//        }

//        public int Timeout
//        {
//            get { return _timeout; }
//            set { _timeout = value; }
//        }

//        public string Protocol
//        {
//            get { return "tcp"; }
//        }

//        public IRpcClientTransaction CreateTransaction()
//        {
//            TcpUri serverUri;

//            lock (_syncRoot) {
//                Socket socket;
//                if (_connections.TryGetValue(serverUri, out socket)) {
//                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                    _connections.Add(serverUri, socket);
//                }
//            }
//            return new RpcTcpClientTransaction(connection);
//        }
//    }

//    class RpcTcpClientConnection
//    {
//        private Socket[] _sockets;

//        public RpcTcpClientConnection()
//        {

//        }
//    }
//}
