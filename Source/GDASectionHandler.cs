using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
namespace GDA.Common.Configuration.Handlers
{
	#if PocketPC
		    public class GDASectionHandler : BaseSectionHandler
#else
	public class GDASectionHandler : BaseSectionHandler, IConfigurationSectionHandler
	#endif
	{
		public GDASectionHandler () : base (null)
		{
		}
		public object Create (object a, object b, XmlNode c)
		{
			root = c;
			return this;
		}
	}
}
