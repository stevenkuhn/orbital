using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Dapper
{
	public interface IDbContext
	{
		IDbConnection GetConnection();
	}

	public class SqlContext : IDbContext, IDisposable
	{
		private readonly string ConnectionString;
		private static readonly object @lock = new object();
		private SqlConnection Connection;

		public SqlContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public IDbConnection GetConnection()
		{
			if (Connection == null)
			{
				lock (@lock)
				{
					if (Connection == null)
					{
						Connection = new SqlConnection(ConnectionString);
						Connection.Disposed += new EventHandler(Connection_Disposed);
						Connection.Open();
					}
				}
			}

			return Connection;
		}

		void Connection_Disposed(object sender, EventArgs e)
		{
			Connection = null;
		}

		public void Dispose()
		{
			if (Connection != null)
			{
				lock (@lock)
				{
					if (Connection != null)
					{
						Connection.Dispose();
					}
				}
			}
		}
	}
}