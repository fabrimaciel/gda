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
        private static object _loadingObjLock = new object();

        private static bool loadDataConfig = false;

        internal const string NodePrefix = "GDA";

        private static Hashtable _providerConfiguration = new Hashtable();

        private static Hashtable _providersConfigurationConstructors = new Hashtable();

        private static Hashtable _providersConfigurationInfo = new Hashtable();

        private static Dictionary<string, ModelsNamespaceInfo> _modelsNamespaces = new Dictionary<string, ModelsNamespaceInfo>();

        private static ConstructorInfo _constructorProvider;

        private static string _defaultProvider;

        private static IProviderConfiguration _defaultProviderConfiguration = null;

        private static bool _enabledDebugTrace;

        private static int _maximumMapperCache = 10;

        private static System.Converter<string, string> _decryptMethod;

        private static System.Converter<string, string> _encryptMethod;

        private static Dictionary<string, IGeneratorKey> _generatorsKey = new Dictionary<string, IGeneratorKey>();

        public static string DefaultProviderName
        {
            get
            {
                return GDASettings._defaultProvider;
            }
        }

        public static int MaximumMapperCache
        {
            get
            {
                return GDASettings._maximumMapperCache;
            }
        }

        public static bool EnabledDebugTrace
        {
            get
            {
                return GDASettings._enabledDebugTrace;
            }
            set
            {
                GDASettings._enabledDebugTrace = value;
            }
        }

        public static System.Converter<string, string> DecryptMethod
        {
            get
            {
                return _decryptMethod;
            }
            set
            {
                _decryptMethod = value;
            }
        }

        public static System.Converter<string, string> EncryptMethod
        {
            get
            {
                return _encryptMethod;
            }
            set
            {
                _encryptMethod = value;
            }
        }

        [Configuration(NodePrefix + "/DefaultProvider")]
        internal static void SetDefaultProvider(string name)
        {
            _defaultProvider = name;
        }

        [Configuration(NodePrefix + "/Cache/MaximumMapper", ConfigKeyPresence.Optional)]
        internal static void SetMaximumMapperCache(int size)
        {
            _maximumMapperCache = size;
        }

        [Configuration(NodePrefix + "/Debug", ConfigKeyPresence.Optional)]
        internal static void Debug(bool trace)
        {
            EnabledDebugTrace = trace;
        }

        public static void AddProviderConfiguration(string name, IProviderConfiguration providerConfiguration)
        {
            if (_providersConfigurationInfo.ContainsKey(name))
                _providersConfigurationInfo.Remove(name);
            _providersConfigurationInfo.Add(name, providerConfiguration);
        }

#if !PocketPC
        [Configuration(NodePrefix + "/CryptoClass", KeyPresenceRequirement = ConfigKeyPresence.Optional)]
        internal static void AddCryptoClass(string classType, string encryptMethod, string decryptMethod)
        {
            Type cType = Type.GetType(classType);
            if (cType == null)
                throw new GDAException("CryptoClass\r\nType {0} not found", classType);
            MethodInfo[] methods = cType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (!string.IsNullOrEmpty(encryptMethod))
            {
                MethodInfo encrypt = Array.Find(methods, delegate (MethodInfo mi) {
                    return mi.Name == encryptMethod;
                });
                if (encrypt == null)
                    throw new GDAException("CryptoClass\r\nMethod {0} not found in {1}.", encryptMethod, cType.FullName);
                GDASettings._encryptMethod = (Converter<string, string>)Delegate.CreateDelegate(typeof(Converter<string, string>), encrypt);
            }
            if (!string.IsNullOrEmpty(decryptMethod))
            {
                MethodInfo decrypt = Array.Find(methods, delegate (MethodInfo mi) {
                    return mi.Name == decryptMethod;
                });
                if (decrypt == null)
                    throw new GDAException("CryptoClass\r\nMethod {0} not found in {1}.", decryptMethod, cType.FullName);
                GDASettings._decryptMethod = (Converter<string, string>)Delegate.CreateDelegate(typeof(Converter<string, string>), decrypt);
            }
        }

#endif
        [Configuration(NodePrefix + "/ProvidersConfiguration/Info")]
        internal static void AddProviderConfigurationInfo(string name, string providerName, string connectionString, string dialect)
        {
            if (_providersConfigurationInfo.ContainsKey(name))
                _providersConfigurationInfo.Remove(name);
            if (_decryptMethod != null)
                try
                {
                    connectionString = _decryptMethod(connectionString);
                }
                catch (Exception ex)
                {
                    throw new GDAException("Fail on decrypt connection string.", ex);
                }
            _providersConfigurationInfo.Add(name, new ProviderConfigurationInfo(name, providerName, connectionString, dialect));
        }

        [Configuration(NodePrefix + "/ModelsNamespace/Namespace", ConfigKeyPresence.Optional)]
        public static void AddModelsNamespace(string assembly, string name)
        {
            if (assembly == "*")
            {
#if PocketPC
				                AddModelsNamespace(System.Reflection.Assembly.GetExecutingAssembly().FullName, name);
#else
                AddModelsNamespace(System.Reflection.Assembly.GetEntryAssembly().FullName, name);
#endif
            }
            else if (!_modelsNamespaces.ContainsKey(assembly))
                GDASettings._modelsNamespaces.Add(assembly, new ModelsNamespaceInfo(name, assembly));
        }

        public static void AddModelsNamespace(Assembly assembly, string name)
        {
            if (!_modelsNamespaces.ContainsKey(assembly.FullName))
                GDASettings._modelsNamespaces.Add(assembly.FullName, new ModelsNamespaceInfo(name, assembly));
        }

        public static void AddGeneratorKey(string name, IGeneratorKey instance)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            else if (instance == null)
                throw new ArgumentNullException("instance");
            if (_generatorsKey.ContainsKey(name))
                throw new GDAException(string.Format("Generator with name '{0}' has been added in the collection configuration", name));
            _generatorsKey.Add(name, instance);
        }

        public static IEnumerable<KeyValuePair<string, IGeneratorKey>> GetGeneratorsKey()
        {
            return _generatorsKey;
        }

        public static IGeneratorKey GetGeneratorKey(string name)
        {
            IGeneratorKey item = null;
            if (_generatorsKey.TryGetValue(name, out item))
                return item;
            return null;
        }

        [Configuration(NodePrefix + "/GenerateKeyHandler", ConfigKeyPresence.Optional)]
        public static void DefineGenerateKeyHandler(string classType, string methodName)
        {
            Type cType = Type.GetType(classType);
            if (cType == null)
                throw new GDAException("GenerateKeyHandler\r\nType {0} not found", classType);
            MethodInfo[] methods = cType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
#if !PocketPC
            MethodInfo method = Array.Find(methods, delegate (MethodInfo mi) {
                return mi.Name == methodName;
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
            if (method == null)
                throw new GDAException("GenerateKeyHandler\r\nMethod {0} not found in {1}.", methodName, cType.FullName);
#if !PocketPC
            GDAOperations.SetGlobalGenerateKeyHandler((GenerateKeyHandler)Delegate.CreateDelegate(typeof(GenerateKeyHandler), method));
#else
			            GDAOperations.SetGlobalGenerateKeyHandler(delegate(object sender, GenerateKeyArgs args)
            {
                method.Invoke(null, new object[] { sender, args });
            });
#endif
        }

        [Configuration(NodePrefix + "/ProviderConfigurationLoadHandler", ConfigKeyPresence.Optional)]
        public static void DefineProviderConfigurationLoadHandler(string classType, string methodName)
        {
            Type cType = Type.GetType(classType);
            if (cType == null)
                throw new GDAException("ProviderConfigurationLoaderHandler\r\nType {0} not found", classType);
            MethodInfo[] methods = cType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
#if !PocketPC
            MethodInfo method = Array.Find(methods, delegate (MethodInfo mi) {
                return mi.Name == methodName;
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
            if (method == null)
                throw new GDAException("ProviderConfigurationLoaderHandler\r\nMethod {0} not found in {1}.", methodName, cType.FullName);
#if !PocketPC
            GDAOperations.SetGlobalProviderConfigurationLoadHandler((ProviderConfigurationLoadHandler)Delegate.CreateDelegate(typeof(ProviderConfigurationLoadHandler), method));
#else
			            GDAOperations.SetGlobalProviderConfigurationLoadHandler(delegate(object sender, ProviderConfigurationLoadArgs args)
            {
                method.Invoke(null, new object[] { sender, args });
            });
#endif
        }

        public static IProviderConfiguration DefaultProviderConfiguration
        {
            get
            {
                LoadConfiguration();
                if (_defaultProviderConfiguration == null && _defaultProvider == null)
                    throw new GDAException("Default provider not found.");
                else if (_defaultProviderConfiguration == null)
                    _defaultProviderConfiguration = GetProviderConfiguration(_defaultProvider);
                return _defaultProviderConfiguration;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                _defaultProviderConfiguration = value;
            }
        }

        public static IProviderConfiguration GetProviderConfiguration(string name)
        {
            LoadConfiguration();
            if (!_providerConfiguration.ContainsKey(name))
            {
                IProviderConfiguration instance = null;
                if (!_providersConfigurationInfo.ContainsKey(name))
                    throw new GDAException("ProviderConfiguration {0} not found.", name);
                ProviderConfigurationInfo pci = (ProviderConfigurationInfo)_providersConfigurationInfo[name];
                if (pci.ProviderName == null)
                    throw new GDAException("Not found provider name in ProviderConfiguration {0}", name);
                if (_providersConfigurationConstructors.ContainsKey(pci.ProviderName))
                {
                    if (pci.ConnectionString == null || pci.ConnectionString == "")
                        throw new GDAException("ConnectionString is null");
                    ConstructorInfo ci = _providersConfigurationConstructors[pci.ProviderName] as ConstructorInfo;
                    try
                    {
                        instance = (IProviderConfiguration)ci.Invoke(new object[] {
                            pci.ConnectionString
                        });
                        instance.Dialect = pci.Dialect;
                        _providerConfiguration[name] = instance;
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }
                else
                    throw new GDAException(String.Format("Info about name {0} not found.", name));
                if (GDAOperations.GlobalProviderConfigurationLoad != null)
                    GDAOperations.GlobalProviderConfigurationLoad(instance, new ProviderConfigurationLoadArgs(instance));
            }
            return _providerConfiguration[name] as IProviderConfiguration;
        }

        public static IProviderConfiguration CreateProviderConfiguration(string name, string connectionString)
        {
            if (_providersConfigurationConstructors.ContainsKey(name))
            {
                if (string.IsNullOrEmpty(connectionString))
                    throw new GDAException("ConnectionString is null");
                ConstructorInfo ci = _providersConfigurationConstructors[name] as ConstructorInfo;
                try
                {
                    IProviderConfiguration instance = (IProviderConfiguration)ci.Invoke(new object[] {
                        connectionString
                    });
                    if (GDAOperations.GlobalProviderConfigurationLoad != null)
                        GDAOperations.GlobalProviderConfigurationLoad(instance, new ProviderConfigurationLoadArgs(instance));
                    return instance;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
            else
                throw new GDAException(String.Format("Info about name {0} not found.", name));
        }

        [Configuration(NodePrefix + "/Providers/Provider")]
        public static void AddProvider(string name, string classNamespace, string assembly)
        {
            Type type = Type.GetType(classNamespace + "." + name + "ProviderConfiguration, " + assembly);
            if (type == null)
                throw new Exception("Not found assembly \"" + classNamespace + "." + name + "ProviderConfiguration, " + assembly);
            try
            {
                ConstructorInfo ci = type.GetConstructor(new Type[] {
                    typeof(string)
                });
                _providersConfigurationConstructors[name] = ci;
            }
            catch (Exception ex)
            {
                throw new Exception("Error on load ProviderConfiguration.", ex);
            }
        }

        [Configuration(NodePrefix + "/Mappings/Mapping", KeyPresenceRequirement = ConfigKeyPresence.Optional)]
        public static void AddMapping(string assemblyName, string resourceName, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
                Mapping.MappingData.Import(fileName);
            else
                Mapping.MappingData.Import(assemblyName, resourceName);
        }

        public static ICollection<ModelsNamespaceInfo> ModelsNamespaces
        {
            get
            {
                return GDASettings._modelsNamespaces.Values;
            }
        }

        public static void LoadConfiguration()
        {
            if (!loadDataConfig)
            {
                lock (_loadingObjLock)
                {
                    if (!loadDataConfig)
                    {
#if !PocketPC
                        try
                        {
                            Configurator.Configure(typeof(GDASettings));
                        }
                        catch (Exception ex)
                        {
                            if (ex is TargetInvocationException)
                                ex = ex.InnerException;
                            throw new GDA.Common.Configuration.Exceptions.LoadConfigurationException(ex);
                        }
#endif
                        loadDataConfig = true;
                    }
                }
            }
        }
    }
}
