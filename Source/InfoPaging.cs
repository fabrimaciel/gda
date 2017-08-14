using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	public class InfoPaging
	{
		private int _startRow;
		private int _pageSize;
		private string _keyFieldName;
		public int StartRow {
			get {
				return _startRow;
			}
			set {
				_startRow = value;
			}
		}
		public int PageSize {
			get {
				return _pageSize;
			}
			set {
				_pageSize = value;
			}
		}
		public string KeyFieldName {
			get {
				return _keyFieldName;
			}
			set {
				_keyFieldName = value;
			}
		}
		public InfoPaging (int a, int b)
		{
			_startRow = a;
			_pageSize = b;
		}
	}
}
