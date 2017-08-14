using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression
{
	public class SqlParserException : Exception
	{
		public SqlParserException (string a) : base (a)
		{
		}
		public SqlParserException (string a, Exception b) : base (a, b)
		{
		}
	}
}
