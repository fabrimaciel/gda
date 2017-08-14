using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class SqlQueryReturnPropertyMapping : ElementMapping
	{
		private string _name;
		private string _column;
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		public string Column {
			get {
				return _column;
			}
			set {
				_column = value;
			}
		}
		public SqlQueryReturnPropertyMapping (XmlElement a)
		{
			if (a == null)
				throw new ArgumentNullException ("element");
			Name = GetAttributeString (a, "name", true);
			Column = GetAttributeString (a, "column", false);
		}
	}
}
