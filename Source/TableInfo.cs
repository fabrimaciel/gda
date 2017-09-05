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
using GDA.Sql.InterpreterExpression;
using System.Reflection;
using GDA.Caching;

namespace GDA.Sql
{
	/// <summary>
	/// Armazena as informações de um tabela.
	/// </summary>
	public class TableInfo
	{
		/// <summary>
		/// Expressão que armazena o nome da tabela.
		/// </summary>
		private InterpreterExpression.Nodes.TableNameExpression _tableNameExpression;

		/// <summary>
		/// Expressão que armazena o apelido da tabela.
		/// </summary>
		private Expression _tableAlias;

		/// <summary>
		/// Nome da tabela.
		/// </summary>
		private TableName _tableName;

		/// <summary>
		/// Lista das colunas relacionadas com a tabela.
		/// </summary>
		private List<ColumnInfo> _columns = new List<ColumnInfo>();

		/// <summary>
		/// Tipo da classe que a tabela está relacionada.
		/// </summary>
		private Type _classTypeRelated;

		private ISelectStatementReferences _references;

		/// <summary>
		/// Armazena as informações do tipo relacionado com o nome da tabela.
		/// </summary>
		private Mapping.TypeInfo _typeInfo;

		/// <summary>
		/// Tipo da classe que a tabela está relacionada.
		/// </summary>
		public Type ClassTypeRelated
		{
			get
			{
				return _classTypeRelated;
			}
			set
			{
				_classTypeRelated = value;
			}
		}

		/// <summary>
		/// Lista das colunas relacionadas com a tabela.
		/// </summary>
		public List<ColumnInfo> Columns
		{
			get
			{
				return _columns;
			}
		}

		/// <summary>
		/// Nome da tabela.
		/// </summary>
		public TableName TableName
		{
			get
			{
				return _tableName;
			}
		}

		/// <summary>
		/// Apelido da tabela.
		/// </summary>
		public string TableAlias
		{
			get
			{
				if(_tableAlias != null)
					return _tableAlias.Text;
				else
					return null;
			}
		}

		/// <summary>
		/// Informações do tipo relacionado com a tabela.
		/// </summary>
		public Mapping.TypeInfo TypeInfo
		{
			get
			{
				if(_typeInfo == null)
					_typeInfo = _references.GetTypeInfo(this);
				return _typeInfo;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="tableName">Referência da expressão que armazena o nome da tabela.</param>
		/// <param name="tableAlias">Expressão que armazena o apelido da tabela.</param>
		internal TableInfo(ISelectStatementReferences references, InterpreterExpression.Nodes.TableNameExpression tableName, Expression tableAlias)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			_references = references;
			_tableNameExpression = tableName;
			_tableName = new TableName {
				Name = tableName.Name,
				Schema = tableName.Schema
			};
			_tableAlias = tableAlias;
		}

		/// <summary>
		/// Verifica se a tabela contem a coluna mencionada.
		/// </summary>
		/// <param name="columnName">Nome da coluna.</param>
		/// <returns></returns>
		internal bool ExistsColumn(string columnName)
		{
			return !string.IsNullOrEmpty(_references.GetPropertyMapping(TypeInfo, columnName));
		}

		/// <summary>
		/// Verifica se a tabela contém a coluna mencionada.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		internal bool ExistsColumn(ColumnInfo column)
		{
			if(!string.IsNullOrEmpty(column.TableNameOrTableAlias) && (string.Compare(column.TableNameOrTableAlias, this.TableAlias, true) != 0 || string.Compare(column.TableNameOrTableAlias, this.TableName.Name, true) != 0))
			{
				return false;
			}
			return ExistsColumn(column.ColumnName);
		}

		/// <summary>
		/// Adiciona um coluna na tabela.
		/// </summary>
		/// <param name="ci"></param>
		internal void AddColumn(ColumnInfo ci)
		{
			ci.DBColumnName = _references.GetPropertyMapping(TypeInfo, ci.ColumnName);
			_columns.Add(ci);
		}

		/// <summary>
		/// Renomeia as entrada para o mapeamento.
		/// </summary>
		internal void RenameToMapper()
		{
			var name = _references.GetTableName(TypeInfo);
			if(name == null)
				throw new GDAException("Table name not found to type \"" + TypeInfo.FullnameWithAssembly + "\"");
			_tableName = name;
			_tableNameExpression.Name = name.Name;
			_tableNameExpression.Schema = name.Schema;
		}

		public override string ToString()
		{
			return TableName.ToString() + (_tableAlias != null ? " AS " + _tableAlias.Text : "");
		}
	}
}
