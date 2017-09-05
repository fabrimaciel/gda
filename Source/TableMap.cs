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
using GDA.Interfaces;

namespace GDA.Analysis
{
	/// <summary>
	/// Representa os dados de uma tabela mapeada do banco de dados.
	/// </summary>
	public class TableMap
	{
		private IProviderConfiguration _provider;

		private FieldList _fields;

		private string _tableName = null;

		private string _tableSchema = null;

		private FieldMap _identityMap = null;

		private int _tableId;

		private bool _isView;

		private string _comment;

		/// <summary>
		/// Instancia do provedor de configuração.
		/// </summary>
		public IProviderConfiguration ProviderConfiguration
		{
			get
			{
				return _provider;
			}
		}

		/// <summary>
		/// Lista de campos da tabela.
		/// </summary>
		public FieldList Fields
		{
			get
			{
				return _fields;
			}
		}

		/// <summary>
		/// Nome da tabela sem quote.
		/// </summary>
		public string TableName
		{
			get
			{
				return _tableName;
			}
			set
			{
				_tableName = value;
			}
		}

		/// <summary>
		/// Esquema onde a tabela está inserida.
		/// </summary>
		public string TableSchema
		{
			get
			{
				return _tableSchema;
			}
			set
			{
				_tableSchema = value;
			}
		}

		/// <summary>
		/// Caso o nome da tabela seja uma palavra reservada, 
		/// armazena o quote para ser usado.
		/// </summary>
		public string QuotedTableName
		{
			get
			{
				return ProviderConfiguration.Provider.QuoteExpression(_tableName);
			}
		}

		/// <summary>
		/// Coluna identidade da tabela.
		/// </summary>
		public FieldMap IdentityMap
		{
			get
			{
				return _identityMap;
			}
			set
			{
				_identityMap = value;
			}
		}

		/// <summary>
		/// Identificador da tabela.
		/// </summary>
		public int TableId
		{
			get
			{
				return _tableId;
			}
			set
			{
				_tableId = value;
			}
		}

		/// <summary>
		/// Identifica se a tabela é um view.
		/// </summary>
		public bool IsView
		{
			get
			{
				return _isView;
			}
			set
			{
				_isView = value;
			}
		}

		/// <summary>
		/// Comentario relacionado com a tabela.
		/// </summary>
		public string Comment
		{
			get
			{
				return _comment;
			}
			set
			{
				_comment = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="tableName">Nome da tabela.</param>
		public TableMap(IProviderConfiguration providerConfiguration, string tableName)
		{
			_tableName = tableName;
			_provider = providerConfiguration;
			_fields = new FieldList();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="tableName">Nome da tabela.</param>
		/// <param name="providerConfiguration"></param>
		/// <param name="tableSchema">Esquema onde a tabela está inserida.</param>
		public TableMap(IProviderConfiguration providerConfiguration, string tableName, string tableSchema) : this(providerConfiguration, tableName)
		{
			_tableSchema = tableSchema;
		}

		/// <summary>
		/// Recupera instancia do <see cref="FieldMap"/> com base no nome da coluna.
		/// </summary>
		/// <param name="columnName">Nome da coluna</param>
		/// <returns>FieldMap relacionado com a coluna.</returns>
		public FieldMap GetFieldMapFromColumn(string columnName)
		{
			return _fields.FindColumn(columnName);
		}

		public override string ToString()
		{
			return TableName;
		}
	}
}
