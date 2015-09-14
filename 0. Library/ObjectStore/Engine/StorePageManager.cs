using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;

namespace Imps.Common.ObjectStore
{
	class StorePageManager
	{
		public List<StoreIndexPage> IndexPages;
		public Queue<StorePage> PreferPages;
		public LRUCacheManager<int, StorePage> CachedPages;

		public StorePage[] _currentIndexPage;
		public StorePage[] _currentDataPage;

		public StorePage GetPage(int id)
		{
			throw new NotImplementedException();
		}

		public StorePage GetEmptyPage(int size)
		{
			throw new NotImplementedException();
		}

		public StorePage WriteLOB(int size)
		{
			throw new NotImplementedException();
		}
	}
}
