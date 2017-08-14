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
		private static readonly string[] APPSETTINGS_FILE =  {
			"GDAConfigFile",
			"ConfigFile"
		};
		private static readonly string[] APPSETTINGS_FOLDER =  {
			"GDAConfigFolder",
			"ConfigFolder"
		};
		public static readonly string CONFIG_FILENAME = "GDA.config";
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
		private string localConfigFilePath;
		public FileConfigHandler () : this (null)
		{
		}
		public FileConfigHandler (string a) : base (null)
		{
			CONFIG_FOLDERS [CONFIG_FOLDERS.Length - 1] = GetExecutingAssemblyLocation ();
			if (a == null)
				a = GetConfigFileInfoFromApplicationConfig ();
			localConfigFilePath = GetLocalPath (a);
			if (localConfigFilePath != null)
				root = LoadXml (GetTextReader (localConfigFilePath));
			else
				throw new GDAException ("No configuration file could be located.");
		}
		internal static string GetLocalPath (string a)
		{
			if (a == null)
				a = CONFIG_FILENAME;
			if (FileSystemUtil.IsValidFilePath (a)) {
				return a;
			}
			else if (FileSystemUtil.IsFileName (a)) {
				return FileSystemUtil.DetermineFileLocation (a, CONFIG_FOLDERS);
			}
			else if (FileSystemUtil.IsFolder (a)) {
				return FileSystemUtil.CombinePathAndFileName (a, CONFIG_FILENAME);
			}
			else
				return a;
		}
		private TextReader GetTextReader (string a)
		{
			if (!FileSystemUtil.IsValidFilePath (a))
				throw new GDAException ("Error load file in " + a);
			FileStream b = new FileStream (a, FileMode.Open, FileAccess.Read, FileShare.Read);
			return new StreamReader (b);
		}
		private void OnValidationError (object a, ValidationEventArgs b)
		{
		}
		private XmlNode LoadXml (TextReader a)
		{
			XmlValidatingReader b = null;
			XmlTextReader c = null;
			ValidationEventHandler d = new ValidationEventHandler (OnValidationError);
			try {
				c = new XmlTextReader (a);
				b = new XmlValidatingReader (c);
				b.ValidationType = ValidationType.Auto;
				b.ValidationEventHandler += d;
				XmlDocument e = new XmlDocument ();
				e.Load (b);
				return e;
			}
			finally {
				if (b != null)
					b.Close ();
			}
		}
		private string GetConfigFileInfoFromApplicationConfig ()
		{
			foreach (string key in APPSETTINGS_FILE) {
				try {
					string a = ConfigurationSettings.AppSettings [key];
					if (a != null && a.Length > 0)
						return a;
				}
				catch {
				}
			}
			foreach (string key in APPSETTINGS_FOLDER) {
				try {
					string a = ConfigurationSettings.AppSettings [key];
					if (a != null && a.Length > 0)
						return Path.Combine (a, CONFIG_FILENAME);
				}
				catch {
				}
			}
			return null;
		}
		private string GetExecutingAssemblyLocation ()
		{
			string a = Assembly.GetExecutingAssembly ().EscapedCodeBase;
			Uri b = new Uri (a);
			string c = b.IsFile ? Path.GetDirectoryName (b.LocalPath) : null;
			return c;
		}
	}
}
