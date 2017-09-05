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
using System.Reflection;
using MySql.Data.MySqlClient;

namespace GDA.Provider.MySql
{
	/// <summary>
	/// Provider da base de dados MySql.
	/// </summary>
	public class MySqlProvider : Provider
	{
		public MySqlProvider() : base("MySqlProvider", "MySql.Data.dll", "MySql.Data.MySqlClient.MySqlConnection", "MySql.Data.MySqlClient.MySqlDataAdapter", "MySql.Data.MySqlClient.MySqlCommand", "MySql.Data.MySqlClient.MySqlParameter", "?", true, "")
		{
			base.ExecuteCommandsOneAtATime = true;
			string uriString = Assembly.GetExecutingAssembly().EscapedCodeBase;
			Uri uri = new Uri(uriString);
			string path = uri.IsFile ? System.IO.Path.GetDirectoryName(uri.LocalPath) : null;
			providerAssembly = Assembly.LoadFrom(path + "\\MySql.Data.dll");
		}

		public override string SqlQueryReturnIdentity
		{
			get
			{
				return "select LAST_INSERT_ID()";
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
				return '`';
			}
		}

		/// <summary>
		/// Quote inicial da expressão.
		/// </summary>
		public override string QuoteExpressionBegin
		{
			get
			{
				return "`";
			}
		}

		/// <summary>
		/// Quote final da expressão.
		/// </summary>
		public override string QuoteExpressionEnd
		{
			get
			{
				return "`";
			}
		}

		public override string QuoteExpression(string word)
		{
			if(word != null)
			{
				var trimWord = word.TrimEnd(' ').TrimStart(' ');
				if(trimWord.Length > 3 && trimWord[0] == '[' && trimWord[trimWord.Length - 1] == ']')
					word = word.Substring(1, word.Length - 2);
			}
			return "`" + word + "`";
		}

		/// <summary>
		/// Identifica que o provider suporta o comando limit
		/// </summary>
		public override bool SupportSQLCommandLimit
		{
			get
			{
				return true;
			}
		}

		public override string SQLCommandLimit(List<Mapper> mapping, string sqlQuery, int startRecord, int size)
		{
			return sqlQuery + " limit " + startRecord.ToString() + "," + size.ToString();
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
			MySqlDbType result = MySqlDbType.Int32;
			if(type.IsEnum)
			{
				switch(Enum.GetUnderlyingType(type).Name)
				{
				case "Int16":
					return (long)MySqlDbType.Int16;
				case "UInt16":
					return (long)MySqlDbType.UInt16;
				case "Int32":
					return (long)MySqlDbType.Int32;
				case "UInt32":
					return (long)MySqlDbType.UInt32;
				case "Byte":
					return (long)MySqlDbType.Byte;
				}
			}
			if(type.Equals(typeof(int)))
				result = MySqlDbType.Int32;
			else if(type.Equals(typeof(uint)))
				result = MySqlDbType.UInt32;
			else if(type.Equals(typeof(short)))
				result = MySqlDbType.Int16;
			else if(type.Equals(typeof(ushort)))
				result = MySqlDbType.UInt16;
			else if(type.Equals(typeof(long)))
				result = MySqlDbType.Int64;
			else if(type.Equals(typeof(double)) || type.Equals(typeof(Single)))
				result = MySqlDbType.Double;
			else if(type.Equals(typeof(decimal)))
				result = MySqlDbType.Decimal;
			else if(type.Equals(typeof(DateTime)))
				result = MySqlDbType.Datetime;
			else if(type.Equals(typeof(bool)) || type.Equals(typeof(Byte)) || type.Equals(typeof(byte)))
				result = MySqlDbType.Byte;
			else if(type.Equals(typeof(string)))
				result = MySqlDbType.String;
			else if(type.Equals(typeof(byte[])) || type.Equals(typeof(Byte[])))
			{
				result = MySqlDbType.LongBlob;
			}
			return (long)result;
		}

		/// <summary>
		/// Esse método converte a string (extraída da tabelas do banco de dados) para o tipo do system
		/// correspondente.
		/// </summary>
		/// <param name="dbType">Nome do tipo usado no banco de dados.</param>
		/// <param name="isUnsigned">Valor boolean que identifica se o tipo é unsigned.</param>
		/// <returns>Valor do enumerator do tipo correspondente do banco de dados. O retorno é um número
		/// inteiro por causa que em alguns provider o enumerations não seguem o padrão do DbType definido
		/// no System.Data.</returns>
		public override long GetDbType(string dbType, bool isUnsigned)
		{
			switch(dbType)
			{
			case "byte":
				return (long)MySqlDbType.Byte;
			case "tinyint":
				return (long)MySqlDbType.Bit;
			case "smallint":
				if(isUnsigned)
					return (long)MySqlDbType.UInt16;
				else
					return (long)MySqlDbType.Int16;
			case "int":
				if(isUnsigned)
					return (long)MySqlDbType.UInt32;
				else
					return (long)MySqlDbType.Int32;
			case "bigint":
				if(isUnsigned)
					return (long)MySqlDbType.UInt64;
				else
					return (long)MySqlDbType.Int64;
			case "float":
				return (long)MySqlDbType.Float;
			case "datetime":
				return (long)MySqlDbType.Datetime;
			case "decimal":
			case "numeric":
				return (long)MySqlDbType.Decimal;
			case "char":
				return (long)MySqlDbType.String;
			case "varchar":
				return (long)MySqlDbType.VarChar;
			case "text":
				return (long)MySqlDbType.String;
			case "tinyblob":
				return (long)MySqlDbType.TinyBlob;
			case "blob":
				return (long)MySqlDbType.Blob;
			case "mediumblob":
				return (long)MySqlDbType.MediumBlob;
			case "longblob":
				return (long)MySqlDbType.LongBlob;
			case "enum":
				return (long)MySqlDbType.Enum;
			default:
				return No_DbType;
			}
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
			case (long)MySqlDbType.Bit:
				return typeof(bool);
			case (long)MySqlDbType.Byte:
				return typeof(byte);
			case (long)MySqlDbType.UInt16:
				return typeof(UInt16);
			case (long)MySqlDbType.Int16:
				return typeof(Int16);
			case (long)MySqlDbType.UInt32:
				return typeof(UInt32);
			case (long)MySqlDbType.Int32:
				return typeof(Int32);
			case (long)MySqlDbType.UInt64:
				return typeof(UInt64);
			case (long)MySqlDbType.Int64:
				return typeof(Int64);
			case (long)MySqlDbType.Float:
				return typeof(float);
			case (long)MySqlDbType.Datetime:
				return typeof(DateTime);
			case (long)MySqlDbType.Decimal:
				return typeof(decimal);
			case (long)MySqlDbType.String:
			case (long)MySqlDbType.VarChar:
				return typeof(string);
			case (long)MySqlDbType.TinyBlob:
			case (long)MySqlDbType.Blob:
			case (long)MySqlDbType.MediumBlob:
			case (long)MySqlDbType.LongBlob:
				return typeof(byte[]);
			case (long)MySqlDbType.Enum:
				return typeof(int);
			default:
				return typeof(object);
			}
		}
	}
}
