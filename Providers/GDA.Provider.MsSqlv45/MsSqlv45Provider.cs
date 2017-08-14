using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GDA.Provider.MsSqlv45
{
	public class MsSqlv45Provider : global::GDA.Provider.MsSql.MsSqlProvider
	{
		public MsSqlv45Provider ()
		{
			this.MsSqlDialect = global::GDA.Provider.MsSql.MsSqlProviderDialects.MsSql2005;
		}
		public override System.Data.IDbCommand CreateCommand ()
		{
			return new SqlCommandFix ((System.Data.SqlClient.SqlCommand)base.CreateCommand ());
		}
	}
}
