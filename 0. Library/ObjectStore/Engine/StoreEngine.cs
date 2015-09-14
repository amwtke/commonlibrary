using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Services.CommonV4;
namespace Imps.Common.ObjectStore
{
	class StoreEngine
	{
		public const int HeaderSize = 64;
		public const int PageSize = 8192;
		public const int MetaSize = PageSize - HeaderSize;

		private object _syncRoot;
		private FileStream _file;
		private StoreHeader _header;
		private StoreMeta _meta;
		private StorePageManager _pageManager;

		public StoreEngine(string fileName, bool createNew)
		{
			if (createNew) {
				_file = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite);
				_header = new StoreHeader();
				_header.VersionHi = 1;
				_header.VersionLo = 0;
				_header.MetaBegin = 1024;
				_header.MetaSize = 65536;

				byte[] buffer;
				byte[] dummy;


				buffer = MarshalHelper.StructToBuffer<StoreHeader>(_header);
				dummy = new byte[HeaderSize - buffer.Length];
				_file.Write(buffer, 0, buffer.Length);
				_file.Write(dummy, 0, dummy.Length);
				_file.Flush();

				_meta = new StoreMeta();
				buffer = BinarySerializer.ToByteArray(_meta);
				dummy = new byte[MetaSize - buffer.Length];
				_file.Write(buffer, 0, buffer.Length);
				_file.Write(dummy, 0, dummy.Length);
				_file.Flush();

				_pageManager = StorePageManager.CreateNew(_file);
			} else {
				_file = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
				_header = MarshalHelper.StructFromStream<StoreHeader>(_file);

				_file.Seek(_header.MetaBegin, SeekOrigin.Begin);
				_meta = BinarySerializer.Deserialize<StoreMeta>(_file);

				_file.Seek(_header.DataBegin, SeekOrigin.Begin);
				_pageManager = StorePageManager.Open(_file);
			}

			//
			// read header

			//
			// read Meta
			//  All Meta
			//  Read PageIndex

			//  Page Does'nt have cache
		}

		public void Close()
		{

		}

		public void CreateTable<K, V>(string name)
		{
			// Meta.AddTable(name, typeof(K), typeof(V));
			int tableId = _meta.Tables.Count;
			MetaPage firstIndexPage = _pageManager.AllocEmptyPage();

			_meta.Tables.Add(
				name,
				new StoreMetaTable {
					KeyType = typeof(K).FullName,
					ValueType = typeof(V).FullName,
					TableName = name,
					FirstIndexPage = page,
				}
			);

			_meta.Pages[page].RowCount = 0;

			_file.Seek(_header.MetaBegin, SeekOrigin.Begin);
			BinarySerializer.Serialize(_file, _meta);
		}
	}
}
