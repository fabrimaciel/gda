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

#if CLS_3_5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA.Collections;

namespace GDA.Sql
{
    /// <summary>
    /// Implementação do seletor de propriedades da consulta.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryPropertySelector<T> : IEnumerable<System.Reflection.PropertyInfo> where T : new()
    {
        private Query _query;

        /// <summary>
        /// Lista das propriedades relacionadas.
        /// </summary>
        private List<System.Reflection.PropertyInfo> _properties = new List<System.Reflection.PropertyInfo>();

        /// <summary>
        /// Recupera a instancia da query.
        /// </summary>
        /// <returns></returns>
        public Query Query
        {
            get
            {
                _query.Select(this.ToString());
                return _query;
            }
        }

        internal QueryPropertySelector(Query query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            _query = query;
        }

        /// <summary>
        /// Adiciona uma propriedade na lista.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <returns></returns>
        public QueryPropertySelector<T> Add(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
        {
            if (propertiesSelector != null)
            {
                foreach (var i in propertiesSelector)
                {
                    if (i == null) continue;
                    var property = i.GetMember() as System.Reflection.PropertyInfo;
                    if (property != null)
                        _properties.Add(property);
                }
            }

            return this;
        }

        public override string ToString()
        {
            return string.Join(",", _properties.Select(f => f.Name).Distinct().ToArray());
        }

        /// <summary>
        /// Recupera o resultado da consulta em forma de cursor.
        /// </summary>        
        /// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
        public GDACursor<T> ToCursor()
        {
            return Query.ToCursor<T>();
        }

        /// <summary>
        /// Recupera o resultado da consulta em forma de cursor.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
        public GDACursor<T> ToCursor(GDASession session)
        {
            return Query.ToCursor<T>(session);
        }

        /// <summary>
        /// Recipera o resultado da consulta em forma de cursor e recupera o resultado 
        /// em objetos de outro tipo informado.
        /// </summary>
        /// <typeparam name="Result">Tipos que estarão no retorno.</typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public virtual IEnumerable<Result> ToCursor<Result>(GDASession session)            
            where Result : new()
        {
            return Query.ToCursor<T, Result>(session);
        }

        /// <summary>
        /// Recipera o resultado da consulta em forma de cursor e recupera o resultado 
        /// em objetos de outro tipo informado.
        /// </summary>
        /// <typeparam name="Result">Tipos que estarão no retorno.</typeparam>        
        /// <returns></returns>
        public virtual IEnumerable<Result> ToCursor<Result>()
            where Result : new()
        {
            return Query.ToCursor<T, Result>(null);
        }

        /// <summary>
        /// Recupera o resultado da consulta em forma de cursor.
        /// </summary>
        /// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
        public GDADataRecordCursor<T> ToDataRecords()
        {
            return Query.ToDataRecords<T>();
        }

        /// <summary>
        /// Recupera o resultado da consulta em forma de cursor.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <returns>Lista dos registros recuperados com base nos parametros informados.</returns>
        public GDADataRecordCursor<T> ToDataRecords(GDASession session)
        {
            return Query.ToDataRecords<T>(session);
        }

        /// <summary>
        /// Recupera o resultado da consulta.
        /// </summary>
        /// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
        public GDAList<T> ToList()
        {
            return Query.ToList<T>();
        }

        /// <summary>
        /// Recupera o resultado da consulta.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <returns>Lista dos elementos recuperados com base nos parametros informados.</returns>
        public GDAList<T> ToList(GDASession session)
        {
            return Query.ToList<T>(session);
        }

        /// <summary>
        /// Recupera o <see cref="GDA.Sql.ResultList"/> do resultado da consulta.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResultList<T> ToResultList(int pageSize)
        {
            return Query.ToResultList<T>(pageSize);
        }

        /// <summary>
        /// Recupera o <see cref="GDA.Sql.ResultList"/> do resultado da consulta.
        /// </summary>
        /// <typeparam name="T">Model que será tratada.</typeparam>
        /// <param name="session"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ResultList<T> ToResultList(GDASession session, int pageSize)
        {
            return Query.ToResultList<T>(session, pageSize);
        }

        /// <summary>
        /// Converte implicitamente para um lista tipada.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>Lista tipada.</returns>
        public static implicit operator string(QueryPropertySelector<T> collection)
        {
            if (collection != null)
                return collection.ToString();
            else
                return null;
        }

        public IEnumerator<System.Reflection.PropertyInfo> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

    }
}
#endif
