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
using System.Data;
using GDA.Interfaces;

namespace GDA
{
	/// <summary>
	/// Gerencia uma seção de conexão onde varios procedimento podem
	/// ser executados em apenas uma conexão.
	/// </summary>
	public class GDASession : IDisposable
	{
		#if PocketPC
		        /// <summary>
        /// Timeout padrão dos comandos
        /// </summary>
        public const int DEFAULT_COMMAND_TIMEOUT = 0;
#else
		/// <summary>
		/// Timeout padrão dos comandos
		/// </summary>
		public const int DEFAULT_COMMAND_TIMEOUT = 30;

		#endif
		private static int _defaultCommandTimeout = DEFAULT_COMMAND_TIMEOUT;

		private bool _isDisposed;

		/// <summary>
		/// Conexão usada na sessão.
		/// </summary>
		private IDbConnection _currentConnection;

		private GDASessionState _state = GDASessionState.Open;

		/// <summary>
		/// Provedor de configuração da sessão.
		/// </summary>
		private IProviderConfiguration _providerConfiguration;

		/// <summary>
		/// Timeout dos comandos criados apartir da seção.
		/// </summary>
		private int _commandTimeout = DefaultCommandTimeout;

		/// <summary>
		/// Evento acionado quando a conexão é criada.
		/// </summary>
		public event Provider.CreateConnectionEvent ConnectionCreated;

		/// <summary>
		/// Identifica se a instancia já foi liberada.
		/// </summary>
		protected bool IsDisposed
		{
			get
			{
				return _isDisposed;
			}
		}

		/// <summary>
		/// Estado da sessão.
		/// </summary>
		public GDASessionState State
		{
			get
			{
				return _state;
			}
		}

		/// <summary>
		/// Timeout em segundos dos comandos criados apartir da seção. Por padrão é 30 segundos
		/// </summary>
		public int CommandTimeout
		{
			get
			{
				return _commandTimeout;
			}
			set
			{
				if(value < 0)
					throw new System.ArgumentException("The property value assigned is less than 0.", "CommandTimeout");
				_commandTimeout = value;
			}
		}

		/// <summary>
		/// Timeout padrão em segundos dos comandos. Por padrão é 30 segundos.
		/// </summary>
		public static int DefaultCommandTimeout
		{
			get
			{
				return _defaultCommandTimeout;
			}
			set
			{
				if(value < 0)
					throw new System.ArgumentException("The property value assigned is less than 0.", "CommandTimeout");
				_defaultCommandTimeout = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GDASession()
		{
			_providerConfiguration = GDASettings.DefaultProviderConfiguration;
		}

		/// <summary>
		/// Construtor.
		/// </summary>
		/// <param name="providerConfiguration">Provider de configuração usado na sessão.</param>
		public GDASession(IProviderConfiguration providerConfiguration)
		{
			_providerConfiguration = providerConfiguration;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~GDASession()
		{
			Dispose(false);
		}

		/// <summary>
		/// Connexão da sessão.
		/// </summary>
		[System.Diagnostics.DebuggerHidden]
		[System.Diagnostics.DebuggerDisplay("Connection")]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public virtual IDbConnection CurrentConnection
		{
			get
			{
				CreateConnection();
				return _currentConnection;
			}
		}

		/// <summary>
		/// Cria a conexão.
		/// </summary>
		internal IDbConnection CreateConnection()
		{
			if(_currentConnection == null)
			{
				_currentConnection = _providerConfiguration.CreateConnection();
				GDAConnectionManager.NotifyConnectionCreated(_currentConnection);
				if(ConnectionCreated != null)
					ConnectionCreated(this, new GDA.Provider.CreateConnectionEventArgs(_currentConnection));
				if(_currentConnection.State != ConnectionState.Open)
				{
					_currentConnection.Open();
					GDAConnectionManager.NotifyConnectionOpened(_currentConnection);
				}
			}
			return _currentConnection;
		}

		/// <summary>
		/// Transaction da sessão.
		/// </summary>
		internal virtual IDbTransaction CurrentTransaction
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Estado da conexão.
		/// </summary>
		public ConnectionState ConnectionState
		{
			get
			{
				return _currentConnection != null ? _currentConnection.State : ConnectionState.Closed;
			}
		}

		/// <summary>
		/// Verifica se a instancia foi liberada.
		/// </summary>
		protected void CheckDisposed()
		{
			if(_isDisposed)
				throw new ObjectDisposedException("GDASession");
		}

		/// <summary>
		/// Define o provedor de configuração.
		/// </summary>
		/// <param name="configuration">Provedor de configuração.</param>
		internal void DefineConfiguration(IProviderConfiguration configuration)
		{
			if(_providerConfiguration != null && _providerConfiguration.ProviderIdentifier != configuration.ProviderIdentifier)
			{
				throw new GDAException("Invalid provider configuration, there is a different.");
			}
			else if(_providerConfiguration == null)
				_providerConfiguration = configuration;
		}

		/// <summary>
		/// Cria uma nova instância de commando.
		/// </summary>
		/// <returns></returns>
		public virtual IDbCommand CreateCommand()
		{
			IDbCommand cmd = CurrentConnection.CreateCommand();
			cmd.Connection = CurrentConnection;
			cmd.Transaction = this.CurrentTransaction;
			cmd.CommandTimeout = _commandTimeout;
			return cmd;
		}

		/// <summary>
		/// Provedor de configuração da sessão.
		/// </summary>
		public IProviderConfiguration ProviderConfiguration
		{
			get
			{
				return _providerConfiguration;
			}
		}

		/// <summary>
		/// Efetua um ping no servidor com os dados da sessão.
		/// </summary>
		public virtual void Ping()
		{
			if(_providerConfiguration == null)
				throw new InvalidOperationException("ProviderConfiguration not defined.");
			using (IDbConnection conn = _providerConfiguration.Provider.CreateConnection())
			{
				GDAConnectionManager.NotifyConnectionCreated(conn);
				conn.ConnectionString = _providerConfiguration.ConnectionString;
				if(conn.State != ConnectionState.Open)
				{
					conn.Open();
					GDAConnectionManager.NotifyConnectionOpened(conn);
				}
				conn.Close();
			}
		}

		/// <summary>
		/// Fecha a sessão.
		/// </summary>
		public void Close()
		{
			if(_state == GDASessionState.Open)
			{
				if(CurrentTransaction != null)
					CurrentTransaction.Dispose();
				_currentConnection.Close();
				_currentConnection.Dispose();
				CurrentTransaction = null;
				_currentConnection = null;
				_state = GDASessionState.Closed;
			}
		}

		/// <summary>
		/// Libera a atual transação.
		/// </summary>
		protected virtual void DisposeCurrentTransaction(bool disposing)
		{
			var transaction = CurrentTransaction;
			if(transaction != null)
				try
				{
					transaction.Dispose();
				}
				catch
				{
					if(disposing)
						throw;
				}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		public virtual void Dispose(bool disposing)
		{
			if(_state == GDASessionState.Open)
			{
				DisposeCurrentTransaction(disposing);
				if(_currentConnection != null)
				{
					try
					{
						if(_currentConnection != null)
							_currentConnection.Close();
					}
					catch
					{
					}
					try
					{
						if(_currentConnection != null)
							_currentConnection.Dispose();
					}
					catch
					{
					}
				}
				CurrentTransaction = null;
				_currentConnection = null;
				_state = GDASessionState.Closed;
			}
			_isDisposed = true;
		}

		/// <summary>
		/// Libera os dados da sessão.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
