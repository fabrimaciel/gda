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
using System.Reflection;
using Oracle.DataAccess.Client;

namespace GDA.Provider.Oracle
{
	/// <summary>
	/// Implementação do provedor do Oracle.
	/// </summary>
	public class OracleProvider : Provider, IParameterConverter
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public OracleProvider() : base("OracleProvider", "Oracle.DataAccess.dll", "Oracle.DataAccess.Client.OracleConnection", "Oracle.DataAccess.Client.OracleDataAdapter", "Oracle.DataAccess.Client.OracleCommand", "Oracle.DataAccess.Client.OracleParameter", ":", true, "")
		{
			base.ExecuteCommandsOneAtATime = true;
			string uriString = Assembly.GetExecutingAssembly().EscapedCodeBase;
			Uri uri = new Uri(uriString);
			string path = uri.IsFile ? System.IO.Path.GetDirectoryName(uri.LocalPath) : null;
			providerAssembly = Assembly.LoadFrom(path + "\\Oracle.DataAccess.dll");
		}

		public override string SqlQueryReturnIdentity
		{
			get
			{
				return "SELECT {0}.currval FROM dual;";
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
				return '"';
			}
		}

		/// <summary>
		/// Quote inicial da expressão.
		/// </summary>
		public override string QuoteExpressionBegin
		{
			get
			{
				return "\"";
			}
		}

		/// <summary>
		/// Quote final da expressão.
		/// </summary>
		public override string QuoteExpressionEnd
		{
			get
			{
				return "\"";
			}
		}

		public override string QuoteExpression(string word)
		{
			string[] parts = word.Split('.');
			string result = "";
			for(int i = 0; i < parts.Length; i++)
				result += "\"" + parts[i] + "\"" + ((i + 1) != parts.Length ? "." : "");
			return result;
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

		public override string ParameterPrefix
		{
			get
			{
				return ":";
			}
		}

		/// <summary>
		/// Esse método com base no nome da tabela e na coluna identidade da tabela 
		/// recupera a consulta SQL que irá recupera o valor da chave identidade gerado
		/// para o registro recentemente inserido.
		/// </summary>
		/// <param name="tableName">Nome da tabela onde o registro será inserido.</param>
		/// <param name="identityColumnName">Nome da coluna identidade da tabela.</param>
		/// <returns>The modified sql string which also retrieves the identity value</returns>
		public override string GetIdentitySelect(string tableName, string identityColumnName)
		{
			string seqName = (tableName + "_seq").ToUpper();
			return String.Format(SqlQueryReturnIdentity, seqName);
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
			OracleDbType result = OracleDbType.Int32;
			if(type.Equals(typeof(byte)) || type.Equals(typeof(Byte)))
				result = OracleDbType.Byte;
			else if(type.Equals(typeof(short)) || type.Equals(typeof(Int16)))
				result = OracleDbType.Int16;
			else if(type.Equals(typeof(int)) || type.Equals(typeof(Int32)) || type.IsEnum)
				result = OracleDbType.Int32;
			else if(type.Equals(typeof(long)) || type.Equals(typeof(Int64)))
				result = OracleDbType.Int64;
			else if(type.Equals(typeof(float)) || type.Equals(typeof(Single)))
				result = OracleDbType.Double;
			else if(type.Equals(typeof(double)))
				result = OracleDbType.Double;
			else if(type.Equals(typeof(decimal)) || type.Equals(typeof(Decimal)))
				result = OracleDbType.Decimal;
			else if(type.Equals(typeof(DateTime)))
				result = OracleDbType.Date;
			else if(type.Equals(typeof(bool)))
				result = OracleDbType.Byte;
			else if(type.Equals(typeof(string)))
				result = OracleDbType.Varchar2;
			else if(type.Equals(typeof(TimeSpan)))
				result = OracleDbType.IntervalDS;
			else if(type.Equals(typeof(byte[])))
				result = OracleDbType.Blob;
			else
				throw new GDAException("Unsupported Property Type");
			return (long)result;
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
			case (long)OracleDbType.Byte:
				return typeof(bool);
			case (long)OracleDbType.Int16:
				return typeof(Int16);
			case (long)OracleDbType.Int32:
				return typeof(Int32);
			case (long)OracleDbType.Int64:
			case (long)OracleDbType.Long:
				return typeof(Int64);
			case (long)OracleDbType.Single:
				return typeof(float);
			case (long)OracleDbType.Double:
				return typeof(double);
			case (long)OracleDbType.Date:
			case (long)OracleDbType.TimeStamp:
			case (long)OracleDbType.TimeStampLTZ:
			case (long)OracleDbType.TimeStampTZ:
				return typeof(DateTime);
			case (long)OracleDbType.Decimal:
				return typeof(decimal);
			case (long)OracleDbType.NVarchar2:
			case (long)OracleDbType.Varchar2:
			case (long)OracleDbType.NChar:
			case (long)OracleDbType.Char:
			case (long)OracleDbType.Clob:
			case (long)OracleDbType.NClob:
			case (long)OracleDbType.XmlType:
				return typeof(string);
			case (long)OracleDbType.Raw:
			case (long)OracleDbType.LongRaw:
			case (long)OracleDbType.Blob:
			case (long)OracleDbType.BFile:
				return typeof(byte[]);
			case (long)OracleDbType.IntervalDS:
				return typeof(TimeSpan);
			default:
				return typeof(object);
			}
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
			string tmp = dbType.ToLower();
			switch(tmp)
			{
			case "bfile":
				return (long)OracleDbType.BFile;
			case "blob":
				return (long)OracleDbType.Blob;
			case "byte":
				return (long)OracleDbType.Byte;
			case "char":
				return (long)OracleDbType.Char;
			case "clob":
				return (long)OracleDbType.Clob;
			case "date":
			case "datetime":
				return (long)OracleDbType.Date;
			case "decimal":
			case "number":
				return (long)OracleDbType.Decimal;
			case "double":
			case "float":
				return (long)OracleDbType.Double;
			case "int16":
				return (long)OracleDbType.Int16;
			case "int32":
				return (long)OracleDbType.Int32;
			case "int64":
				return (long)OracleDbType.Int64;
			case "intervalds":
			case "intervaldaytosecond":
			case "interval day to second":
				return (long)OracleDbType.IntervalDS;
			case "intervalym":
			case "intervalyeartomonth":
			case "interval year to month":
				return (long)OracleDbType.IntervalYM;
			case "long":
				return (long)OracleDbType.Long;
			case "longraw":
			case "long raw":
				return (long)OracleDbType.LongRaw;
			case "nchar":
				return (long)OracleDbType.NChar;
			case "nclob":
				return (long)OracleDbType.NClob;
			case "nvarchar":
			case "nvarchar2":
				return (long)OracleDbType.NVarchar2;
			case "raw":
				return (long)OracleDbType.Raw;
			case "cursor":
			case "ref cursor":
			case "refcursor":
				return (long)OracleDbType.RefCursor;
			case "single":
				return (long)OracleDbType.Single;
			case "timestamp":
				return (long)OracleDbType.TimeStamp;
			case "timestamplocal":
			case "timestamp with local time zone":
			case "timestampltz":
				return (long)OracleDbType.TimeStampLTZ;
			case "timestampwithtz":
			case "timestamp with time zone":
			case "timestamptz":
				return (long)OracleDbType.TimeStampTZ;
			case "varchar":
			case "varchar2":
				return (long)OracleDbType.Varchar2;
			case "xmltype":
				return (long)OracleDbType.XmlType;
			case "rowid":
				return (long)OracleDbType.Varchar2;
			default:
				return No_DbType;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="value"></param>
		public override void SetParameterValue(System.Data.IDbDataParameter parameter, object value)
		{
			if(value != null && value.GetType().IsEnum)
				value = (int)value;
			base.SetParameterValue(parameter, value);
		}

		/// <summary>
		/// Converte o parametro do GDA.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public System.Data.IDbDataParameter Convert(GDAParameter parameter)
		{
			var p = this.CreateParameter();
			p.DbType = parameter.DbType;
			if(p.Direction != parameter.Direction)
				p.Direction = parameter.Direction;
			p.Size = parameter.Size;
			try
			{
				if(parameter.ParameterName[0] == '?')
					p.ParameterName = ParameterPrefix + parameter.ParameterName.Substring(1) + ParameterSuffix;
				else
					p.ParameterName = parameter.ParameterName;
			}
			catch(Exception ex)
			{
				throw new GDAException("Error on convert parameter name '" + parameter.ParameterName + "'.", ex);
			}
			SetParameterValue(p, parameter.Value == null ? DBNull.Value : parameter.Value);
			return p;
		}
	}
}
