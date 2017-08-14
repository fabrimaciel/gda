using System;
using System.Collections.Generic;
using System.Text;
using GDA.Common.Configuration.Attributes;
using GDA.Interfaces;
using System.Reflection;
using System.Collections;
using GDA.Common.Configuration;
namespace GDA
{
	public class GDASettings
	{
		private static object _loadingObjLock = new object ();
		private static bool loadDataConfig = false;
		internal const string NodePrefix = "GDA";
		private static Hashtable _providerConfiguration = new Hashtable ();
		private static Hashtable _providersConfigurationConstructors = new Hashtable ();
		private static Hashtable _providersConfigurationInfo = new Hashtable ();
		private static Dictionary<string, ModelsNamespaceInfo> _modelsNamespaces = new Dictionary<string, ModelsNamespaceInfo> ();
		private static ConstructorInfo _constructorProvider;
		private static string _defaultProvider;
		private static IProviderConfiguration _defaultProviderConfiguration = null;
		private static bool _enabledDebugTrace;
		private static int _maximumMapperCache = 10;
		private static System.Converter<string, string> _decryptMethod;
		private static System.Converter<string, string> _encryptMethod;
		private static Dictionary<string, IGeneratorKey> _generatorsKey = new Dictionary<string, IGeneratorKey> ();
		public static string DefaultProviderName {
			get {
				return GDASettings._defaultProvider;
			}
		}
		public static int MaximumMapperCache {
			get {
				return GDASettings._maximumMapperCache;
			}
		}
		public static bool EnabledDebugTrace {
			get {
				return GDASettings._enabledDebugTrace;
			}
			set {
				GDASettings._enabledDebugTrace = value;
			}
		}
		public static System.Converter<string, string> DecryptMethod {
			get {
				return _decryptMethod;
			}
			set {
				_decryptMethod = value;
			}
		}
		public static System.Converter<string, string> EncryptMethod {
			get {
				return _encryptMethod;
			}
			set {
				_encryptMethod = value;
			}
		}
		[Configuration (NodePrefix + "/DefaultProvider")]
		internal static void SetDefaultProvider (string a)
		{
			_defaultProvider = a;
		}
		[Configuration (NodePrefix + "/Cache/MaximumMapper", ConfigKeyPresence.Optional)]
		internal static void SetMaximumMapperCache (int a)
		{
			_maximumMapperCache = a;
		}
		[Configuration (NodePrefix + "/Debug", ConfigKeyPresence.Optional)]
		internal static void Debug (bool a)
		{
			EnabledDebugTrace = a;
		}
		public static void AddProviderConfiguration (string a, IProviderConfiguration b)
		{
			if (_providersConfigurationInfo.ContainsKey (a))
				_providersConfigurationInfo.Remove (a);
			_providersConfigurationInfo.Add (a, b);
		}
		#if !PocketPC
		[Configuration (NodePrefix + "/CryptoClass", KeyPresenceRequirement = ConfigKeyPresence.Optional)]
		internal static void AddCryptoClass (string a, string b, string c)
		{
			Type d = Type.GetType (a);
			if (d == null)
				throw new GDAException ("CryptoClass\r\nType {0} not found", a);
			MethodInfo[] e = d.GetMethods (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			if (!string.IsNullOrEmpty (b)) {
				MethodInfo f = Array.Find (e, delegate (MethodInfo g) {
					return g.Name == b;
				});
				if (f == null)
					throw new GDAException ("CryptoClass\r\nMethod {0} not found in {1}.", b, d.FullName);
				GDASettings._encryptMethod = (Converter<string, string>)Delegate.CreateDelegate (typeof(Converter<string, string>), f);
			}
			if (!string.IsNullOrEmpty (c)) {
				MethodInfo h = Array.Find (e, delegate (MethodInfo g) {
					return g.Name == c;
				});
				if (h == null)
					throw new GDAException ("CryptoClass\r\nMethod {0} not found in {1}.", c, d.FullName);
				GDASettings._decryptMethod = (Converter<string, string>)Delegate.CreateDelegate (typeof(Converter<string, string>), h);
			}
		}
		#endif
		[Configuration (NodePrefix + "/ProvidersConfiguration/Info")]
		internal static void AddProviderConfigurationInfo (string a, string b, string c, string d)
		{
			if (_providersConfigurationInfo.ContainsKey (a))
				_providersConfigurationInfo.Remove (a);
			if (_decryptMethod != null)
				try {
					c = _decryptMethod (c);
				}
				catch (Exception ex) {
					throw new GDAException ("Fail on decrypt connection string.", ex);
				}
			_providersConfigurationInfo.Add (a, new ProviderConfigurationInfo (a, b, c, d));
		}
		[Configuration (NodePrefix + "/ModelsNamespace/Namespace", ConfigKeyPresence.Optional)]
		public static void AddModelsNamespace (string a, string b)
		{
			if (a == "*") {
				#if PocketPC
								                AddModelsNamespace(System.Reflection.Assembly.GetExecutingAssembly().FullName, name);
#else
				AddModelsNamespace (System.Reflection.Assembly.GetEntryAssembly ().FullName, b);
				#endif
			}
			else if (!_modelsNamespaces.ContainsKey (a))
				GDASettings._modelsNamespaces.Add (a, new ModelsNamespaceInfo (b, a));
		}
		public static void AddModelsNamespace (Assembly a, string b)
		{
			if (!_modelsNamespaces.ContainsKey (a.FullName))
				GDASettings._modelsNamespaces.Add (a.FullName, new ModelsNamespaceInfo (b, a));
		}
		public static void AddGeneratorKey (string a, IGeneratorKey b)
		{
			if (string.IsNullOrEmpty (a))
				throw new ArgumentNullException ("name");
			else if (b == null)
				throw new ArgumentNullException ("instance");
			if (_generatorsKey.ContainsKey (a))
				throw new GDAException (string.Format ("Generator with name '{0}' has been added in the collection configuration", a));
			_generatorsKey.Add (a, b);
		}
		public static IEnumerable<KeyValuePair<string, IGeneratorKey>> GetGeneratorsKey ()
		{
			return _generatorsKey;
		}
		public static IGeneratorKey GetGeneratorKey (string a)
		{
			IGeneratorKey b = null;
			if (_generatorsKey.TryGetValue (a, out b))
				return b;
			return null;
		}
		[Configuration (NodePrefix + "/GenerateKeyHandler", ConfigKeyPresence.Optional)]
		public static void DefineGenerateKeyHandler (string a, string b)
		{
			Type c = Type.GetType (a);
			if (c == null)
				throw new GDAException ("GenerateKeyHandler\r\nType {0} not found", a);
			MethodInfo[] d = c.GetMethods (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			#if !PocketPC
			MethodInfo e = Array.Find (d, delegate (MethodInfo f) {
				return f.Name == b;
			});
			#else
						            MethodInfo method = null;
            foreach(var i in methods)
                if (i.Name == methodName)
                {
                    method = i;
                    break;
                }
#endif
			if (e == null)
				throw new GDAException ("GenerateKeyHandler\r\nMethod {0} not found in {1}.", b, c.FullName);
			#if !PocketPC
			GDAOperations.SetGlobalGenerateKeyHandler ((GenerateKeyHandler)Delegate.CreateDelegate (typeof(GenerateKeyHandler), e));
			#else
						            GDAOperations.SetGlobalGenerateKeyHandler(delegate(object sender, GenerateKeyArgs args)
            {
                method.Invoke(null, new object[] { sender, args });
            });
#endif
		}
		[Configuration (NodePrefix + "/ProviderConfigurationLoadHandler", ConfigKeyPresence.Optional)]
		public static void DefineProviderConfigurationLoadHandler (string a, string b)
		{
			Type c = Type.GetType (a);
			if (c == null)
				throw new GDAException ("ProviderConfigurationLoaderHandler\r\nType {0} not found", a);
			MethodInfo[] d = c.GetMethods (System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			#if !PocketPC
			MethodInfo e = Array.Find (d, delegate (MethodInfo f) {
				return f.Name == b;
			});
			#else
						            MethodInfo method = null;
            foreach(var i in methods)
                if (i.Name == methodName)
                {
                    method = i;
                    break;
                }
#endif
			if (e == null)
				throw new GDAException ("ProviderConfigurationLoaderHandler\r\nMethod {0} not found in {1}.", b, c.FullName);
			#if !PocketPC
			GDAOperations.SetGlobalProviderConfigurationLoadHandler ((ProviderConfigurationLoadHandler)Delegate.CreateDelegate (typeof(ProviderConfigurationLoadHandler), e));
			#else
						            GDAOperations.SetGlobalProviderConfigurationLoadHandler(delegate(object sender, ProviderConfigurationLoadArgs args)
            {
                method.Invoke(null, new object[] { sender, args });
            });
#endif
		}
		public static IProviderConfiguration DefaultProviderConfiguration {
			get {
				LoadConfiguration ();
				if (_defaultProviderConfiguration == null && _defaultProvider == null)
					throw new GDAException ("Default provider not found.");
				else if (_defaultProviderConfiguration == null)
					_defaultProviderConfiguration = GetProviderConfiguration (_defaultProvider);
				return _defaultProviderConfiguration;
			}
			set {
				if (value == null)
					throw new ArgumentNullException ();
				_defaultProviderConfiguration = value;
			}
		}
		public static IProviderConfiguration GetProviderConfiguration (string a)
		{
			LoadConfiguration ();
			if (!_providerConfiguration.ContainsKey (a)) {
				IProviderConfiguration b = null;
				if (!_providersConfigurationInfo.ContainsKey (a))
					throw new GDAException ("ProviderConfiguration {0} not found.", a);
				ProviderConfigurationInfo c = (ProviderConfigurationInfo)_providersConfigurationInfo [a];
				if (c.ProviderName == null)
					throw new GDAException ("Not found provider name in ProviderConfiguration {0}", a);
				if (_providersConfigurationConstructors.ContainsKey (c.ProviderName)) {
					if (c.ConnectionString == null || c.ConnectionString == "")
						throw new GDAException ("ConnectionString is null");
					ConstructorInfo d = _providersConfigurationConstructors [c.ProviderName] as ConstructorInfo;
					try {
						b = (IProviderConfiguration)d.Invoke (new object[] {
							c.ConnectionString
						});
						b.Dialect = c.Dialect;
						_providerConfiguration [a] = b;
					}
					catch (TargetInvocationException ex) {
						throw ex.InnerException;
					}
				}
				else
					throw new GDAException (String.Format ("Info about name {0} not found.", a));
				if (GDAOperations.GlobalProviderConfigurationLoad != null)
					GDAOperations.GlobalProviderConfigurationLoad (b, new ProviderConfigurationLoadArgs (b));
			}
			return _providerConfiguration [a] as IProviderConfiguration;
		}
		public static IProviderConfiguration CreateProviderConfiguration (string a, string b)
		{
			if (_providersConfigurationConstructors.ContainsKey (a)) {
				if (string.IsNullOrEmpty (b))
					throw new GDAException ("ConnectionString is null");
				ConstructorInfo c = _providersConfigurationConstructors [a] as ConstructorInfo;
				try {
					IProviderConfiguration d = (IProviderConfiguration)c.Invoke (new object[] {
						b
					});
					if (GDAOperations.GlobalProviderConfigurationLoad != null)
						GDAOperations.GlobalProviderConfigurationLoad (d, new ProviderConfigurationLoadArgs (d));
					return d;
				}
				catch (TargetInvocationException ex) {
					throw ex.InnerException;
				}
			}
			else
				throw new GDAException (String.Format ("Info about name {0} not found.", a));
		}
		[Configuration (NodePrefix + "/Providers/Provider")]
		public static void AddProvider (string a, string b, string c)
		{
			Type d = Type.GetType (b + "." + a + "ProviderConfiguration, " + c);
			if (d == null)
				throw new Exception ("Not found assembly \"" + b + "." + a + "ProviderConfiguration, " + c);
			try {
				ConstructorInfo e = d.GetConstructor (new Type[] {
					typeof(string)
				});
				_providersConfigurationConstructors [a] = e;
			}
			catch (Exception ex) {
				throw new Exception ("Error on load ProviderConfiguration.", ex);
			}
		}
		[Configuration (NodePrefix + "/Mappings/Mapping", KeyPresenceRequirement = ConfigKeyPresence.Optional)]
		public static void AddMapping (string a, string b, string c)
		{
			if (!string.IsNullOrEmpty (c))
				Mapping.MappingData.Import (c);
			else
				Mapping.MappingData.Import (a, b);
		}
		public static ICollection<ModelsNamespaceInfo> ModelsNamespaces {
			get {
				return GDASettings._modelsNamespaces.Values;
			}
		}
		public static void LoadConfiguration ()
		{
			if (!loadDataConfig) {
				lock (_loadingObjLock) {
					if (!loadDataConfig) {
						#if !PocketPC
						try {
							Configurator.Configure (typeof(GDASettings));
						}
						catch (Exception ex) {
							if (ex is TargetInvocationException)
								ex = ex.InnerException;
							throw new GDA.Common.Configuration.Exceptions.LoadConfigurationException (ex);
						}
						#endif
						loadDataConfig = true;
					}
				}
			}
		}
	}
}
