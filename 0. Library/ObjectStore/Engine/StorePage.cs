using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Common.ObjectStore
{
    enum PageType
    {
        Index,
        Data,
        Empty,
        LargeObject,
    }

    [ProtoContract]
    class StorePage
    {
        IntPtr PageCache;

        public PageType Type
        {
            get { return PageCache; }
        }
        
    }
}
