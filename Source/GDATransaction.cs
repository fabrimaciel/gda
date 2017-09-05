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
	public class GDATransaction : GDASession
	{
		private IDbTransaction _transaction;

		private IsolationLevel _isolationLevel;

		/// <summary>
		/// Evento acionado quando a transação receber um commit.
		/// </summary>
		public event EventHandler Commited;

		/// <summary>
		/// Evento acionado quando a transação receber um rollback.
		/// </summary>
		public event EventHandler RolledBack;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GDATransaction() : this(IsolationLevel.Unspecified)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isolationLevel">Nível de isolação da transaction.</param>
		public GDATransaction(IsolationLevel isolationLevel)
		{
			_isolationLevel = isolationLevel;
		}

		/// <summary>
		/// Cria uma instancia com o provedor de configuração.
		/// </summary>
		/// <param name="providerConfiguration">Instancia do provedor de configuração.</param>
		/// <param name="isolationLevel"></param>
		public GDATransaction(IProviderConfiguration providerConfiguration, IsolationLevel isolationLevel) : base(providerConfiguration)
		{
			_isolationLevel = isolationLevel;
		}

		/// <summary>
		/// Cria uma instancia com o provedor de configuração.
		/// </summary>
		/// <param name="providerConfiguration">Instancia do provedor de configuração.</param>
		public GDATransaction(IProviderConfiguration providerConfiguration) : this(providerConfiguration, IsolationLevel.Unspecified)
		{
		}

		/// <summary>
		/// Recupera a instancia da atual transação.
		/// </summary>
		internal override IDbTransaction CurrentTransaction
		{
			get
			{
				BeginTransaction();
				return _transaction;
			}
			set
			{
				_transaction = null;
			}
		}

		/// <summary>
		/// Cria uma nova instancia de comando.
		/// </summary>
		/// <returns></returns>
		public override IDbCommand CreateCommand()
		{
			BeginTransaction();
			var command = base.CreateCommand();
			if(command.Transaction == null && _transaction != null)
				command.Transaction = _transaction;
			return command;
		}

		/// <summary>
		/// Inicia a transação.
		/// </summary>
		public void BeginTransaction()
		{
			CheckDisposed();
			if(_transaction == null)
			{
				CreateConnection();
				if(_isolationLevel == IsolationLevel.Unspecified)
					_transaction = CurrentConnection.BeginTransaction();
				else
					_transaction = CurrentConnection.BeginTransaction(_isolationLevel);
			}
		}

		/// <summary>
		/// Commit na transação do banco de dados.
		/// </summary>
		public void Commit()
		{
			CheckDisposed();
			if(_transaction != null && _transaction.Connection != null && _transaction.Connection.State != ConnectionState.Closed)
			{
				_transaction.Commit();
				if(Commited != null)
					Commited(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Rollback na transação do bando de dados para um estado pedente.
		/// </summary>
		public void Rollback()
		{
			CheckDisposed();
			if(_transaction != null && _transaction.Connection != null && _transaction.Connection.State != ConnectionState.Closed)
			{
				_transaction.Rollback();
				if(RolledBack != null)
					RolledBack(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Libera a atual transação.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void DisposeCurrentTransaction(bool disposing)
		{
			if(_transaction != null)
				try
				{
					_transaction.Dispose();
				}
				catch
				{
					if(disposing)
						throw;
				}
		}
	}
}
