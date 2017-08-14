using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace GDA.Interfaces
{
	public interface IPersistenceObjectBase
	{
		IProvider UserProvider {
			get;
		}
		IProviderConfiguration Configuration {
			get;
		}
		IDbConnection CreateConnection ();
		int ExecuteCommand (string a, params GDAParameter[] b);
		int ExecuteCommand (string a);
		int ExecuteSqlQueryCount (string a, params GDAParameter[] b);
		object ExecuteScalar (string a, params GDAParameter[] b);
		object ExecuteScalar (string a);
	}
}
