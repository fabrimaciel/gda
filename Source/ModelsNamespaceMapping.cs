using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ModelsNamespaceMapping : ElementMapping
	{
		public string Namespace {
			get;
			set;
		}
		public string Assembly {
			get;
			set;
		}
		public ModelsNamespaceMapping (XmlElement a)
		{
			Namespace = GetAttributeString (a, "name", true);
			Assembly = GetAttributeString (a, "assembly", true);
		}
	}
}
