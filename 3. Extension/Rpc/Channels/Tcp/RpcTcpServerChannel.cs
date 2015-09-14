//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Collections.Generic;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc
//{
//    public sealed class RpcTcpServerChannel: IRpcServerChannel
//    {
//        private const int MaxPendingConnection = 100;

//        private bool _started;
//        private int _port;
//        private Socket _socket;

//        private List<RpcTcpServerTransaction> _trans;

//        public RpcTcpServerChannel(int port)
//        {
//            _port = port;
//            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        }

//        public bool Started
//        {
//            get { return _started; }
//        }

//        public string Protocol
//        {
//            get { return "tcp"; }
//        }

//        public string UrlPrefix
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public void Start()
//        {
//            _socket.Bind(new IPEndPoint(IPAddress.Any, _port));
//            _socket.Listen(MaxPendingConnection);

//            _socket.BeginAccept(0, AcceptCallback, null);
//        }

//        public void Stop()
//        {
//            throw new NotImplementedException();
//        }

//        private void AcceptCallback(IAsyncResult ar) 
//        {
//            try {
//                Socket socket = _socket.EndAccept(ar);
//                _tracing.InfoFmt("connection from {0} accepted", socket.RemoteEndPoint.ToString());

//                BeginReceive(socket);
//            } catch (Exception ex) {
//                SystemLog.Error(LogEventID.NetworkFailed, ex, "RpcTcpServerChannel AcceptFailed");
//            }
//        }

//        private void BeginReceive(Socket socket)
//        {
//            byte[] buffer = new byte[RpcTcpBinaryHeader.Size];
//            socket.BeginReceive(
//                buffer,
//                0,
//                RpcTcpBinaryHeader.Size,
//                SocketFlags.None,
//                delegate (IAsyncResult ar) {
//                    try {
//                        SocketError se;
//                        socket.EndReceive(ar, out se);
//                        OnReceiveHeader(socket, buffer);
//                    } catch (Exception ex) {
//                        SystemLog.Error(LogEventID.NetworkFailed, ex, "RpcTcpServerChannel Receive Failed");
//                        socket.Disconnect(false);
//                    }
//                },
//                null
//            );
//        }

//        private void OnReceiveHeader(Socket socket, byte[] hbuf)
//        {
//            try {
//                RpcTcpBinaryHeader header = RpcTcpBinaryHeader.Parse(hbuf);
//                // read to buffer
//                RpcTcpServerTransaction trans = new RpcTcpServerTransaction(socket, header);

//            } catch (Exception ex) {
//            }
//            // RpcTcpServerTransaction 
//        }

//        internal void SendResponse(ComboClass<object, Socket> socket, byte[] header, byte[] body)
//        {

//        }

//        public event Action<IRpcServerTransaction> TransactionStart;

//        private static ITracing _tracing = TracingManager.GetTracing(typeof(RpcTcpServerChannel));
//    }
//}
