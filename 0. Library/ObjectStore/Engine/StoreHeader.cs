using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Common.ObjectStore
{
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
	struct StoreHeader
	{
        [MarshalAs(UnmanagedType.I2)]
        public short VersionHi;

        [MarshalAs(UnmanagedType.I2)]
        public short VersionLo;

        [MarshalAs(UnmanagedType.I8)]
        public long MetaBegin;

        [MarshalAs(UnmanagedType.I8)]
        public long MetaSize;

        [MarshalAs(UnmanagedType.I8)]
        public long DataBegin;

        [MarshalAs(UnmanagedType.I8)]
        public long DataSize;
	}
}
