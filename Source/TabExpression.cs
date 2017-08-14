using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression
{
	class TabExpression : Expression
	{
		public TabExpression (int a, ExpressionLine b, string c) : base (a, 1, b, c)
		{
		}
	}
}
