namespace GDA.Common.Configuration.Targets
{
	using System.Xml;
	using GDA.Common.Configuration.Attributes;
	internal abstract class ElementTarget
	{
		internal readonly string XmlNodePath;
		internal readonly ConfigKeyPresence KeyPresenceRequirement;
		public ElementTarget (ConfigurationAttribute a)
		{
			this.XmlNodePath = a.XmlNodePath;
			this.KeyPresenceRequirement = a.KeyPresenceRequirement;
		}
		public abstract void Configure (object a, XmlNode b);
		public void Configure (object a, XmlNodeList b)
		{
			foreach (XmlNode node in b)
				Configure (a, node);
		}
	}
}
