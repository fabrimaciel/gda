using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
namespace GDA.Mapping
{
	public class ProviderMapping : ElementMapping
	{
		public string Name {
			get;
			set;
		}
		public string ConfigurationName {
			get;
			set;
		}
		public string ConnectionString {
			get;
			set;
		}
		public ProviderMapping (XmlElement a)
		{
			Name = GetAttributeString (a, "name");
			ConfigurationName = GetAttributeString (a, "configurationName", true);
			ConnectionString = GetAttributeString (a, "connectionString");
		}
		public ProviderMapping (string a, string b, string c)
		{
			if (string.IsNullOrEmpty (b))
				throw new ArgumentNullException ("configurationName");
			this.Name = a;
			this.ConfigurationName = ConfigurationName;
			this.ConnectionString = c;
		}
		public PersistenceProviderAttribute GetPersistenceProvider ()
		{
			return new PersistenceProviderAttribute {
				ProviderName = Name,
				ConnectionString = ConnectionString,
				ProviderConfigurationName = ConfigurationName
			};
		}
	}
}
