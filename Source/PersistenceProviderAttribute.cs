using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class PersistenceProviderAttribute : Attribute
	{
		private string _providerConfigurationName;
		private string _providerName;
		private string _connectionString;
		public string ProviderConfigurationName {
			get {
				return _providerConfigurationName;
			}
			set {
				_providerConfigurationName = value;
			}
		}
		public string ProviderName {
			get {
				return _providerName;
			}
			set {
				_providerName = value;
			}
		}
		public string ConnectionString {
			get {
				return _connectionString;
			}
			set {
				_connectionString = value;
			}
		}
		public PersistenceProviderAttribute ()
		{
		}
		public PersistenceProviderAttribute (string a)
		{
			_providerConfigurationName = a;
		}
	}
}
