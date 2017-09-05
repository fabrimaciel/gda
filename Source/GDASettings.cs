/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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

		/// <summary>
		/// Identifica que os dados de configuração já foram carregados.
		/// </summary>
		private static bool loadDataConfig = false;

		internal const string NodePrefix = "GDA";

		/// <summary>
		/// Lista que armazena os provider já instânciados.
		/// </summary>
		private static Hashtable _providerConfiguration = new Hashtable();

		/// <summary>
		/// Lista que armazena os construtores dos providers.
		/// </summary>
		private static Hashtable _providersConfigurationConstructors = new Hashtable();

		/// <summary>
		/// Lista que armazena as informações dos provider a serem construídos.
		/// </summary>
		private static Hashtable _providersConfigurationInfo = new Hashtable();

		/// <summary>
		/// Lista que armazena os namespaces aonde as models estão alocacadas.
		/// </summary>
		private static Dictionary<string, ModelsNamespaceInfo> _modelsNamespaces = new Dictionary<string, ModelsNamespaceInfo>();

		/// <summary>
		/// Construtor padrão do provider.
		/// </summary>
		private static ConstructorInfo _constructorProvider;

		/// <summary>
		/// Nome do provider padrão.
		/// </summary>
		private static string _defaultProvider;

		private static IProviderConfiguration _defaultProviderConfiguration = null;

		/// <summary>
		/// Identifica se está habilitado o debug.
		/// </summary>
		private static bool _enabledDebugTrace;

		/// <summary>
		/// Tamanho máximo do cache para armazenar os mapeamentos.
		/// </summary>
		private static int _maximumMapperCache = 10;

		/// <summary>
		/// Instancia do método usado na descriptografia dos dados.
		/// </summary>
		private static System.Converter<string, string> _decryptMethod;

		/// <summary>
		/// Instancia do método usado na criptografia dos dados.
		/// </summary>
		private static System.Converter<string, string> _encryptMethod;

		/// <summary>
		/// Instancias dos geradores de chave carregados no sistema.
		/// </summary>
		private static Dictionary<string, IGeneratorKey> _generatorsKey = new Dictionary<string, IGeneratorKey>();

		/// <summary>
		/// Nome do provider padrão.
		/// </summary>
		public static string DefaultProviderName
		{
			get
			{
				return GDASettings._defaultProvider;
			}
		}

		/// <summary>
		/// Tamanho máximo do cache para armazenar os mapeamentos. Default 10.
		/// </summary>
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

		/// <summary>
		/// Instancia do método usado na descriptografia dos dados.
		/// </summary>
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

		/// <summary>
		/// Instancia do método usado na criptografia dos dados.
		/// </summary>
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

		/// <summary>
		/// Connection string a ser usada no provider.
		/// </summary>
		[Configuration(NodePrefix + "/DefaultProvider")]
		internal static void SetDefaultProvider(string name)
		{
			_defaultProvider = name;
		}

		/// <summary>
		/// Define o tamanho máximo do cache para o mapeamentos das classes.
		/// </summary>
		/// <param name="size"></param>
		[Configuration(NodePrefix + "/Cache/MaximumMapper", ConfigKeyPresence.Optional)]
		internal static void SetMaximumMapperCache(int size)
		{
			_maximumMapperCache = size;
		}

		/// <summary>
		/// Identifica se será usado o trace para o debug.
		/// </summary>
		/// <param name="trace"></param>
		[Configuration(NodePrefix + "/Debug", ConfigKeyPresence.Optional)]
		internal static void Debug(bool trace)
		{
			EnabledDebugTrace = trace;
		}

		/// <summary>
		/// Adiciona um provedor de configuração.
		/// </summary>
		/// <param name="name">Nome do provedor de configuração.</param>
		/// <param name="providerConfiguration">Instancia do provedor.</param>
		public static void AddProviderConfiguration(string name, IProviderConfiguration providerConfiguration)
		{
			if(_providersConfigurationInfo.ContainsKey(name))
				_providersConfigurationInfo.Remove(name);
			_providersConfigurationInfo.Add(name, providerConfiguration);
		}

		#if !PocketPC
		/// <summary>
		/// Registra a classe que ficara responsável pela criptografia dos dados.
		/// </summary>
		/// <param name="classType"></param>
		/// <param name="name"></param>
		[Configuration(NodePrefix + "/CryptoClass", KeyPresenceRequirement = ConfigKeyPresence.Optional)]
		internal static void AddCryptoClass(string classType, string encryptMethod, string decryptMethod)
		{
			Type cType = Type.GetType(classType);
			if(cType == null)
				throw new GDAException("CryptoClass\r\nType {0} not found", classType);
			MethodInfo[] methods = cType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			if(!string.IsNullOrEmpty(encryptMethod))
			{
				MethodInfo encrypt = Array.Find(methods, delegate(MethodInfo mi) {
					return mi.Name == encryptMethod;
				});
				if(encrypt == null)
					throw new GDAException("CryptoClass\r\nMethod {0} not found in {1}.", encryptMethod, cType.FullName);
				GDASettings._encryptMethod = (Converter<string, string>)Delegate.CreateDelegate(typeof(Converter<string, string>), encrypt);
			}
			if(!string.IsNullOrEmpty(decryptMethod))
			{
				MethodInfo decrypt = Array.Find(methods, delegate(MethodInfo mi) {
					return mi.Name == decryptMethod;
				});
				if(decrypt == null)
					throw new GDAException("CryptoClass\r\nMethod {0} not found in {1}.", decryptMethod, cType.FullName);
				GDASettings._decryptMethod = (Converter<string, string>)Delegate.CreateDelegate(typeof(Converter<string, string>), decrypt);
			}
		}

		#endif
		/// <summary>
		/// Adiciona novas informação das configurações dos provider disponíveis.
		/// </summary>
		/// <param name="name">Nome do providerConfigurationInfo.</param>
		/// <param name="providerName">Nome do provider relacionado.</param>
		/// <param name="connectionString">Connection string usada pelo provider.</param>
		[Configuration(NodePrefix + "/ProvidersConfiguration/Info")]
		internal static void AddProviderConfigurationInfo(string name, string providerName, string connectionString, string dialect)
		{
			if(_providersConfigurationInfo.ContainsKey(name))
				_providersConfigurationInfo.Remove(name);
			if(_decryptMethod != null)
				try
				{
					connectionString = _decryptMethod(connectionString);
				}
				catch(Exception ex)
				{
					throw new GDAException("Fail on decrypt connection string.", ex);
				}
			_providersConfigurationInfo.Add(name, new ProviderConfigurationInfo(name, providerName, connectionString, dialect));
		}

		/// <summary>
		/// Adiciona um novo namespace que as models mapeadas estão localizadas.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="name">Nome do namespace.</param>
		[Configuration(NodePrefix + "/ModelsNamespace/Namespace", ConfigKeyPresence.Optional)]
		public static void AddModelsNamespace(string assembly, string name)
		{
			if(assembly == "*")
			{
				#if PocketPC
				                AddModelsNamespace(System.Reflection.Assembly.GetExecutingAssembly().FullName, name);
#else
				AddModelsNamespace(System.Reflection.Assembly.GetEntryAssembly().FullName, name);
				#endif
			}
			else if(!_modelsNamespaces.ContainsKey(assembly))
				GDASettings._modelsNamespaces.Add(assembly, new ModelsNamespaceInfo(name, assembly));
		}

		/// <summary>
		/// Adiciona um novo namespace que as models mapeadas estão localizadas.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="name">Nome do namespace.</param>
		public static void AddModelsNamespace(Assembly assembly, string name)
		{
			if(!_modelsNamespaces.ContainsKey(assembly.FullName))
				GDASettings._modelsNamespaces.Add(assembly.FullName, new ModelsNamespaceInfo(name, assembly));
		}

		/// <summary>
		/// Adiciona a instancia de um gerador de chave que será usada no sistema.
		/// </summary>
		/// <param name="name">Nome do gerador.</param>
		/// <param name="instance">Instancia do gerador.</param>
		public static void AddGeneratorKey(string name, IGeneratorKey instance)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			else if(instance == null)
				throw new ArgumentNullException("instance");
			if(_generatorsKey.ContainsKey(name))
				throw new GDAException(string.Format("Generator with name '{0}' has been added in the collection configuration", name));
			_generatorsKey.Add(name, instance);
		}

		/// <summary>
		/// Recupera os geradores de chave do sistema.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<KeyValuePair<string, IGeneratorKey>> GetGeneratorsKey()
		{
			return _generatorsKey;
		}

		/// <summary>
		/// Recupera o gerador de chave com base no nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns>Instancia do gerador ou null caso não seja encontrado.</returns>
		public static IGeneratorKey GetGeneratorKey(string name)
		{
			IGeneratorKey item = null;
			if(_generatorsKey.TryGetValue(name, out item))
				return item;
			return null;
		}

		/// <summary>
		/// Define o método que irá manipular a geração de chaves.
		/// </summary>
		/// <param name="classType"></param>
		/// <param name="methodName"></param>
		[Configuration(NodePrefix + "/GenerateKeyHandler", ConfigKeyPresence.Optional)]
		public static void DefineGenerateKeyHandler(string classType, string methodName)
		{
			Type cType = Type.GetType(classType);
			if(cType == null)
				throw new GDAException("GenerateKeyHandler\r\nType {0} not found", classType);
			MethodInfo[] methods = cType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			#if !PocketPC
			MethodInfo method = Array.Find(methods, delegate(MethodInfo mi) {
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
			if(method == null)
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

		/// <summary>
		/// Define o método que irá manipular a inicialização dos provedores de configuração.
		/// </summary>
		/// <param name="classType"></param>
		/// <param name="methodName"></param>
		[Configuration(NodePrefix + "/ProviderConfigurationLoadHandler", ConfigKeyPresence.Optional)]
		public static void DefineProviderConfigurationLoadHandler(string classType, string methodName)
		{
			Type cType = Type.GetType(classType);
			if(cType == null)
				throw new GDAException("ProviderConfigurationLoaderHandler\r\nType {0} not found", classType);
			MethodInfo[] methods = cType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			#if !PocketPC
			MethodInfo method = Array.Find(methods, delegate(MethodInfo mi) {
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
			if(method == null)
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

		/// <summary>
		/// Provider para acesso aos dados.
		/// </summary>
		public static IProviderConfiguration DefaultProviderConfiguration
		{
			get
			{
				LoadConfiguration();
				if(_defaultProviderConfiguration == null && _defaultProvider == null)
					throw new GDAException("Default provider not found.");
				else if(_defaultProviderConfiguration == null)
					_defaultProviderConfiguration = GetProviderConfiguration(_defaultProvider);
				return _defaultProviderConfiguration;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException();
				_defaultProviderConfiguration = value;
			}
		}

		/// <summary>
		/// Obtem o <see cref="IProviderConfiguration"/> com base no nome.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static IProviderConfiguration GetProviderConfiguration(string name)
		{
			LoadConfiguration();
			if(!_providerConfiguration.ContainsKey(name))
			{
				IProviderConfiguration instance = null;
				if(!_providersConfigurationInfo.ContainsKey(name))
					throw new GDAException("ProviderConfiguration {0} not found.", name);
				ProviderConfigurationInfo pci = (ProviderConfigurationInfo)_providersConfigurationInfo[name];
				if(pci.ProviderName == null)
					throw new GDAException("Not found provider name in ProviderConfiguration {0}", name);
				if(_providersConfigurationConstructors.ContainsKey(pci.ProviderName))
				{
					if(pci.ConnectionString == null || pci.ConnectionString == "")
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
					catch(TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				}
				else
					throw new GDAException(String.Format("Info about name {0} not found.", name));
				if(GDAOperations.GlobalProviderConfigurationLoad != null)
					GDAOperations.GlobalProviderConfigurationLoad(instance, new ProviderConfigurationLoadArgs(instance));
			}
			return _providerConfiguration[name] as IProviderConfiguration;
		}

		/// <summary>
		/// Cria um provider.
		/// </summary>
		/// <param name="name">Nome do provider.</param>
		/// <param name="connectionString"></param>
		/// <returns>Instancia do provider.</returns>
		public static IProviderConfiguration CreateProviderConfiguration(string name, string connectionString)
		{
			if(_providersConfigurationConstructors.ContainsKey(name))
			{
				if(string.IsNullOrEmpty(connectionString))
					throw new GDAException("ConnectionString is null");
				ConstructorInfo ci = _providersConfigurationConstructors[name] as ConstructorInfo;
				try
				{
					IProviderConfiguration instance = (IProviderConfiguration)ci.Invoke(new object[] {
						connectionString
					});
					if(GDAOperations.GlobalProviderConfigurationLoad != null)
						GDAOperations.GlobalProviderConfigurationLoad(instance, new ProviderConfigurationLoadArgs(instance));
					return instance;
				}
				catch(TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}
			else
				throw new GDAException(String.Format("Info about name {0} not found.", name));
		}

		/// <summary>
		/// Adiciona um novo provider de configuração.
		/// </summary>
		/// <param name="name">Nome do provider.</param>
		/// <param name="classNamespace">Classe que representa o provider.</param>
		/// <param name="assembly"></param>
		[Configuration(NodePrefix + "/Providers/Provider")]
		public static void AddProvider(string name, string classNamespace, string assembly)
		{
			Type type = Type.GetType(classNamespace + "." + name + "ProviderConfiguration, " + assembly);
			if(type == null)
				throw new Exception("Not found assembly \"" + classNamespace + "." + name + "ProviderConfiguration, " + assembly);
			try
			{
				ConstructorInfo ci = type.GetConstructor(new Type[] {
					typeof(string)
				});
				_providersConfigurationConstructors[name] = ci;
			}
			catch(Exception ex)
			{
				throw new Exception("Error on load ProviderConfiguration.", ex);
			}
		}

		/// <summary>
		/// Adiciona um mapeamento.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly onde o mapeamento está inserido.</param>
		/// <param name="resourceName">Nome do resource do mapeamento.</param>
		/// <param name="fileName">Arquivo aonde o mapemento está inserido.</param>
		[Configuration(NodePrefix + "/Mappings/Mapping", KeyPresenceRequirement = ConfigKeyPresence.Optional)]
		public static void AddMapping(string assemblyName, string resourceName, string fileName)
		{
			if(!string.IsNullOrEmpty(fileName))
				Mapping.MappingData.Import(fileName);
			else
				Mapping.MappingData.Import(assemblyName, resourceName);
		}

		/// <summary>
		/// Lista que armazena os namespaces aonde as models estão alocacadas.
		/// </summary>
		public static ICollection<ModelsNamespaceInfo> ModelsNamespaces
		{
			get
			{
				return GDASettings._modelsNamespaces.Values;
			}
		}

		/// <summary>
		/// Carrega o arquivo de configuracao.
		/// </summary>
		public static void LoadConfiguration()
		{
			if(!loadDataConfig)
			{
				lock (_loadingObjLock)
				{
					if(!loadDataConfig)
					{
						#if !PocketPC
						try
						{
							Configurator.Configure(typeof(GDASettings));
						}
						catch(Exception ex)
						{
							if(ex is TargetInvocationException)
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
