using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Imps.Common.ObjectStore;

namespace Imps.Services.CommonV4
{
	public class ObjectStore: IDisposable
	{
        public static ObjectStore Create(string fileName)
        {
            throw new NotImplementedException();
        }

        public static ObjectStore Open(string fileName)
        {
            throw new NotFiniteNumberException();
        }

        public static void Cki(string fileName)
        {
        }

        private string _fileName;
        private StoreEngine _engine;

        private ObjectStore()
        {
        }

        public void CreateTable<K, V>(string tableName)
        {
        }

        public bool Read<K,V>(string tableName, K key, out V val)
        {
            throw new NotImplementedException();
        }

        public void Write<K, V>(string tableName, K key, V val)
        {
            throw new NotImplementedException();
        }

        public void Insert<K, V>(string tableName, K key, V val)
        {
            throw new NotImplementedException();
        }

        public void Update<K, V>(string tableName, K key, V val)
        {
            throw new NotImplementedException();
        }

        public void Delete<K, V>(string tableName, K key, V val)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<V> Select<K, V>(string tableName, Func<K, bool> func)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<V> Scan<V>(string tableName)
        {
            return Scan<V>(tableName, null);
        }

        public IEnumerable<V> Scan<V>(string tableName, Func<V, bool> func)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
