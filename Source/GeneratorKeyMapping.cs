using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class GeneratorKeyMapping : ElementMapping
	{
		public string Name {
			get;
			set;
		}
		public Type ClassType {
			get;
			set;
		}
		public GeneratorKeyMapping (XmlElement a)
		{
			Name = GetAttributeString (a, "name", true);
			var b = GetAttributeString (a, "classType", true);
			ClassType = Type.GetType (b, false, true);
			if (ClassType == null)
				throw new GDAMappingException ("Generator key classe \"{0}\" not found.", b);
		}
	}
}
