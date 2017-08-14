using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class SelectVariable : SqlExpression
	{
		public SelectVariable (Expression a) : base (a)
		{
		}
	}
}
