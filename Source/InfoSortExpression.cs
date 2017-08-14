using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class InfoSortExpression
	{
		private string _sortColumn;
		private bool _reverse;
		private string _aliasTable;
		private string _defaultFieldSort;
		public string SortColumn {
			get {
				return _sortColumn;
			}
			set {
				_sortColumn = value;
			}
		}
		public bool Reverse {
			get {
				return _reverse;
			}
			set {
				_reverse = value;
			}
		}
		public string AliasTable {
			get {
				return _aliasTable;
			}
			set {
				_aliasTable = value;
			}
		}
		public string DefaultFieldSort {
			get {
				return _defaultFieldSort;
			}
			set {
				_defaultFieldSort = value;
			}
		}
		public InfoSortExpression ()
		{
		}
		public InfoSortExpression (string a)
		{
			if (a == null || a == "")
				return;
			_reverse = a.EndsWith (" desc", StringComparison.InvariantCultureIgnoreCase);
			if (_reverse)
				_sortColumn = a.Substring (0, a.Length - 5);
			else {
				if (a.EndsWith (" asc", StringComparison.InvariantCultureIgnoreCase))
					a = a.Substring (0, a.Length - " asc".Length);
				_sortColumn = a;
			}
		}
		public InfoSortExpression (string a, string b) : this (a)
		{
			_defaultFieldSort = b;
			if (a == null || a == "")
				a = b;
		}
		public InfoSortExpression (string a, string b, string c) : this (a, b)
		{
			_aliasTable = c;
		}
	}
}
