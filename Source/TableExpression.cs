using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class TableNameExpression
	{
		private string _schema;
		private Expression _innerExpression;
		public string Name {
			get {
				return _innerExpression.Text;
			}
			set {
				_innerExpression.Text = value;
			}
		}
		public string Schema {
			get {
				return _schema;
			}
			set {
				_schema = value;
			}
		}
		public Expression InnerExpression {
			get {
				return _innerExpression;
			}
			set {
				_innerExpression = value;
			}
		}
		public TableNameExpression (Expression a)
		{
			_innerExpression = a;
		}
	}
	class TableExpression
	{
		private TableNameExpression _tableName;
		private Select _selectInfo;
		private Expression _tableAlias;
		private Expression _asExpression;
		private Expression _leftOrRight;
		private Expression _outerOrInnerOrCrossOrNatural;
		private Expression _join;
		private Expression _on;
		private ContainerSqlExpression _onExpressions;
		public TableNameExpression TableName {
			get {
				return _tableName;
			}
			set {
				_tableName = value;
			}
		}
		public Select SelectInfo {
			get {
				return _selectInfo;
			}
			set {
				_selectInfo = value;
			}
		}
		public Expression TableAlias {
			get {
				return _tableAlias;
			}
			set {
				_tableAlias = value;
			}
		}
		public Expression AsExpression {
			get {
				return _asExpression;
			}
			set {
				_asExpression = value;
			}
		}
		public Expression LeftOrRight {
			get {
				return _leftOrRight;
			}
			set {
				_leftOrRight = value;
			}
		}
		public Expression OuterOrInnerOrCrossOrNatural {
			get {
				return _outerOrInnerOrCrossOrNatural;
			}
			set {
				_outerOrInnerOrCrossOrNatural = value;
			}
		}
		public Expression Join {
			get {
				return _join;
			}
			set {
				_join = value;
			}
		}
		public Expression On {
			get {
				return _on;
			}
			set {
				_on = value;
			}
		}
		public ContainerSqlExpression OnExpressions {
			get {
				return _onExpressions;
			}
			set {
				_onExpressions = value;
			}
		}
	}
}
