using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class OrderByExpression
	{
		private ContainerSqlExpression _expression;
		private Expression _ascOrDesc;
		private bool _asc = true;
		private bool _nulls = false;
		private bool _first = true;
		public ContainerSqlExpression Expression {
			get {
				return _expression;
			}
			set {
				_expression = value;
			}
		}
		public Expression AscOrDesc {
			get {
				return _ascOrDesc;
			}
			set {
				_ascOrDesc = value;
			}
		}
		public bool Asc {
			get {
				return _asc;
			}
			set {
				_asc = value;
			}
		}
		public bool Nulls {
			get {
				return _nulls;
			}
			set {
				_nulls = value;
			}
		}
		public bool First {
			get {
				return _first;
			}
			set {
				_first = value;
			}
		}
	}
}
