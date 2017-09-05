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

namespace GDA
{
    /// <summary>
    /// Representa uma lista de propriedade tratadas pelo GDA.
    /// </summary>
    public class GDAPropertySelector<T> : IEnumerable<System.Reflection.PropertyInfo>
    {
        private T _instance;

        /// <summary>
        /// Lista das propriedades relacionadas.
        /// </summary>
        private List<System.Reflection.PropertyInfo> _properties = new List<System.Reflection.PropertyInfo>();

        internal GDAPropertySelector(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _instance = instance;
        }

        /// <summary>
        /// Adiciona uma propriedade na lista.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <returns></returns>
        public GDAPropertySelector<T> Add(params System.Linq.Expressions.Expression<Func<T, object>>[] propertiesSelector)
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

        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public uint Insert()
        {
            return GDAOperations.Insert(_instance, this.ToString());
        }

        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public uint Insert(GDASession session)
        {
            return GDAOperations.Insert(session, _instance, this.ToString());
        }

        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public uint Insert(GDASession session, DirectionPropertiesName direction)
        {
            return GDAOperations.Insert(session, _instance, this.ToString(), direction);
        }

        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public int Update()
        {
            return GDAOperations.Update(_instance, this.ToString());
        }

        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public int Update(GDASession session)
        {
            return GDAOperations.Update(session, _instance, this.ToString());
        }

        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public int Update(GDASession session, DirectionPropertiesName direction)
        {
            return GDAOperations.Update(session, _instance, this.ToString(), direction);
        }

        public override string ToString()
        {
            return string.Join(",", _properties.Select(f => f.Name).Distinct().ToArray());
        }

        /// <summary>
        /// Converte implicitamente para um lista tipada.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>Lista tipada.</returns>
        public static implicit operator string(GDAPropertySelector<T> collection)
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
