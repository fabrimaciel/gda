using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public class QueryException : GDAException
	{
		public QueryException (string a) : base (a)
		{
		}
		public QueryException (Exception a) : base (a.Message, a)
		{
		}
		public QueryException (string a, Exception b) : base (a, b)
		{
		}
		public QueryException (string a, params object[] b) : this (String.Format (a, b))
		{
		}
	}
}
