using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Google.ProtoBuf;

namespace Imps.Common.ObjectStore
{
    [ProtoContract]
    class StoreMetaTable
    {
        [ProtoMember(1)]
        public string TableName;

		public int TableId;

        [ProtoMember(2)]
        public string KeyType;

        [ProtoMember(3)]
        public string ValueType;

        [ProtoMember(4)]
        public int FirstIndexPage;
    }
}
