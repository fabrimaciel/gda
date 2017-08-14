using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	internal class Column
	{
		private string _name;
		private string _tableName;
		private string _alias;
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		public string TableName {
			get {
				return _tableName;
			}
			set {
				_tableName = value;
			}
		}
		public string Alias {
			get {
				return _alias;
			}
			set {
				_alias = value;
			}
		}
		public Column (string a, string b, string c)
		{
			_tableName = a;
			_name = b;
			_alias = c;
		}
		internal Column (SqlExpression a, SqlExpression b)
		{
			string c = a.Value.Text;
			int d = c.IndexOf ('.');
			if (d >= 0) {
				_tableName = c.Substring (0, d);
				_name = c.Substring (d + 1, c.Length - d - 1);
			}
			else
				_name = c;
			if (b != null)
				_alias = b.Value.Text;
		}
	}
}
