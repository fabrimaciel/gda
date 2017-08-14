using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ValidatorParamMapping : ElementMapping
	{
		public string Name {
			get;
			set;
		}
		public string Value {
			get;
			set;
		}
		public ValidatorParamMapping (XmlElement a)
		{
			if (a == null)
				throw new ArgumentNullException ("element");
			Name = GetAttributeString (a, "name", true);
			Value = a.IsEmpty ? null : a.InnerText;
		}
		public ValidatorParamMapping (string a, string b)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			this.Name = a;
			this.Value = b;
		}
	}
}
