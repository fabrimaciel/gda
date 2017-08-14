using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public abstract class ForeignItemMapping : ElementMapping
	{
		public TypeInfo TypeOfClassRelated {
			get;
			set;
		}
		public string PropertyName {
			get;
			set;
		}
		public string GroupOfRelationship {
			get;
			set;
		}
		public ForeignItemMapping (XmlElement a, string b, string c)
		{
			var d = GetAttributeString (a, "typeOfClassRelated", true);
			TypeOfClassRelated = new TypeInfo (d, b, c);
			PropertyName = GetAttributeString (a, "propertyName", true);
			GroupOfRelationship = GetAttributeString (a, "groupOfRelationship");
		}
		public ForeignItemMapping (TypeInfo a, string b, string c)
		{
			if (a == null)
				throw new ArgumentNullException ("typeOfClassRelated");
			else if (string.IsNullOrEmpty (b))
				throw new ArgumentNullException ("propertyName");
			this.TypeOfClassRelated = a;
			this.PropertyName = b;
			this.GroupOfRelationship = c;
		}
	}
}
