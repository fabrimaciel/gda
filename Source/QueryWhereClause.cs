using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace GDA.Sql
{
	public class QueryWhereClause : WhereClause<QueryWhereClause>
	{
		private Query _query;
		public QueryWhereClause (Query a, ConditionalContainer b) : base (b)
		{
			if (a == null)
				throw new ArgumentNullException ("query");
			_query = a;
		}
		public Query Query {
			get {
				return _query;
			}
		}
	}
}
