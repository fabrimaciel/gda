using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public class NativeWhereClause : WhereClause<NativeWhereClause>
	{
		public NativeWhereClause (ConditionalContainer a) : base (a)
		{
		}
	}
}
