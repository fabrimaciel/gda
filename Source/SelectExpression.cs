using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression.Enums;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class SelectExpression
	{
		private SqlExpression _columnName;
		private SqlExpression _columnAlias;
		private Expression _asExpression;
		public Expression AsExpression {
			get {
				return _asExpression;
			}
			set {
				_asExpression = value;
			}
		}
		public SqlExpression ColumnName {
			get {
				return _columnName;
			}
			set {
				_columnName = value;
			}
		}
		public SqlExpression ColumnAlias {
			get {
				return _columnAlias;
			}
			set {
				_columnAlias = value;
			}
		}
		public Column Column {
			get {
				if (ColumnName.Type == SqlExpressionType.Column)
					return new Column (ColumnName, ColumnAlias);
				else
					throw new SqlParserException ("Invalid load column.");
			}
		}
		public SelectExpression ()
		{
		}
		public SelectExpression (SqlExpression a) : this (a, null)
		{
		}
		public SelectExpression (SqlExpression a, SqlExpression b) : this (a, b, null)
		{
		}
		public SelectExpression (SqlExpression a, SqlExpression b, Expression c)
		{
			_columnName = a;
			_columnAlias = b;
			_asExpression = c;
		}
	}
}
