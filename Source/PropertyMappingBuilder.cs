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
using System.Linq.Expressions;

namespace GDA.Mapping
{
    /// <summary>
    /// Classe que auxilia na construção do mapeamento das propriedades.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyMappingBuilder<T> : IEnumerable<PropertyMapping>
    {
        private List<PropertyMapping> _properties = new List<PropertyMapping>();

        /// <summary>
        /// Adiciona as propridades para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <returns></returns>
        public static PropertyMappingBuilder<T> Create(params Expression<Func<T, object>>[] propertiesSelector)
        {
            return new PropertyMappingBuilder<T>().Add(propertiesSelector);
        }

        /// <summary>
        /// Adiciona as propridades para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <returns></returns>
        public PropertyMappingBuilder<T> Add(params Expression<Func<T, object>>[] propertiesSelector)
        {
            if (propertiesSelector == null)
                throw new ArgumentNullException("propertiesSelector");

            foreach(var i in propertiesSelector.Where(f => f != null))
                _properties.Add(new PropertyMapping(GDA.Extensions.GetMember(i).Name, null, PersistenceParameterType.Field, 0, false, false, DirectionParameter.InputOutput, null, null, null, null));

            return this;
        }

        /// <summary>
        /// Adiciona a propriedade para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public PropertyMappingBuilder<T> Add(Expression<Func<T, object>> propertySelector, string column)
        {
            if (propertySelector == null)
                throw new ArgumentNullException("propertySelector");

            _properties.Add(new PropertyMapping(GDA.Extensions.GetMember(propertySelector).Name, column, PersistenceParameterType.Field, 0, false, false, DirectionParameter.InputOutput, null, null, null, null));

            return this;
        }

        /// <summary>
        /// Adiciona a propriedade para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public PropertyMappingBuilder<T> Add(Expression<Func<T, object>> propertySelector, string column, DirectionParameter direction)
        {
            if (propertySelector == null)
                throw new ArgumentNullException("propertySelector");

            _properties.Add(new PropertyMapping(GDA.Extensions.GetMember(propertySelector).Name, column, PersistenceParameterType.Field, 0, false, false, direction, null, null, null, null));

            return this;
        }

        /// <summary>
        /// Adiciona a propriedade para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public PropertyMappingBuilder<T> Add(Expression<Func<T, object>> propertySelector, string column, PersistenceParameterType parameterType)
        {
            if (propertySelector == null)
                throw new ArgumentNullException("propertySelector");

            _properties.Add(new PropertyMapping(GDA.Extensions.GetMember(propertySelector).Name, column, parameterType, 0, false, false, DirectionParameter.InputOutput, null, null, null, null));

            return this;
        }

        /// <summary>
        /// Adiciona a propriedade para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public PropertyMappingBuilder<T> Add(Expression<Func<T, object>> propertySelector, DirectionParameter direction)
        {
            if (propertySelector == null)
                throw new ArgumentNullException("propertySelector");

            _properties.Add(new PropertyMapping(GDA.Extensions.GetMember(propertySelector).Name, null, PersistenceParameterType.Field, 0, false, false, direction, null, null, null, null));

            return this;
        }

         /// <summary>
        /// Adiciona a propriedade para o mapeamento.
        /// </summary>
        /// <param name="propertiesSelector"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public PropertyMappingBuilder<T> Add(PropertyMapping mapping)
        {
            if (mapping == null)
                throw new ArgumentNullException("mapping");

            _properties.Add(mapping);

            return this;
        }

        public IEnumerator<PropertyMapping> GetEnumerator()
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
