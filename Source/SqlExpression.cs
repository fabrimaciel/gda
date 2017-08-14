using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression.Enums;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class SqlExpression
	{
		private Expression _value;
		private SqlExpressionType _type;
		public Expression Value {
			get {
				return _value;
			}
			set {
				_value = value;
			}
		}
		public SqlExpressionType Type {
			get {
				return _type;
			}
			set {
				_type = value;
			}
		}
		internal SqlExpression (Expression a)
		{
			switch (a.Token) {
			case TokenID.Identifier:
				if (a.Text [0] == '?' || a.Text [0] == '@')
					_type = SqlExpressionType.Variable;
				else
					_type = SqlExpressionType.Column;
				break;
			case TokenID.IntLiteral:
			case TokenID.DecimalLiteral:
			case TokenID.RealLiteral:
				_type = SqlExpressionType.NumericLiteral;
				break;
			case TokenID.StringLiteral:
				_type = SqlExpressionType.StringLiteral;
				break;
			case TokenID.Star:
			case TokenID.Plus:
			case TokenID.Slash:
			case TokenID.Minus:
			case TokenID.kLike:
			case TokenID.kBetween:
				_type = SqlExpressionType.Operation;
				break;
			case TokenID.EqualEqual:
			case TokenID.Equal:
			case TokenID.Greater:
			case TokenID.GreaterEqual:
			case TokenID.Less:
			case TokenID.LessEqual:
			case TokenID.NotEqual:
			case TokenID.kNot:
			case TokenID.kIs:
			case TokenID.kIsNull:
				_type = SqlExpressionType.Comparation;
				break;
			case TokenID.kSelect:
				_type = SqlExpressionType.Select;
				break;
			case TokenID.kAnd:
			case TokenID.kOr:
				_type = SqlExpressionType.Boolean;
				break;
			case TokenID.kIn:
			case TokenID.kAny:
			case TokenID.kSome:
			case TokenID.kAll:
				_type = SqlExpressionType.ComparerScalar;
				break;
			default:
				_type = SqlExpressionType.Column;
				break;
			}
			_value = a;
		}
		public SqlExpression (Expression a, SqlExpressionType b)
		{
			_type = b;
			if (b == SqlExpressionType.Column && a.Length > 0 && (a.Text [0] == '?' || a.Text [0] == '@'))
				_type = SqlExpressionType.Variable;
			_value = a;
		}
		public override string ToString ()
		{
			return _value.Text;
		}
	}
}
