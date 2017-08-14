using System;
using System.Collections.Generic;
using System.Text;
using GDA.Common.Configuration.Attributes;
using System.Reflection;
using GDA.Common.Helper;
using System.Xml;
using System.Collections;
namespace GDA.Common.Configuration.Targets
{
	internal class MethodTarget : ElementTarget
	{
		private object _lock = new object ();
		private MethodDispatcher dispatcher;
		public MethodTarget (ConfigurationAttribute a, MethodInfo b) : base (a)
		{
			dispatcher = new MethodDispatcher (new MethodInvoker (b, a.RequiredParameters));
		}
		public void AddCallbackMethod (MethodInfo a, int b)
		{
			lock (_lock) {
				dispatcher.AddInvoker (new MethodInvoker (a, b));
			}
		}
		private Hashtable ExtractAttributes (XmlNode a)
		{
			Hashtable b = new Hashtable ();
			foreach (XmlAttribute attr in a.Attributes) {
				b [attr.Name.ToLower ()] = attr.Value;
			}
			return b;
		}
		public override void Configure (object a, XmlNode b)
		{
			Hashtable c = ExtractAttributes (b);
			dispatcher.Invoke (a, c);
		}
	}
}
