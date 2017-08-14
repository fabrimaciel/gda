using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class WherePart
	{
		private Expression _where;
		private List<SqlExpression> _expressions;
		public Expression Where {
			get {
				return _where;
			}
			set {
				_where = value;
			}
		}
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
