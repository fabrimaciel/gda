using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class FromPart
	{
		private List<TableExpression> _tableExpressions = new List<TableExpression> ();
		public List<TableExpression> TableExpressions {
			get {
				return _tableExpressions;
			}
			set {
				_tableExpressions = value;
			}
		}
	}
}
