using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imps.Services.CommonV4
{
	public class DatabaseUri: ServerUri
	{
		public DatabaseUri(string uri)
			: base("db")
		{
		}
	}
}
