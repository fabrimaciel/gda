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
using GDA.Sql.InterpreterExpression.Nodes;
using System.Reflection;
using System.Data;
using GDA.Caching;
using GDA.Collections;

namespace GDA.Sql
{
	/// <summary>
	/// Implementação de um Statement para Select.
	/// </summary>
	public class SelectStatement : BaseQuery
	{
		/// <summary>
		/// Tipo do comando que será executado.
		/// </summary>
		private CommandType _commandType = CommandType.Text;

		private int _commandTimeout = GDASession.DefaultCommandTimeout;

		/// <summary>
		/// Número de passos gasto para pecorrer todo o parser.
		/// </summary>
		private int _steps = 0;

		/// <summary>
		/// Parser relacionado.
		/// </summary>
		private readonly Parser _parser;

		/// <summary>
		/// Lista que armazenas as informações das tabela usadas no comando.
		/// </summary>
		private Dictionary<string, TableInfo> _tablesInfo = new Dictionary<string, TableInfo>();

		/// <summary>
		/// Lista das colunas encontrars no sql.
		/// </summary>
		private Dictionary<string, ColumnInfo> _columnsInfo = new Dictionary<string, ColumnInfo>();

		/// <summary>
		/// Lista das informações da tabelas.
		/// </summary>
		private List<TableInfo> _tablesInfoList;

		/// <summary>
		/// Lista das informações das colunas.
		/// </summary>
		private List<ColumnInfo> _columnsInfoList;

		/// <summary>
		/// Lista com as informações da variáveis.
		/// </summary>
		private List<VariableInfo> _variablesInfo = new List<VariableInfo>();

		/// <summary>
		/// Lista dos parametros relacionados com a consulta.
		/// </summary>
		private GDAParameterCollection parameters = new GDAParameterCollection();

		private TableInfo _firstTable = null;

		/// <summary>
		/// Instacia da classe de referencia usada.
		/// </summary>
		private ISelectStatementReferences _references;

		/// <summary>
		/// Nome do campo chave da consulta.
		/// </summary>
		private string _keyFieldName;

		/// <summary>
		/// Lista das tabelas encontradas no comando sql.
		/// </summary>
		public List<TableInfo> TablesInfo
		{
			get
			{
				return _tablesInfoList;
			}
		}

		/// <summary>
		/// Lista das informações das colunas.
		/// </summary>
		internal List<ColumnInfo> ColumnsInfo
		{
			get
			{
				return _columnsInfoList;
			}
		}

		/// <summary>
		/// Informações das variáveis processadas.
		/// </summary>
		internal List<VariableInfo> VariablesInfo
		{
			get
			{
				return _variablesInfo;
			}
		}

		/// <summary>
		/// Comando sql.
		/// </summary>
		public string Command
		{
			get
			{
				return _parser.Lex.Command;
			}
		}

		/// <summary>
		/// Número de passos gasto para pecorrer todo o parser.
		/// </summary>
		internal int Steps
		{
			get
			{
				return _steps;
			}
			set
			{
				_steps = value;
			}
		}

		/// <summary>
		/// Recupera a lista de parametros
		/// </summary>
		public List<GDAParameter> Parameters
		{
			get
			{
				return parameters;
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
		/// Nome da propriedade chave da consulta.
		/// </summary>
		public string KeyFieldName
		{
			get
			{
				if(string.IsNullOrEmpty(_keyFieldName))
				{
					TableInfo tInfo = null;
					string p = null;
					foreach (var i in TablesInfo)
					{
						p = _references.GetFirstKeyPropertyMapping(i.TypeInfo);
						if(!string.IsNullOrEmpty(p))
						{
							tInfo = i;
							break;
						}
					}
					if(p != null)
						_keyFieldName = (!string.IsNullOrEmpty(tInfo.TableAlias) ? tInfo.TableAlias + "." : null) + _references.GetPropertyMapping(tInfo.TypeInfo, p);
					else
					{
						foreach (var i in TablesInfo)
						{
							foreach (var j in i.Columns)
							{
								_keyFieldName = (!string.IsNullOrEmpty(i.TableAlias) ? i.TableAlias + "." : null) + _references.GetPropertyMapping(i.TypeInfo, j.ColumnName);
								break;
							}
							if(_keyFieldName != null)
								break;
						}
						if(_keyFieldName == null)
							foreach (var i in TablesInfo)
							{
								foreach (var j in _references.GetPropertiesMapping(i.TypeInfo))
								{
									_keyFieldName = (!string.IsNullOrEmpty(i.TableAlias) ? i.TableAlias + "." : null) + j.Column;
									break;
								}
								if(_keyFieldName != null)
									break;
							}
					}
				}
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
		/// <param name="references"></param>
		/// <param name="parser"></param>
		internal SelectStatement(ISelectStatementReferences references, Parser parser)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			_references = references;
			_parser = parser;
			foreach (Select select in parser.SelectParts)
			{
				GetSelectInfo(select);
			}
			foreach (KeyValuePair<string, ColumnInfo> ci in _columnsInfo)
			{
				if(ci.Value.TableNameOrTableAlias == null)
				{
					bool found = false;
					foreach (KeyValuePair<string, TableInfo> ti in _tablesInfo)
					{
						if(ti.Value.ExistsColumn(ci.Value))
						{
							if(!found)
							{
								ti.Value.AddColumn(ci.Value);
								found = true;
							}
							else
							{
								_firstTable.AddColumn(ci.Value);
							}
						}
					}
				}
				else
				{
					foreach (KeyValuePair<string, TableInfo> ti in _tablesInfo)
					{
						if(ci.Value.TableNameOrTableAlias == ti.Value.TableName.Name || ci.Value.TableNameOrTableAlias == ti.Value.TableAlias)
						{
							ti.Value.AddColumn(ci.Value);
							break;
						}
					}
				}
			}
			_tablesInfoList = new List<TableInfo>(_tablesInfo.Values);
			_columnsInfoList = new List<ColumnInfo>(_columnsInfo.Values);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parser"></param>
		internal SelectStatement(Parser parser) : this(NativeSelectStatementReferences.Instance, parser)
		{
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula WHERE.
		/// </summary>
		/// <param name="wp"></param>
		internal SelectStatement(ISelectStatementReferences references, WherePart wp)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			_references = references;
			foreach (SqlExpression se in wp.Expressions)
				ColumnName(se);
			_columnsInfoList = new List<ColumnInfo>(_columnsInfo.Values);
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula WHERE.
		/// </summary>
		/// <param name="wp"></param>
		internal SelectStatement(WherePart wp) : this(NativeSelectStatementReferences.Instance, wp)
		{
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula ORDER BY.
		/// </summary>
		/// <param name="references"></param>
		/// <param name="op"></param>
		internal SelectStatement(ISelectStatementReferences references, OrderByPart op)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			_references = references;
			foreach (OrderByExpression oe in op.OrderByExpressions)
				ColumnName(oe.Expression);
			_columnsInfoList = new List<ColumnInfo>(_columnsInfo.Values);
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula ORDER BY.
		/// </summary>
		/// <param name="op"></param>
		internal SelectStatement(OrderByPart op) : this(NativeSelectStatementReferences.Instance, op)
		{
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula GROUP BY.
		/// </summary>
		/// <param name="gbp"></param>
		internal SelectStatement(ISelectStatementReferences references, GroupByPart gbp)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			_references = references;
			foreach (SqlExpression sqle in gbp.Expressions)
				ColumnName(sqle);
			_columnsInfoList = new List<ColumnInfo>(_columnsInfo.Values);
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula GROUP BY.
		/// </summary>
		/// <param name="gbp"></param>
		internal SelectStatement(GroupByPart gbp) : this(NativeSelectStatementReferences.Instance, gbp)
		{
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula HAVING.
		/// </summary>
		/// <param name="references"></param>
		/// <param name="havingPart"></param>
		internal SelectStatement(ISelectStatementReferences references, HavingPart havingPart)
		{
			if(references == null)
				throw new ArgumentNullException("references");
			_references = references;
			foreach (SqlExpression se in havingPart.Expressions)
				ColumnName(se);
			_columnsInfoList = new List<ColumnInfo>(_columnsInfo.Values);
		}

		/// <summary>
		/// Recupera as informações contidas dentro da clausula HAVING.
		/// </summary>
		/// <param name="havingPart"></param>
		internal SelectStatement(HavingPart havingPart) : this(NativeSelectStatementReferences.Instance, havingPart)
		{
		}

		/// <summary>
		/// Recupera as informacoes do Select.
		/// </summary>
		/// <param name="select"></param>
		private void GetSelectInfo(Select select)
		{
			Steps++;
			foreach (SelectExpression se in select.SelectPart.SelectionExpressions)
			{
				ColumnName(se.ColumnName);
			}
			if(select.FromPart != null)
			{
				Steps++;
				foreach (TableExpression te in select.FromPart.TableExpressions)
				{
					if(te.SelectInfo != null)
					{
						GetSelectInfo(te.SelectInfo);
					}
					else
					{
						Steps++;
						TableInfo ti = new TableInfo(_references, te.TableName, te.TableAlias);
						if(_firstTable == null)
							_firstTable = ti;
						if(!_tablesInfo.ContainsKey(ti.ToString()))
						{
							_tablesInfo.Add(ti.ToString(), ti);
						}
					}
					if(te.OnExpressions != null)
					{
						Steps++;
						foreach (SqlExpression se in te.OnExpressions.Expressions)
							ColumnName(se);
					}
				}
			}
			if(select.WherePart != null)
			{
				Steps++;
				foreach (SqlExpression se in select.WherePart.Expressions)
					ColumnName(se);
			}
			if(select.GroupByPart != null)
			{
				Steps++;
				foreach (SqlExpression se in select.GroupByPart.Expressions)
					ColumnName(se);
			}
			if(select.HavingPart != null)
			{
				Steps++;
				foreach (SqlExpression se in select.HavingPart.Expressions)
					ColumnName(se);
			}
			if(select.OrderByPart != null)
			{
				Steps++;
				foreach (OrderByExpression oe in select.OrderByPart.OrderByExpressions)
					ColumnName(oe.Expression);
			}
		}

		/// <summary>
		/// Recupera o nome da coluna.
		/// </summary>
		/// <param name="se"></param>
		private void ColumnName(SqlExpression se)
		{
			Steps++;
			if(se is ContainerSqlExpression)
			{
				foreach (SqlExpression se1 in ((ContainerSqlExpression)se).Expressions)
					ColumnName(se1);
			}
			else if(se is Select)
			{
				GetSelectInfo((Select)se);
			}
			else if(se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
				AddColumnInfo(se);
			else if(se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Variable)
				AddVariableInfo(se);
			else if(se is SqlFunction)
			{
				foreach (List<SqlExpression> parameter in ((SqlFunction)se).Parameters)
				{
					foreach (SqlExpression pSe in parameter)
					{
						ColumnName(pSe);
					}
				}
			}
			else if(se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Boolean)
			{
				switch(se.Value.Token)
				{
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kAnd:
					if(se.Value.Text == "&&")
						se.Value.Text = "AND";
					break;
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kOr:
					if(se.Value.Text == "||")
						se.Value.Text = "OR";
					break;
				}
			}
		}

		/// <summary>
		/// Adiciona as informacoes da coluna.
		/// </summary>
		/// <param name="se"></param>
		private void AddColumnInfo(SqlExpression se)
		{
			ColumnInfo ci = new ColumnInfo(se.Value);
			string text = ci.ToString().ToLower();
			if(_columnsInfo.ContainsKey(text))
			{
				_columnsInfo[text].AddColumn(ci);
			}
			else
			{
				_columnsInfo.Add(text, ci);
			}
		}

		/// <summary>
		/// Adiciona as informações de uma variável.
		/// </summary>
		/// <param name="se"></param>
		private void AddVariableInfo(SqlExpression se)
		{
			var variableInfo = new VariableInfo(se.Value);
			var index = _variablesInfo.BinarySearch(variableInfo, VariableInfo.VariableInfoComparer.Instance);
			if(index < 0)
				_variablesInfo.Insert(~index, variableInfo);
		}

		/// <summary>
		/// Processa as informações das variáveis.
		/// </summary>
		/// <param name="parameterContainer"></param>
		private void ProcessVariablesInfo(IGDAParameterContainer parameterContainer)
		{
			foreach (var variableInfo in VariablesInfo)
			{
			}
		}

		/// <summary>
		/// Converte a consulta para string.
		/// </summary>
		/// <returns></returns>
		public string Parser()
		{
			return Parser2(null);
		}

		/// <summary>
		/// Passa a consulta para string.
		/// </summary>
		/// <param name="provider">Provider usado para gerar a consulta.</param>
		/// <returns></returns>
		public string Parser(GDA.Interfaces.IProvider provider)
		{
			return Parser2(provider, null);
		}

		/// <summary>
		/// Converte a consulta para string.
		/// </summary>
		/// <param name="parameterContainer">Container dos parametros que serão usado no processo.</param>
		/// <returns></returns>
		public string Parser2(IGDAParameterContainer parameterContainer)
		{
			var providerConfig = GDASettings.DefaultProviderConfiguration;
			var provider = providerConfig != null ? providerConfig.Provider : null;
			foreach (TableInfo ti in TablesInfo)
			{
				ti.RenameToMapper();
				foreach (ColumnInfo ci in ti.Columns)
					ci.RenameToMapper(provider);
			}
			foreach (var variableInfo in VariablesInfo)
				variableInfo.Replace(provider, parameterContainer, null);
			return (new ParserToSqlCommand(this._parser)).SqlCommand;
		}

		/// <summary>
		/// Passa a consulta para string.
		/// </summary>
		/// <param name="provider">Provider usado para gerar a consulta.</param>
		/// <param name="parameterContainer"></param>
		/// <returns></returns>
		public string Parser2(GDA.Interfaces.IProvider provider, IGDAParameterContainer parameterContainer)
		{
			foreach (TableInfo ti in TablesInfo)
			{
				ti.RenameToMapper();
				foreach (ColumnInfo ci in ti.Columns)
					ci.RenameToMapper(provider);
			}
			foreach (var variableInfo in VariablesInfo)
				variableInfo.Replace(provider, parameterContainer, null);
			return (new ParserToSqlCommand(this._parser, provider.QuoteExpressionBegin, provider.QuoteExpressionEnd)).SqlCommand;
		}

		public static bool operator ==(SelectStatement ss1, SelectStatement ss2)
		{
			return ss1.Steps == ss2.Steps;
		}

		public static bool operator !=(SelectStatement ss1, SelectStatement ss2)
		{
			return ss1.Steps != ss2.Steps;
		}

		/// <summary>
		/// Adiciona um novo parametro na consulta.
		/// </summary>
		/// <param name="parameter">Paramentro a ser adicionado.</param>
		/// <returns>Retorna a referencia da consulta aonde o parametro foi adicionado.</returns>
		public SelectStatement Add(GDAParameter parameter)
		{
			if(parameter != null)
			{
				var index = this.parameters.FindIndex(f => f.ParameterName == parameter.ParameterName);
				if(index >= 0)
					this.parameters.RemoveAt(index);
				this.parameters.Add(parameter);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um novo conjunto de parametros na consulta.
		/// </summary>
		/// <param name="parameters">Parametros a serem adicionados.</param>
		/// <returns>Retorna a referencia da consulta aonde os parametros foram adicionados.</returns>
		public SelectStatement Add(params GDAParameter[] parameters)
		{
			this.parameters.AddRange(parameters);
			return this;
		}

		/// <summary>
		/// Adiciona um novo conjunto de parametros na consulta.
		/// </summary>
		/// <param name="parameters">Parametros a serem adicionados.</param>
		/// <returns>Retorna a referencia da consulta aonde os parametros foram adicionados.</returns>
		public SelectStatement Add(IEnumerable<GDAParameter> parameters)
		{
			foreach (var i in parameters)
			{
				var index = this.parameters.FindIndex(f => f.ParameterName == i.ParameterName);
				if(index >= 0)
					this.parameters.RemoveAt(index);
				this.parameters.Add(i);
			}
			return this;
		}

		/// <summary>
		/// Adiciona um parametro na consulta.
		/// </summary>
		/// <param name="dbtype">Tipo usado na base de dados</param>
		/// <param name="value">Valor do parametro.</param>
		public SelectStatement Add(DbType dbtype, object value)
		{
			return Add("", dbtype, value);
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="dbtype">Tipo usado na base de dados.</param>
		/// <param name="value">parameter value</param>
		public SelectStatement Add(string name, DbType dbtype, object value)
		{
			return Add(name, dbtype, 0, value);
		}

		public SelectStatement Add(string name, object value)
		{
			return Add(new GDAParameter(name, value));
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public SelectStatement Add(DbType dbtype, int size, object value)
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
		public SelectStatement Add(string name, DbType dbtype, int size, object value)
		{
			GDAParameter p = new GDAParameter();
			p.ParameterName = name;
			p.DbType = dbtype;
			p.Size = size;
			p.Value = value;
			var index = this.parameters.FindIndex(f => f.ParameterName == p.ParameterName);
			if(index >= 0)
				this.parameters.RemoveAt(index);
			this.parameters.Add(p);
			return this;
		}

		/// <summary>
		/// Define o tipo de comando qeu será executado.
		/// </summary>
		/// <param name="commandType"></param>
		/// <returns></returns>
		public SelectStatement SetCommandType(CommandType commandType)
		{
			_commandType = commandType;
			return this;
		}

		/// <summary>
		/// Define o Timeout do comando.
		/// </summary>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public SelectStatement SetCommandTimeout(int commandTimeout)
		{
			_commandTimeout = commandTimeout;
			return this;
		}

		/// <summary>
		/// Salta um número especifico de registros antes de recuperar os resultado.
		/// </summary>
		/// <param name="count">Quantidade de registros que serão saltados.</param>
		/// <returns></returns>
		public SelectStatement Skip(int count)
		{
			_skipCount = count;
			return this;
		}

		/// <summary>
		/// Define a quantidade de registro que serão recuperados.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public SelectStatement Take(int count)
		{
			_takeCount = count;
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
		/// Recupera o <see cref="ResultList"/> do resultado da consulta.
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
			return new DataAccess().LoadResult(null, this.CommandType, this.CommandTimeout, Parser2(GDA.GDASettings.DefaultProviderConfiguration.Provider, parameters), this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging(this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray());
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
		public GDADataRecordCursor ToDataRecords(GDASession session)
		{
			return new DataAccess().LoadResult(session, this.CommandType, this.CommandTimeout, Parser2(session.ProviderConfiguration.Provider, parameters), this.TakeCount > 0 || this.SkipCount > 0 ? new InfoPaging(this.SkipCount, this.TakeCount) {
				KeyFieldName = this.KeyFieldName
			} : null, this.Parameters.ToArray());
		}

		/// <summary>
		/// Constrói o resultado com as informações que serão processadas.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo<T>(GDA.Interfaces.IProviderConfiguration configuration)
		{
			return new QueryReturnInfo(Parser2(configuration.Provider, parameters), parameters, MappingManager.GetMappers<T>(null, null));
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo(string aggregationFunction)
		{
			return BuildResultInfo2(GDA.GDASettings.DefaultProviderConfiguration.Provider, aggregationFunction);
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction)
		{
			return BuildResultInfo2(provider, aggregationFunction, new Dictionary<string, Type>());
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="provider">Provider que será utilizado no build.</param>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <param name="classesDictionary">Dicionário com as classe que já foram processadas.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction, Dictionary<string, Type> classesDictionary)
		{
			var query = Parser2(provider, parameters);
			query = query.TrimStart(' ', '\r', '\n', '\t');
			if(query.IndexOf("SELECT ", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				var fromPos = query.IndexOf(" FROM ");
				if(fromPos > 0)
					query = query.Substring(0, "SELECT ".Length) + aggregationFunction + query.Substring(fromPos);
			}
			var orderByIndex = query.LastIndexOf("ORDER BY");
			if(orderByIndex >= 0)
				query = query.Substring(0, orderByIndex);
			return new QueryReturnInfo(query, parameters, new List<Mapper>());
		}

		/// <summary>
		/// Recupera uma consulta nomeada.
		/// </summary>
		/// <param name="queryName"></param>
		/// <returns></returns>
		public static SelectStatement GetNamedQuery(string queryName)
		{
			if(string.IsNullOrEmpty(queryName))
				throw new ArgumentNullException("queryName");
			var mapping = Mapping.MappingData.GetSqlQuery(queryName);
			if(mapping == null)
				throw new QueryException("Query \"{0}\" not found.", queryName);
			if(mapping.UseDatabaseSchema)
				throw new NotSupportedException("Query use database schema.");
			var query = SqlBuilder.P(mapping.Query);
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
						#if !PocketPC
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
	}
}
