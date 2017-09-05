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
using GDA.Common.Configuration.Handlers;
using System.Collections;
using System.Xml;
using System.IO;
using System.Configuration;

namespace GDA.Common.Configuration
{
	public class Configurator
	{
		private static object cfgLock = new object();

		/// <summary>
		/// Lista do handlers de configuração.
		/// </summary>
		private static IList handlers = new ArrayList();

		/// <summary>
		/// Os handlers registrados nessa lista, são para obter um handler pelo nome de armazenamento
		/// </summary>
		private static Hashtable namedHandlers = new Hashtable();

		/// <summary>
		/// Identifica que os dados já foram inicializados.
		/// </summary>
		private static bool isInitialized;

		private static Hashtable targets = new Hashtable();

		/// <summary>
		/// Identifica se o log de evento está abilitado.
		/// </summary>
		private static bool isLoggingEnabled = true;

		/// <summary>
		/// Constante de quebra de linha
		/// </summary>
		private static readonly string CRLF = "\r\n";

		/// <summary>
		/// String que armazena o log de erros.
		/// </summary>
		private static StringBuilder errorLog = new StringBuilder();

		/// <summary>
		/// String que armazena as exceptions ocorridas
		/// </summary>
		private static StringBuilder exceptionLog = new StringBuilder();

		private static bool ignoreEmptyConfiguration = false;

		/// <summary>
		/// Identifica se é para ignorar configurações vazias.
		/// </summary>
		public static bool IgnoreEmptyConfiguration
		{
			get
			{
				return Configurator.ignoreEmptyConfiguration;
			}
			set
			{
				Configurator.ignoreEmptyConfiguration = value;
			}
		}

		/// <summary>
		/// Construtor privado
		/// </summary>
		private Configurator()
		{
		}

		/// <summary>
		/// Esse método cria um handler de configuração para um sessão especifica
		/// do arquivo de configuração padrão do .NET.
		/// </summary>
		/// <param name="configStoreName">O nome em que o handler está associado. Se for null
		/// o handler será usado para as conigurações do GDA, senão quando o método configure for
		/// chamado com o nome especificado</param>
		/// <param name="sectionName">Nome da sessão GDASectionHandler está declarada no arquivo de
		/// configuração padrão do .NET.</param>
		public static void AddSectionHandler(string configStoreName, string sectionName)
		{
			string errorMsg = String.Format("Unable to create GDASectionHandler for section " + "named \"{0}\" in file \"{1}\".{2}", sectionName, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, CRLF);
			try
			{
				GDASectionHandler handler = (GDASectionHandler)ConfigurationSettings.GetConfig(sectionName);
				if(handler != null && handler.IsValid)
					AddHandler(configStoreName, handler, false);
				else
					LogMessage(errorMsg);
			}
			catch(Exception e)
			{
				LogMessage(errorMsg);
				LogException(e);
			}
		}

		/// <summary>
		/// Esse método cria um handler de configuração para um sessão especifica
		/// do arquivo de configuração padrão do .NET.
		/// </summary>
		/// <param name="sectionName">Nome da sessão GDASectionHandler está declarada no arquivo de
		/// configuração padrão do .NET.</param>
		public static void AddSectionHandler(string sectionName)
		{
			AddSectionHandler(null, sectionName);
		}

		/// <summary>
		/// Esse método cria um handler de configuração com base em arquivo com o nome especificado.
		/// O nome do arquivo não deve conter o caminho completo, apenas o nome (o GDA usa diretorios pré-
		/// definido para localizar o arquivo.
		/// </summary>
		/// <param name="configStoreName">O nome em que o handler está associado. Se for null
		/// o handler será usado para as conigurações do GDA, senão quando o método configure for
		/// chamado com o nome especificado</param>
		/// <param name="fileName">Nome do arquivo de configuração.</param>
		public static void AddFileHandler(string configStoreName, string fileName)
		{
			try
			{
				if(!string.IsNullOrEmpty(FileConfigHandler.GetLocalPath(fileName)))
				{
					FileConfigHandler handler = new FileConfigHandler(fileName);
					AddHandler(configStoreName, handler, false);
				}
			}
			catch(Exception e)
			{
				string file = fileName == null ? FileConfigHandler.CONFIG_FILENAME : fileName;
				LogMessage("Unable to create FileHandler for file " + file + ".");
				LogMessage("This usually means that the file could not be found in any of " + "the default search locations." + CRLF);
				LogException(e);
			}
		}

		/// <summary>
		/// This method creates a configuration handler for the specified file name. The
		/// file name must not include path information (GDA will search for it in specific
		/// predefined locations).
		/// </summary>
		/// <param name="fileName">The name of the configuration file.</param>
		public static void AddFileHandler(string fileName)
		{
			AddFileHandler(null, fileName);
		}

		/// <summary>
		/// This method creates a configuration handler for the given XML fragment,
		/// which (if valid) will be used as configuration source.
		/// </summary>
		/// <param name="configStoreName">The name with which to associate this handler. If null is given
		/// the handler will be used to configure GDA settings, otherwise it will only be used when
		/// Configure is called with a matching name.</param>
		/// <param name="root">The root node of the XML fragment to use as 
		/// configuration source.</param>
		public static void AddExternalHandler(string configStoreName, XmlNode root)
		{
			try
			{
				AddHandler(configStoreName, new BaseSectionHandler(root), true);
			}
			catch(Exception e)
			{
				LogMessage("Unable to use supplied XML fragment as configuration source:");
				LogMessage(root != null ? root.OuterXml : "null");
				LogException(e);
			}
		}

		/// <summary>
		/// This method creates a configuration handler for the given XML fragment,
		/// which (if valid) will be used as configuration source.
		/// </summary>
		/// <param name="root">The root node of the XML fragment to use as 
		/// configuration source.</param>
		public static void AddExternalHandler(XmlNode root)
		{
			AddExternalHandler(null, root);
		}

		/// <summary>
		/// This method creates a configuration handler for the specified stream.
		/// </summary>
		/// <param name="configStoreName">The name with which to associate this handler. If null is given
		/// the handler will be used to configure GDA settings, otherwise it will only be used when
		/// Configure is called with a matching name.</param>
		/// <param name="stream">The stream from which to read the configuration XML document/snippet.</param>
		public static void AddStreamHandler(string configStoreName, Stream stream)
		{
			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(stream);
				AddHandler(configStoreName, new BaseSectionHandler(doc), false);
			}
			catch(Exception e)
			{
				LogMessage("Unable to use supplied stream as a configuration source." + CRLF);
				LogException(e);
			}
		}

		/// <summary>
		/// This method creates a configuration handler for the specified stream.
		/// </summary>
		/// <param name="stream">The stream from which to read the configuration XML document/snippet.</param>
		public static void AddStreamHandler(Stream stream)
		{
			AddStreamHandler(null, stream);
		}

		/// <summary>
		/// Configure all targets in the specified instance.
		/// </summary>
		/// <param name="configStoreName">The name of the handler to use as configuration source.
		/// If null is given, GDA's configuration store will be used (effectively this means
		/// one of the automatically registered handlers).</param>
		/// <param name="instance">The object to be configured</param>
		public static void Configure(string configStoreName, object instance)
		{
			InitializeHandlers();
			lock (cfgLock)
			{
				ConfigurationMap cm = GetConfigurationMap(instance.GetType());
				if(configStoreName != null)
				{
					IList list = new ArrayList(1);
					list.Add(namedHandlers[configStoreName]);
					cm.Configure(list, instance);
				}
				else
					cm.Configure(handlers, instance);
			}
		}

		/// <summary>
		/// Configure all targets in the specified instance.
		/// </summary>
		/// <param name="instance">The object to be configured</param>
		public static void Configure(object instance)
		{
			Configure(null, instance);
		}

		/// <summary>
		/// Configure all targets in the specified type (static members).
		/// </summary>
		/// <param name="configStoreName">The name of the handler to use as configuration source.
		/// If null is given, GDA's configuration store will be used (effectively this means
		/// one of the automatically registered handlers).</param>
		/// <param name="type">The type to be configured</param>
		public static void Configure(string configStoreName, Type type)
		{
			InitializeHandlers();
			lock (cfgLock)
			{
				ConfigurationMap cm = GetConfigurationMap(type);
				if(configStoreName != null)
				{
					IList list = new ArrayList(1);
					list.Add(namedHandlers[configStoreName]);
					cm.Configure(list, type);
				}
				else
					cm.Configure(handlers, type);
			}
		}

		/// <summary>
		/// Configure all targets in the specified type (static members).
		/// </summary>
		/// <param name="type">The type to be configured</param>
		public static void Configure(Type type)
		{
			Configure(null, type);
		}

		/// <summary>
		/// This method creates the default configuration handlers provided 
		/// by GDA itself.
		/// </summary>
		private static void InitializeHandlers()
		{
			if(!isInitialized)
			{
				isInitialized = true;
				AddSectionHandler("gda");
				AddFileHandler(null);
			}
			lock (cfgLock)
			{
				if(!ignoreEmptyConfiguration && handlers.Count == 0)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("FATAL ERROR: No configuration store was found!" + CRLF);
					sb.Append("GDA is unable to continue!" + CRLF + CRLF);
					sb.Append("The handlers emitted the following error messages:" + CRLF);
					sb.Append(errorLog.ToString());
					sb.Append("The handlers threw the following exceptions:" + CRLF);
					sb.Append(exceptionLog.ToString());
					throw new GDAException(sb.ToString());
				}
				else
				{
					errorLog = null;
					exceptionLog = null;
				}
			}
		}

		private static void AddHandler(string configStoreName, BaseSectionHandler handler, bool isFirst)
		{
			lock (cfgLock)
			{
				if(handler != null && handler.IsValid)
				{
					handlers.Insert(isFirst ? 0 : handlers.Count, handler);
					if(configStoreName != null)
						namedHandlers[configStoreName] = handler;
				}
				else
				{
					LogMessage("Attempt to add configuration handler failed.");
					if(handler != null)
						LogMessage("Handler: {0}  IsValid: {1}", handler.GetType(), handler.IsValid);
				}
			}
		}

		private static void LogMessage(string msg, params object[] args)
		{
			if(errorLog == null)
				errorLog = new StringBuilder();
			if(args != null && args.Length > 0)
				errorLog.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, msg, args);
			else
				errorLog.Append(msg);
			errorLog.Append(CRLF);
		}

		private static void LogException(Exception e)
		{
			if(e != null)
			{
				exceptionLog.Append(e.ToString() + CRLF + CRLF);
			}
		}

		private static ConfigurationMap GetConfigurationMap(Type type)
		{
			ConfigurationMap cm;
			if(!targets.ContainsKey(type))
			{
				cm = new ConfigurationMap(type);
				targets[type] = cm;
			}
			else
				cm = (ConfigurationMap)targets[type];
			return cm;
		}

		/// <summary>
		/// Accessor to turn logging on or off. This property is updated when the configuration 
		/// is being accessed.
		/// </summary>
		public static bool IsLoggingEnabled
		{
			get
			{
				return isLoggingEnabled;
			}
			set
			{
				isLoggingEnabled = value;
			}
		}

		/// <summary>
		/// Returns the number of valid registered configuration handlers. Only the first handler
		/// is used used to obtain configuration options, but if HandlerCount is 0 then GDA
		/// has not been able to find any valid configuration source.
		/// </summary>
		public int HandlerCount
		{
			get
			{
				lock (cfgLock)
				{
					InitializeHandlers();
					return handlers.Count;
				}
			}
		}
	}
}
