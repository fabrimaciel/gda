using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Interfaces;
namespace GDA.Collections
{
	internal class GDACursorParameters
	{
		public readonly IProvider Provider;
		public readonly GDASession Session;
		public readonly IDbConnection Connection;
		public readonly IDbCommand Command;
		public readonly TranslatorDataInfoCollection TranslatorDataInfos;
		public readonly bool UsingPaging;
		public readonly int StartPage;
		public readonly int PageSize;
		public readonly EventHandler StartProcess;
		public GDACursorParameters (IProvider a, GDASession b, IDbConnection c, IDbCommand d, TranslatorDataInfoCollection e, bool f, int g, int h, EventHandler i)
		{
			if (d == null)
				throw new ArgumentNullException ("command");
			Session = b;
			Connection = c;
			Command = d;
			TranslatorDataInfos = e;
			UsingPaging = f;
			StartPage = g;
			PageSize = h;
			StartProcess = i;
			Provider = a;
		}
	}
	public class GDACursor<Model> : IEnumerable<Model>, IDisposable where Model : new()
	{
		private GDASession _session;
		private IDbConnection _connection;
		private IDbCommand _command;
		private TranslatorDataInfoCollection _translatorDataInfo;
		private int _startPage = 0;
		private int _pageSize = 0;
		private IProvider _provider;
		private bool _usingPaging;
		private EventHandler _startProcess;
		private bool _isLoadValues;
		private bool _enumeratorCreated = false;
		internal GDACursor (GDACursorParameters a)
		{
			_session = a.Session;
			_connection = a.Connection;
			_command = a.Command;
			_translatorDataInfo = a.TranslatorDataInfos;
			_usingPaging = a.UsingPaging;
			_startPage = a.StartPage;
			_pageSize = a.PageSize;
			_startProcess = a.StartProcess;
			_provider = a.Provider;
			_isLoadValues = false;
		}
		internal GDACursor (IProvider a, GDASession b, IDbConnection c, IDbCommand d, EventHandler e)
		{
			if (_command == null)
				throw new ArgumentNullException ("command");
			_session = b;
			_connection = c;
			_command = d;
			_startProcess = e;
			_provider = a;
			_isLoadValues = true;
		}
		~GDACursor ()
		{
			Dispose (false);
		}
		private void SendMessageDebugTrace (string a)
		{
			#if DEBUG
						            //System.Diagnostics.Debug.WriteLine(message);
#endif
			if (GDASettings.EnabledDebugTrace)
				GDAOperations.CallDebugTrace (this, a);
		}
		public List<Model> ToList ()
		{
			return new List<Model> (this);
		}
		public IEnumerator<Model> GetEnumerator ()
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
				SendMessageDebugTrace ("GDACursor: " + _command.CommandText);
				using (var b = Diagnostics.GDATrace.CreateExecutionHandler (_command))
					try {
						a = _command.ExecuteReader ();
					}
					catch (Exception ex) {
						ex = new GDAException (ex);
						b.Fail (ex);
						throw ex;
					}
				if (a == null)
					throw new InvalidOperationException (string.Format ("Execute Reader result from IDbCommand \"{0}\" couldn't be null.", _command.GetType ().FullName));
				_translatorDataInfo.ProcessFieldsPositions (a);
				int c = _startPage;
				int d = 0;
				while (a.Read ()) {
					if (d == 0 && _startProcess != null)
						_startProcess (this, EventArgs.Empty);
					if (_usingPaging && !_provider.SupportSQLCommandLimit && c < _pageSize) {
						c++;
						continue;
					}
					if (!_isLoadValues) {
						Model e = new Model ();
						IDataRecord f = a;
						PersistenceObject<Model>.RecoverValueOfResult (ref f, _translatorDataInfo, ref e, false);
						yield return e;
					}
					else {
						if (a [0] == DBNull.Value)
							yield return default(Model);
						else
							yield return (Model)a [0];
					}
					d++;
					if (_usingPaging && !_provider.SupportSQLCommandLimit && d >= _pageSize)
						break;
				}
			}
			finally {
				try {
					if (a != null) {
						a.Close ();
						a.Dispose ();
					}
					_command.Dispose ();
					_command = null;
				}
				finally {
					if (_session == null)
						try {
							_connection.Close ();
							_connection.Dispose ();
							_connection = null;
						}
						catch {
							SendMessageDebugTrace ("Error close connection.");
						}
				}
			}
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator ();
		}
		public static implicit operator List<Model> (GDACursor<Model> a) {
			return new List<Model> (a);
		}
		public static implicit operator GDAItemCollection<Model> (GDACursor<Model> collection) {
			return new GDAItemCollection<Model> (collection);
		}
		public static implicit operator GDAList<Model> (GDACursor<Model> collection) {
			return new GDAList<Model> (collection);
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
						_connection = null;
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
}
