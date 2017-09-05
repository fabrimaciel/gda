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
using GDA.Interfaces;
using System.Reflection;

namespace GDA.Caching
{
	public class MappingManager
	{
		/// <summary>
		/// Armazena o resutlado das propriedades de persistencia.
		/// </summary>
		internal class LoadPersistencePropertyAttributesResult
		{
			/// <summary>
			/// Mapeamento das propriedades.
			/// </summary>
			public List<Mapper> Mappers;

			/// <summary>
			/// Mapeamento das chaves estrangeiras
			/// </summary>
			public List<GroupOfRelationshipInfo> ForeignKeyMappers;

			/// <summary>
			/// Mapeamento das membros estrangeiros.
			/// </summary>
			public List<ForeignMemberMapper> ForeignMembers;
		}

		/// <summary>
		/// Dicionário que armazena as DAO relacionadas as models
		/// </summary>
		internal static Dictionary<Type, ISimpleBaseDAO> MembersDAO = new Dictionary<Type, ISimpleBaseDAO>();

		/// <summary>
		/// Dicionário que armazena os namespaces já carregados, juntamento com as tipos das models já carregadas.
		/// </summary>
		internal static Dictionary<string, Dictionary<string, string>> ModelsNamespaceLoaded = new Dictionary<string, Dictionary<string, string>>();

		/// <summary>
		/// Matriz que identifica a quantidade de acessos ao mapeamento. 
		/// </summary>
		private static int[] _accessModelsMapper;

		/// <summary>
		/// Armazena o nomes da models mapeados na ordem correspondente a matriz accessModelsMapper.
		/// </summary>
		private static string[] _typeNamesModelsMapper;

		/// <summary>
		/// Armazena as lista de mapeamentos das models
		/// </summary>
		private static List<Mapper>[] _listsModelsMapper;

		/// <summary>
		/// Armazena as listas de foreignkeys mapeadas.
		/// </summary>
		private static List<GroupOfRelationshipInfo>[] _listForeignKeyMapper;

		/// <summary>
		/// Armazena as lista de foreign members mapeados.
		/// </summary>
		private static List<ForeignMemberMapper>[] _listForeignMemberMapper;

		private static object _loadPersistencePropertyAttributesLock = new object();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		static MappingManager()
		{
			try
			{
				GDASettings.LoadConfiguration();
			}
			catch
			{
			}
		}

		internal static PersistenceBaseDAOAttribute GetPersistenceBaseDAOAttribute(Type type)
		{
			object[] obj = type.GetCustomAttributes(typeof(PersistenceBaseDAOAttribute), false);
			if(obj.Length == 0)
			{
				var mapping = Mapping.MappingData.GetMapping(type);
				return mapping != null && mapping.BaseDAO != null ? mapping.BaseDAO.GetPersistenceBaseDAO() : null;
			}
			else
				return (PersistenceBaseDAOAttribute)obj[0];
		}

		/// <summary>
		/// Captura os dados do provedor do tipo.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static PersistenceProviderAttribute GetPersistenceProviderAttribute(Type type)
		{
			object[] obj = type.GetCustomAttributes(typeof(PersistenceProviderAttribute), true);
			if(obj.Length == 0)
			{
				var mapping = Mapping.MappingData.GetMapping(type);
				return mapping != null && mapping.Provider != null ? mapping.Provider.GetPersistenceProvider() : null;
			}
			else
				return (PersistenceProviderAttribute)obj[0];
		}

		/// <summary>
		/// Captura o <see cref="PersistenceClassAttribute"/> contido o type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>null se não for encontrado o attributo.</returns>
		internal static PersistenceClassAttribute GetPersistenceClassAttribute(Type type)
		{
			object[] obj = type.GetCustomAttributes(typeof(PersistenceClassAttribute), true);
			if(obj.Length == 0)
			{
				var mapping = Mapping.MappingData.GetMapping(type);
				if(mapping == null && type.BaseType != typeof(object))
					mapping = Mapping.MappingData.GetMapping(type.BaseType);
				if(mapping == null)
					foreach (var i in type.GetInterfaces())
					{
						mapping = Mapping.MappingData.GetMapping(i);
						if(mapping != null)
							break;
					}
				return mapping != null ? mapping.GetPersistenceClass() : null;
			}
			else
				return (PersistenceClassAttribute)obj[0];
		}

		/// <summary>
		/// Captura a PersistencePropertyAttribute relacionado com a propriedade.
		/// </summary>
		/// <param name="pInfo">Propriedade.</param>
		/// <returns>O atributo ou null se não for localizado.</returns>
		internal static PersistencePropertyAttribute GetPersistenceProperty(PropertyInfo pInfo)
		{
			object[] obj = pInfo.GetCustomAttributes(typeof(PersistencePropertyAttribute), true);
			if(obj != null && obj.Length > 0)
			{
				if(string.IsNullOrEmpty(((PersistencePropertyAttribute)obj[0]).Name))
					((PersistencePropertyAttribute)obj[0]).Name = pInfo.Name;
				return (PersistencePropertyAttribute)obj[0];
			}
			else
			{
				Type modelType = pInfo.DeclaringType;
				var mapping = Mapping.MappingData.GetMapping(pInfo.DeclaringType);
				if(mapping == null && pInfo.DeclaringType != pInfo.ReflectedType)
				{
					mapping = Mapping.MappingData.GetMapping(pInfo.ReflectedType);
					modelType = pInfo.ReflectedType;
				}
				if(mapping != null)
				{
					do
					{
						GDA.Mapping.PropertyMapping p = null;
						if(mapping != null)
							p = mapping.Properties.Find(f => f.Name == pInfo.Name);
						if(p != null)
							return p.GetPersistenceProperty();
						else
						{
							modelType = modelType.BaseType;
							mapping = Mapping.MappingData.GetMapping(modelType);
						}
					}
					while (modelType != typeof(object));
				}
			}
			return null;
		}

		/// <summary>
		/// Recupera as informações do mapeamento da propriedade da tipo especificado.
		/// </summary>
		/// <param name="typeModel"></param>
		/// <param name="propertyName">Nome da propriedade onde o mapeamento está contido.</param>
		/// <returns></returns>
		internal static Mapper GetPropertyMapper(Type typeModel, string propertyName)
		{
			var propertyAttributes = MappingManager.LoadPersistencePropertyAttributes(typeModel);
			return propertyAttributes.Mappers.Find(delegate(Mapper m) {
				return m.PropertyMapperName == propertyName;
			});
		}

		/// <summary>
		/// Carrega a lisa de propriedades mapeadas para o tipo passado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>Posição do mapeamento no vetor</returns>
		internal static LoadPersistencePropertyAttributesResult LoadPersistencePropertyAttributes(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			int pos = -1;
			List<Mapper> mapping = null;
			List<GroupOfRelationshipInfo> fkMapping = null;
			List<ForeignMemberMapper> fmMapping = null;
			lock (_loadPersistencePropertyAttributesLock)
			{
				if(_accessModelsMapper == null)
				{
					_accessModelsMapper = new int[GDASettings.MaximumMapperCache];
					_typeNamesModelsMapper = new string[GDASettings.MaximumMapperCache];
					_listsModelsMapper = new List<Mapper>[GDASettings.MaximumMapperCache];
					_listForeignKeyMapper = new List<GroupOfRelationshipInfo>[GDASettings.MaximumMapperCache];
					_listForeignMemberMapper = new List<ForeignMemberMapper>[GDASettings.MaximumMapperCache];
					for(int i = 0; i < _accessModelsMapper.Length; i++)
						_accessModelsMapper[i] = -1;
				}
				for(int i = 0; i < _typeNamesModelsMapper.Length; i++)
					if(_typeNamesModelsMapper[i] != null && _typeNamesModelsMapper[i] == type.FullName)
					{
						_accessModelsMapper[i]++;
						pos = i;
					}
					else
					{
						if(_accessModelsMapper[i] > int.MinValue)
							_accessModelsMapper[i]--;
					}
				if(pos >= 0)
				{
					return new LoadPersistencePropertyAttributesResult {
						Mappers = _listsModelsMapper[pos],
						ForeignKeyMappers = _listForeignKeyMapper[pos],
						ForeignMembers = _listForeignMemberMapper[pos]
					};
				}
				mapping = new List<Mapper>();
				fkMapping = new List<GroupOfRelationshipInfo>();
				fmMapping = new List<ForeignMemberMapper>();
				pos = 0;
				for(int i = 1; i < _accessModelsMapper.Length; i++)
					if(_accessModelsMapper[i] < _accessModelsMapper[pos])
						pos = i;
				if(pos != -1 && _listsModelsMapper[pos] != null)
				{
				}
			}
			LoadPersistencePropertyAttributes(type, mapping, fkMapping, fmMapping);
			lock (_loadPersistencePropertyAttributesLock)
			{
				_accessModelsMapper[pos] = 10;
				_typeNamesModelsMapper[pos] = type.FullName;
				_listsModelsMapper[pos] = mapping;
				_listForeignKeyMapper[pos] = fkMapping;
				_listForeignMemberMapper[pos] = fmMapping;
			}
			return new LoadPersistencePropertyAttributesResult {
				Mappers = mapping,
				ForeignKeyMappers = fkMapping,
				ForeignMembers = fmMapping
			};
		}

		/// <summary>
		/// Carrega a lista das propriedades mapeadas.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="mapping">Lista para armazenar a propriedades mapeadas.</param>
		/// <param name="fkMapping"></param>
		/// <param name="fmMapping">Mapeamento dos membros estrangeiros.</param>
		private static void LoadPersistencePropertyAttributes(Type type, List<Mapper> mapping, List<GroupOfRelationshipInfo> fkMapping, List<ForeignMemberMapper> fmMapping)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			var classMapping = Mapping.MappingData.GetMapping(type);
			foreach (PropertyInfo property in properties)
			{
				Mapping.PropertyMapping pMapping = null;
				if(classMapping != null)
					pMapping = classMapping.Properties.Find(f => f.Name == property.Name);
				PersistencePropertyAttribute ppa = GetPersistenceProperty(property);
				Mapper currentMapper = null;
				if(ppa != null)
				{
					currentMapper = new Mapper(type, ppa, property);
					mapping.Add(currentMapper);
				}
				object[] attrs = property.GetCustomAttributes(typeof(PersistenceForeignKeyAttribute), true);
				if((attrs == null || attrs.Length == 0) && pMapping != null && pMapping.ForeignKey != null)
					attrs = new object[] {
						pMapping.ForeignKey.GetPersistenceForeignKey()
					};
				foreach (object obj in attrs)
				{
					PersistenceForeignKeyAttribute fk = (PersistenceForeignKeyAttribute)obj;
					GroupOfRelationshipInfo group = new GroupOfRelationshipInfo(type, fk.TypeOfClassRelated, fk.GroupOfRelationship);
					int index = fkMapping.FindIndex(delegate(GroupOfRelationshipInfo gri) {
						return gri == group;
					});
					if(index < 0)
					{
						fkMapping.Add(group);
					}
					else
						group = fkMapping[index];
					ForeignKeyMapper fkm = new ForeignKeyMapper(fk, property);
					group.AddForeignKey(fkm);
					if(currentMapper != null)
						currentMapper.AddForeignKey(fkm);
				}
				attrs = property.GetCustomAttributes(typeof(ValidatorAttribute), true);
				if((attrs == null || attrs.Length == 0) && pMapping != null && pMapping.Validators.Count > 0)
				{
					attrs = new object[pMapping.Validators.Count];
					for(int i = 0; i < attrs.Length; i++)
						attrs[i] = pMapping.Validators[i].GetValidator();
				}
				if(attrs.Length > 0)
				{
					ValidatorAttribute[] vAttrs = new ValidatorAttribute[attrs.Length];
					for(int i = 0; i < attrs.Length; i++)
						vAttrs[i] = (ValidatorAttribute)attrs[i];
					currentMapper.Validation = new ValidationMapper(currentMapper, vAttrs);
				}
				attrs = property.GetCustomAttributes(typeof(PersistenceForeignMemberAttribute), true);
				if((attrs == null || attrs.Length == 0) && pMapping != null && pMapping.ForeignMember != null)
					attrs = new object[] {
						pMapping.ForeignMember.GetPersistenceForeignMember()
					};
				if(attrs.Length > 0)
				{
					fmMapping.Add(new ForeignMemberMapper((PersistenceForeignMemberAttribute)attrs[0], property));
				}
			}
			if(!type.IsInterface)
			{
				Type[] interfaces = type.GetInterfaces();
				PersistenceClassAttribute pca;
				for(int i = 0; i < interfaces.Length; i++)
				{
					pca = GetPersistenceClassAttribute(interfaces[i]);
					if(pca != null)
					{
						LoadPersistencePropertyAttributes(interfaces[i], mapping, fkMapping, fmMapping);
					}
				}
			}
		}

		/// <summary>
		/// Carrega o tipo relacionado ao nome do tipo da model passado.
		/// </summary>
		/// <param name="modelTypeName">Nome do tipo da model</param>
		internal static Type LoadModel(string modelTypeName)
		{
			GDASettings.LoadConfiguration();
			string tReturn = null;
			foreach (KeyValuePair<string, Dictionary<string, string>> k in ModelsNamespaceLoaded)
			{
				if(k.Value.TryGetValue(modelTypeName, out tReturn))
					return Type.GetType(tReturn);
			}
			foreach (ModelsNamespaceInfo ns in GDASettings.ModelsNamespaces)
			{
				string nn = modelTypeName;
				if(nn.IndexOf(ns.Namespace) < 0)
					nn = ns.Namespace + "." + modelTypeName;
				var currentAssembly = ns.CurrentAssembly;
				if(currentAssembly == null)
					throw new GDAException("Fail on load assembly \"{0}\" for model namespace.", ns.AssemblyName);
				Type t = ns.CurrentAssembly.GetType(nn);
				if(t == null)
					t = ns.CurrentAssembly.GetType(modelTypeName);
				if(t != null)
				{
					if(!ModelsNamespaceLoaded.ContainsKey(ns.Namespace))
						ModelsNamespaceLoaded.Add(ns.Namespace, new Dictionary<string, string>());
					ModelsNamespaceLoaded[ns.Namespace].Add(modelTypeName, t.FullName + ", " + t.Assembly.FullName);
					return t;
				}
			}
			Type rt = null;
			#if PocketPC
			            if (Assembly.GetExecutingAssembly() != null)
                rt = Assembly.GetExecutingAssembly().GetType(modelTypeName);
#else
			var entry = Assembly.GetEntryAssembly();
			if(entry != null)
				rt = entry.GetType(modelTypeName);
			else
			{
				entry = Assembly.GetCallingAssembly();
				if(entry != null)
					rt = entry.GetType(modelTypeName);
				else
				{
					entry = Assembly.GetExecutingAssembly();
					if(entry != null)
						rt = entry.GetType(modelTypeName);
				}
			}
			#endif
			if(rt != null)
			{
				GDASettings.AddModelsNamespace(rt.Assembly, rt.Namespace);
				if(!ModelsNamespaceLoaded.ContainsKey(rt.Namespace))
					ModelsNamespaceLoaded.Add(rt.Namespace, new Dictionary<string, string>());
				ModelsNamespaceLoaded[rt.Namespace].Add(modelTypeName, rt.FullName);
				return rt;
			}
			return Type.GetType(modelTypeName, false);
		}

		/// <summary>
		/// Recupera do tipo o nome da tabela que ele representa.
		/// </summary>
		/// <param name="type">Tipo a ser usado para localizar o nome</param>
		/// <returns>Nome da tabela relacionada</returns>
		/// <exception cref="GDATableNameRepresentNotExistsException"></exception>
		internal static GDA.Sql.TableName GetTableName(Type type)
		{
			var param = GetPersistenceClassAttribute(type);
			if(param == null || string.IsNullOrEmpty(param.Name))
			{
				Type[] interfaces = type.GetInterfaces();
				PersistenceClassAttribute pca;
				for(int i = 0; i < interfaces.Length; i++)
				{
					pca = MappingManager.GetPersistenceClassAttribute(interfaces[i]);
					if(pca != null && pca.Name != null)
						return pca.GetTableName();
				}
				throw new GDATableNameRepresentNotExistsException("The class: " + type.FullName + ", not found PersistenceClassAttribute");
			}
			return param.GetTableName();
		}

		/// <summary>
		/// Recupera os mapeamentos da model.
		/// </summary>
		/// <typeparam name="Model">Tipo da model onde o mepeamento está contido.</typeparam>
		/// <returns></returns>
		internal static IList<Mapper> GetMappers<Model>()
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(typeof(Model));
			return persistencePropertyAttributes.Mappers;
		}

		/// <summary>
		/// Recupera os mapeamentos de membros estrangeiros da model.
		/// </summary>
		/// <param name="modelType">Tipo da model onde o mepeamento está contido.</param>
		/// <returns></returns>
		internal static IList<ForeignMemberMapper> GetForeignMemberMapper(Type modelType)
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(modelType);
			return persistencePropertyAttributes.ForeignMembers;
		}

		/// <summary>
		/// Recupera os mapeamentos da model.
		/// </summary>
		/// <param name="modelType">Tipo da model onde o mepeamento está contido.</param>
		/// <returns></returns>
		internal static List<Mapper> GetMappers(Type modelType)
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(modelType);
			return persistencePropertyAttributes.Mappers;
		}

		/// <summary>
		/// Captura os attributes PersistenceProperty das propriedades da classe refenciada.
		/// </summary>
		/// <typeparam name="Model">Tipo do modelo de onde será recuperado o mapeamento.</typeparam>
		/// <param name="typesParam">Tipos de parametros a serem filtrados. null para não se aplicar nenhum filtro.</param>
		/// <param name="directions">Sentido dos atributos a serem filtrados. Default Input, InputOutput</param>
		/// <returns>Lista com todas os atributos, obedecendo o filtro.</returns>
		internal static List<Mapper> GetMappers<Model>(PersistenceParameterType[] typesParam, DirectionParameter[] directions)
		{
			return GetMappers<Model>(typesParam, directions, false);
		}

		/// <summary>
		/// Captura os attributes PersistenceProperty das propriedades da classe refenciada.
		/// </summary>
		/// <typeparam name="Model">Tipo do modelo de onde será recuperado o mapeamento.</typeparam>
		/// <param name="typesParam">Tipos de parametros a serem filtrados. null para não se aplicar nenhum filtro.</param>
		/// <param name="directions">Sentido dos atributos a serem filtrados. Default Input, InputOutput</param>
		/// <param name="returnFirstFound">True para retorna o primeiro valor encontrado.</param>
		/// <returns>Lista com todas os atributos, obedecendo o filtro.</returns>
		internal static List<Mapper> GetMappers<Model>(PersistenceParameterType[] typesParam, DirectionParameter[] directions, bool returnFirstFound)
		{
			return GetMappers(typeof(Model), typesParam, directions, returnFirstFound);
		}

		/// <summary>
		/// Captura os attributes PersistenceProperty das propriedades da classe refenciada.
		/// </summary>
		/// <param name="typeModel">Tipo do modelo de onde será recuperado o mapeamento.</param>
		/// <param name="typesParam">Tipos de parametros a serem filtrados. null para não se aplicar nenhum filtro.</param>
		/// <param name="directions">Sentido dos atributos a serem filtrados. Default Input, InputOutput</param>
		/// <returns>Lista com todas os atributos, obedecendo o filtro.</returns>
		internal static List<Mapper> GetMappers(Type typeModel, PersistenceParameterType[] typesParam, DirectionParameter[] directions)
		{
			return GetMappers(typeModel, typesParam, directions, false);
		}

		/// <summary>
		/// Captura os attributes PersistenceProperty das propriedades da classe refenciada.
		/// </summary>
		/// <param name="typeModel">Tipo do modelo de onde será recuperado o mapeamento.</param>
		/// <param name="typesParam">Tipos de parametros a serem filtrados. null para não se aplicar nenhum filtro.</param>
		/// <param name="directions">Sentido dos atributos a serem filtrados. Default Input, InputOutput</param>
		/// <param name="returnFirstFound">True para retorna o primeiro valor encontrado.</param>
		/// <returns>Lista com todas os atributos, obedecendo o filtro.</returns>
		public static List<Mapper> GetMappers(Type typeModel, PersistenceParameterType[] typesParam, DirectionParameter[] directions, bool returnFirstFound)
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(typeModel);
			List<Mapper> result = new List<Mapper>();
			var propertiesMapped = persistencePropertyAttributes.Mappers;
			if(directions == null)
				directions = new DirectionParameter[] {
					DirectionParameter.Input,
					DirectionParameter.InputOptionalOutput,
					DirectionParameter.InputOutput,
					DirectionParameter.OutputOnlyInsert,
					DirectionParameter.InputOptional,
					DirectionParameter.InputOptionalOutput
				};
			bool itemFound;
			if(propertiesMapped != null)
				foreach (Mapper mapper in propertiesMapped)
				{
					if(mapper == null)
						continue;
					if(typesParam != null)
					{
						itemFound = false;
						foreach (PersistenceParameterType ppt in typesParam)
							if(ppt == mapper.ParameterType)
							{
								itemFound = true;
								break;
							}
						if(!itemFound)
							continue;
					}
					if(directions != null)
					{
						itemFound = false;
						foreach (DirectionParameter dp in directions)
							if(dp == mapper.Direction)
							{
								itemFound = true;
								break;
							}
						if(!itemFound)
							continue;
					}
					result.Add(mapper);
					if(returnFirstFound)
						break;
				}
			return result;
		}

		/// <summary>
		/// Carrega o mapeamento da classe.
		/// </summary>
		/// <param name="typeModel">Tipo da model.</param>
		internal static void LoadClassMapper(Type typeModel)
		{
			LoadPersistencePropertyAttributes(typeModel);
		}

		/// <summary>
		/// Recupera a lista das chaves estrangeiras da model.
		/// </summary>
		/// <param name="typeModel">Tipo da model onde estão registradas as chaves estrangeiras.</param>
		/// <returns>Dicionario contendo as informações das chaves estrangeiras.</returns>
		internal static List<GroupOfRelationshipInfo> GetForeignKeyAttributes(Type typeModel)
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(typeModel);
			return (persistencePropertyAttributes.ForeignKeyMappers == null ? new List<GroupOfRelationshipInfo>() : persistencePropertyAttributes.ForeignKeyMappers);
		}

		/// <summary>
		/// Carrega os relacionamentos do grupo de chaves estrangeiras informado.
		/// </summary>
		/// <param name="info">Informações do grupo das chaves estrangeiras.</param>
		/// <returns>Lista da com os relacionamentos da foreignKey.</returns>
		internal static List<ForeignKeyMapper> LoadRelationships(Type typeModel, GroupOfRelationshipInfo info)
		{
			List<GroupOfRelationshipInfo> groups = MappingManager.GetForeignKeyAttributes(typeModel);
			int index = groups.FindIndex(delegate(GroupOfRelationshipInfo gri) {
				return gri == info;
			});
			if(index >= 0)
				return groups[index].ForeignKeys;
			else
				return new List<ForeignKeyMapper>();
		}

		/// <summary>
		/// Verifica se no mapeamento do model existe algum campo identidade.
		/// </summary>
		/// <param name="typeModel">Tipo da model onde será feita a consulta.</param>
		/// <returns></returns>
		internal static bool CheckExistsIdentityKey(Type typeModel)
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(typeModel);
			List<Mapper> propertiesMapped = persistencePropertyAttributes.Mappers;
			if(propertiesMapped.FindIndex(delegate(Mapper m) {
				return m.ParameterType == PersistenceParameterType.IdentityKey;
			}) >= 0)
				return true;
			return false;
		}

		/// <summary>
		/// Recupera a propriedade identidade do mapeamento.
		/// </summary>
		/// <param name="typeModel">Tipo do modelo mapeado.</param>
		/// <returns></returns>
		internal static Mapper GetIdentityKey(Type typeModel)
		{
			var persistencePropertyAttributes = MappingManager.LoadPersistencePropertyAttributes(typeModel);
			List<Mapper> propertiesMapped = persistencePropertyAttributes.Mappers;
			return propertiesMapped.Find(delegate(Mapper m) {
				return m.ParameterType == PersistenceParameterType.IdentityKey;
			});
		}
	}
}
