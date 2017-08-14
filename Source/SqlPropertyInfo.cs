using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public class SqlPropertyInfo
	{
		private string _propertyName;
		private object _value;
		private Operator _sqlOperator;
		private bool _useOrderBy;
		private int _orderByPos;
		private Order _orderType = Order.Ascending;
		public SqlPropertyInfo (string a, object b) : this (a, b, Operator.Equals)
		{
		}
		public SqlPropertyInfo (string a, object b, Operator c)
		{
			_propertyName = a.Trim ();
			_value = b;
			_sqlOperator = c;
		}
		public string PropertyName {
			get {
				return _propertyName;
			}
			set {
				_propertyName = value.Trim ();
			}
		}
		public object Value {
			get {
				return _value;
			}
			set {
				_value = value;
			}
		}
		public Operator SqlOperator {
			get {
				return _sqlOperator;
			}
			set {
				_sqlOperator = value;
			}
		}
		public bool UseOrderBy {
			get {
				return _useOrderBy;
			}
			set {
				_useOrderBy = value;
			}
		}
		public int OrderByPos {
			get {
				return _orderByPos;
			}
			set {
				_orderByPos = value;
			}
		}
		public Order OrderType {
			get {
				return _orderType;
			}
			set {
				_orderType = value;
			}
		}
	}
}
