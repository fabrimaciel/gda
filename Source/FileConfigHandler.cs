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
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace GDA.Common.Configuration.Handlers
{
	public class FileConfigHandler : BaseSectionHandler
	{
		/// <summary>
		/// The key used to lookup a custom config file location in the standard .NET configuration file.
		/// If this key is present the specified filename will be used before searching elsewhere.
		/// </summary>
		private static readonly string[] APPSETTINGS_FILE =  {
			"GDAConfigFile",
			"ConfigFile"
		};

		/// <summary>
		/// Chaves usadas para fazer um link para o arquivo de configuração customizado contido na mesma
		/// localização aonde os arquivos de configuração padrão do .NET estão localizados.
		/// </summary>
		private static readonly string[] APPSETTINGS_FOLDER =  {
			"GDAConfigFolder",
			"ConfigFolder"
		};

		/// <summary>
		/// Préconfigura o nome do arquivo que essa classe aponta para carregar o arquivo de configuração.
		/// </summary>
		public static readonly string CONFIG_FILENAME = "GDA.config";

		/// <summary>
		/// List of folders that GDA will search (in addition to any custom folder specified in the
		/// regular .NET configuration file (usually App.config or Web.config). Folders are searched in
		/// order of appearance. All paths are expanded relative to the location of the GDA assembly.
		/// </summary>
		private static readonly string[] CONFIG_FOLDERS =  {
			AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
			Environment.CurrentDirectory,
			"./",
			"./../",
			"./../../",
			"./../../../",
			"./../Configuration/",
			"./../../Configuration/",
			"./../../../Configuration/",
			null
		};

		/// <summary>
		/// The full local path and file name of the configuration file. This variable is initialized
		/// once the full location and filename has been determined. Thus, it contains the local file 
		/// path (and name) of the file this handler is using.
		/// </summary>
		private string localConfigFilePath;

		/// <summary>
		/// Constructor to use when GDA should search for a file called GDA.config
		/// in a number of predefined locations.
		/// </summary>
		public FileConfigHandler() : this(null)
		{
		}

		/// <summary>
		/// Construtor usado para especificar a caminho e no do arquivo manualmente.
		/// </summary>
		/// <param name="file">Caminho completo ou nome do arquivo de configuração.</param>
		/// <exception cref="GDA.GDAException">No configuration file could be located.</exception>
		public FileConfigHandler(string file) : base(null)
		{
			CONFIG_FOLDERS[CONFIG_FOLDERS.Length - 1] = GetExecutingAssemblyLocation();
			if(file == null)
				file = GetConfigFileInfoFromApplicationConfig();
			localConfigFilePath = GetLocalPath(file);
			if(localConfigFilePath != null)
				root = LoadXml(GetTextReader(localConfigFilePath));
			else
				throw new GDAException("No configuration file could be located.");
		}

		/// <summary>
		/// Captura o caminho do arquivo de configuração.
		/// </summary>
		/// <param name="path">Informação do arquivo do configuração.</param>
		/// <returns>Caminho do arquivo de configuração.</returns>
		internal static string GetLocalPath(string path)
		{
			if(path == null)
				path = CONFIG_FILENAME;
			if(FileSystemUtil.IsValidFilePath(path))
			{
				return path;
			}
			else if(FileSystemUtil.IsFileName(path))
			{
				return FileSystemUtil.DetermineFileLocation(path, CONFIG_FOLDERS);
			}
			else if(FileSystemUtil.IsFolder(path))
			{
				return FileSystemUtil.CombinePathAndFileName(path, CONFIG_FILENAME);
			}
			else
				return path;
		}

		/// <summary>
		/// Abre o arquivo referenciado.
		/// </summary>
		/// <param name="localFilePath">Caminho do arquivo.</param>
		/// <returns>Stream do arquivo</returns>
		/// <exception cref="GDA.GDAException">Error ao carregar o arquivo.</exception>
		private TextReader GetTextReader(string localFilePath)
		{
			if(!FileSystemUtil.IsValidFilePath(localFilePath))
				throw new GDAException("Error load file in " + localFilePath);
			FileStream fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new StreamReader(fs);
		}

		private void OnValidationError(object sender, ValidationEventArgs args)
		{
		}

		/// <summary>
		/// Carrega os dados do arquivo configuração.
		/// </summary>
		/// <param name="reader">Stream do arquivo.</param>
		/// <returns>Xml.</returns>
		private XmlNode LoadXml(TextReader reader)
		{
			XmlValidatingReader xvr = null;
			XmlTextReader xtr = null;
			ValidationEventHandler eventHandler = new ValidationEventHandler(OnValidationError);
			try
			{
				xtr = new XmlTextReader(reader);
				xvr = new XmlValidatingReader(xtr);
				xvr.ValidationType = ValidationType.Auto;
				xvr.ValidationEventHandler += eventHandler;
				XmlDocument xd = new XmlDocument();
				xd.Load(xvr);
				return xd;
			}
			finally
			{
				if(xvr != null)
					xvr.Close();
			}
		}

		/// <summary>
		/// Captura o arquivo de configuração do GDA dentro do arquivo de configuração da aplicação.
		/// </summary>
		/// <returns>Caminho do arquivo</returns>
		private string GetConfigFileInfoFromApplicationConfig()
		{
			foreach (string key in APPSETTINGS_FILE)
			{
				try
				{
					string result = ConfigurationSettings.AppSettings[key];
					if(result != null && result.Length > 0)
						return result;
				}
				catch
				{
				}
			}
			foreach (string key in APPSETTINGS_FOLDER)
			{
				try
				{
					string result = ConfigurationSettings.AppSettings[key];
					if(result != null && result.Length > 0)
						return Path.Combine(result, CONFIG_FILENAME);
				}
				catch
				{
				}
			}
			return null;
		}

		/// <summary>
		/// Captura o caminho onde o assembly está sendo executado.
		/// </summary>
		/// <returns></returns>
		private string GetExecutingAssemblyLocation()
		{
			string uriString = Assembly.GetExecutingAssembly().EscapedCodeBase;
			Uri uri = new Uri(uriString);
			string path = uri.IsFile ? Path.GetDirectoryName(uri.LocalPath) : null;
			return path;
		}
	}
}
