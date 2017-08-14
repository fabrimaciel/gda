using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class GroupByPart
	{
		private Expression _group;
		private Expression _by;
		private List<SqlExpression> _expressions = new List<SqlExpression> ();
		public Expression Group {
			get {
				return _group;
			}
			set {
				_group = value;
			}
		}
		public Expression By {
			get {
				return _by;
			}
			set {
				_by = value;
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
