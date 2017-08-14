using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression
{
	public class SqlLexerException : Exception
	{
		public SqlLexerException (string a) : base (a)
		{
		}
		public SqlLexerException (string a, Exception b) : base (a, b)
		{
		}
	}
}
