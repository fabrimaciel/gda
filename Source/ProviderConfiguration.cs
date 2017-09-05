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
using GDA.Analysis;

namespace GDA.Provider
{
	public class ProviderConfiguration : GDA.Interfaces.IProviderConfiguration
	{
		/// <summary>
		/// Provider relacionado.
		/// </summary>
		private GDA.Interfaces.IProvider _Provider;

		/// <summary>
		/// ConnectionString do provider.
		/// </summary>
		private string _ConnectionString;

		/// <summary>
		/// Dialeto que ser� usado.
		/// </summary>
		private string _dialect;

		/// <summary>
		/// Identificador �nico do provider.
		/// </summary>
		private Guid _providerIdentifier = Guid.NewGuid();

		/// <summary>
		/// Evento acionado quando uma conex�o � criada pelo provedor.
		/// </summary>
		public event CreateConnectionEvent ConnectionCreated;

		/// <summary>
		/// Identificador �nico do provider.
		/// </summary>
		public Guid ProviderIdentifier
		{
			get
			{
				return _providerIdentifier;
			}
		}

		/// <summary>
		/// Gets And Sets o provider relacionado.
		/// </summary>
		public virtual GDA.Interfaces.IProvider Provider
		{
			get
			{
				return _Provider;
			}
			set
			{
				_Provider = value;
			}
		}

		/// <summary>
		/// Gets And Sets o connectionString do provider.
		/// </summary>
		public virtual string ConnectionString
		{
			get
			{
				return _ConnectionString;
			}
			set
			{
				_ConnectionString = value;
			}
		}

		/// <summary>
		/// Gets And Sets do dialeto que ser� usado pelo provider.
		/// </summary>
		public virtual string Dialect
		{
			get
			{
				return _dialect;
			}
			set
			{
				_dialect = value;
			}
		}

		/// <summary>
		/// Construtor Padr�o.
		/// </summary>
		/// <param name="connectionString">ConnectionString a ser utilizado pelo provider.</param>
		/// <param name="provider">Provider a ser tratado.</param>
		public ProviderConfiguration(string connectionString, GDA.Interfaces.IProvider provider)
		{
			_ConnectionString = connectionString;
			_Provider = provider;
		}

		/// <summary>
		/// Cria uma conex�o com banco de dados j� com o connection string configurado.
		/// </summary>
		/// <returns></returns>
		public virtual IDbConnection CreateConnection()
		{
			IDbConnection connection = Provider.CreateConnection();
			connection.ConnectionString = ConnectionString;
			if(ConnectionCreated != null)
				ConnectionCreated(this, new CreateConnectionEventArgs(connection));
			return connection;
		}

		/// <summary>
		/// Analyzer relacionado com o provider.
		/// </summary>
		public virtual DatabaseAnalyzer GetDatabaseAnalyzer()
		{
			throw new NotImplementedException();
		}
	}
}
