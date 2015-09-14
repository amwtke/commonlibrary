using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Common.ObjectStore
{
    [ProtoContract]
	class StoreMeta
	{
        [ProtoMember(1)]
        public Dictionary<string, StoreMetaTable> Tables;
	}
}
