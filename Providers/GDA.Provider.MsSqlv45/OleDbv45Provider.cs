using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GDA.Provider.MsSqlv45
{
	public class OleDbv45Provider : global::GDA.Provider.MsSql.MsSqlProvider
	{
		public override System.Data.IDbCommand CreateCommand ()
		{
			return new System.Data.OleDb.OleDbCommand ();
		}
		public override System.Data.IDbConnection CreateConnection ()
		{
			return new System.Data.OleDb.OleDbConnection ();
		}
		public override System.Data.IDbDataAdapter CreateDataAdapter ()
		{
			return new System.Data.OleDb.OleDbDataAdapter ();
		}
		public override System.Data.Common.DbParameter CreateParameter ()
		{
			return new System.Data.OleDb.OleDbParameter ();
		}
	}
}
