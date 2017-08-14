using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ClassMapping : ElementMapping
	{
		public TypeInfo TypeInfo {
			get;
			set;
		}
		public string Table {
			get;
			set;
		}
		public string Schema {
			get;
			set;
		}
		public BaseDAOMapping BaseDAO {
			get;
			set;
		}
		public ProviderMapping Provider {
			get;
			set;
		}
		public List<PropertyMapping> Properties {
			get;
			set;
		}
		public ClassMapping (XmlElement a, string b, string c, string d, string e, string f, string g)
		{
			var h = GetAttributeString (a, "name", true);
			this.TypeInfo = new TypeInfo (h, b, c);
			Table = GetAttributeString (a, "table", this.TypeInfo.Name);
			Schema = GetAttributeString (a, "schema");
			if (string.IsNullOrEmpty (Schema))
				Schema = g;
			XmlElement j = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("baseDAO"));
			if (j != null)
				BaseDAO = new BaseDAOMapping (j, b, c);
			var k = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("provider"));
			if (k != null)
				Provider = new ProviderMapping (k);
			else if (!string.IsNullOrEmpty (e)) {
				Provider = new ProviderMapping (d, e, f);
			}
			Properties = new List<PropertyMapping> ();
			foreach (XmlElement i in a.GetElementsByTagName ("property")) {
				var l = new PropertyMapping (i, b, c);
				if (!Properties.Exists (m => m.Name == l.Name))
					Properties.Add (l);
			}
		}
		public ClassMapping (TypeInfo a, string b, string c, BaseDAOMapping d, ProviderMapping e, IEnumerable<PropertyMapping> f)
		{
			if (a == null)
				throw new ArgumentNullException ("typeInfo");
			TypeInfo = a;
			if (string.IsNullOrEmpty (b))
				Table = a.Name;
			else
				Table = b;
			Schema = c;
			BaseDAO = d;
			Provider = e;
			Properties = new List<PropertyMapping> ();
			if (f != null)
				foreach (var i in f)
					if (!Properties.Exists (g => g.Name == i.Name))
						Properties.Add (i);
		}
		public PersistenceClassAttribute GetPersistenceClass ()
		{
			return new PersistenceClassAttribute (Table) {
				Schema = Schema
			};
		}
	}
}
