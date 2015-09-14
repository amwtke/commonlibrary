using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	struct RpcPacketHeader
	{
		public static readonly int Size = Marshal.SizeOf(typeof(RpcPacketHeader));

		public const int MagicMark = 0x00BADBEE;

		[MarshalAs(UnmanagedType.I4)]
		public int Mark;

		[MarshalAs(UnmanagedType.I4)]
		public int ContextSize;

		[MarshalAs(UnmanagedType.I4)]
		public int BodySize;

		public static byte[] ToByteArray(RpcPacketHeader header)
		{
			byte[] buffer = new byte[Size];
			IntPtr structPtr = Marshal.AllocHGlobal(Size);
			Marshal.StructureToPtr(header, structPtr, true);
			Marshal.Copy(structPtr, buffer, 0, Size);
			Marshal.FreeHGlobal(structPtr);
			return buffer;  
		}

		public static RpcPacketHeader FromByteArray(byte[] buffer)
		{
			IntPtr structPtr = Marshal.AllocHGlobal(Size);
			Marshal.Copy(buffer, 0, structPtr, Size);
			object obj = Marshal.PtrToStructure(structPtr, typeof(RpcPacketHeader));
			Marshal.FreeHGlobal(structPtr);
			return (RpcPacketHeader)obj;
		}
	}
}
