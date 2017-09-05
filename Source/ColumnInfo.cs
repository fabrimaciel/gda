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

namespace GDA.Sql
{
	/// <summary>
	/// Armazena as informações de uma coluna.
	/// </summary>
	public class ColumnInfo
	{
		/// <summary>
		/// Expressão associada com a coluna.
		/// </summary>
		private Expression _expression;

		/// <summary>
		/// Lista de referencias do nome da coluna.
		/// </summary>
		private List<ColumnInfo> _columnExpressionsRef;

		/// <summary>
		/// Nome ou apelido real da tabela relacionado, quer dizer contendo os caracteres especiais.
		/// </summary>
		private string _realTableNameOrTablesAlias;

		/// <summary>
		/// Nome ou apelido antigo da tabela relacionada.
		/// </summary>
		private string _oldTableNameOrTableAlias;

		/// <summary>
		/// Nome ou apelido da tabela relacionada.
		/// </summary>
		private string _tableNameOrTableAlias;

		/// <summary>
		/// Nome da coluna.
		/// </summary>
		private string _columnName;

		/// <summary>
		/// Mapeamento da coluna.
		/// </summary>
		/// <summary>
		/// Nome da coluna no Banco de Dados.
		/// </summary>
		private string _dbColumnName;

		/// <summary>
		/// Nome original da coluna.
		/// </summary>
		private string _originalColumnName;

		/// <summary>
		/// Mapeamento da coluna.
		/// </summary>
		/// <summary>
		/// Nome ou apelido da tabela relacionada.
		/// </summary>
		public string TableNameOrTableAlias
		{
			get
			{
				return _tableNameOrTableAlias;
			}
			internal set
			{
				_tableNameOrTableAlias = value;
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
		}

		/// <summary>
		/// Nome da coluna no BD.
		/// </summary>
		public string DBColumnName
		{
			get
			{
				return _dbColumnName;
			}
			set
			{
				_dbColumnName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="columnNameExpression">Referência da primeira expressão que armazena o nome da coluna.</param>
		internal ColumnInfo(Expression columnNameExpression)
		{
			_expression = columnNameExpression;
			string[] info = columnNameExpression.Text.Split('.');
			_columnName = info[info.Length - 1];
			_originalColumnName = _columnName;
			if(info.Length > 1)
			{
				_tableNameOrTableAlias = columnNameExpression.Text.Substring(0, columnNameExpression.Text.Length - (_columnName.Length + 1));
				_realTableNameOrTablesAlias = _tableNameOrTableAlias;
				if(_tableNameOrTableAlias[0] == '`' || _tableNameOrTableAlias[0] == '[')
					_tableNameOrTableAlias = _tableNameOrTableAlias.Substring(1, _tableNameOrTableAlias.Length - 2);
				_oldTableNameOrTableAlias = _tableNameOrTableAlias;
			}
			if(_columnName[0] == '`' || _columnName[0] == '[')
				_columnName = _columnName.Substring(1, _columnName.Length - 2);
		}

		/// <summary>
		/// Adiciona mais uma referência do nome da coluna.
		/// </summary>
		/// <param name="column"></param>
		internal void AddColumn(ColumnInfo column)
		{
			if(_columnExpressionsRef == null)
				_columnExpressionsRef = new List<ColumnInfo>();
			_columnExpressionsRef.Add(column);
		}

		/// <summary>
		/// Renomeia a coluna.
		/// </summary>
		/// <param name="newColumnName">Nome nome da coluna.</param>
		internal void Rename(string newColumnName)
		{
			if(newColumnName == null)
				throw new ArgumentNullException("newColumnName");
			if(_tableNameOrTableAlias != _oldTableNameOrTableAlias)
			{
				if(_realTableNameOrTablesAlias != null)
				{
					string newName = _realTableNameOrTablesAlias.Replace(_oldTableNameOrTableAlias, _tableNameOrTableAlias);
					_expression.Text = _expression.Text.Replace(_realTableNameOrTablesAlias + ".", newName + ".");
					_realTableNameOrTablesAlias = newName;
				}
				else
				{
					_expression.Text = _tableNameOrTableAlias + "." + _expression.Text;
					_realTableNameOrTablesAlias = _tableNameOrTableAlias;
				}
				_oldTableNameOrTableAlias = _tableNameOrTableAlias;
			}
			_expression.Text = _expression.Text.Replace(_originalColumnName, newColumnName);
			_originalColumnName = newColumnName;
			_columnName = newColumnName;
			if(_columnExpressionsRef != null)
				foreach (ColumnInfo col in _columnExpressionsRef)
				{
					col.TableNameOrTableAlias = this.TableNameOrTableAlias;
					col.Rename(newColumnName);
				}
		}

		/// <summary>
		/// Renomea a coluna para o nome do mapeamento.
		/// </summary>
		/// <param name="provider">Provider que será usado para renomear.</param>
		internal void RenameToMapper(GDA.Interfaces.IProvider provider)
		{
			if(!string.IsNullOrEmpty(DBColumnName))
				Rename(provider != null ? provider.QuoteExpression(DBColumnName) : DBColumnName);
		}

		public override string ToString()
		{
			return (_tableNameOrTableAlias != null ? _tableNameOrTableAlias + "." : "") + _columnName;
		}
	}
}
