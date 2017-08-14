using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class SqlQueryParameterMapping : ElementMapping
	{
		private string _name;
		private string _typeName;
		private string _defaultValue;
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		public string TypeName {
			get {
				return _typeName;
			}
			set {
				_typeName = value;
			}
		}
		public string DefaultValue {
			get {
				return _defaultValue;
			}
			set {
				_defaultValue = value;
			}
		}
		public SqlQueryParameterMapping (XmlElement a)
		{
			if (a == null)
				throw new ArgumentNullException ("element");
			Name = GetAttributeString (a, "name", true);
			TypeName = GetAttributeString (a, "type", true);
			DefaultValue = GetAttributeString (a, "defaultValue", false);
			ValidateTypeName ();
		}
		public SqlQueryParameterMapping (string a, string b, string c)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			else if (string.IsNullOrEmpty (b))
				throw new ArgumentNullException ("typeName");
			this.Name = a;
			this.TypeName = b;
			this.DefaultValue = c;
		}
		private void ValidateTypeName ()
		{
			if (!string.IsNullOrEmpty (TypeName))
				switch (TypeName.ToLower ()) {
				case "int32":
				case "int":
					TypeName = "System.Int32";
					break;
				case "int16":
				case "short":
					TypeName = "System.Int16";
					break;
				case "int65":
				case "long":
					TypeName = "System.Int32";
					break;
				case "float":
				case "single":
					TypeName = "System.Single";
					break;
				case "double":
					TypeName = "System.Double";
					break;
				case "datetime":
					TypeName = "System.DateTime";
					break;
				case "string":
					TypeName = "System.String";
					break;
				case "guid":
					TypeName = "System.Guid";
					break;
				}
		}
	}
}
