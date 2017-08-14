using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression.Enums;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class ContainerSqlExpression : SqlExpression
	{
		private List<SqlExpression> _expressions = new List<SqlExpression> ();
		private TokenID _containerToken;
		public List<SqlExpression> Expressions {
			get {
				return _expressions;
			}
			set {
				_expressions = value;
			}
		}
		public TokenID ContainerToken {
			get {
				return _containerToken;
			}
			set {
				_containerToken = value;
			}
		}
		public ContainerSqlExpression (Expression a) : base (a, SqlExpressionType.Container)
		{
			_containerToken = a.Token;
		}
	}
}
