using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class SqlFunction : SqlExpression
	{
		private List<List<SqlExpression>> _parameters = new List<List<SqlExpression>> ();
		public List<List<SqlExpression>> Parameters {
			get {
				return _parameters;
			}
			set {
				_parameters = value;
			}
		}
		public SqlFunction (Expression a) : base (a, GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function)
		{
		}
		public override string ToString ()
		{
			StringBuilder a = new StringBuilder (base.Value.Text).Append ("(");
			bool b = false;
			foreach (List<SqlExpression> l in Parameters) {
				if (b)
					a.Append (", ");
				foreach (SqlExpression se in l)
					a.Append (se.Value.ToString ());
				b = true;
			}
			a.Append (")");
			return a.ToString ();
		}
	}
}
