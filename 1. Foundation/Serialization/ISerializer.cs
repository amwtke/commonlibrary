using System;
using System.IO;

namespace Imps.Services.CommonV4
{
	public interface ISerializer
	{
		void Serialize<T>(Stream stream, T obj);
		T Deserialize<T>(Stream stream);
	}
}
