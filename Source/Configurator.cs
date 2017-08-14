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
		private static object cfgLock = new object ();
		private static IList handlers = new ArrayList ();
		private static Hashtable namedHandlers = new Hashtable ();
		private static bool isInitialized;
		private static Hashtable targets = new Hashtable ();
		private static bool isLoggingEnabled = true;
		private static readonly string CRLF = "\r\n";
		private static StringBuilder errorLog = new StringBuilder ();
		private static StringBuilder exceptionLog = new StringBuilder ();
		private static bool ignoreEmptyConfiguration = false;
		public static bool IgnoreEmptyConfiguration {
			get {
				return Configurator.ignoreEmptyConfiguration;
			}
			set {
				Configurator.ignoreEmptyConfiguration = value;
			}
		}
		private Configurator ()
		{
		}
		public static void AddSectionHandler (string a, string b)
		{
			string c = String.Format ("Unable to create GDASectionHandler for section " + "named \"{0}\" in file \"{1}\".{2}", b, AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, CRLF);
			try {
				GDASectionHandler d = (GDASectionHandler)ConfigurationSettings.GetConfig (b);
				if (d != null && d.IsValid)
					AddHandler (a, d, false);
				else
					LogMessage (c);
			}
			catch (Exception e) {
				LogMessage (c);
				LogException (e);
			}
		}
		public static void AddSectionHandler (string a)
		{
			AddSectionHandler (null, a);
		}
		public static void AddFileHandler (string a, string b)
		{
			try {
				if (!string.IsNullOrEmpty (FileConfigHandler.GetLocalPath (b))) {
					FileConfigHandler c = new FileConfigHandler (b);
					AddHandler (a, c, false);
				}
			}
			catch (Exception e) {
				string d = b == null ? FileConfigHandler.CONFIG_FILENAME : b;
				LogMessage ("Unable to create FileHandler for file " + d + ".");
				LogMessage ("This usually means that the file could not be found in any of " + "the default search locations." + CRLF);
				LogException (e);
			}
		}
		public static void AddFileHandler (string a)
		{
			AddFileHandler (null, a);
		}
		public static void AddExternalHandler (string a, XmlNode b)
		{
			try {
				AddHandler (a, new BaseSectionHandler (b), true);
			}
			catch (Exception e) {
				LogMessage ("Unable to use supplied XML fragment as configuration source:");
				LogMessage (b != null ? b.OuterXml : "null");
				LogException (e);
			}
		}
		public static void AddExternalHandler (XmlNode a)
		{
			AddExternalHandler (null, a);
		}
		public static void AddStreamHandler (string a, Stream b)
		{
			try {
				XmlDocument c = new XmlDocument ();
				c.Load (b);
				AddHandler (a, new BaseSectionHandler (c), false);
			}
			catch (Exception e) {
				LogMessage ("Unable to use supplied stream as a configuration source." + CRLF);
				LogException (e);
			}
		}
		public static void AddStreamHandler (Stream a)
		{
			AddStreamHandler (null, a);
		}
		public static void Configure (string a, object b)
		{
			InitializeHandlers ();
			lock (cfgLock) {
				ConfigurationMap c = GetConfigurationMap (b.GetType ());
				if (a != null) {
					IList d = new ArrayList (1);
					d.Add (namedHandlers [a]);
					c.Configure (d, b);
				}
				else
					c.Configure (handlers, b);
			}
		}
		public static void Configure (object a)
		{
			Configure (null, a);
		}
		public static void Configure (string a, Type b)
		{
			InitializeHandlers ();
			lock (cfgLock) {
				ConfigurationMap c = GetConfigurationMap (b);
				if (a != null) {
					IList d = new ArrayList (1);
					d.Add (namedHandlers [a]);
					c.Configure (d, b);
				}
				else
					c.Configure (handlers, b);
			}
		}
		public static void Configure (Type a)
		{
			Configure (null, a);
		}
		private static void InitializeHandlers ()
		{
			if (!isInitialized) {
				isInitialized = true;
				AddSectionHandler ("gda");
				AddFileHandler (null);
			}
			lock (cfgLock) {
				if (!ignoreEmptyConfiguration && handlers.Count == 0) {
					StringBuilder a = new StringBuilder ();
					a.Append ("FATAL ERROR: No configuration store was found!" + CRLF);
					a.Append ("GDA is unable to continue!" + CRLF + CRLF);
					a.Append ("The handlers emitted the following error messages:" + CRLF);
					a.Append (errorLog.ToString ());
					a.Append ("The handlers threw the following exceptions:" + CRLF);
					a.Append (exceptionLog.ToString ());
					throw new GDAException (a.ToString ());
				}
				else {
					errorLog = null;
					exceptionLog = null;
				}
			}
		}
		private static void AddHandler (string a, BaseSectionHandler b, bool c)
		{
			lock (cfgLock) {
				if (b != null && b.IsValid) {
					handlers.Insert (c ? 0 : handlers.Count, b);
					if (a != null)
						namedHandlers [a] = b;
				}
				else {
					LogMessage ("Attempt to add configuration handler failed.");
					if (b != null)
						LogMessage ("Handler: {0}  IsValid: {1}", b.GetType (), b.IsValid);
				}
			}
		}
		private static void LogMessage (string a, params object[] b)
		{
			if (errorLog == null)
				errorLog = new StringBuilder ();
			if (b != null && b.Length > 0)
				errorLog.AppendFormat (System.Globalization.CultureInfo.InvariantCulture, a, b);
			else
				errorLog.Append (a);
			errorLog.Append (CRLF);
		}
		private static void LogException (Exception a)
		{
			if (a != null) {
				exceptionLog.Append (a.ToString () + CRLF + CRLF);
			}
		}
		private static ConfigurationMap GetConfigurationMap (Type a)
		{
			ConfigurationMap b;
			if (!targets.ContainsKey (a)) {
				b = new ConfigurationMap (a);
				targets [a] = b;
			}
			else
				b = (ConfigurationMap)targets [a];
			return b;
		}
		public static bool IsLoggingEnabled {
			get {
				return isLoggingEnabled;
			}
			set {
				isLoggingEnabled = value;
			}
		}
		public int HandlerCount {
			get {
				lock (cfgLock) {
					InitializeHandlers ();
					return handlers.Count;
				}
			}
		}
	}
}
