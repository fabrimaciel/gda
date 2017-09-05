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
using System.Text.RegularExpressions;
using System.Data;
using GDA.Caching;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Sql.InterpreterExpression;
using GDA.Collections;

namespace GDA.Sql
{
	/// <summary>
	/// Armazena os dados de uma consulta nativa no banco de dados.
	/// </summary>
	public class NativeQuery : BaseQuery, IGDAParameterContainer
	{
		/// <summary>
		/// Armanena o texto da consulta que será executada.
		/// </summary>
		private string _commandText;

		/// <summary>
		/// Tipo do comando que será executado.
		/// </summary>
		private CommandType _commandType = CommandType.Text;

		private int _commandTimeout = GDASession.DefaultCommandTimeout;

		/// <summary>
		/// Lista dos parametros relacionados com a consulta.
		/// </summary>
		private GDAParameterCollection _parameters = new GDAParameterCollection();

		/// <summary>
		/// Clausula de ordenação
		/// </summary>
		private string _orderClause;

		/// <summary>
		/// Nome das propriedades a serem recuperadas pela consulta.
		/// </summary>
		private string _selectProperties;

		/// <summary>
		/// Nome do campo chave da consulta.
		/// </summary>
		private string _keyFieldName;

		/// <summary>
		/// Armanena o texto da consulta que será executada.
		/// </summary>
		public string CommandText
		{
			get
			{
				return _commandText;
			}
			set
			{
				_commandText = value;
			}
		}

		/// <summary>
		/// Recupera a lista de parametros
		/// </summary>
		public List<GDAParameter> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Recupera e define a clausula ORDER BY.
		/// Não é usada a palavra chave ORDER BY na clausula.
		/// </summary>
		public string Order
		{
			get
			{
				return _orderClause;
			}
			set
			{
				_orderClause = value;
			}
		}

		/// <summary>
		/// Nome das propriedades a serem recuperadas pela consulta.
		/// </summary>
		public string SelectProperties
		{
			get
			{
				return _selectProperties;
			}
		}

		/// <summary>
		/// Tipo do comando que será executado.
		/// </summary>
		public CommandType CommandType
		{
			get
			{
				return _commandType;
			}
			set
			{
				_commandType = value;
			}
		}

		public int CommandTimeout
		{
			get
			{
				return _commandTimeout;
			}
			set
			{
				_commandTimeout = value;
			}
		}

		/// <summary>
		/// Nome do campo chave da consulta.
		/// </summary>
		public string KeyFieldName
		{
			get
			{
				return _keyFieldName;
			}
			set
			{
				_keyFieldName = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public NativeQuery()
		{
		}

		/// <summary>
		/// Inicializa a consulta já informando o texto.
		/// </summary>
		/// <param name="queryText"></param>
		public NativeQuery(string queryText)
		{
			_commandText = queryText;
		}

		/// <summary>
		/// Adiciona um novo parametro na consulta.
		/// </summary>
		/// <param name="parameter">Paramentro a ser adicionado.</param>
		/// <returns>Retorna a referencia da consulta aonde o parametro foi adicionado.</returns>
		public NativeQuery Add(GDAParameter parameter)
		{
			if(parameter != null)
			{
				var index = this._parameters.FindIndex(f => f.ParameterName == parameter.ParameterName);
				if(index >= 0)
					this._parameters.RemoveAt(index);
				this._parameters.Add(parameter);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um novo conjunto de parametros na consulta.
		/// </summary>
		/// <param name="parameters">Parametros a serem adicionados.</param>
		/// <returns>Retorna a referencia da consulta aonde os parametros foram adicionados.</returns>
		public NativeQuery Add(params GDAParameter[] parameters)
		{
			foreach (var i in parameters)
			{
				var index = this._parameters.FindIndex(f => f.ParameterName == i.ParameterName);
				if(index >= 0)
					this._parameters.RemoveAt(index);
				this._parameters.Add(i);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um novo conjunto de parametros na consulta.
		/// </summary>
		/// <param name="parameters">Parametros a serem adicionados.</param>
		/// <returns>Retorna a referencia da consulta aonde os parametros foram adicionados.</returns>
		public NativeQuery Add(IEnumerable<GDAParameter> parameters)
		{
			foreach (var i in parameters)
			{
				var index = this._parameters.FindIndex(f => f.ParameterName == i.ParameterName);
				if(index >= 0)
					this._parameters.RemoveAt(index);
				this._parameters.Add(i);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um parametro na consulta.
		/// </summary>
		/// <param name="dbtype">Tipo usado na base de dados</param>
		/// <param name="value">Valor do parametro.</param>
		public NativeQuery Add(DbType dbtype, object value)
		{
			return Add("", dbtype, value);
		}

		/// <summary>
		/// Adiciona um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="dbtype">Tipo usado na base de dados.</param>
		/// <param name="value">parameter value</param>
		public NativeQuery Add(string name, DbType dbtype, object value)
		{
			return Add(name, dbtype, 0, value);
		}

		/// <summary>
		/// Adiciona um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro</param>
		/// <param name="value">Valor do parametro.</param>
		/// <returns></returns>
		public NativeQuery Add(string name, object value)
		{
			return Add(new GDAParameter(name, value));
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public NativeQuery Add(DbType dbtype, int size, object value)
		{
			return Add("", dbtype, size, value);
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="name">parameter name</param>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public NativeQuery Add(string name, DbType dbtype, int size, object value)
		{
			GDAParameter p = new GDAParameter();
			p.ParameterName = name;
			p.DbType = dbtype;
			p.Size = size;
			p.Value = value;
			var index = this._parameters.FindIndex(f => f.ParameterName == p.ParameterName);
			if(index >= 0)
				this._parameters.RemoveAt(index);
			this._parameters.Add(p);
			return this;
		}

		/// <summary>
		/// Define os nomes das propriedades que serão recuperadas pela consulta.
		/// </summary>
		/// <param name="selectProperties"></param>
		/// <returns></returns>
		public NativeQuery Select(string selectProperties)
		{
			_selectProperties = selectProperties;
			return this;
		}

		/// <summary>
		/// Define a clausula ORDER BY.
		/// </summary>
		/// <param name="orderClause">Clausula ORDER BY.</param>
		/// <returns>Referência da consulta.</returns>
		public NativeQuery SetOrder(string orderClause)
		{
			this._orderClause = orderClause;
			return this;
		}

		/// <summary>
		/// Define o tipo de comando qeu será executado.
		/// </summary>
		/// <param name="commandType"></param>
		/// <returns></returns>
		public NativeQuery SetCommandType(CommandType commandType)
		{
			_commandType = commandType;
			return this;
		}

		/// <summary>
		/// Define o Timeout do comando.
		/// </summary>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public NativeQuery SetCommandTimeout(int commandTimeout)
		{
			_commandTimeout = commandTimeout;
			return this;
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count()
		{
			return Count(null);
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count(GDASession session)
		{
			return GDAOperations.Count(session, this);
		}

		/// <summary>
		/// Salta um número especifico de registros antes de recuperar os resultado.
		/// </summary>
		/// <param name="count">Quantidade de registros que serão saltados.</param>
		/// <returns></returns>
		public NativeQuery Skip(int count)
		{
			_skipCount = count;
			return this;
		}

		/// <summary>
		/// Define a quantidade de registro que serão recuperados.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public NativeQuery Take(int count)
		{
			_takeCount = count;
			return this;
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns>Quantidade de linhas afetadas.</returns>
		public int Execute()
		{
			return new DataAccess().ExecuteCommand(null, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray());
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns></returns>
		public object ExecuteScalar(GDASession session)
		{
			DataAccess dataAccess = null;
			if(session != null)
				dataAccess = new DataAccess(session.ProviderConfiguration);
			else
				dataAccess = new DataAccess();
			return dataAccess.ExecuteScalar(session, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray());
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns></returns>
		public object ExecuteScalar()
		{
			return new DataAccess().ExecuteScalar(null, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray());
		}

		/// <summary>
		/// Executa a consulta.
		/// </summary>
		/// <returns>Quantidade de linhas afetadas.</returns>
		public int Execute(GDASession session)
		{
			DataAccess dataAccess = null;
			if(session != null)
				dataAccess = new DataAccess(session.ProviderConfiguration);
			else
				dataAccess = new DataAccess();
			return dataAccess.ExecuteCommand(session, this.CommandType, this.CommandTimeout, this.CommandText, this.Parameters.ToArray());
		}

		/// <summary>
		/// Recupera o <see cref="ResultList<T>"/> do resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public ResultList<T> ToResultList<T>(int pageSize) where T : new()
		{
			return new ResultList<T>(this, pageSize);
		}

		/// <summary>
		/// Recupera o <see cref="GDA.Sql.ResultList"/> do resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public ResultList<T> ToResultList<T>(GDASession session, int pageSize) where T : new()
		{
			return new ResultList<T>(this, session, pageSize);
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
		public GDADataRecordCursor ToDataRecords()
		{
			return new DataAccess().LoadResult(null, this.CommandType, this.CommandTimeout, this.CommandText, this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging(this.SkipCount, this.TakeCount) : null, this.Parameters.ToArray());
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
		public GDADataRecordCursor ToDataRecords(GDASession session)
		{
			DataAccess dataAccess = null;
			if(session != null)
				dataAccess = new DataAccess(session.ProviderConfiguration);
			else
				dataAccess = new DataAccess();
			return dataAccess.LoadResult(session, this.CommandType, this.CommandTimeout, this.CommandText, this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging(this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray());
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction)
		{
			return BuildResultInfo2(provider, aggregationFunction, new Dictionary<string, Type>());
		}

		//// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="provider">Provider que será utilizado no build.</param>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <param name="classesDictionary">Dicionário com as classe que já foram processadas.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction, Dictionary<string, Type> classesDictionary)
		{
			if(string.IsNullOrEmpty(_commandText))
				throw new QueryException("Command text not informed.");
			var query = _commandText;
			query = query.TrimStart(' ', '\r', '\n', '\t');
			var match = Regex.Match(_commandText, "SELECT(?<selectpart>(\r|\n|\r\n|.*?)*?)FROM", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			if(!match.Success)
				throw new QueryException("Invalid aggregation function.\r\nNot found SELECT ... FROM for substitution in command");
			query = query.Replace(match.Groups["selectpart"].Value, " " + aggregationFunction + " ");
			var orderByIndex = query.LastIndexOf("ORDER BY");
			if(orderByIndex >= 0)
				query = query.Substring(0, orderByIndex);
			return new QueryReturnInfo(query, this.Parameters, new List<Mapper>());
		}

		/// <summary>
		/// Prepara o comando para ser executado.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="command"></param>
		public void PrepareCommand(GDASession session, IDbCommand command)
		{
			DataAccess dataAccess = null;
			if(session != null)
				dataAccess = new DataAccess(session.ProviderConfiguration);
			else
				dataAccess = new DataAccess();
			dataAccess.PrepareCommand(session, command, this.CommandText, this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging(this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray());
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo(string aggregationFunction)
		{
			return BuildResultInfo2(null, aggregationFunction);
		}

		/// <summary>
		/// Constrói o resultado com as informações que serão processadas.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo<T>(GDA.Interfaces.IProviderConfiguration configuration)
		{
			if(string.IsNullOrEmpty(_commandText))
				throw new QueryException("Command text not informed.");
			var query = _commandText;
			if(!string.IsNullOrEmpty(Order))
			{
				var orderByIndex = query.LastIndexOf("ORDER BY");
				if(orderByIndex >= 0)
					query = query.Substring(0, orderByIndex);
				query += " ORDER BY " + Order;
			}
			var mapping = MappingManager.GetMappers<T>(null, null);
			var selectProps = new List<Mapper>(mapping);
			if(!string.IsNullOrEmpty(_selectProperties) && _selectProperties != "*")
			{
				List<string> functions = new List<string>();
				Parser p = new Parser(new Lexer(_selectProperties));
				SelectPart sp = p.ExecuteSelectPart();
				selectProps = new List<Mapper>(sp.SelectionExpressions.Count);
				foreach (SelectExpression se in sp.SelectionExpressions)
				{
					if(se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
					{
						Column col = se.Column;
						foreach (Mapper mp in mapping)
						{
							if(string.Compare(se.ColumnName.Value.Text, mp.PropertyMapperName, true) == 0 && (mp.Direction == DirectionParameter.Input || mp.Direction == DirectionParameter.InputOutput || mp.Direction == DirectionParameter.OutputOnlyInsert))
							{
								if(!selectProps.Exists(f => f.PropertyMapperName == mp.PropertyMapperName))
									selectProps.Add(mp);
							}
						}
						if(col.Name == "*")
							throw new GDAException("Invalid expression {0}", se.ColumnName.Value.Text);
					}
					else if(se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function)
						throw new QueryException("NativeQuery not support function in select part");
				}
			}
			return new QueryReturnInfo(query, this.Parameters, selectProps);
		}

		/// <summary>
		/// Recupera uma consulta nomeada.
		/// </summary>
		/// <param name="queryName"></param>
		/// <returns></returns>
		public static NativeQuery GetNamedQuery(string queryName)
		{
			GDA.GDASettings.LoadConfiguration();
			if(string.IsNullOrEmpty(queryName))
				throw new ArgumentNullException("queryName");
			var mapping = Mapping.MappingData.GetSqlQuery(queryName);
			if(mapping == null)
				throw new QueryException("Query \"{0}\" not found.", queryName);
			if(!mapping.UseDatabaseSchema)
				throw new NotSupportedException("Query not use database schema.");
			var query = new NativeQuery(mapping.Query);
			var selectProperties = new List<string>();
			if(mapping.Return != null)
				foreach (var i in mapping.Return.ReturnProperties)
					selectProperties.Add(i.Name);
			foreach (var i in mapping.Parameters)
			{
				if(i.DefaultValue != null)
				{
					var pType = Type.GetType(i.TypeName, false);
					if(pType != null)
					{
						#if PocketPC
						#else
						var converter = System.ComponentModel.TypeDescriptor.GetConverter(pType);
						#endif
						try
						{
							#if PocketPC
							                             query.Add(i.Name, DataAccess.ConvertValue(i.DefaultValue, pType));
#else
							query.Add(i.Name, converter.ConvertFrom(i.DefaultValue));
							#endif
						}
						catch(Exception ex)
						{
							throw new QueryException(string.Format("Fail on convert parameter \"{0}\" to \"{1}\" in named query \"{2}\".", i.Name, pType.FullName, queryName), ex);
						}
						continue;
					}
				}
				query.Add(i.Name, null);
			}
			return query;
		}

		void IGDAParameterContainer.Add(GDAParameter parameter)
		{
			if(parameter == null)
				throw new ArgumentNullException("parameter");
			this._parameters.Add(parameter);
		}

		/// <summary>
		/// Tenta recupera o parametro pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		bool IGDAParameterContainer.TryGet(string name, out GDAParameter parameter)
		{
			return _parameters.TryGet(name, out parameter);
		}

		/// <summary>
		/// Verifica se existe algum parametro com o nome informado.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <returns></returns>
		bool IGDAParameterContainer.ContainsKey(string name)
		{
			return _parameters.ContainsKey(name);
		}

		/// <summary>
		/// Remove o parametro pelo nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool IGDAParameterContainer.Remove(string name)
		{
			return _parameters.Remove(name);
		}

		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator()
		{
			return this._parameters.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._parameters.GetEnumerator();
		}
	}
}
