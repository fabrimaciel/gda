using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
namespace GDA.Provider
{
	public class Provider : GDA.Interfaces.IProvider
	{
		public const long No_DbType = -1;
		private Type m_ConnectionType;
		private string m_ConnectionTypeName;
		private Type m_CommandType;
		private string m_CommandTypeName;
		private Type m_AdapterType;
		private string m_AdapterTypeName;
		private Type m_ParameterType;
		private string m_ParameterTypeName;
		private string m_ParameterPrefix;
		private string m_ParameterSuffix;
		private string m_Name;
		protected Assembly providerAssembly;
		private string m_AssemblyName;
		private string m_SqlQueryReturnIdentity;
		private bool m_GenerateIdentity = false;
		private bool m_ExecuteCommandsOneAtATime = false;
		private System.Collections.Generic.List<string> _reservedsWords;
		public bool ExecuteCommandsOneAtATime {
			get {
				return m_ExecuteCommandsOneAtATime;
			}
			set {
				m_ExecuteCommandsOneAtATime = value;
			}
		}
		public virtual string SqlQueryReturnIdentity {
			get {
				return m_SqlQueryReturnIdentity;
			}
		}
		public virtual string GetIdentitySelect (string a, string b)
		{
			if (string.IsNullOrEmpty (SqlQueryReturnIdentity))
				throw new GDAException ("SqlQueryReturnIdentuty not found in provider.");
			else
				return SqlQueryReturnIdentity;
		}
		public virtual string GetIdentitySelect (GDA.Sql.TableName a, string b)
		{
			return GetIdentitySelect (a != null ? a.Name : null, b);
		}
		public bool GenerateIdentity {
			get {
				return m_GenerateIdentity;
			}
			set {
				m_GenerateIdentity = value;
			}
		}
		public Type ConnectionType {
			get {
				if (m_ConnectionType == null) {
					m_ConnectionType = ProviderAssembly.GetType (m_ConnectionTypeName, false);
					if (m_ConnectionType == null) {
						throw new GDAException (string.Format ("N�o � poss�vel carrega a classe de conex�o: {0} do assmbly: {1}", m_ConnectionTypeName, m_AssemblyName));
					}
				}
				return m_ConnectionType;
			}
		}
		public Type CommandType {
			get {
				if (m_CommandType == null) {
					m_CommandType = ProviderAssembly.GetType (m_CommandTypeName, false);
					if (m_CommandType == null) {
						throw new GDAException (string.Format ("N�o � poss�vel carrega a classe de commando: {0} do assmbly: {1}", m_CommandTypeName, m_AssemblyName));
					}
				}
				return m_CommandType;
			}
		}
		public Type DataAdapterType {
			get {
				if (m_AdapterType == null) {
					m_AdapterType = ProviderAssembly.GetType (m_AdapterTypeName, false);
					if (m_AdapterType == null) {
						throw new GDAException (string.Format ("N�o � poss�vel carrega a classe de adapter: {0} do assmbly: {1}", m_AdapterTypeName, m_AssemblyName));
					}
				}
				return m_AdapterType;
			}
		}
		public Type ParameterType {
			get {
				if (m_ParameterType == null) {
					m_ParameterType = ProviderAssembly.GetType (m_ParameterTypeName, false);
					if (m_ParameterType == null) {
						throw new GDAException (string.Format ("N�o � poss�vel carrega a classe de paramater: {0} do assmbly: {1}", m_ParameterTypeName, m_AssemblyName));
					}
				}
				return m_ParameterType;
			}
		}
		public string Name {
			get {
				return m_Name;
			}
		}
		public virtual string ParameterPrefix {
			get {
				return m_ParameterPrefix;
			}
		}
		public virtual string ParameterSuffix {
			get {
				return m_ParameterSuffix;
			}
		}
		public virtual bool SupportSQLCommandLimit {
			get {
				return false;
			}
		}
		public virtual DateTime MinimumSupportedDateTime {
			get {
				return new DateTime (1800, 1, 1);
			}
		}
		public virtual DateTime MaximumSupportedDateTime {
			get {
				return new DateTime (3000, 1, 1);
			}
		}
		public virtual string QuoteExpressionBegin {
			get {
				return "";
			}
		}
		public virtual string QuoteExpressionEnd {
			get {
				return "";
			}
		}
		public Provider (string a, Type b, Type c, Type d)
		{
			if (b == null)
				throw new ArgumentNullException ("connection");
			else if (c == null)
				throw new ArgumentNullException ("dataAdapter");
			else if (d == null)
				throw new ArgumentNullException ("command");
			m_Name = a;
			m_ConnectionTypeName = b.FullName;
			m_AdapterTypeName = c.FullName;
			m_CommandTypeName = d.FullName;
			m_ConnectionType = b;
			m_AdapterType = c;
			m_CommandType = d;
		}
		public Provider (string a, Type b, Type c, Type d, string e) : this (a, b, c, d)
		{
			m_SqlQueryReturnIdentity = e;
		}
		public Provider (string a, Type b, Type c, Type d, bool e) : this (a, b, c, d)
		{
			m_GenerateIdentity = e;
		}
		public Provider (string a, Type b, Type c, Type d, Type e, string f)
		{
			if (b == null)
				throw new ArgumentNullException ("connection");
			else if (c == null)
				throw new ArgumentNullException ("dataAdapter");
			else if (d == null)
				throw new ArgumentNullException ("command");
			else if (e == null)
				throw new ArgumentNullException ("parameter");
			else if (f == "" || f == null)
				throw new ArgumentNullException ("paramterPrefix");
			m_Name = a;
			m_ConnectionTypeName = b.FullName;
			m_AdapterTypeName = c.FullName;
			m_CommandTypeName = d.FullName;
			m_ParameterTypeName = e.FullName;
			m_ConnectionType = b;
			m_AdapterType = c;
			m_CommandType = d;
			m_ParameterType = e;
			m_ParameterPrefix = f;
		}
		public Provider (string a, Type b, Type c, Type d, Type e, string f, string g) : this (a, b, c, d, e, f)
		{
			m_SqlQueryReturnIdentity = g;
		}
		public Provider (string a, Type b, Type c, Type d, Type e, string f, string g, bool h) : this (a, b, c, d, e, f, g)
		{
			m_GenerateIdentity = h;
		}
		public Provider (string a, Type b, Type c, Type d, Type e, string f, bool g) : this (a, b, c, d, e, f)
		{
			m_GenerateIdentity = g;
		}
		public Provider (string a, string b, string c, string d, string e)
		{
			m_Name = a;
			m_AssemblyName = b;
			m_ConnectionTypeName = c;
			m_AdapterTypeName = d;
			m_CommandTypeName = e;
		}
		public Provider (string a, string b, string c, string d, string e, string f) : this (a, b, c, d, e)
		{
			m_SqlQueryReturnIdentity = f;
		}
		public Provider (string a, string b, string c, string d, string e, bool f) : this (a, b, c, d, e)
		{
			m_GenerateIdentity = f;
		}
		public Provider (string a, string b, string c, string d, string e, string f, string g) : this (a, b, c, d, e)
		{
			m_ParameterTypeName = f;
			m_ParameterPrefix = g;
		}
		public Provider (string a, string b, string c, string d, string e, string f, string g, string h) : this (a, b, c, d, e, f, g)
		{
			m_SqlQueryReturnIdentity = h;
		}
		public Provider (string a, string b, string c, string d, string e, string f, string g, bool h) : this (a, b, c, d, e, f, g)
		{
			m_GenerateIdentity = h;
		}
		public Provider (string a, string b, string c, string d, string e, string f, string g, bool h, string i) : this (a, b, c, d, e, f, g)
		{
			m_SqlQueryReturnIdentity = i;
			m_GenerateIdentity = h;
		}
		public Assembly ProviderAssembly {
			get {
				if (providerAssembly == null) {
					#if PocketPC
										                    providerAssembly = Assembly.Load(m_AssemblyName);
#else
					if (m_AssemblyName.IndexOf (',') == -1) {
						providerAssembly = Assembly.LoadWithPartialName (m_AssemblyName);
					}
					else {
						providerAssembly = Assembly.Load (m_AssemblyName);
					}
					#endif
				}
				return providerAssembly;
			}
		}
		public virtual IDbConnection CreateConnection ()
		{
			object a = null;
			a = Activator.CreateInstance (ConnectionType);
			if (a == null)
				throw new GDAException (string.Format ("N�o � poss�vel criar a classe connection: {0} do assmbly: {1}", m_ConnectionTypeName, m_AssemblyName));
			return (IDbConnection)a;
		}
		public virtual IDbCommand CreateCommand ()
		{
			object a = null;
			a = Activator.CreateInstance (CommandType);
			if (a == null)
				throw new GDAException (string.Format ("N�o � poss�vel criar a classe command: {0} do assmbly: {1}", m_CommandTypeName, m_AssemblyName));
			return (IDbCommand)a;
		}
		public virtual IDbDataAdapter CreateDataAdapter ()
		{
			object a = Activator.CreateInstance (DataAdapterType);
			if (a == null)
				throw new GDAException (string.Format ("N�o � poss�vel criar a classe adapter: {0} do assmbly: {1}", m_AdapterTypeName, m_AssemblyName));
			return (IDbDataAdapter)a;
		}
		public virtual System.Data.Common.DbParameter CreateParameter ()
		{
			object a = Activator.CreateInstance (ParameterType);
			if (a == null)
				throw new GDAException (string.Format ("N�o � poss�vel criar a classe parameter: {0} do assmbly: {1}", m_ParameterTypeName, m_AssemblyName));
			return (System.Data.Common.DbParameter)a;
		}
		public virtual string SQLCommandLimit (List<Mapper> a, string b, int c, int d)
		{
			return b;
		}
		public virtual string BuildTableName (GDA.Sql.TableName a)
		{
			if (a == null)
				return null;
			if (!string.IsNullOrEmpty (a.Schema))
				return string.Format ("{0}.{1}", QuoteExpression (a.Schema), QuoteExpression (a.Name));
			else
				return QuoteExpression (a.Name);
		}
		public virtual long GetDbType (Type a)
		{
			throw new NotImplementedException ();
		}
		public virtual long GetDbType (string a, bool b)
		{
			throw new NotImplementedException ();
		}
		public virtual string StatementTerminator {
			get {
				return ";";
			}
		}
		public virtual bool IsReservedWord (string a)
		{
			return false;
		}
		public virtual char QuoteCharacter {
			get {
				return '\0';
			}
		}
		public virtual Capability Capabilities {
			get {
				return Capability.BatchQuery | Capability.Paging | Capability.NamedParameters;
			}
		}
		public virtual Type GetSystemType (long a)
		{
			return null;
		}
		public virtual System.Collections.Generic.List<string> ReservedsWords {
			get {
				if (_reservedsWords == null)
					_reservedsWords = new System.Collections.Generic.List<string> ();
				return _reservedsWords;
			}
		}
		public virtual string QuoteExpression (string a)
		{
			return a;
		}
		public virtual void SetParameterValue (IDbDataParameter a, object b)
		{
			a.Value = b;
		}
	}
}
