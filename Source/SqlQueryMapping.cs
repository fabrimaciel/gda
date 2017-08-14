using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class SqlQueryMapping : ElementMapping
	{
		private string _name;
		private bool _useDatabaseSchema = true;
		private List<SqlQueryParameterMapping> _parameters = new List<SqlQueryParameterMapping> ();
		private SqlQueryReturnMapping _return;
		private string _query;
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		public bool UseDatabaseSchema {
			get {
				return _useDatabaseSchema;
			}
			set {
				_useDatabaseSchema = value;
			}
		}
		public List<SqlQueryParameterMapping> Parameters {
			get {
				return _parameters;
			}
		}
		public SqlQueryReturnMapping Return {
			get {
				return _return;
			}
			set {
				_return = value;
			}
		}
		public string Query {
			get {
				return _query;
			}
			set {
				_query = value;
			}
		}
		public SqlQueryMapping (XmlElement a)
		{
			Name = GetAttributeString (a, "name", true);
			var b = false;
			string c = GetAttributeString (a, "use-database-schema", "true");
			#if PocketPC
						            if (GDA.Helper.GDAHelper.TryParse(val, out boolVal))
#else
			if (bool.TryParse (c, out b))
				#endif
				UseDatabaseSchema = b;
			else
				UseDatabaseSchema = true;
			var d = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("parameters"));
			if (d != null)
				foreach (XmlElement i in d.GetElementsByTagName ("param")) {
					var e = new SqlQueryParameterMapping (i);
					if (!Parameters.Exists (f => f.Name == e.Name))
						Parameters.Add (e);
				}
			var g = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("return"));
			if (g != null)
				Return = new SqlQueryReturnMapping (g);
			var h = FirstOrDefault<XmlElement> (a.GetElementsByTagName ("commandText"));
			if (h != null || !string.IsNullOrEmpty (h.InnerText))
				Query = h.InnerText.TrimStart ('\n', '\t').TrimEnd ('\n', '\t');
		}
		public SqlQueryMapping (string a, bool b, SqlQueryReturnMapping c, string d, IEnumerable<SqlQueryParameterMapping> e)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			this.Name = a;
			this.UseDatabaseSchema = b;
			this.Return = c;
			this.Query = d;
			if (e != null)
				foreach (var i in e)
					if (!Parameters.Exists (f => f.Name == i.Name))
						Parameters.Add (i);
		}
	}
}
