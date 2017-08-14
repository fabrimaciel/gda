using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ReferenceMapping : ElementMapping, IEquatable<ReferenceMapping>
	{
		public string AssemblyName {
			get;
			set;
		}
		public string ResourceName {
			get;
			set;
		}
		public string FileName {
			get;
			set;
		}
		public ReferenceMapping (XmlElement a)
		{
			AssemblyName = GetAttributeString (a, "assemblyName");
			ResourceName = GetAttributeString (a, "resourceName");
			FileName = GetAttributeString (a, "fileName");
		}
		public bool Equals (ReferenceMapping a)
		{
			if (a == null)
				return false;
			return this.AssemblyName == a.AssemblyName && this.FileName == a.FileName && this.ResourceName == a.ResourceName;
		}
	}
}
