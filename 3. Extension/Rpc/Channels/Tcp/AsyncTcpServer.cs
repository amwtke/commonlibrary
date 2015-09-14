//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Collections.Generic;
//using System.Text;

//namespace Imps.CommonV4.Net
//{
//    public class AsyncTcpServer
//    {
//        private Socket _socket;

//        public AsyncTcpServer(int port)
//        {
//            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
//            _socket.Listen(100);
//            _socket.BeginAccept(256, AcceptCallback, null);
//        }

//        private void AcceptCallback(IAsyncResult ret)
//        {
//            byte[] buffer = new byte[256];
//            _socket.EndAccept(out buffer, ret);

//            // OnDataReceived();
//            // _socket.BeginReceive(out buffer, 100, 200, 300, );
//        }
//    }
//}
