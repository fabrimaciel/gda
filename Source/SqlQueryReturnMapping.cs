using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class SqlQueryReturnMapping : ElementMapping
	{
		private string _classTypeName;
		private List<SqlQueryReturnPropertyMapping> _returnProperties = new List<SqlQueryReturnPropertyMapping> ();
		public string ClassTypeName {
			get {
				return _classTypeName;
			}
			set {
				_classTypeName = value;
			}
		}
		public List<SqlQueryReturnPropertyMapping> ReturnProperties {
			get {
				return _returnProperties;
			}
		}
		public SqlQueryReturnMapping (XmlElement a)
		{
			if (a == null)
				throw new ArgumentNullException ("element");
			ClassTypeName = GetAttributeString (a, "class", false);
			foreach (XmlElement i in a.GetElementsByTagName ("return-property")) {
				var b = new SqlQueryReturnPropertyMapping (i);
				if (!ReturnProperties.Exists (c => c.Name == b.Name))
					ReturnProperties.Add (b);
			}
		}
		public SqlQueryReturnMapping (string a, IEnumerable<SqlQueryReturnPropertyMapping> b)
		{
			this.ClassTypeName = a;
			if (b != null)
				foreach (var i in b)
					if (!ReturnProperties.Exists (c => c.Name == i.Name))
						ReturnProperties.Add (i);
		}
	}
}
