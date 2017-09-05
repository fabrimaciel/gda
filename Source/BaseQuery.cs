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
using GDA.Collections;

namespace GDA.Sql
{
	/// <summary>
	/// Implementação básica de uma consulta.
	/// </summary>
	public abstract class BaseQuery : IQuery
	{
		protected Type _returnTypeQuery;

		/// <summary>
		/// Número de registros que deverão ser pulados.
		/// </summary>
		protected int _skipCount = 0;

		/// <summary>
		/// Quantidade de registros que serão recuperados.
		/// </summary>
		protected int _takeCount = 0;

		/// <summary>
		/// Nome da propriedade para utiliza uma função de agregação.
		/// </summary>
		protected string _aggregationFunctionProperty;

		/// <summary>
		/// Nome da propriedade para utiliza uma função de agregação.
		/// </summary>
		public string AggregationFunctionProperty
		{
			get
			{
				return _aggregationFunctionProperty;
			}
		}

		/// <summary>
		/// Tipo do retorno da consulta.
		/// </summary>
		public Type ReturnTypeQuery
		{
			get
			{
				return _returnTypeQuery;
			}
			set
			{
				_returnTypeQuery = value;
			}
		}

		/// <summary>
		/// Número de registros que deverão ser pulados.
		/// </summary>
		public int SkipCount
		{
			get
			{
				return _skipCount;
			}
		}

		/// <summary>
		/// Quantidade de registros que serão recuperados.
		/// </summary>
		public int TakeCount
		{
			get
			{
				return _takeCount;
			}
		}

		/// <summary>
		/// Salta um número especifico de registros antes de recuperar os resultado.
		/// </summary>
		/// <param name="count">Quantidade de registros que serão saltados.</param>
		/// <returns></returns>
		internal BaseQuery BaseSkip(int count)
		{
			_skipCount = count;
			return this;
		}

		/// <summary>
		/// Define a quantidade de registro que serão recuperados.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		internal BaseQuery BaseTake(int count)
		{
			_takeCount = count;
			return this;
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
		public GDACursor<T> ToCursor<T>() where T : new()
		{
			return GDAOperations.GetDAO<T>().Select(null, this);
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
		public GDACursor<T> ToCursor<T>(GDASession session) where T : new()
		{
			return GDAOperations.GetDAO<T>().Select(session, this);
		}

		/// <summary>
		/// Recipera o resultado da consulta em forma de cursor e recupera o resultado 
		/// em objetos de outro tipo informado.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <typeparam name="Result">Tipos que estarão no retorno.</typeparam>
		/// <param name="session"></param>
		/// <returns></returns>
		public virtual IEnumerable<Result> ToCursor<T, Result>(GDASession session) where T : new() where Result : new()
		{
			return GDAOperations.GetDAO<T>().SelectToDataRecord(session, this).Select<Result>();
		}

		/// <summary>
		/// Recipera o resultado da consulta em forma de cursor e recupera o resultado 
		/// em objetos de outro tipo informado.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <typeparam name="Result">Tipos que estarão no retorno.</typeparam>        
		/// <returns></returns>
		public virtual IEnumerable<Result> ToCursor<T, Result>() where T : new() where Result : new()
		{
			return ToCursor<T, Result>(null);
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
		public GDADataRecordCursor<T> ToDataRecords<T>()
		{
			return GDAOperations.GetSimpleDAO<T>().SelectToDataRecord(null, this);
		}

		/// <summary>
		/// Recupera o resultado da consulta em forma de cursor.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
		public GDADataRecordCursor<T> ToDataRecords<T>(GDASession session)
		{
			return GDAOperations.GetSimpleDAO<T>().SelectToDataRecord(session, this);
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public virtual GDAPropertyValue GetValue<T>(string propertyName)
		{
			return GetValue<T>(null, propertyName);
		}

		/// <summary>
		/// Recupera o valor da propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="session"></param>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public virtual GDAPropertyValue GetValue<T>(GDASession session, string propertyName)
		{
			foreach (var i in ToDataRecords<T>(session))
				return i[propertyName];
			return new GDAPropertyValue(null, false);
		}

		/// <summary>
		/// Recupera os valores da propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam> 
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public IEnumerable<GDAPropertyValue> GetValues<T>(string propertyName)
		{
			return GetValues<T>(null, propertyName);
		}

		/// <summary>
		/// Recupera os valores da propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="session"></param>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public virtual IEnumerable<GDAPropertyValue> GetValues<T>(GDASession session, string propertyName)
		{
			foreach (var i in ToDataRecords<T>(session))
				yield return i[propertyName];
		}

		#if CLS_3_5
		
        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Nome da propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue GetValue<T>(System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            return GetValue<T>(propertySelector.GetMember().Name);
        }

        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="propertySelector">Nome da propriedade que será recuperada.</param>
        /// <returns></returns>
        public GDAPropertyValue GetValue<T>(GDASession session, System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            return GetValue<T>(session, propertySelector.GetMember().Name);
        }

        /// <summary>
        /// Recupera os valores da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="propertySelector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public IEnumerable<GDAPropertyValue> GetValues<T>(System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            return GetValues<T>(null, propertySelector.GetMember().Name);
        }

        /// <summary>
        /// Recupera os valores da propriedade.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="propertySelector">Propriedade que será recuperada.</param>
        /// <returns></returns>
        public IEnumerable<GDAPropertyValue> GetValues<T>(GDASession session, System.Linq.Expressions.Expression<Func<T, object>> propertySelector)
        {
            var propertyName = propertySelector.GetMember().Name;
            foreach (var i in ToDataRecords<T>(session))
                yield return i[propertyName];
        }

#endif
		/// <summary>
		/// Recupera o resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
		public GDAList<T> ToList<T>() where T : new()
		{
			return GDAOperations.GetDAO<T>().Select(this);
		}

		/// <summary>
		/// Recupera o resultado da consulta.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
		public GDAList<T> ToList<T>(GDASession session) where T : new()
		{
			return GDAOperations.GetDAO<T>().Select(session, this);
		}

		/// <summary>
		/// Recupera o primeiro elemento da sequencia obedecendo as condições especificadas.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <returns>Valor do primeiro item encontrado.</returns>
		public T First<T>() where T : new()
		{
			return First<T>(null);
		}

		/// <summary>
		/// Recupera o primeiro elemento da sequencia obedecendo as condições especificadas.
		/// </summary>
		/// <typeparam name="T">Model que será tratada.</typeparam>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Valor do primeiro item encontrado.</returns>
		public T First<T>(GDASession session) where T : new()
		{
			foreach (var i in ToCursor<T>(session))
				return i;
			return default(T);
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de dados que será tratado na consulta.</typeparam>
		/// <returns>Consulta SQL gerada.</returns>
		public virtual QueryReturnInfo BuildResultInfo<T>()
		{
			return BuildResultInfo<T>(GDASettings.DefaultProviderConfiguration);
		}

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <typeparam name="T">Tipo de dados que será tratado na consulta.</typeparam>
		/// <returns>Consulta SQL gerada.</returns>
		public abstract QueryReturnInfo BuildResultInfo<T>(GDA.Interfaces.IProviderConfiguration configuration);

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <returns></returns>
		public abstract QueryReturnInfo BuildResultInfo(string aggregationFunction);

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <param name="provider">Provider que será utilizado no build.</param>
		/// <returns></returns>
		public abstract QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction);

		/// <summary>
		/// Constrói as informações para o resultado da consulta.
		/// </summary>
		/// <param name="provider">Provider que será utilizado no build.</param>
		/// <param name="aggregationFunction">Função de agregação usada para recuperar o resultado.</param>
		/// <param name="classesDictionary">Dicionário com as classe que já foram processadas.</param>
		/// <returns></returns>
		public abstract QueryReturnInfo BuildResultInfo2(GDA.Interfaces.IProvider provider, string aggregationFunction, Dictionary<string, Type> classesDictionary);
	}
}
