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

namespace GDA.Sql
{
	/// <summary>
	/// Implementação nativa.
	/// </summary>
	class NativeSelectStatementReferences : ISelectStatementReferences
	{
		private static NativeSelectStatementReferences _instance;

		/// <summary>
		/// Recupera a instancia unica da classe.
		/// </summary>
		public static NativeSelectStatementReferences Instance
		{
			get
			{
				if(_instance == null)
					_instance = new NativeSelectStatementReferences();
				return _instance;
			}
		}

		private NativeSelectStatementReferences()
		{
		}

		/// <summary>
		/// Recupera o nome da coluna mapeada para a propriedade do tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string GetPropertyMapping(GDA.Mapping.TypeInfo typeInfo, string propertyName)
		{
			var type = Type.GetType(typeInfo.FullnameWithAssembly);
			if(type == null)
				throw new QueryException(string.Format("Type {0} not found.", typeInfo.FullnameWithAssembly));
			var mapper = Caching.MappingManager.GetMappers(type).Find(f => f.PropertyMapperName == propertyName);
			return mapper != null ? mapper.Name : null;
		}

		/// <summary>
		/// Recupera a primeira propriedade chave mapeada para o tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		public string GetFirstKeyPropertyMapping(Mapping.TypeInfo typeInfo)
		{
			var mapper = Caching.MappingManager.GetMappers(Type.GetType(typeInfo.FullnameWithAssembly)).Find(f => f.ParameterType == PersistenceParameterType.Key || f.ParameterType == PersistenceParameterType.IdentityKey);
			return mapper != null ? mapper.Name : null;
		}

		/// <summary>
		/// Recupera as propriedades mapeadas.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		public IEnumerable<Mapping.IPropertyMappingInfo> GetPropertiesMapping(Mapping.TypeInfo typeInfo)
		{
			foreach (var f in Caching.MappingManager.GetMappers(Type.GetType(typeInfo.FullnameWithAssembly)))
				yield return (Mapping.IPropertyMappingInfo)new Mapping.PropertyMappingInfo {
					Name = f.PropertyMapperName,
					Column = f.Name,
					Direction = f.Direction,
					ParameterType = f.ParameterType
				};
		}

		/// <summary>
		/// Recupera o nome da tabela com base no tipo.
		/// </summary>
		/// <param name="typeInfo"></param>
		/// <returns></returns>
		public TableName GetTableName(GDA.Mapping.TypeInfo typeInfo)
		{
			var i = Caching.MappingManager.GetPersistenceClassAttribute(Type.GetType(typeInfo.FullnameWithAssembly));
			return i != null ? new TableName {
				Name = i.Name,
				Schema = i.Schema
			} : null;
		}

		/// <summary>
		/// Recupera a informação do tipo.
		/// </summary>
		/// <param name="tableInfo"></param>
		/// <returns></returns>
		public Mapping.TypeInfo GetTypeInfo(TableInfo tableInfo)
		{
			var type = Caching.MappingManager.LoadModel(tableInfo.TableName.Name);
			if(type == null)
			{
				if(string.IsNullOrEmpty(tableInfo.TableAlias))
					throw new GDAException("Not found type info for table {0}.", tableInfo.TableName.Name);
				return new Mapping.TypeInfo(tableInfo.TableAlias, null, null);
			}
			else
			{
				Caching.MappingManager.LoadClassMapper(type);
				return new Mapping.TypeInfo(type);
			}
		}
	}
}
