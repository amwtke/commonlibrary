using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Imps.Services.CommonV4.DbAccess
{
	public interface IDatabaseOperation
	{
		/// <summary>Count in Seconds</summary>
		int CommandTimeout { get; set; }

		int SpExecuteNonQuery(string spName, string[] paramters, params object[] values);
		T SpExecuteScalar<T>(string spName, string[] parameters, params object[] values);
		DataReader SpExecuteReader(string spName, string[] paramters, params object[] values);
		DataTable SpExecuteTable(string spName, string[] parameters, params object[] values);
		DataSet SpExecuteDataSet(string spName, string[] paramters, params object[] values);
		int BulkInsert<T>(string tableName, IEnumerable<T> values);
		int BulkInsert(string tableName, DataTable table);
        int BulkInsert(string tableName, DataTable table, string[] colNames);

		int ExecuteNonQuery(string sql, params object[] parameters);
		T ExecuteScalar<T>(string sql, params object[] parameters);
		DataReader ExecuteReader(string sql, params object[] parameters);
		DataTable ExecuteTable(string sql, params object[] parameters);


        string GetDatabaseName();
		string FormatSql(string spName, string[] parameters, params object[] values);
	}
}
