using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class OrderByPart
	{
		private List<OrderByExpression> _orderByExpressions = new List<OrderByExpression> ();
		public List<OrderByExpression> OrderByExpressions {
			get {
				return _orderByExpressions;
			}
			set {
				_orderByExpressions = value;
			}
		}
	}
}
