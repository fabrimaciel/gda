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
using GDA.Mapping;

namespace GDA.Sql
{
	/// <summary>
	/// Implementação generica do ISelectStatementReferences.
	/// </summary>
	public class MappingSelectStatementReferences : ISelectStatementReferences
	{
		private Dictionary<string, ClassMapping> _mappings = new Dictionary<string, ClassMapping>();

		/// <summary>
		/// Recupera o mapeamento do classes.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public ClassMapping GetMapping(string className)
		{
			ClassMapping mapping = null;
			if(_mappings.TryGetValue(className, out mapping))
				return mapping;
			return null;
		}

		/// <summary>
		/// Recupera os mapeamentos do sistema.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ClassMapping> GetMappings()
		{
			return _mappings.Values;
		}

		/// <summary>
		/// Adiciona o mapeamento da classe.
		/// </summary>
		/// <param name="mapping"></param>
		public void AddMapping(ClassMapping mapping)
		{
			if(mapping == null)
				throw new ArgumentNullException("mapping");
			if(_mappings.ContainsKey(mapping.TypeInfo.Fullname))
				_mappings.Remove(mapping.TypeInfo.Name);
			_mappings.Add(mapping.TypeInfo.Fullname, mapping);
		}

		public string GetPropertyMapping(GDA.Mapping.TypeInfo typeInfo, string propertyName)
		{
			ClassMapping mapping = null;
			if(_mappings.TryGetValue(typeInfo.Fullname, out mapping))
			{
				var property = mapping.Properties.Find(f => f.Name == propertyName);
				return property != null ? property.Column : null;
			}
			return null;
		}

		/// <summary>
		/// Recupera a primeira propriedade chave mapeada para o tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		public string GetFirstKeyPropertyMapping(Mapping.TypeInfo typeInfo)
		{
			ClassMapping mapping = null;
			if(_mappings.TryGetValue(typeInfo.Fullname, out mapping))
			{
				var property = mapping.Properties.Find(f => f.ParameterType == PersistenceParameterType.Key || f.ParameterType == PersistenceParameterType.IdentityKey);
				return property != null ? property.Column : null;
			}
			return null;
		}

		/// <summary>
		/// Recupera as propriedades mapeadas.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		public IEnumerable<IPropertyMappingInfo> GetPropertiesMapping(Mapping.TypeInfo typeInfo)
		{
			ClassMapping mapping = null;
			if(_mappings.TryGetValue(typeInfo.Fullname, out mapping))
			{
				foreach (var i in mapping.Properties)
					yield return (IPropertyMappingInfo)i;
			}
		}

		public TableName GetTableName(GDA.Mapping.TypeInfo typeInfo)
		{
			ClassMapping mapping = null;
			if(_mappings.TryGetValue(typeInfo.Fullname, out mapping))
				return new TableName {
					Name = mapping.Table,
					Schema = mapping.Schema
				};
			return null;
		}

		public GDA.Mapping.TypeInfo GetTypeInfo(TableInfo tableInfo)
		{
			return new TypeInfo(tableInfo.TableName.Name);
		}
	}
}
