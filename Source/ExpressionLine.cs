using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression
{
	class ExpressionLine
	{
		private List<Expression> _expressions = new List<Expression> ();
		private int beginPoint;
		private int length;
		public int BeginPoint {
			get {
				return beginPoint;
			}
			internal set {
				beginPoint = value;
			}
		}
		public int Length {
			get {
				return length;
			}
			internal set {
				length = value;
			}
		}
		public List<Expression> Expressions {
			get {
				return _expressions;
			}
		}
		public ExpressionLine (int a)
		{
			this.beginPoint = a;
		}
		public override string ToString ()
		{
			StringBuilder a = new StringBuilder ();
			foreach (Expression ex in _expressions)
				a.Append (ex.ToString ()).Append (" ");
			return a.ToString ();
		}
	}
}
