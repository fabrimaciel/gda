using System;
using System.Collections.Generic;
using System.Text;
using GDA.Interfaces;
using System.Data;
namespace GDA.Collections
{
	public abstract class BaseGDADataRecordCursor : IDisposable
	{
		protected GDASession _session;
		private IDbConnection _connection;
		protected IDbCommand _command;
		protected string _commandText;
		protected IDictionary<string, int> _mapFields;
		protected bool _mapFieldsLoaded = false;
		protected int _startPage = 0;
		protected int _pageSize = 0;
		protected IProvider _provider;
		protected bool _usingPaging;
		protected EventHandler _startProcess;
		private bool _enumeratorCreated = false;
		protected IDbConnection Connection {
			get {
				return _connection;
			}
		}
		public GDA.Interfaces.IProvider Provider {
			get {
				return _provider;
			}
		}
		internal BaseGDADataRecordCursor (GDACursorParameters a)
		{
			if (a == null)
				throw new ArgumentNullException ("parameters");
			_session = a.Session;
			_connection = a.Connection;
			_command = a.Command;
			if (a.Command != null)
				_commandText = a.Command.CommandText;
			_usingPaging = a.UsingPaging;
			_startPage = a.StartPage;
			_pageSize = a.PageSize;
			_startProcess = a.StartProcess;
			_provider = a.Provider;
		}
		public BaseGDADataRecordCursor ()
		{
		}
		~BaseGDADataRecordCursor ()
		{
			Dispose (false);
		}
		protected void SendMessageDebugTrace (string a)
		{
			#if DEBUG
						            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if (GDASettings.EnabledDebugTrace)
				GDAOperations.CallDebugTrace (this, a);
		}
		protected virtual IDictionary<string, int> OnLoadTranslator (IDataReader a)
		{
			return null;
		}
		protected abstract T CreateDataRecord<T> (IDataRecord a, IDictionary<string, int> b) where T : GDADataRecord;
		protected IEnumerator<T> CreateEnumerator<T> () where T : GDADataRecord
		{
			_enumeratorCreated = true;
			if (_session == null && _connection.State != ConnectionState.Open) {
				try {
					_connection.Open ();
				}
				catch (Exception ex) {
					try {
						_connection.Dispose ();
					}
					catch {
					}
					_connection = null;
					throw new GDAException (ex);
				}
				GDAConnectionManager.NotifyConnectionOpened (_connection);
			}
			IDataReader a = null;
			try {
				StringBuilder b = new StringBuilder ();
				b.AppendFormat ("GDADataRecordCursor: {0}\r\n", _command.CommandText);
				if (_command.Parameters.Count > 0) {
					b.Append ("--> Parameters:");
					foreach (IDataParameter parameter in _command.Parameters)
						if (parameter.Value is byte[])
							b.Append ("\r\n").Append (parameter.ParameterName).Append (" = byte[]");
						else
							b.Append ("\r\n").Append (parameter.ParameterName).Append (" = ").Append (parameter.Value == null ? "NULL" : '"' + parameter.Value.ToString () + '"');
				}
				SendMessageDebugTrace (b.ToString ());
				using (var c = Diagnostics.GDATrace.CreateExecutionHandler (_command))
					try {
						a = _command.ExecuteReader ();
					}
					catch (Exception ex) {
						ex = new GDAException (ex);
						c.Fail (ex);
						throw ex;
					}
				if (!_mapFieldsLoaded) {
					_mapFields = OnLoadTranslator (a);
					_mapFieldsLoaded = true;
				}
				int d = _startPage;
				int e = 0;
				while (a.Read ()) {
					if (e == 0 && _startProcess != null)
						_startProcess (this, EventArgs.Empty);
					if (_usingPaging && !_provider.SupportSQLCommandLimit && d < _pageSize) {
						d++;
						continue;
					}
					yield return CreateDataRecord<T> (a, _mapFields);
					e++;
					if (_usingPaging && !_provider.SupportSQLCommandLimit && e >= _pageSize)
						break;
				}
			}
			finally {
				try {
					if (a != null) {
						a.Close ();
						a.Dispose ();
					}
					if (_command != null)
						_command.Dispose ();
					_command = null;
				}
				finally {
					if (_session == null)
						try {
							_connection.Close ();
							_connection.Dispose ();
						}
						catch {
							SendMessageDebugTrace ("Error close connection.");
						}
				}
			}
		}
		protected virtual void Dispose (bool a)
		{
			if (_command != null)
				_command.Dispose ();
			_command = null;
			if (!_enumeratorCreated && _connection != null)
				if (_session == null)
					try {
						_connection.Close ();
						_connection.Dispose ();
					}
					catch {
						SendMessageDebugTrace ("Error close connection.");
					}
			_session = null;
		}
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
	}
	public class GDADataRecordCursor : BaseGDADataRecordCursor, IEnumerable<GDADataRecord>
	{
		internal GDADataRecordCursor (GDACursorParameters a) : base (a)
		{
		}
		protected override T CreateDataRecord<T> (IDataRecord a, IDictionary<string, int> b)
		{
			return (T)new GDADataRecord (a, b);
		}
		public virtual IEnumerator<GDADataRecord> GetEnumerator ()
		{
			return CreateEnumerator<GDADataRecord> ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}
	}
	public class GDADataRecordCursor<Model> : GDADataRecordCursor, IEnumerable<GDADataRecord<Model>>
	{
		private TranslatorDataInfoCollection _translatorDataInfos;
		internal GDADataRecordCursor (GDACursorParameters parameters) : base (parameters)
		{
			_translatorDataInfos = parameters.TranslatorDataInfos;
		}
		protected override IDictionary<string, int> OnLoadTranslator (IDataReader a)
		{
			_translatorDataInfos.ProcessFieldsPositions (a);
			return _translatorDataInfos;
		}
		protected override T CreateDataRecord<T> (IDataRecord record, IDictionary<string, int> mapFields)
		{
			return (T)(GDADataRecord)new GDADataRecord<Model> (record, mapFields as TranslatorDataInfoCollection);
		}
		public IEnumerable<T> Select<T> () where T : new()
		{
			foreach (var i in new GDADataRecordCursorEx<T> (new GDACursorParameters (_provider, _session, Connection, _command, _translatorDataInfos, _usingPaging, _startPage, _pageSize, _startProcess)))
				yield return i.GetInstance ();
		}
		public IEnumerator<GDADataRecord<Model>> GetEnumerator ()
		{
			return CreateEnumerator<GDADataRecord<Model>> ();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}
	}
	public class GDADataRecordCursorEx<Model> : GDADataRecordCursor<Model> where Model : new()
	{
		internal GDADataRecordCursorEx (GDACursorParameters a) : base (a)
		{
		}
		protected override T CreateDataRecord<T> (IDataRecord a, IDictionary<string, int> b)
		{
			return (T)(GDADataRecord)new GDADataRecordEx<Model> (a, b as TranslatorDataInfoCollection);
		}
	}
}
