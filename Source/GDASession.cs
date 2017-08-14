using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Interfaces;
namespace GDA
{
	public class GDASession : IDisposable
	{
		#if PocketPC
				        /// <summary>
        /// Timeout padrão dos comandos
        /// </summary>
        public const int DEFAULT_COMMAND_TIMEOUT = 0;
#else
		public const int DEFAULT_COMMAND_TIMEOUT = 30;
		#endif
		private static int _defaultCommandTimeout = DEFAULT_COMMAND_TIMEOUT;
		private bool _isDisposed;
		private IDbConnection _currentConnection;
		private GDASessionState _state = GDASessionState.Open;
		private IProviderConfiguration _providerConfiguration;
		private int _commandTimeout = DefaultCommandTimeout;
		public event Provider.CreateConnectionEvent ConnectionCreated;
		protected bool IsDisposed {
			get {
				return _isDisposed;
			}
		}
		public GDASessionState State {
			get {
				return _state;
			}
		}
		public int CommandTimeout {
			get {
				return _commandTimeout;
			}
			set {
				if (value < 0)
					throw new System.ArgumentException ("The property value assigned is less than 0.", "CommandTimeout");
				_commandTimeout = value;
			}
		}
		public static int DefaultCommandTimeout {
			get {
				return _defaultCommandTimeout;
			}
			set {
				if (value < 0)
					throw new System.ArgumentException ("The property value assigned is less than 0.", "CommandTimeout");
				_defaultCommandTimeout = value;
			}
		}
		public GDASession ()
		{
			_providerConfiguration = GDASettings.DefaultProviderConfiguration;
		}
		public GDASession (IProviderConfiguration a)
		{
			_providerConfiguration = a;
		}
		~GDASession ()
		{
			Dispose (false);
		}
		[System.Diagnostics.DebuggerHidden]
		[System.Diagnostics.DebuggerDisplay ("Connection")]
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public virtual IDbConnection CurrentConnection {
			get {
				CreateConnection ();
				return _currentConnection;
			}
		}
		internal IDbConnection CreateConnection ()
		{
			if (_currentConnection == null) {
				_currentConnection = _providerConfiguration.CreateConnection ();
				GDAConnectionManager.NotifyConnectionCreated (_currentConnection);
				if (ConnectionCreated != null)
					ConnectionCreated (this, new GDA.Provider.CreateConnectionEventArgs (_currentConnection));
				if (_currentConnection.State != ConnectionState.Open) {
					_currentConnection.Open ();
					GDAConnectionManager.NotifyConnectionOpened (_currentConnection);
				}
			}
			return _currentConnection;
		}
		internal virtual IDbTransaction CurrentTransaction {
			get {
				return null;
			}
			set {
			}
		}
		public ConnectionState ConnectionState {
			get {
				return _currentConnection != null ? _currentConnection.State : ConnectionState.Closed;
			}
		}
		protected void CheckDisposed ()
		{
			if (_isDisposed)
				throw new ObjectDisposedException ("GDASession");
		}
		internal void DefineConfiguration (IProviderConfiguration a)
		{
			if (_providerConfiguration != null && _providerConfiguration.ProviderIdentifier != a.ProviderIdentifier) {
				throw new GDAException ("Invalid provider configuration, there is a different.");
			}
			else if (_providerConfiguration == null)
				_providerConfiguration = a;
		}
		public virtual IDbCommand CreateCommand ()
		{
			IDbCommand a = CurrentConnection.CreateCommand ();
			a.Connection = CurrentConnection;
			a.Transaction = this.CurrentTransaction;
			a.CommandTimeout = _commandTimeout;
			return a;
		}
		public IProviderConfiguration ProviderConfiguration {
			get {
				return _providerConfiguration;
			}
		}
		public virtual void Ping ()
		{
			if (_providerConfiguration == null)
				throw new InvalidOperationException ("ProviderConfiguration not defined.");
			using (IDbConnection a = _providerConfiguration.Provider.CreateConnection ()) {
				GDAConnectionManager.NotifyConnectionCreated (a);
				a.ConnectionString = _providerConfiguration.ConnectionString;
				if (a.State != ConnectionState.Open) {
					a.Open ();
					GDAConnectionManager.NotifyConnectionOpened (a);
				}
				a.Close ();
			}
		}
		public void Close ()
		{
			if (_state == GDASessionState.Open) {
				if (CurrentTransaction != null)
					CurrentTransaction.Dispose ();
				_currentConnection.Close ();
				_currentConnection.Dispose ();
				CurrentTransaction = null;
				_currentConnection = null;
				_state = GDASessionState.Closed;
			}
		}
		protected virtual void DisposeCurrentTransaction (bool a)
		{
			var b = CurrentTransaction;
			if (b != null)
				try {
					b.Dispose ();
				}
				catch {
					if (a)
						throw;
				}
		}
		public virtual void Dispose (bool a)
		{
			if (_state == GDASessionState.Open) {
				DisposeCurrentTransaction (a);
				if (_currentConnection != null) {
					try {
						if (_currentConnection != null)
							_currentConnection.Close ();
					}
					catch {
					}
					try {
						if (_currentConnection != null)
							_currentConnection.Dispose ();
					}
					catch {
					}
				}
				CurrentTransaction = null;
				_currentConnection = null;
				_state = GDASessionState.Closed;
			}
			_isDisposed = true;
		}
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
	}
}
