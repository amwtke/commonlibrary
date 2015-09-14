using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.Rpc
{
	public class RpcTcpBinaryHeader
	{
		public const int Size = 16;

		public const int MagicStartMark = 0x00BADBEE;

		public int Sequence;

		public int HeaderSize;

		public int BodySize;

		public static RpcTcpBinaryHeader Parse(byte[] buffer)
		{
			MemoryStream stream = new MemoryStream(buffer);
			BinaryReader reader = new BinaryReader(stream);
			int mark = reader.ReadInt32();
			if (mark != MagicStartMark) {
				throw new FormatException("Unexcepted Header");
			}

			RpcTcpBinaryHeader header = new RpcTcpBinaryHeader();
			header.Sequence = reader.ReadInt32();
			header.HeaderSize = reader.ReadInt32();
			header.BodySize = reader.ReadInt32();
			return header;
		}

		public void WriteToStream(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(MagicStartMark);
			writer.Write(Sequence);
			writer.Write(HeaderSize);
			writer.Write(BodySize);
		}
	}
}
