using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
namespace GDA.Common.Configuration.Handlers
{
	public class BaseSectionHandler
	{
		protected XmlNode root;
		public bool IsValid {
			get {
				return root != null;
			}
		}
		#if !PocketPC
		public IXPathNavigable XmlRoot {
			get {
				return root;
			}
		}
		#endif
		public BaseSectionHandler (XmlNode a)
		{
			root = a;
		}
		public XmlNode GetNode (string a)
		{
			return root.SelectSingleNode (a);
		}
		public XmlNodeList GetNodes (string a)
		{
			return root.SelectNodes (a);
		}
	}
}
