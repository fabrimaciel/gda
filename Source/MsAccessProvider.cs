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
using System.Data.OleDb;

namespace GDA.Provider.MsAccess
{
	/// <summary>
	/// Provider da base de dados Microsoft Access.
	/// </summary>
	public class MsAccessProvider : Provider
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public MsAccessProvider() : base("MsAccess", typeof(OleDbConnection), typeof(OleDbDataAdapter), typeof(OleDbCommand), typeof(OleDbParameter), "@", "select @@IDENTITY;", true)
		{
			base.ExecuteCommandsOneAtATime = true;
		}

		/// <summary>
		/// Quote inicial da expressão.
		/// </summary>
		public override string QuoteExpressionBegin
		{
			get
			{
				return "[";
			}
		}

		/// <summary>
		/// Quote final da expressão.
		/// </summary>
		public override string QuoteExpressionEnd
		{
			get
			{
				return "]";
			}
		}

		/// <summary>
		/// Métodos responsável por tratar o comando sql e adapta-lo para executar
		/// estruturas de paginação.
		/// </summary>
		/// <param name="sqlQuery">Clausula SQL que será tratada.</param>
		/// <param name="startRecord">Apartir da qual deve começar o resultado.</param>
		/// <param name="size">Quantidade de registros por resultado.</param>
		/// <returns>Consulta sql tratada</returns>
		public override string SQLCommandLimit(List<Mapper> mapping, string sqlQuery, int startRecord, int size)
		{
			string order = "";
			string[] fieldOrderBy = null;
			string[] directionFieldOrderBy = null;
			if(sqlQuery.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) < 0)
			{
				if(mapping == null)
					throw new GDAException("Mapping not found");
				Mapper field = mapping.Find(delegate(Mapper m) {
					return (m.ParameterType == PersistenceParameterType.IdentityKey || m.ParameterType == PersistenceParameterType.Key);
				});
				if(field == null)
				{
					field = mapping[0];
				}
				sqlQuery += " ORDER BY " + QuoteExpression(field.Name);
			}
			int orderBy = sqlQuery.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase);
			if(orderBy >= 0)
			{
				order = sqlQuery.Substring(orderBy + "ORDER BY".Length, sqlQuery.Length - (orderBy + "ORDER BY".Length));
				fieldOrderBy = order.Split(',');
				directionFieldOrderBy = new string[fieldOrderBy.Length];
				for(int i = 0; i < fieldOrderBy.Length; i++)
				{
					int posDi = fieldOrderBy[i].IndexOf(" DESC", 0, StringComparison.OrdinalIgnoreCase);
					if(posDi >= 0)
					{
						directionFieldOrderBy[i] = "DESC";
						fieldOrderBy[i] = fieldOrderBy[i].Substring(0, posDi).Trim();
					}
					else
					{
						directionFieldOrderBy[i] = "ASC";
						posDi = fieldOrderBy[i].IndexOf(" ASC", 0, StringComparison.OrdinalIgnoreCase);
						if(posDi >= 0)
							fieldOrderBy[i] = fieldOrderBy[i].Substring(0, posDi).Trim();
						else
							fieldOrderBy[i] = fieldOrderBy[i].Trim();
					}
				}
			}
			int pos = sqlQuery.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
			sqlQuery = String.Format("SELECT TOP {0}", (startRecord + size).ToString()) + sqlQuery.Substring(pos + "SELECT".Length);
			sqlQuery = String.Format("SELECT * FROM (SELECT TOP {0} * FROM ({1}) AS inner_tbl", size, sqlQuery);
			if(orderBy >= 0)
			{
				sqlQuery += " ORDER BY ";
				for(int i = 0; i < fieldOrderBy.Length; i++)
				{
					sqlQuery += fieldOrderBy[i] + " " + ((directionFieldOrderBy[i] == "ASC") ? "DESC" : "ASC");
					if((i + 1) != fieldOrderBy.Length)
						sqlQuery += ", ";
				}
			}
			sqlQuery += ") AS outer_tbl";
			if(orderBy >= 0)
			{
				sqlQuery += " ORDER BY ";
				for(int i = 0; i < fieldOrderBy.Length; i++)
				{
					sqlQuery += fieldOrderBy[i] + " " + directionFieldOrderBy[i];
					if((i + 1) != fieldOrderBy.Length)
						sqlQuery += ", ";
				}
			}
			return sqlQuery;
		}

		/// <summary>
		/// Identifica que o provider suporta paginação usando o comando sql.
		/// </summary>
		public override bool SupportSQLCommandLimit
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Obtem o caracter usado para delimitar os parametros de string.
		/// </summary>
		/// <returns>The quote character.</returns>
		public override char QuoteCharacter
		{
			get
			{
				return '\'';
			}
		}

		/// <summary>
		/// Obtem uma container (Quote) para permitir que colunas com nomes especiais
		/// sejam inseridas na consulta.
		/// </summary>
		/// <param name="word">Nome da coluna ou do paramenrto.</param>
		/// <returns>Expressão com a formatação adequada.</returns>
		public override string QuoteExpression(string word)
		{
			return "[" + word + "]";
		}

		/// <summary>
		/// Obtem um número inteiro que corresponde ao tipo da base de dados que representa o tipo
		/// informado. O valor de retorno pode ser convertido em um tipo válido (enum value) para 
		/// o atual provider. Esse method é chamado para traduzir os tipos do sistema para os tipos
		/// do banco de dados que não são convertidos explicitamento.
		/// </summary>
		/// <param name="type">Tipo do sistema.</param>
		/// <returns>Tipo correspondente da base de dados.</returns>
		public override long GetDbType(Type type)
		{
			if(type.IsEnum)
			{
				switch(Enum.GetUnderlyingType(type).Name)
				{
				case "Int16":
					return (long)OleDbType.SmallInt;
				case "UInt16":
					return (long)OleDbType.SmallInt;
				case "Int32":
					return (long)OleDbType.Integer;
				case "UInt32":
					return (long)OleDbType.Integer;
				case "Byte":
					return (long)OleDbType.Char;
				}
			}
			if(type == typeof(string))
				return (long)OleDbType.VarChar;
			else if(type == typeof(int) || type == typeof(uint))
				return (long)OleDbType.Integer;
			else if(type == typeof(short) || type == typeof(ushort))
				return (long)OleDbType.SmallInt;
			else if(type == typeof(bool))
				return (long)OleDbType.Boolean;
			else if(type == typeof(DateTime))
				return (long)OleDbType.Date;
			else if(type == typeof(char))
				return (long)OleDbType.Char;
			else if(type == typeof(decimal))
				return (long)OleDbType.Decimal;
			else if(type == typeof(double))
				return (long)OleDbType.Double;
			else if(type == typeof(float))
				return (long)OleDbType.Single;
			else if(type == typeof(byte[]))
				return (long)OleDbType.Binary;
			else if(type == typeof(Guid))
				return (long)OleDbType.Guid;
			else if(type == typeof(byte))
				return (long)OleDbType.Char;
			return (long)OleDbType.Empty;
		}

		/// <summary>
		/// Esse método retorna o tipo do sistema correspodente ao tipo specifico indicado no long.
		/// A implementação padrão não retorna exception, mas sim null.
		/// </summary>
		/// <param name="dbType">Tipo especifico do provider.</param>
		/// <returns>Tipo do sistema correspondente.</returns>
		public override Type GetSystemType(long dbType)
		{
			switch(dbType)
			{
			case (long)OleDbType.Boolean:
				return typeof(bool);
			case (long)OleDbType.Char:
				return typeof(byte);
			case (long)OleDbType.SmallInt:
				return typeof(Int16);
			case (long)OleDbType.Integer:
				return typeof(Int32);
			case (long)OleDbType.Single:
				return typeof(float);
			case (long)OleDbType.Date:
				return typeof(DateTime);
			case (long)OleDbType.Decimal:
				return typeof(decimal);
			case (long)OleDbType.VarChar:
				return typeof(string);
			case (long)OleDbType.Binary:
				return typeof(byte[]);
			default:
				return typeof(object);
			}
		}
	}
}
