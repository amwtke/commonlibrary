using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Common.ObjectStore
{
    [ProtoContract]
    class StoreIndex<T>
    {
        [ProtoMember(1)]
        public T Key;

        [ProtoMember(2)]
        public ushort Page;
    }
}
