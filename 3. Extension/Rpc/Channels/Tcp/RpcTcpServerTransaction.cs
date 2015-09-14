//using System;

//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Imps.Services.CommonV4.Rpc
//{
//    class RpcTcpServerTransaction : IRpcServerTransaction
//    {
//        private byte[] _body;
//        private object _sendSync;
//        private Socket _socket;
//        private RpcRequestHeader _request;

//        internal RpcTcpServerTransaction(Socket socket, object sendSync, RpcTcpBinaryHeader header)
//        {
//            _socket = socket;
//            _sendSync = sendSync;

//            NetworkStream stream = new NetworkStream(socket);
//            _request = Formatter.ReadRequestHeader(stream);
//            _body = new byte[header.BodySize];
//            stream.Read(_body, 0, _body.Length);
//        }

//        public RpcRequestHeader ReceiveRequestHeader()
//        {
//            return _request;
//        }

//        public T ReceiceRequest<T>()
//        {
//            return ProtoBufSerializer.FromByteArray<T>(_body);
//        }

//        public void SendResponse<T>(RpcResponseHeader response, T results)
//        {
//            NetworkStream stream = new NetworkStream(_socket);

//        }
//    }

//    public class Formatter
//    {
//        public static string ReadString(BinaryReader reader)
//        {
//            int len = reader.ReadByte();
//            byte[] buf = reader.ReadBytes(len);
//            return Encoding.ASCII.GetString(buf);
//        }

//        public static void WriteString(BinaryWriter writer, string str)
//        {
//            byte[] buf = Encoding.ASCII.GetBytes(str);
//            if (buf.Length > 255)
//                throw new NotSupportedException("Header to long:" + str);
//            writer.Write((byte)buf.Length);
//            writer.Write(buf);
//        }

//        public static RpcRequestHeader ReadRequestHeader(Stream stream)
//        {
//            BinaryReader reader = new BinaryReader(stream);

//            return new RpcRequestHeader() {
//                FromComputer = Formatter.ReadString(reader),
//                FromService = Formatter.ReadString(reader),
//                HasBody = reader.ReadBoolean(),
//                Method = Formatter.ReadString(reader),
//                Service = Formatter.ReadString(reader),
//                ToUri = Formatter.ReadString(reader)
//            };
//        }

//        public static void WriteRequestHeader(Stream stream, RpcRequestHeader header)
//        {
//            BinaryWriter writer = new BinaryWriter(stream);
//            WriteString(writer, header.FromComputer);
//            WriteString(writer, header.FromService);
//            WriteString(writer, header.Method);
//            WriteString(writer, header.Service);
//            WriteString(writer, header.ToUri);
//        }

//        public static RpcResponseHeader ReadResponseHeader(Stream stream)
//        {
//            BinaryReader reader = new BinaryReader(stream);

//            return new RpcResponseHeader() {
//                 //Error = ReadString(reader);
//                 //ErrorCode = RpcErrorCode.OK,
//                 //HasBody = reader.ReadBoolean();
//            };
//        }

//        public static void WriteResponseHeader(Stream stream, RpcResponseHeader header)
//        {
//            BinaryWriter writer = new BinaryWriter(stream);
//            WriteString(writer, header.ErrorCode);
//        }
//    }
//}

//// HEADER
//// REQUEST
//// BODY
//// REWQuest
