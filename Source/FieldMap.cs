﻿/* 
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

namespace GDA.Analysis
{
	/// <summary>
	/// Reprensenta as informação de um campo mapeado do banco de dados.
	/// </summary>
	public class FieldMap
	{
		private TableMap _table;

		private string _comment;

		private string _columnName;

		private bool _isReservedWord = false;

		private bool _isNullable = false;

		private bool _isAutoGenerated = false;

		private bool _isPrimaryKey = false;

		private string _foreignKeyTableName = null;

		private string _foreignKeyTableSchema = null;

		private string _foreignKeyColumnName = null;

		private string _foreignKeyConstraintName = null;

		private string _dbTypeName;

		private long _dbType;

		private int _size;

		private int _columnId;

		private bool _isReadOnly = false;

		/// <summary>
		/// Tabela que a coluna pertence.
		/// </summary>
		public TableMap Table
		{
			get
			{
				return _table;
			}
		}

		/// <summary>
		/// Tipo que a coluna representa.
		/// </summary>
		public Type MemberType
		{
			get
			{
				Type t = _table.ProviderConfiguration.Provider.GetSystemType(this.DbType);
				if(IsNullable && t.IsValueType)
					t = typeof(Nullable<>).MakeGenericType(t);
				return t;
			}
		}

		/// <summary>
		/// Comentário relacionado com a coluna.
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
		/// Nome da coluna.
		/// </summary>
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				_columnName = value;
			}
		}

		/// <summary>
		/// Identifica se o nome do coluna é um palavra reservada, e necessita
		/// de quote para representá-la.
		/// </summary>
		public bool IsReservedWord
		{
			get
			{
				return _isReservedWord;
			}
			set
			{
				_isReservedWord = value;
			}
		}

		/// <summary>
		/// Identifica se a coluna aceita valores nulos.
		/// </summary>
		public bool IsNullable
		{
			get
			{
				return _isNullable;
			}
			set
			{
				_isNullable = value;
			}
		}

		/// <summary>
		/// Identifica se o valor da coluna é gerado quando é feita
		/// a inserção no banco de dados.
		/// </summary>
		public bool IsAutoGenerated
		{
			get
			{
				return _isAutoGenerated;
			}
			set
			{
				_isAutoGenerated = value;
			}
		}

		/// <summary>
		/// Identifica se a coluna é um chave primária.
		/// </summary>
		public bool IsPrimaryKey
		{
			get
			{
				return _isPrimaryKey;
			}
			set
			{
				_isPrimaryKey = value;
			}
		}

		/// <summary>
		/// Nome da constraint da chave estrangeira.
		/// </summary>
		public string ForeignKeyConstraintName
		{
			get
			{
				return _foreignKeyConstraintName;
			}
			set
			{
				_foreignKeyConstraintName = value;
			}
		}

		/// <summary>
		/// Caso a coluna seja uma chave estrangeira, armazena
		/// o nome da tabela dessa chave.
		/// </summary>
		public string ForeignKeyTableName
		{
			get
			{
				return _foreignKeyTableName;
			}
			set
			{
				_foreignKeyTableName = value;
			}
		}

		/// <summary>
		/// Caso a coluna seja uma chave estrangeira, armazena
		/// o esquema da tabela da chave estrangeira.
		/// </summary>
		public string ForeignKeyTableSchema
		{
			get
			{
				return _foreignKeyTableSchema;
			}
			set
			{
				_foreignKeyTableSchema = value;
			}
		}

		/// <summary>
		/// Caso a coluna seja um chave estrangeira, armazena o 
		/// nome da coluna da tabela dessa chave.
		/// </summary>
		public string ForeignKeyColumnName
		{
			get
			{
				return _foreignKeyColumnName;
			}
			set
			{
				_foreignKeyColumnName = value;
			}
		}

		/// <summary>
		/// Nome do tipo no banco de dados.
		/// </summary>
		public string DbTypeName
		{
			get
			{
				return _dbTypeName;
			}
			set
			{
				_dbTypeName = value;
			}
		}

		/// <summary>
		/// Tipo da coluna no banco de dados.
		/// </summary>
		public long DbType
		{
			get
			{
				return _dbType;
			}
		}

		/// <summary>
		/// Tamanho da coluna.
		/// </summary>
		public int Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Identifica se a coluna é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			set
			{
				_isReadOnly = value;
			}
		}

		/// <summary>
		/// Identificador da coluna que é usado internamente no banco de dados.
		/// </summary>
		public int ColumnId
		{
			get
			{
				return _columnId;
			}
			set
			{
				_columnId = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="table">Tabela que a coluna está contida.</param>
		/// <param name="columnName"></param>
		public FieldMap(TableMap table, string columnName)
		{
			_table = table;
			_columnName = columnName;
		}

		/// <summary>
		/// Define o tipo da coluna no banco de dados.
		/// </summary>
		/// <param name="dbType"></param>
		public void SetDbType(long dbType)
		{
			_dbType = dbType;
		}

		/// <summary>
		/// Define o tipo da coluna no banco de dados.
		/// </summary>
		/// <param name="dbType"></param>
		public void SetDbType(DbType dbType)
		{
			_dbType = (long)dbType;
		}

		/// <summary>
		/// Define o tipo da coluna no banco de dados.
		/// </summary>
		/// <param name="dbType">Descrição do tipo.</param>
		/// <param name="isUnsigned">Identifica se o campo é unsigned.</param>
		public void SetDbType(string dbType, bool isUnsigned)
		{
			try
			{
				long dbt = Table.ProviderConfiguration.Provider.GetDbType(dbType, isUnsigned);
				if(dbt != Provider.Provider.No_DbType)
					_dbType = dbt;
			}
			catch(Exception fe)
			{
				throw new GDAException("Unsupported column type");
			}
		}

		public override string ToString()
		{
			return ColumnName;
		}
	}
}
