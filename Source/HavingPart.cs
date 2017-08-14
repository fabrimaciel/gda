using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class HavingPart
	{
		private List<SqlExpression> _expressions = new List<SqlExpression> ();
		public List<SqlExpression> Expressions {
			get {
				return _expressions;
			}
			set {
				_expressions = value;
			}
		}
	}
}
