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
using System.Xml;

namespace GDA.Mapping
{
	/// <summary>
	/// Armazena os dados do mapeamento de uma consulta sql.
	/// </summary>
	public class SqlQueryMapping : ElementMapping
	{
		private string _name;

		private bool _useDatabaseSchema = true;

		private List<SqlQueryParameterMapping> _parameters = new List<SqlQueryParameterMapping>();

		private SqlQueryReturnMapping _return;

		private string _query;

		/// <summary>
		/// Nome da consulta.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Verifica se é usado o esquema do banco de dados para fazer a consulta.
		/// </summary>
		public bool UseDatabaseSchema
		{
			get
			{
				return _useDatabaseSchema;
			}
			set
			{
				_useDatabaseSchema = value;
			}
		}

		/// <summary>
		/// Parametros usados na consulta.
		/// </summary>
		public List<SqlQueryParameterMapping> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Informações sobre o retorno da consulta.
		/// </summary>
		public SqlQueryReturnMapping Return
		{
			get
			{
				return _return;
			}
			set
			{
				_return = value;
			}
		}

		/// <summary>
		/// SQL da consulta.
		/// </summary>
		public string Query
		{
			get
			{
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento da consulta.
		/// </summary>
		/// <param name="element"></param>
		public SqlQueryMapping(XmlElement element)
		{
			Name = GetAttributeString(element, "name", true);
			var boolVal = false;
			string val = GetAttributeString(element, "use-database-schema", "true");
			#if PocketPC
			            if (GDA.Helper.GDAHelper.TryParse(val, out boolVal))
#else
			if(bool.TryParse(val, out boolVal))
				#endif
				UseDatabaseSchema = boolVal;
			else
				UseDatabaseSchema = true;
			var parameters = FirstOrDefault<XmlElement>(element.GetElementsByTagName("parameters"));
			if(parameters != null)
				foreach (XmlElement i in parameters.GetElementsByTagName("param"))
				{
					var pm = new SqlQueryParameterMapping(i);
					if(!Parameters.Exists(f => f.Name == pm.Name))
						Parameters.Add(pm);
				}
			var returnInfo = FirstOrDefault<XmlElement>(element.GetElementsByTagName("return"));
			if(returnInfo != null)
				Return = new SqlQueryReturnMapping(returnInfo);
			var commandText = FirstOrDefault<XmlElement>(element.GetElementsByTagName("commandText"));
			if(commandText != null || !string.IsNullOrEmpty(commandText.InnerText))
				Query = commandText.InnerText.TrimStart('\n', '\t').TrimEnd('\n', '\t');
		}

		/// <summary>
		/// Constrói uma instancia do mapeamento da consulta.
		/// </summary>
		/// <param name="name">Nome da consulta.</param>
		/// <param name="useDatabaseSchema">Identifica se sera usado o esquema do banco de dados para fazer a consulta.</param>
		/// <param name="returnMapping">Informações sobre o retorno da consulta.</param>
		/// <param name="query">SQL da consulta.</param>
		/// <param name="parameters">Parametros usados na consulta.</param>
		public SqlQueryMapping(string name, bool useDatabaseSchema, SqlQueryReturnMapping returnMapping, string query, IEnumerable<SqlQueryParameterMapping> parameters)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			this.Name = name;
			this.UseDatabaseSchema = useDatabaseSchema;
			this.Return = returnMapping;
			this.Query = query;
			if(parameters != null)
				foreach (var i in parameters)
					if(!Parameters.Exists(f => f.Name == i.Name))
						Parameters.Add(i);
		}
	}
}
