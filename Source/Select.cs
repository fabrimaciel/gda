using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class Select : SqlExpression
	{
		private SelectPart _selectPart;
		private FromPart _fromPart;
		private WherePart _wherePart;
		private GroupByPart _groupByPart;
		private HavingPart _havingPart;
		private OrderByPart _orderByPart;
		public SelectPart SelectPart {
			get {
				return _selectPart;
			}
			set {
				_selectPart = value;
			}
		}
		public FromPart FromPart {
			get {
				return _fromPart;
			}
			set {
				_fromPart = value;
			}
		}
		public WherePart WherePart {
			get {
				return _wherePart;
			}
			set {
				_wherePart = value;
			}
		}
		public GroupByPart GroupByPart {
			get {
				return _groupByPart;
			}
			set {
				_groupByPart = value;
			}
		}
		public HavingPart HavingPart {
			get {
				return _havingPart;
			}
			set {
				_havingPart = value;
			}
		}
		public OrderByPart OrderByPart {
			get {
				return _orderByPart;
			}
			set {
				_orderByPart = value;
			}
		}
		internal Select (Expression a) : base (a)
		{
		}
	}
}
