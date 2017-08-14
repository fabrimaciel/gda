using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public class TableName
	{
		public string Name {
			get;
			set;
		}
		public string Schema {
			get;
			set;
		}
		public TableName ()
		{
		}
		public TableName (string a, string b)
		{
			this.Name = a;
			this.Schema = b;
		}
		public override string ToString ()
		{
			return !string.IsNullOrEmpty (Schema) ? Schema + "." + Name : Name;
		}
	}
}
