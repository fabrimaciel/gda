using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Common.Configuration.Attributes
{
	[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	internal sealed class ConfigurationAttribute : Attribute
	{
		private string _xmlNodePath;
		private ConfigKeyPresence _keyPresenceRequirement = ConfigKeyPresence.Mandatory;
		private int _requiredParameters;
		public string XmlNodePath {
			get {
				return _xmlNodePath;
			}
			set {
				_xmlNodePath = value;
			}
		}
		public ConfigKeyPresence KeyPresenceRequirement {
			get {
				return _keyPresenceRequirement;
			}
			set {
				_keyPresenceRequirement = value;
			}
		}
		public int RequiredParameters {
			get {
				return _requiredParameters;
			}
			set {
				_requiredParameters = value;
			}
		}
		public ConfigurationAttribute (string a)
		{
			_xmlNodePath = a;
		}
		public ConfigurationAttribute (string a, ConfigKeyPresence b) : this (a)
		{
			_keyPresenceRequirement = b;
		}
		public ConfigurationAttribute (string a, ConfigKeyPresence b, int c) : this (a, b)
		{
			_requiredParameters = c;
		}
	}
}
