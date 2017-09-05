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

namespace GDA
{
	public delegate void DebugTraceDelegate (object sender, string message);
	public delegate void PersistenceObjectOperation<T> (object sender, ref T model);
	/// <summary>
	/// Argumentos usados para a geração de uma chave identidade.
	/// </summary>
	public class GenerateKeyArgs : EventArgs
	{
		private GDASession _session;

		private Type _modelType;

		private Mapper _identityProperty;

		private string _tableName;

		private string _columnName;

		private uint _keyValue;

		private bool _cancel;

		private object _model;

		private string _schemaName;

		/// <summary>
		/// Sessão de conexão que está sendo usada.
		/// </summary>
		public GDASession Session
		{
			get
			{
				return _session;
			}
		}

		/// <summary>
		/// Tipo da model para onde será gerado o valor da chave.
		/// </summary>
		public Type ModelType
		{
			get
			{
				return _modelType;
			}
		}

		/// <summary>
		/// Informações da proprieade mapeada.
		/// </summary>
		public Mapper IdentityProperty
		{
			get
			{
				return _identityProperty;
			}
		}

		/// <summary>
		/// Nome da tabela para onde a chave será gerada.
		/// </summary>
		public string TableName
		{
			get
			{
				return _tableName;
			}
		}

		/// <summary>
		/// Nome do esquema da tabela.
		/// </summary>
		public string SchemaName
		{
			get
			{
				return _schemaName;
			}
		}

		/// <summary>
		/// Nome da coluna identidade da tabela.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
		}

		/// <summary>
		/// Valor da chave gerada.
		/// </summary>
		public uint KeyValue
		{
			get
			{
				return _keyValue;
			}
			set
			{
				_keyValue = value;
			}
		}

		/// <summary>
		/// Determina que é para cancelar a operação
		/// e utilizar outros meios para recuperação da chave.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}

		/// <summary>
		/// Instancia com os dados que serão salvos.
		/// </summary>
		public object Model
		{
			get
			{
				return _model;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="modelType"></param>
		/// <param name="identityProperty"></param>
		/// <param name="schemaName"></param>
		/// <param name="tableName"></param>
		/// <param name="columnName"></param>
		/// <param name="model"></param>
		public GenerateKeyArgs(GDASession session, Type modelType, Mapper identityProperty, string schemaName, string tableName, string columnName, object model)
		{
			_session = session;
			_modelType = modelType;
			_identityProperty = identityProperty;
			_schemaName = schemaName;
			_tableName = tableName;
			_columnName = columnName;
			_model = model;
		}
	}
	/// <summary>
	/// Representa o método que será usado na geração de uma chave identidade
	/// para um linha que será inserida em uma tabela.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void GenerateKeyHandler (object sender, GenerateKeyArgs args);
	/// <summary>
	/// Argumentos usados para a carga do provedor de configuração.
	/// </summary>
	public class ProviderConfigurationLoadArgs : EventArgs
	{
		private Interfaces.IProviderConfiguration _provideCronfiguration;

		/// <summary>
		/// Instancia do provedor de configuração que está sendo carregado.
		/// </summary>
		public Interfaces.IProviderConfiguration ProvideConfiguration
		{
			get
			{
				return _provideCronfiguration;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="providerConfiguration"></param>
		public ProviderConfigurationLoadArgs(Interfaces.IProviderConfiguration providerConfiguration)
		{
			_provideCronfiguration = providerConfiguration;
		}
	}
	/// <summary>
	/// Representa o método que será acionado quando um provedor
	/// de configuração for carregado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void ProviderConfigurationLoadHandler (object sender, ProviderConfigurationLoadArgs args);
}
