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
using System.Data;
using GDA.Sql.InterpreterExpression;
using GDA.Provider;
using GDA.Interfaces;
using GDA.Sql.InterpreterExpression.Nodes;
using GDA.Caching;
using GDA.Collections;

namespace GDA.Sql
{
	/// <summary>
	/// Representa uma query que limita os registros afetados
	/// por um comando de execlusão ou alguma consulta SELECT.
	/// </summary>
	public class Query : BaseQuery, IEquatable<Query>, IGDAParameterContainer
	{
		/// <summary>
		/// Lista de parametros.
		/// </summary>
		private GDAParameterCollection _parameters = new GDAParameterCollection(3);

		private ConditionalContainer _conditional;

		private ConditionalContainer _having;

		private string orderClause;

		private string groupByClause;

		private bool _useDistinct = false;

		private string _mainAlias;

		/// <summary>
		/// Joins relacionados com a consulta.
		/// </summary>
		private List<JoinInfo> _joins = new List<JoinInfo>();

		/// <summary>
		/// Nome das propriedades a serem recuperadas pela consulta.
		/// </summary>
		private string _selectProperties;

		/// <summary>
		/// Alias da tabela principal.
		/// </summary>
		public string MainAlias
		{
			get
			{
				return _mainAlias;
			}
			set
			{
				_mainAlias = value;
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
		/// Joins relacionados com a consulta.
		/// </summary>
		public List<JoinInfo> Join
		{
			get
			{
				return _joins;
			}
		}

		/// <summary>
		/// Condicional para a parte do WHERE.
		/// </summary>
		protected ConditionalContainer WhereConditional
		{
			get
			{
				if(_conditional == null)
					_conditional = new ConditionalContainer("");
				return _conditional;
			}
			set
			{
				if(value == null)
					_conditional = new ConditionalContainer("");
				else
					_conditional = value;
			}
		}

		/// <summary>
		/// Condicional para a parte do HAVING.
		/// </summary>
		protected ConditionalContainer HavingConditional
		{
			get
			{
				if(_having == null)
					_having = new ConditionalContainer("");
				return _having;
			}
			set
			{
				if(value == null)
					_having = new ConditionalContainer("");
				else
					_having = value;
			}
		}

		/// <summary>
		/// Cria uma nova consulta
		/// </summary>
		public Query() : this(null)
		{
		}

		/// <summary>
		/// Cria uma nova consulta com base na clausula WHERE.
		/// </summary>
		/// <param name="whereClause">Clausula WHERE.</param>
		public Query(string whereClause) : this(whereClause, null, null)
		{
		}

		/// <summary>
		/// Cria uma nova consulta com base na clausula WHERE e na clausula ORDER BY.
		/// </summary>
		/// <param name="whereClause">Clausula WHERE</param>
		/// <param name="orderClause">Clausula ORDER BY</param>
		public Query(string whereClause, string orderClause) : this(whereClause, orderClause, null)
		{
		}

		/// <summary>
		/// Cria uma nova consulta com base na clausula WHERE e na lista de parametros passados
		/// </summary>
		/// <param name="whereClause">Clausula WHERE</param>
		/// <param name="parameters">Lista de parametros</param>
		public Query(string whereClause, IEnumerable<GDAParameter> parameters) : this(whereClause, null, parameters)
		{
		}

		/// <summary>
		/// Cria uma nova consulta com base na clausula WHERE, na clausula ORDER BY
		/// e na lista de parametros.
		/// </summary>
		/// <param name="whereClause">WHERE clause</param>
		/// <param name="orderClause">ORDER BY clause</param>
		/// <param name="parameters">list of parameters</param>
		public Query(string whereClause, string orderClause, IEnumerable<GDAParameter> parameters)
		{
			Where = whereClause;
			this.orderClause = orderClause;
			if(parameters != null)
				foreach (var i in parameters)
				{
					var index = this._parameters.FindIndex(f => f.ParameterName == i.ParameterName);
					if(index >= 0)
						this._parameters.RemoveAt(index);
					this._parameters.Add(i);
				}
		}

		/// <summary>
		/// Define o apelido para a tabela principal.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public Query Alias(string alias)
		{
			_mainAlias = alias;
			return this;
		}

		/// <summary>
		/// Adiciona um Distinct na consulta.
		/// </summary>
		/// <returns></returns>
		public Query Distinct()
		{
			_useDistinct = true;
			return this;
		}

		/// <summary>
		/// Remove o distinct da consulta se existir.
		/// </summary>
		/// <returns></returns>
		public Query RemoveDistinct()
		{
			_useDistinct = false;
			return this;
		}

		/// <summary>
		/// Salta um número especifico de registros antes de recuperar os resultado.
		/// </summary>
		/// <param name="count">Quantidade de registros que serão saltados.</param>
		/// <returns></returns>
		public Query Skip(int count)
		{
			_skipCount = count;
			return this;
		}

		/// <summary>
		/// Define a quantidade de registro que serão recuperados.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public Query Take(int count)
		{
			_takeCount = count;
			return this;
		}

		/// <summary>
		/// Recupera e define a clausula WHERE.
		/// Não é usada a palavra chave WHERE na clausula
		/// </summary>
		public string Where
		{
			get
			{
				return WhereConditional.ToString();
			}
			set
			{
				WhereConditional = new ConditionalContainer(value) {
					ParameterContainer = this
				};
			}
		}

		/// <summary>
		/// Clausula condicional WHERE.
		/// </summary>
		public QueryWhereClause WhereClause
		{
			get
			{
				return new QueryWhereClause(this, WhereConditional);
			}
		}

		/// <summary>
		/// Cria um container de contição para a consulta.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ConditionalContainer CreateWhereClause(string expression)
		{
			Where = expression;
			return _conditional;
		}

		/// <summary>
		/// Define a classe de consulta.
		/// </summary>
		/// <param name="clause"></param>
		/// <returns></returns>
		public Query SetWhereClause(ConditionalContainer clause)
		{
			if(clause == null)
				Where = "";
			else
			{
				_conditional = clause;
				_conditional.ParameterContainer = this;
			}
			return this;
		}

		/// <summary>
		/// Having.
		/// </summary>
		public string HavingText
		{
			get
			{
				return HavingConditional.ToString();
			}
			set
			{
				HavingConditional = new ConditionalContainer(value) {
					ParameterContainer = this
				};
			}
		}

		/// <summary>
		/// Clausula condicional HAVING.
		/// </summary>
		public QueryWhereClause HavingClause
		{
			get
			{
				return new QueryWhereClause(this, HavingConditional);
			}
		}

		/// <summary>
		/// Cria um container de contição para o Having.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public ConditionalContainer Having(string expression)
		{
			HavingText = expression;
			return _having;
		}

		/// <summary>
		/// Define a classe do HAVING.
		/// </summary>
		/// <param name="clause"></param>
		/// <returns></returns>
		public Query SetHavingClause(ConditionalContainer clause)
		{
			if(clause == null)
				HavingText = "";
			else
			{
				_having = clause;
				_having.ParameterContainer = this;
			}
			return this;
		}

		/// <summary>
		/// Recupera e define a clausula ORDER BY.
		/// Não é usada a palavra chave ORDER BY na clausula.
		/// </summary>
		public string Order
		{
			get
			{
				return orderClause;
			}
			set
			{
				orderClause = value;
			}
		}

		/// <summary>
		/// Nome das propriedades que serão agrupadas.
		/// </summary>
		public string GroupByClause
		{
			get
			{
				return groupByClause;
			}
			set
			{
				groupByClause = value;
			}
		}

		/// <summary>
		/// Define os nomes das propriedades que serão recuperadas pela consulta.
		/// </summary>
		/// <param name="selectProperties"></param>
		/// <returns></returns>
		public Query Select(string selectProperties)
		{
			_selectProperties = selectProperties;
			return this;
		}

		#if CLS_3_5
		
        /// <summary>
        /// Recupera o seletor de propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertiesSelector"></param>
        /// <returns></returns>
        public QueryPropertySelector<T> PropertySelector<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector) where T : new()
        {
            return new QueryPropertySelector<T>(this).Add(propertiesSelector);
        }

        /// <summary>
        /// Seleciona o primeiro registro para a consulta e filtra as colunas 
        /// do tipo que irão para o resultado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">Sessão que será usada na execução do comando.</param>
        /// <param name="propertiesSelector">Seletor das propriedades que deverão ser retornadas.</param>
        /// <returns></returns>
        public GDADataRecord SelectFirst<T>(GDASession session, params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            if (propertiesSelector != null)
            {
                var properties = new List<string>();
                foreach (var i in propertiesSelector)
                {
                    if (i == null) continue;
                    var property = i.GetMember() as System.Reflection.PropertyInfo;
                    if (property != null)
                        properties.Add(property.Name);
                }

                if (properties.Count > 0)
                    Select(string.Join(", ", properties.ToArray()));
            }

            using (var enumerator = ToDataRecords<T>(session).GetEnumerator())
            {
                if (enumerator.MoveNext())
                    return enumerator.Current;
            }

            return null;
        }

        /// <summary>
        /// Seleciona o primeiro registro para a consulta e filtra as colunas 
        /// do tipo que irão para o resultado.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertiesSelector">Seletor das propriedades que deverão ser retornadas.</param>
        /// <returns></returns>
        public GDADataRecord SelectFirst<T>(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            GDASession session = null;
            return SelectFirst<T>(session, propertiesSelector);
        }

        /// <summary>
        /// Executa a consulta selecionar o valor da coluna
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session">Sessão que será usada na execução do comando.</param>
        /// <param name="selector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue SelectFirstValue<T>(GDASession session, System.Linq.Expressions.Expression<Func<T, object>> selector)
        {
            if (selector == null)
                throw new ArgumentException("selector");

            var property = selector.GetMember() as System.Reflection.PropertyInfo;

            Select(property.Name);

            using (var enumerator = ToDataRecords<T>(session).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var record = enumerator.Current;
                    if (record[0].ValueExists)
                        return record[0];
                }
            }

            return new GDAPropertyValue(null, false);
        }

        /// <summary>
        /// Executa a consulta selecionar o valor da coluna
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue SelectFirstValue<T>(System.Linq.Expressions.Expression<Func<T, object>> selector)
        {
            return SelectFirstValue<T>(null, selector);
        }
#endif
		/// <summary>
		/// Adiciona o join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <returns></returns>
		public Query InnerJoin<ClassJoin>()
		{
			return InnerJoin<ClassJoin>(null);
		}

		/// <summary>
		/// Adiciona o join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <returns></returns>
		public Query InnerJoin<ClassJoin>(string classTypeJoinAlias)
		{
			return InnerJoin<ClassJoin>(classTypeJoinAlias, null);
		}

		/// <summary>
		/// Adiciona o join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query InnerJoin<ClassJoin>(string classTypeJoinAlias, string groupRelationship)
		{
			_joins.Add(new JoinInfo(JoinType.InnerJoin, null, typeof(ClassJoin), null, classTypeJoinAlias, groupRelationship));
			return this;
		}

		/// <summary>
		/// Adiciona um join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <returns></returns>
		public Query InnerJoin<ClassMain, ClassJoin>()
		{
			return InnerJoin<ClassMain, ClassJoin>(null);
		}

		/// <summary>
		/// Adiciona um join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query InnerJoin<ClassMain, ClassJoin>(string groupRelationship)
		{
			return InnerJoin<ClassMain, ClassJoin>(null, null, groupRelationship);
		}

		/// <summary>
		/// Adiciona um join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <param name="classTypeMainAlias">Apelido da classe principal do join.</param>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query InnerJoin<ClassMain, ClassJoin>(string classTypeMainAlias, string classTypeJoinAlias, string groupRelationship)
		{
			_joins.Add(new JoinInfo(JoinType.InnerJoin, typeof(ClassMain), typeof(ClassJoin), classTypeMainAlias, classTypeJoinAlias, groupRelationship));
			return this;
		}

		/// <summary>
		/// Adicionar o left join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <returns></returns>
		public Query LeftJoin<ClassJoin>()
		{
			return LeftJoin<ClassJoin>(null);
		}

		/// <summary>
		/// Adiciona o join com a classE especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <returns></returns>
		public Query LeftJoin<ClassJoin>(string classTypeJoinAlias)
		{
			return LeftJoin<ClassJoin>(classTypeJoinAlias, null);
		}

		/// <summary>
		/// Adiciona o join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query LeftJoin<ClassJoin>(string classTypeJoinAlias, string groupRelationship)
		{
			_joins.Add(new JoinInfo(JoinType.LeftJoin, null, typeof(ClassJoin), null, classTypeJoinAlias, groupRelationship));
			return this;
		}

		/// <summary>
		/// Adiciona um left join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <returns></returns>
		public Query LeftJoin<ClassMain, ClassJoin>()
		{
			return LeftJoin<ClassMain, ClassJoin>(null);
		}

		/// <summary>
		/// Adiciona um left join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query LeftJoin<ClassMain, ClassJoin>(string groupRelationship)
		{
			return LeftJoin<ClassMain, ClassJoin>(null, null, groupRelationship);
		}

		/// <summary>
		/// Adiciona um left join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <param name="classTypeMainAlias">Apelido da classe principal do join.</param>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query LeftJoin<ClassMain, ClassJoin>(string classTypeMainAlias, string classTypeJoinAlias, string groupRelationship)
		{
			_joins.Add(new JoinInfo(JoinType.LeftJoin, typeof(ClassMain), typeof(ClassJoin), classTypeMainAlias, classTypeJoinAlias, groupRelationship));
			return this;
		}

		/// <summary>
		/// Adicionar o right join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <returns></returns>
		public Query RightJoin<ClassJoin>()
		{
			return RightJoin<ClassJoin>(null);
		}

		/// <summary>
		/// Adiciona o right join com a classE especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <returns></returns>
		public Query RightJoin<ClassJoin>(string classTypeJoinAlias)
		{
			return RightJoin<ClassJoin>(classTypeJoinAlias, null);
		}

		/// <summary>
		/// Adiciona o right join com a class especificada.
		/// </summary>
		/// <typeparam name="ClassJoin">Tipo da classe que será feito o join.</typeparam>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query RightJoin<ClassJoin>(string classTypeJoinAlias, string groupRelationship)
		{
			_joins.Add(new JoinInfo(JoinType.RightJoin, null, typeof(ClassJoin), null, classTypeJoinAlias, groupRelationship));
			return this;
		}

		/// <summary>
		/// Adiciona um right join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <returns></returns>
		public Query RightJoin<ClassMain, ClassJoin>()
		{
			return RightJoin<ClassMain, ClassJoin>(null);
		}

		/// <summary>
		/// Adiciona um right join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query RightJoin<ClassMain, ClassJoin>(string groupRelationship)
		{
			return RightJoin<ClassMain, ClassJoin>(null, null, groupRelationship);
		}

		/// <summary>
		/// Adiciona um right join na consulta.
		/// </summary>
		/// <typeparam name="ClassMain"></typeparam>
		/// <typeparam name="ClassJoin"></typeparam>
		/// <param name="classTypeMainAlias">Apelido da classe principal do join.</param>
		/// <param name="classTypeJoinAlias">Apelido da classe do join.</param>
		/// <param name="groupRelationship">Nome do grupo de relacionamento.</param>
		/// <returns></returns>
		public Query RightJoin<ClassMain, ClassJoin>(string classTypeMainAlias, string classTypeJoinAlias, string groupRelationship)
		{
			_joins.Add(new JoinInfo(JoinType.RightJoin, typeof(ClassMain), typeof(ClassJoin), classTypeMainAlias, classTypeJoinAlias, groupRelationship));
			return this;
		}

		/// <summary>
		/// Adiciona um novo parametro na consulta.
		/// </summary>
		/// <param name="parameter">Paramentro a ser adicionado.</param>
		/// <returns>Retorna a referencia da consulta aonde o parametro foi adicionado.</returns>
		public Query Add(GDAParameter parameter)
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
		public Query Add(params GDAParameter[] parameters)
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
		public Query Add(IEnumerable<GDAParameter> parameters)
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
		/// Define a clausula WHERE na consulta.
		/// </summary>
		/// <param name="whereClause">Clausula WHERE.</param>
		/// <returns>Referência da consulta.</returns>
		public Query SetWhere(string whereClause)
		{
			Where = whereClause;
			return this;
		}

		/// <summary>
		/// Define a clausula ORDER BY.
		/// </summary>
		/// <param name="orderClause">Clausula ORDER BY.</param>
		/// <returns>Referência da consulta.</returns>
		public Query SetOrder(string orderClause)
		{
			this.orderClause = orderClause;
			return this;
		}

		/// <summary>
		/// Define a cláusula GROUP BY.
		/// </summary>
		/// <param name="groupByClause">Clausula GROUP BY</param>
		/// <returns></returns>
		public Query SetGroupBy(string groupByClause)
		{
			this.groupByClause = groupByClause;
			return this;
		}

		/// <summary>
		/// Recupera o <see cref="GDA.Sql.ResultList"/> do resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public ResultList<T> ToResultList<T>(int pageSize) where T : new()
		{
			return new ResultList<T>(this, pageSize);
		}

		/// <summary>
		/// Recupera o <see cref="GDA.Sql.ResultList<T>"/> do resultado da consulta.
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
		/// Recipera o resultado da consulta em forma de cursor e recupera o resultado 
		/// em objetos de outro tipo informado.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <typeparam name="Result">Tipos que estarão no retorno.</typeparam>
		/// <param name="session"></param>
		/// <returns></returns>
		public override IEnumerable<Result> ToCursor<T, Result>(GDASession session)
		{
			if(string.IsNullOrEmpty(_selectProperties))
			{
				var pNames = new List<string>();
				var resultProperties = typeof(Result).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.SetField);
				foreach (var i in MappingManager.GetMappers<T>(null, new DirectionParameter[] {
					DirectionParameter.Input,
					DirectionParameter.InputOutput
				}))
				{
					if(Helper.GDAHelper.Exists(resultProperties, f => f.Name == i.PropertyMapperName))
						pNames.Add(i.PropertyMapperName);
				}
				_selectProperties = string.Join(", ", pNames.ToArray());
			}
			return base.ToCursor<T, Result>(session);
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="session"></param>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public override GDAPropertyValue GetValue<T>(GDASession session, string propertyName)
		{
			var aux = _selectProperties;
			_selectProperties = propertyName;
			try
			{
				foreach (var i in ToDataRecords<T>(session))
					return i[propertyName];
				return new GDAPropertyValue(null, false);
			}
			finally
			{
				_selectProperties = aux;
			}
		}

		/// <summary>
		/// Recupera os valores da propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="session"></param>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public override IEnumerable<GDAPropertyValue> GetValues<T>(GDASession session, string propertyName)
		{
			var aux = _selectProperties;
			_selectProperties = propertyName;
			try
			{
				foreach (var i in ToDataRecords<T>(session))
					yield return i[propertyName];
			}
			finally
			{
				_selectProperties = aux;
			}
		}

		/// <summary>
		/// Apaga os registros com base no critério informado.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Número de linhas afetadas.</returns>
		public int Delete<T>()
		{
			return Delete<T>(null);
		}

		/// <summary>
		/// Apaga os registros com base no critério informado.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		public int Delete<T>(GDASession session)
		{
			_returnTypeQuery = typeof(T);
			IProviderConfiguration providerConfig = GDAOperations.GetDAO(_returnTypeQuery).Configuration;
			List<Mapper> mapping = MappingManager.GetMappers(_returnTypeQuery, null, null);
			var tableName = MappingManager.GetTableName(_returnTypeQuery);
			StringBuilder sb = new StringBuilder(string.Format("DELETE FROM {0}", providerConfig.Provider.BuildTableName(tableName)));
			if(!string.IsNullOrEmpty(this.Where))
			{
				Parser parser = new Parser(new Lexer(this.Where));
				WherePart wp = parser.ExecuteWherePart();
				SelectStatement ss = new SelectStatement(wp);
				foreach (ColumnInfo ci in ss.ColumnsInfo)
				{
					if(string.IsNullOrEmpty(ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == "main")
					{
						ci.TableNameOrTableAlias = providerConfig.Provider.BuildTableName(tableName);
					}
					Mapper m = mapping.Find(delegate(Mapper mp) {
						return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
					});
					if(m == null)
					{
						m = MappingManager.GetPropertyMapper(_returnTypeQuery, ci.ColumnName);
						if(m == null)
							throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName);
					}
					ci.DBColumnName = m.Name;
					ci.RenameToMapper(providerConfig.Provider);
				}
				ParserToSqlCommand psc = new ParserToSqlCommand(wp, providerConfig.Provider.QuoteExpressionBegin, providerConfig.Provider.QuoteExpressionEnd);
				sb.Append(" WHERE " + psc.SqlCommand);
			}
			DataAccess da = new DataAccess(providerConfig);
			return da.ExecuteCommand(session, sb.ToString(), this.Parameters.ToArray());
		}

		/// <summary>
		/// Recupera uma instância para o tipo definido.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Query From<T>(string whereClause) where T : new()
		{
			Query q = new Query(whereClause);
			q.ReturnTypeQuery = typeof(T);
			return q;
		}

		/// <summary>
		/// Recupera uma instância para o tipo definido.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Query From<T>() where T : new()
		{
			Query q = new Query();
			q.ReturnTypeQuery = typeof(T);
			return q;
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de dados que será tratado na consulta.</typeparam>
		/// <returns>Consulta SQL gerada.</returns>
		public override QueryReturnInfo BuildResultInfo<T>(GDA.Interfaces.IProviderConfiguration configuration)
		{
			this._returnTypeQuery = typeof(T);
			if(configuration != null)
				configuration = GDA.GDASettings.DefaultProviderConfiguration;
			return this.BuildResultInfo2(configuration.Provider, null);
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de dados que será tratado na consulta.</typeparam>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns>Consulta SQL gerada.</returns>
		public QueryReturnInfo BuildResultInfo<T>(string aggregationFunction)
		{
			this._returnTypeQuery = typeof(T);
			return this.BuildResultInfo(aggregationFunction);
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo(string aggregationFunction)
		{
			if(_returnTypeQuery == null)
				throw new QueryException("Type return query not found.");
			IProvider provider = GDAOperations.GetDAO(_returnTypeQuery).Configuration.Provider;
			return BuildResultInfo2(provider, aggregationFunction);
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public override QueryReturnInfo BuildResultInfo2(IProvider provider, string aggregationFunction)
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
			if(_returnTypeQuery == null)
				throw new QueryException("Type return query not found.");
			if(classesDictionary == null)
				classesDictionary = new Dictionary<string, Type>();
			var mainTableAlias = "main";
			if(string.IsNullOrEmpty(MainAlias))
			{
				for(var i = 1; classesDictionary.ContainsKey(mainTableAlias); i++)
					mainTableAlias = string.Format("main{0}", i);
			}
			else
				mainTableAlias = MainAlias;
			classesDictionary.Add(mainTableAlias, _returnTypeQuery);
			foreach (JoinInfo ji in _joins)
			{
				if(ji.ClassTypeMain == null)
				{
					ji.ClassTypeMain = _returnTypeQuery;
					ji.ClassTypeMainAlias = mainTableAlias;
				}
				else
				{
					if(string.IsNullOrEmpty(ji.ClassTypeMainAlias))
						ji.ClassTypeMainAlias = ji.ClassTypeJoin.Name.ToLower();
					if(!classesDictionary.ContainsKey(ji.ClassTypeMainAlias))
						classesDictionary.Add(ji.ClassTypeMainAlias, ji.ClassTypeMain);
				}
				if(string.IsNullOrEmpty(ji.ClassTypeJoinAlias))
					ji.ClassTypeJoinAlias = ji.ClassTypeJoin.Name.ToLower();
				if(!classesDictionary.ContainsKey(ji.ClassTypeJoinAlias))
					classesDictionary.Add(ji.ClassTypeJoinAlias, ji.ClassTypeJoin);
			}
			List<Mapper> recoverProperties = new List<Mapper>();
			List<Mapper> columns = new List<Mapper>();
			List<Mapper> mapping = MappingManager.GetMappers(_returnTypeQuery, null, null);
			List<GroupOfRelationshipInfo> fkMapping = MappingManager.GetForeignKeyAttributes(_returnTypeQuery);
			if(mapping.Count == 0)
			{
			}
			StringBuilder buf = new StringBuilder(128).Append("SELECT ");
			if(_useDistinct)
				buf.Append("DISTINCT ");
			bool append = false;
			if(!string.IsNullOrEmpty(_selectProperties))
			{
				var functions = new List<string>();
				Parser p = new Parser(new Lexer(_selectProperties));
				SelectPart selectPart = p.ExecuteSelectPart();
				var selectProps = new List<string>(selectPart.SelectionExpressions.Count);
				var projection = new List<SelectExpression>();
				foreach (SelectExpression se in selectPart.SelectionExpressions)
				{
					if(se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
					{
						Column col = se.Column;
						if(string.IsNullOrEmpty(col.TableName))
						{
							se.ColumnName.Value.Text = string.Format("{0}.{1}", mainTableAlias, se.ColumnName.Value.Text);
							col = se.Column;
						}
						if(col.TableName == mainTableAlias && col.Name == "*")
						{
							foreach (Mapper mp in mapping)
							{
								if(mp.Direction == DirectionParameter.Input || mp.Direction == DirectionParameter.InputOutput || mp.Direction == DirectionParameter.OutputOnlyInsert)
								{
									var columnExpression = new SelectExpression(new SqlExpression(new Expression(0, 0) {
										Text = string.Format("{0}.{1}", mainTableAlias, mp.PropertyMapperName)
									}), new SqlExpression(new Expression(0, 0) {
										Text = mp.PropertyMapperName
									}));
									projection.Add(columnExpression);
								}
							}
							continue;
						}
						else if(col.Name == "*")
							throw new GDAException("Invalid expression {0}", se.ColumnName.Value.Text);
						else
						{
							projection.Add(se);
						}
					}
					else if(se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function)
					{
						SqlFunction func = (SqlFunction)se.ColumnName;
						if(func.Parameters.Count > 0)
						{
							foreach (List<SqlExpression> listSqlEx in func.Parameters)
							{
								foreach (SqlExpression ss1 in listSqlEx)
								{
									if(ss1.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
									{
										ColumnInfo col = new ColumnInfo(ss1.Value);
										if(string.IsNullOrEmpty(col.TableNameOrTableAlias))
											col.TableNameOrTableAlias = mainTableAlias;
										if(col.ColumnName == "*")
											break;
										if(classesDictionary.ContainsKey(col.TableNameOrTableAlias))
										{
											var mapperList = MappingManager.GetMappers(classesDictionary[col.TableNameOrTableAlias], null, null);
											bool found = false;
											foreach (Mapper mapper in mapperList)
												if(string.Compare(col.ColumnName, mapper.PropertyMapperName, true) == 0)
												{
													col.DBColumnName = mapper.Name;
													col.RenameToMapper(provider);
													found = true;
													break;
												}
											if(!found)
												throw new GDAException("Property {0} not found for type {1}.", col.ColumnName, classesDictionary[col.TableNameOrTableAlias].FullName);
										}
										else
										{
											throw new GDAException("Classe alias {0} not found.", col.TableNameOrTableAlias);
										}
									}
								}
							}
						}
						projection.Add(se);
					}
				}
				//// Pecorre as possíveis propriedades da classe principal que serão recuperadas na consulta
				append = false;
				foreach (var se in projection)
				{
					if(se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
					{
						var column = se.Column;
						Type tableType = null;
						if(!classesDictionary.TryGetValue(column.TableName, out tableType))
							throw new GDAException("Not found class for alias {0}", column.TableName);
						var tableMapping = MappingManager.GetMappers(tableType, null, null);
						if(tableMapping == null)
							continue;
						var mapper = tableMapping.Find(f => StringComparer.InvariantCultureIgnoreCase.Equals(column.Name, f.PropertyMapperName));
						if(mapper == null)
							throw new GDAException("Not fount mapping {0} for type {1}", column.Name, tableType.FullName);
						if(append)
							buf.Append(", ");
						else
							append = true;
						buf.Append(provider.QuoteExpression(column.TableName)).Append(".").Append(provider.QuoteExpression(mapper.Name));
						if(!string.IsNullOrEmpty(column.Alias))
						{
							var mainMapper = mapping.Find(f => StringComparer.InvariantCultureIgnoreCase.Equals(column.Alias, f.PropertyMapperName));
							if(mainMapper != null)
							{
								recoverProperties.Add(mainMapper);
								buf.Append(" AS ").Append(provider.QuoteExpression(mainMapper.Name));
							}
							else
								buf.Append(" AS ").Append(provider.QuoteExpression(column.Alias));
						}
						else
							recoverProperties.Add(mapper);
					}
					else if(se.ColumnName.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Function)
					{
						if(append)
							buf.Append(", ");
						else
							append = true;
						buf.Append(se.ColumnName.ToString());
						if(se.ColumnAlias != null)
						{
							var columnAlias = se.ColumnAlias.ToString();
							var mainMapper = mapping.Find(f => StringComparer.InvariantCultureIgnoreCase.Equals(columnAlias, f.PropertyMapperName));
							if(mainMapper != null)
							{
								recoverProperties.Add(mainMapper);
								buf.Append(" AS ").Append(provider.QuoteExpression(mainMapper.Name));
							}
							else
								buf.Append(" AS ").Append(provider.QuoteExpression(columnAlias));
						}
					}
				}
				append = !string.IsNullOrEmpty(_selectProperties);
			}
			append = false;
			foreach (Mapper column in mapping)
			{
				switch(column.Direction)
				{
				case DirectionParameter.Input:
				case DirectionParameter.InputOutput:
				case DirectionParameter.OutputOnlyInsert:
					if(string.IsNullOrEmpty(_selectProperties))
					{
						if(append)
							buf.Append(", ");
						else
							append = true;
						buf.Append(provider.QuoteExpression(mainTableAlias)).Append(".").Append(provider.QuoteExpression(column.Name));
						recoverProperties.Add(column);
					}
					columns.Add(column);
					break;
				}
			}
			IList<ForeignMemberMapper> fmms = MappingManager.GetForeignMemberMapper(_returnTypeQuery);
			foreach (ForeignMemberMapper fmm in fmms)
			{
				int index = _joins.FindIndex(delegate(JoinInfo ji) {
					return ji.ClassTypeJoin == fmm.TypeOfClassRelated && ji.GroupOfRelationship == fmm.GroupOfRelationship;
				});
				if(index >= 0)
				{
					if(append)
						buf.Append(", ");
					else
						append = true;
					PersistencePropertyAttribute ppaRelated = MappingManager.GetPersistenceProperty(fmm.PropertyOfClassRelated);
					if(ppaRelated == null)
						throw new GDAException("Fail on ForeignKeyMember, property {0} in {1} isn't mapped.", fmm.PropertyOfClassRelated.Name, fmm.TypeOfClassRelated.FullName);
					buf.Append(provider.QuoteExpression(_joins[index].ClassTypeJoinAlias)).Append(".").Append(provider.QuoteExpression(ppaRelated.Name)).Append(" AS ").Append(provider.QuoteExpression(fmm.PropertyModel.Name));
					recoverProperties.Add(new Mapper(_returnTypeQuery, new PersistencePropertyAttribute(fmm.PropertyModel.Name, DirectionParameter.InputOptional), fmm.PropertyModel));
				}
			}
			if(!string.IsNullOrEmpty(aggregationFunction))
			{
				Parser aggreParser = new Parser(new Lexer(aggregationFunction));
				SelectPart sp = aggreParser.ExecuteSelectPart();
				if(sp.SelectionExpressions.Count == 0)
					throw new GDAException("Not found aggregation function.");
				foreach (SelectExpression sqlEx in sp.SelectionExpressions)
					if(sqlEx.ColumnName is SqlFunction)
					{
						SqlFunction func = (SqlFunction)sqlEx.ColumnName;
						if(func.Parameters.Count > 0)
						{
							foreach (List<SqlExpression> listSqlEx in func.Parameters)
							{
								foreach (SqlExpression ss1 in listSqlEx)
								{
									if(ss1.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
									{
										Column col = new Column(ss1, null);
										if(string.IsNullOrEmpty(col.TableName))
											col.TableName = mainTableAlias;
										if(col.Name == "*")
											break;
										if(classesDictionary.ContainsKey(col.TableName))
										{
											List<Mapper> listMapper = MappingManager.GetMappers(classesDictionary[col.TableName], null, null);
											bool found = false;
											foreach (Mapper mapper in listMapper)
												if(string.Compare(col.Name, mapper.PropertyMapperName, true) == 0)
												{
													aggregationFunction = aggregationFunction.Substring(0, ss1.Value.BeginPoint) + provider.QuoteExpression(col.TableName) + "." + provider.QuoteExpression(mapper.Name) + aggregationFunction.Substring(ss1.Value.BeginPoint + ss1.Value.Length);
													found = true;
													break;
												}
											if(!found)
												throw new GDAException("Property {0} not found.", col.Name);
										}
										else
										{
											throw new GDAException("Classe alias {0} not found.", col.TableName);
										}
									}
								}
							}
						}
					}
				buf = new StringBuilder("SELECT ").Append(aggregationFunction);
			}
			buf.Append(" FROM ").Append(provider.BuildTableName(MappingManager.GetTableName(_returnTypeQuery))).Append(" ").Append(provider.QuoteExpression(mainTableAlias)).Append(" ");
			StringBuilder sbJoins = new StringBuilder();
			if(_joins.Count > 0)
			{
				int indexCurrentJoin = 1;
				for(int i = 0; i < _joins.Count; i++)
				{
					JoinInfo ji = _joins[i];
					GroupOfRelationshipInfo info = fkMapping.Find(delegate(GroupOfRelationshipInfo gri) {
						return gri.TypeOfClassRelated == ji.ClassTypeJoin && gri.GroupOfRelationship == ji.GroupOfRelationship;
					});
					if(info == null)
					{
						info = MappingManager.GetForeignKeyAttributes(ji.ClassTypeJoin).Find(delegate(GroupOfRelationshipInfo gri) {
							return gri.TypeOfClassRelated == _returnTypeQuery && gri.GroupOfRelationship == ji.GroupOfRelationship;
						});
						if(info == null)
						{
							foreach (GroupOfRelationshipInfo gri2 in fkMapping)
							{
								MappingManager.LoadClassMapper(gri2.TypeOfClassRelated);
								info = MappingManager.GetForeignKeyAttributes(gri2.TypeOfClassRelated).Find(delegate(GroupOfRelationshipInfo gri) {
									return gri.TypeOfClassRelated == ji.ClassTypeJoin && gri.GroupOfRelationship == ji.GroupOfRelationship;
								});
								if(info != null)
								{
									ji.ClassTypeMain = info.TypeOfClass;
									int j = 0;
									for(j = 0; j < _joins.Count; j++)
										if(_joins[j].ClassTypeJoin == info.TypeOfClass)
										{
											ji.ClassTypeMainAlias = _joins[j].ClassTypeJoinAlias;
											break;
										}
									if(j >= _joins.Count)
									{
										ji.ClassTypeMainAlias = ji.ClassTypeMain.Name;
										_joins.Add(new JoinInfo(ji.Type, ReturnTypeQuery, info.TypeOfClass, ji.ClassTypeMain.Name, ji.ClassTypeJoinAlias, info.GroupOfRelationship));
									}
									break;
								}
							}
							if(info == null)
							{
								foreach (KeyValuePair<string, Type> it in classesDictionary)
								{
									if(it.Key == mainTableAlias)
										continue;
									MappingManager.LoadClassMapper(it.Value);
									info = MappingManager.GetForeignKeyAttributes(it.Value).Find(delegate(GroupOfRelationshipInfo gri) {
										return gri.TypeOfClassRelated == ji.ClassTypeJoin && gri.GroupOfRelationship == ji.GroupOfRelationship;
									});
									if(info != null)
									{
										ji.ClassTypeMain = info.TypeOfClass;
										int j = 0;
										for(j = 0; j < _joins.Count; j++)
											if(_joins[j].ClassTypeJoin == info.TypeOfClass)
											{
												ji.ClassTypeMainAlias = _joins[j].ClassTypeJoinAlias;
												break;
											}
										if(j >= _joins.Count)
										{
											ji.ClassTypeMainAlias = ji.ClassTypeMain.Name;
											_joins.Add(new JoinInfo(ji.Type, ReturnTypeQuery, info.TypeOfClass, ji.ClassTypeMain.Name, ji.ClassTypeJoinAlias, info.GroupOfRelationship));
										}
										break;
									}
								}
							}
							if(info == null)
								throw new QueryException("Not found foreign key with {0}", ji.ClassTypeJoin.FullName);
						}
					}
					switch(ji.Type)
					{
					case JoinType.InnerJoin:
						sbJoins.Append("INNER JOIN ");
						break;
					case JoinType.LeftJoin:
						sbJoins.Append("LEFT JOIN ");
						break;
					case JoinType.RightJoin:
						sbJoins.Append("RIGHT JOIN ");
						break;
					case JoinType.CrossJoin:
						sbJoins.Append("CROSS JOIN ");
						break;
					}
					sbJoins.Append(provider.BuildTableName(MappingManager.GetTableName(ji.ClassTypeJoin))).Append(" AS ").Append(provider.QuoteExpression(ji.ClassTypeJoinAlias)).Append(" ON(");
					for(int x = 0; x < info.ForeignKeys.Count; x++)
					{
						ForeignKeyMapper fk = info.ForeignKeys[x];
						var persistenceProperty = MappingManager.GetPersistenceProperty(fk.PropertyOfClassRelated);
						if(persistenceProperty == null)
							throw new GDAException("Not found mapper for property '{0}' of type '{1}'.", fk.PropertyOfClassRelated.Name, fk.PropertyOfClassRelated.DeclaringType.FullName);
						sbJoins.Append(provider.QuoteExpression(ji.ClassTypeMainAlias)).Append(".").Append(provider.QuoteExpression(MappingManager.GetPersistenceProperty(fk.PropertyModel).Name)).Append("=").Append(provider.QuoteExpression(ji.ClassTypeJoinAlias)).Append(".").Append(provider.QuoteExpression(persistenceProperty.Name));
						if((x + 1) < info.ForeignKeys.Count)
							sbJoins.Append(" AND ");
					}
					sbJoins.Append(") ");
					indexCurrentJoin++;
				}
			}
			buf.Append(sbJoins.ToString());
			if(this._conditional != null && this._conditional.Count > 0)
			{
				Parser parser = new Parser(new Lexer(this.Where));
				WherePart wp = parser.ExecuteWherePart();
				SelectStatement ss = new SelectStatement(wp);
				ProcessConditionalSelectStatement(provider, classesDictionary, mainTableAlias, columns, ss, false);
				foreach (var variableInfo in ss.VariablesInfo)
					variableInfo.Replace(provider, this, classesDictionary);
				ParserToSqlCommand psc = new ParserToSqlCommand(wp, provider.QuoteExpressionBegin, provider.QuoteExpressionEnd);
				buf.Append(" WHERE ").Append(psc.SqlCommand);
			}
			if(!string.IsNullOrEmpty(this.GroupByClause))
			{
				Parser parser = new Parser(new Lexer(this.GroupByClause));
				GroupByPart gbp = parser.ExecuteGroupByPart();
				SelectStatement ss = new SelectStatement(gbp);
				foreach (ColumnInfo ci in ss.ColumnsInfo)
				{
					if(string.IsNullOrEmpty(ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == mainTableAlias)
					{
						ci.TableNameOrTableAlias = mainTableAlias;
					}
					else
					{
						foreach (JoinInfo ji in this._joins)
						{
							if(ji.ClassTypeJoin.Name == ci.TableNameOrTableAlias || ji.ClassTypeJoin.FullName == ci.TableNameOrTableAlias || ji.ClassTypeJoinAlias == ci.TableNameOrTableAlias)
							{
								List<Mapper> mapperAux = MappingManager.GetMappers(ji.ClassTypeJoin, null, null);
								Mapper mAux = mapperAux.Find(delegate(Mapper mp) {
									return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
								});
								if(mAux == null)
								{
									mAux = MappingManager.GetPropertyMapper(ji.ClassTypeJoin, ci.ColumnName);
									if(mAux == null)
										throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, ji.ClassTypeJoin.FullName);
								}
								ci.TableNameOrTableAlias = ji.ClassTypeJoinAlias;
								ci.DBColumnName = mAux.Name;
								ci.RenameToMapper(provider);
								break;
							}
						}
						if(string.IsNullOrEmpty(ci.DBColumnName))
							throw new QueryException("Field {0} not found in mapping.", ci.ToString());
						continue;
					}
					Mapper m = columns.Find(delegate(Mapper mp) {
						return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
					});
					if(m == null)
					{
						m = MappingManager.GetPropertyMapper(_returnTypeQuery, ci.ColumnName);
						if(m == null)
							throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName);
					}
					ci.DBColumnName = m.Name;
					ci.RenameToMapper(provider);
				}
				ParserToSqlCommand psc = new ParserToSqlCommand(gbp, provider.QuoteExpressionBegin, provider.QuoteExpressionEnd);
				buf.Append(" GROUP BY ").Append(psc.SqlCommand);
			}
			if(this._having != null && this._having.Count > 0)
			{
				Parser parser = new Parser(new Lexer(this.HavingText));
				HavingPart havingPart = parser.ExecuteHavingPart();
				SelectStatement ss = new SelectStatement(havingPart);
				ProcessConditionalSelectStatement(provider, classesDictionary, mainTableAlias, columns, ss, true);
				foreach (var variableInfo in ss.VariablesInfo)
					variableInfo.Replace(provider, this, classesDictionary);
				ParserToSqlCommand psc = new ParserToSqlCommand(havingPart, provider.QuoteExpressionBegin, provider.QuoteExpressionEnd);
				buf.Append(" HAVING ").Append(psc.SqlCommand);
			}
			if(!string.IsNullOrEmpty(this.Order) && string.IsNullOrEmpty(aggregationFunction))
			{
				Parser parser = new Parser(new Lexer(this.Order));
				OrderByPart op = parser.ExecuteOrderByPart();
				SelectStatement ss = new SelectStatement(op);
				foreach (ColumnInfo ci in ss.ColumnsInfo)
				{
					if(string.IsNullOrEmpty(ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == mainTableAlias)
					{
						Mapper m = columns.Find(delegate(Mapper mp) {
							return string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0;
						});
						if(m == null)
						{
							GDAOperations.CallDebugTrace(this, string.Format("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName));
							continue;
						}
						ci.DBColumnName = m.Name;
						ci.RenameToMapper(provider);
					}
					else
					{
						foreach (JoinInfo ji in this._joins)
						{
							if(ji.ClassTypeJoin.Name == ci.TableNameOrTableAlias || ji.ClassTypeJoin.FullName == ci.TableNameOrTableAlias || ji.ClassTypeJoinAlias == ci.TableNameOrTableAlias)
							{
								List<Mapper> mapperAux = MappingManager.GetMappers(ji.ClassTypeJoin, null, null);
								Mapper mAux = mapperAux.Find(delegate(Mapper mp) {
									return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
								});
								if(mAux == null)
								{
									mAux = MappingManager.GetPropertyMapper(ji.ClassTypeJoin, ci.ColumnName);
									if(mAux == null)
									{
										GDAOperations.CallDebugTrace(this, string.Format("Property {0} not exists in {1} or not mapped.", ci.ColumnName, ji.ClassTypeJoin.FullName));
										continue;
									}
								}
								ci.TableNameOrTableAlias = ji.ClassTypeJoinAlias;
								ci.DBColumnName = mAux.Name;
								ci.RenameToMapper(provider);
								break;
							}
						}
					}
					if(string.IsNullOrEmpty(ci.DBColumnName))
						GDAOperations.CallDebugTrace(this, string.Format("Field {0} not found in mapping.", ci.ToString()));
				}
				ParserToSqlCommand psc = new ParserToSqlCommand(op, provider.QuoteExpressionBegin, provider.QuoteExpressionEnd);
				buf.Append(" ORDER BY ").Append(psc.SqlCommand);
			}
			return new QueryReturnInfo(buf.ToString(), this.Parameters, recoverProperties);
		}

		/// <summary>
		/// Processa o Select Statement.
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="classesDictionary"></param>
		/// <param name="mainTableAlias"></param>
		/// <param name="columns"></param>
		/// <param name="ss"></param>
		/// <param name="supportWildcardColumn">Identifica se deve possui suporte para coluna curinga "*"</param>
		private void ProcessConditionalSelectStatement(GDA.Interfaces.IProvider provider, Dictionary<string, Type> classesDictionary, string mainTableAlias, List<Mapper> columns, SelectStatement ss, bool supportWildcardColumn)
		{
			foreach (ColumnInfo ci in ss.ColumnsInfo)
			{
				if(string.IsNullOrEmpty(ci.TableNameOrTableAlias) || ci.TableNameOrTableAlias == mainTableAlias)
				{
					ci.TableNameOrTableAlias = provider.QuoteExpression(mainTableAlias);
				}
				else
				{
					foreach (JoinInfo ji in this._joins)
					{
						if(ji.ClassTypeJoin.Name == ci.TableNameOrTableAlias || ji.ClassTypeJoin.FullName == ci.TableNameOrTableAlias || ji.ClassTypeJoinAlias == ci.TableNameOrTableAlias)
						{
							List<Mapper> mapperAux = MappingManager.GetMappers(ji.ClassTypeJoin, null, null);
							Mapper mAux = mapperAux.Find(delegate(Mapper mp) {
								return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
							});
							if(mAux == null)
							{
								mAux = MappingManager.GetPropertyMapper(ji.ClassTypeJoin, ci.ColumnName);
								if(mAux == null)
									throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, ji.ClassTypeJoin.FullName);
							}
							ci.TableNameOrTableAlias = ji.ClassTypeJoinAlias;
							ci.DBColumnName = mAux.Name;
							ci.RenameToMapper(provider);
							break;
						}
					}
					if(string.IsNullOrEmpty(ci.DBColumnName))
					{
						Type type = null;
						if(classesDictionary.TryGetValue(ci.TableNameOrTableAlias, out type))
						{
							List<Mapper> mapperAux = MappingManager.GetMappers(type, null, null);
							Mapper mAux = mapperAux.Find(delegate(Mapper mp) {
								return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
							});
							if(mAux == null)
							{
								mAux = MappingManager.GetPropertyMapper(type, ci.ColumnName);
								if(mAux == null)
									throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, type.FullName);
							}
							ci.DBColumnName = mAux.Name;
							ci.RenameToMapper(provider);
						}
						if(string.IsNullOrEmpty(ci.DBColumnName))
							throw new QueryException("Field {0} not found in mapping.", ci.ToString());
					}
					continue;
				}
				Mapper m = columns.Find(delegate(Mapper mp) {
					return (string.Compare(mp.PropertyMapperName, ci.ColumnName, true) == 0);
				});
				if(m == null)
				{
					m = MappingManager.GetPropertyMapper(_returnTypeQuery, ci.ColumnName);
					if(m == null && (!supportWildcardColumn || ci.ColumnName != "*"))
						throw new GDAException("Property {0} not exists in {1} or not mapped.", ci.ColumnName, _returnTypeQuery.FullName);
				}
				if(m != null)
					ci.DBColumnName = m.Name;
				ci.RenameToMapper(provider);
			}
		}

		/// <summary>
		/// Verifica a igualdade com outro objeto.
		/// </summary>
		/// <param name="obj">Outro objeto</param>
		/// <returns>True se for igual, false senão.</returns>
		public override bool Equals(object obj)
		{
			if(obj is Query)
				return Equals((Query)obj);
			else
				return false;
		}

		/// <summary>
		/// Verifica a igualdade com outro objeto Query.
		/// </summary>
		/// <param name="other">outro objeto Query</param>
		/// <returns>True se for igual, false senão.</returns>
		public bool Equals(Query other)
		{
			return Where == null ? other.Where == null : Where.Equals(other.Where) && orderClause == null ? other.orderClause == null : orderClause.Equals(other.orderClause);
		}

		/// <summary>
		/// Gets the hash code for this instance.
		/// </summary>
		/// <returns>hash code</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Adiciona um parametro na consulta.
		/// </summary>
		/// <param name="dbtype">Tipo usado na base de dados</param>
		/// <param name="value">Valor do parametro.</param>
		public Query Add(DbType dbtype, object value)
		{
			return Add("", dbtype, value);
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="dbtype">Tipo usado na base de dados.</param>
		/// <param name="value">parameter value</param>
		public Query Add(string name, DbType dbtype, object value)
		{
			return Add(name, dbtype, 0, value);
		}

		public Query Add(string name, object value)
		{
			return Add(new GDAParameter(name, value));
		}

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		/// <param name="dbtype">database data type</param>
		/// <param name="size">size of the database data type</param>
		/// <param name="value">parameter value</param>
		public Query Add(DbType dbtype, int size, object value)
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
		public Query Add(string name, DbType dbtype, int size, object value)
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
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count<T>() where T : new()
		{
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Count(this);
		}

		/// <summary>
		/// Recupera a quantidade de registros com base na Query.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Quantidade de registro encontrados com base na consulta.</returns>
		public long Count<T>(GDASession session) where T : new()
		{
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Count(session, this);
		}

		/// <summary>
		/// Efetua a soma de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="propertyName">Propriedade que sera somada.</param>
		/// <returns>Soma dos valores da propriedade identificada.</returns>
		public double Sum<T>(string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Sum(null, this);
		}

		/// <summary>
		/// Efetua a soma de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertyName">Propriedade que sera somada.</param>
		/// <returns>Soma dos valores da propriedade identificada.</returns>
		public double Sum<T>(GDASession session, string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Sum(session, this);
		}

		/// <summary>
		/// Recupera o valor máximo de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="propertyName">Propriedade onde será recupera o valor máximo.</param>
		/// <returns>Valor máximo da propriedade identificada.</returns>
		public double Max<T>(string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Max(null, this);
		}

		/// <summary>
		/// Recupera o valor máximo de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertyName">Propriedade onde será recupera o valor máximo.</param>
		/// <returns>Valor máximo da propriedade identificada.</returns>
		public double Max<T>(GDASession session, string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Max(session, this);
		}

		/// <summary>
		/// Recupera o valor mínimo de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="propertyName">Propriedade onde será recupera o valor mínimo.</param>
		/// <returns>Valor mínimo da propriedade identificada.</returns>
		public double Min<T>(string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Min(null, this);
		}

		/// <summary>
		/// Recupera o valor mínimo de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertyName">Propriedade onde será recupera o valor mínimo.</param>
		/// <returns>Valor mínimo da propriedade identificada.</returns>
		public double Min<T>(GDASession session, string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Min(session, this);
		}

		/// <summary>
		/// Recupera o valor médio de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="propertyName">Propriedade onde será recupera o valor médio.</param>
		/// <returns>Valor médio da propriedade identificada.</returns>
		public double Avg<T>(string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Avg(null, this);
		}

		/// <summary>
		/// Recupera o valor médio de uma determina propriedade da classe T definida.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertyName">Propriedade onde será recupera o valor médio.</param>
		/// <returns>Valor médio da propriedade identificada.</returns>
		public double Avg<T>(GDASession session, string propertyName) where T : new()
		{
			_aggregationFunctionProperty = propertyName;
			_returnTypeQuery = typeof(T);
			return GDAOperations.GetDAO<T>().Avg(session, this);
		}

		void IGDAParameterContainer.Add(GDAParameter parameter)
		{
			var index = this._parameters.FindIndex(f => f.ParameterName == parameter.ParameterName);
			if(index >= 0)
				this._parameters.RemoveAt(index);
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
			return _parameters.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}
	}
}
