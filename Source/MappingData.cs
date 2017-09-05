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
using System.Xml;

namespace GDA.Mapping
{
	/// <summary>
	/// Armazena os dados do mapeamento.
	/// </summary>
	public class MappingData
	{
		private static object objLock = new object();

		private static Dictionary<string, ClassMapping> _classes = new Dictionary<string, ClassMapping>();

		private static Dictionary<string, SqlQueryMapping> _queries = new Dictionary<string, SqlQueryMapping>();

		/// <summary>
		/// Relação das referencias já carregadas.
		/// </summary>
		private static List<ReferenceMapping> _references = new List<ReferenceMapping>();

		/// <summary>
		/// Recupera o mapeamento do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ClassMapping GetMapping(Type type)
		{
			if(type == null)
				return null;
			ClassMapping mapping = null;
			if(_classes.TryGetValue(type.FullName, out mapping))
				return mapping;
			return null;
		}

		/// <summary>
		/// Recupera o mapeamento do classes.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public static ClassMapping GetMapping(string className)
		{
			ClassMapping mapping = null;
			if(_classes.TryGetValue(className, out mapping))
				return mapping;
			return null;
		}

		/// <summary>
		/// Recupera os mapeamentos do sistema.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ClassMapping> GetMappings()
		{
			return _classes.Values;
		}

		/// <summary>
		/// Adiciona o mapeamento da classe.
		/// </summary>
		/// <param name="mapping"></param>
		public static void AddMapping(ClassMapping mapping)
		{
			if(mapping == null)
				throw new ArgumentNullException("mapping");
			if(_classes.ContainsKey(mapping.TypeInfo.Fullname))
				_classes.Remove(mapping.TypeInfo.Name);
			_classes.Add(mapping.TypeInfo.Fullname, mapping);
		}

		/// <summary>
		/// Remove o mapeamento.
		/// </summary>
		/// <param name="typeInfoFullname">Nome do tipo mapeado.</param>
		public static void RemoteMapping(string typeInfoFullname)
		{
			if(_classes.ContainsKey(typeInfoFullname))
				_classes.Remove(typeInfoFullname);
		}

		/// <summary>
		/// Recupera a consulta SQL mapeada.
		/// </summary>
		/// <param name="queryName"></param>
		/// <returns></returns>
		public static SqlQueryMapping GetSqlQuery(string queryName)
		{
			SqlQueryMapping mapping = null;
			if(_queries.TryGetValue(queryName, out mapping))
				return mapping;
			return null;
		}

		/// <summary>
		/// Recupera as consultas mapeadas no sistema.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<SqlQueryMapping> GetSqlQueries()
		{
			return _queries.Values;
		}

		/// <summary>
		/// Adiciona o mapeamento de uma consulta no sistema.
		/// </summary>
		/// <param name="query">Dados da consulta.</param>
		public static void AddSqlQuery(SqlQueryMapping query)
		{
			if(query == null)
				throw new ArgumentNullException("query");
			if(_queries.ContainsKey(query.Name))
				_queries.Remove(query.Name);
			_queries.Add(query.Name, query);
		}

		/// <summary>
		/// Remove a consulta SQL mapeada no sistema.
		/// </summary>
		/// <param name="queryName">Nome da consulta.</param>
		public static void RemoteSqlQuery(string queryName)
		{
			if(_queries.ContainsKey(queryName))
				_queries.Remove(queryName);
		}

		/// <summary>
		/// Importa os dados do mapeamento.
		/// </summary>
		/// <param name="assemblyString">Assembly onde o mapeamento está inserido.</param>
		/// <param name="resourceName"></param>
		public static void Import(string assemblyString, string resourceName)
		{
			var assembly = System.Reflection.Assembly.Load(assemblyString);
			if(assembly == null)
				return;
			using (System.IO.Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
			{
				if(resourceStream == null)
					throw new GDAMappingException("Not found resource \"{0}\" in \"{1}\".", resourceName, assemblyString);
				Import(resourceStream);
			}
		}

		/// <summary>
		/// Importa os dados de um arquivo.
		/// </summary>
		/// <param name="fileName"></param>
		public static void Import(string fileName)
		{
			if(string.IsNullOrEmpty(fileName))
				throw new ArgumentNullException("fileName");
			if(!System.IO.File.Exists(fileName))
				throw new GDAMappingException("Mapping file \"{0}\" not exists.", fileName);
			using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
				Import(fs);
		}

		/// <summary>
		/// Importa os dados do mapeamento contido na stream.
		/// </summary>
		/// <param name="inputStream"></param>
		public static void Import(System.IO.Stream inputStream)
		{
			if(inputStream == null)
				throw new ArgumentNullException("inputStream");
			try
			{
				var reader = XmlReader.Create(inputStream, new XmlReaderSettings {
					IgnoreWhitespace = true,
					IgnoreComments = true,
				});
				var doc = new System.Xml.XmlDocument();
				doc.Load(reader);
				var root = doc.DocumentElement;
				if(!(root.LocalName == "gda-mapping" && root.NamespaceURI == "urn:gda-mapping-1.0"))
					return;
				var mappingNamespace = ElementMapping.GetAttributeString(root, "namespace");
				var mappingAssembly = ElementMapping.GetAttributeString(root, "assembly");
				var defaultProviderName = ElementMapping.GetAttributeString(root, "defaultProviderName");
				var defaultProviderConfigurationName = ElementMapping.GetAttributeString(root, "defaultProviderConfigurationName");
				var defaultConnectionString = ElementMapping.GetAttributeString(root, "defaultConnectionString");
				var defaultSchema = ElementMapping.GetAttributeString(root, "defaultSchema");
				foreach (XmlElement referencesElement in root.GetElementsByTagName("references"))
				{
					foreach (XmlElement i in referencesElement.GetElementsByTagName("reference"))
					{
						var refMapping = new ReferenceMapping(i);
						bool exists = false;
						lock (objLock)
							exists = _references.Exists(f => f.Equals(refMapping));
						if(!exists)
						{
							lock (objLock)
								_references.Add(refMapping);
							if(!string.IsNullOrEmpty(refMapping.FileName))
								Import(refMapping.FileName);
							else
								Import(refMapping.AssemblyName, refMapping.ResourceName);
						}
					}
					break;
				}
				lock (objLock)
				{
					foreach (XmlElement i in root.GetElementsByTagName("class"))
					{
						var classMap = new ClassMapping(i, mappingNamespace, mappingAssembly, defaultProviderName, defaultProviderConfigurationName, defaultConnectionString, defaultSchema);
						if(!_classes.ContainsKey(classMap.TypeInfo.Fullname))
							_classes.Add(classMap.TypeInfo.Fullname, classMap);
					}
					foreach (XmlElement i in root.GetElementsByTagName("sql-query"))
					{
						var queryMap = new SqlQueryMapping(i);
						if(!_queries.ContainsKey(queryMap.Name))
							_queries.Add(queryMap.Name, queryMap);
					}
					var modelsNamespace = ElementMapping.FirstOrDefault<XmlElement>(root.GetElementsByTagName("modelsNamespace"));
					if(modelsNamespace != null)
						foreach (XmlElement i in modelsNamespace.GetElementsByTagName("namespace"))
						{
							var ns = new ModelsNamespaceMapping(i);
							GDASettings.AddModelsNamespace(ns.Assembly, ns.Namespace);
						}
					XmlElement generateKeyHandlerElement = ElementMapping.FirstOrDefault<XmlElement>(root.GetElementsByTagName("generateKeyHandler"));
					if(generateKeyHandlerElement != null)
					{
						GDASettings.DefineGenerateKeyHandler(ElementMapping.GetAttributeString(generateKeyHandlerElement, "classType", true), ElementMapping.GetAttributeString(generateKeyHandlerElement, "methodName", true));
					}
					var generatorsElement = ElementMapping.FirstOrDefault<XmlElement>(root.GetElementsByTagName("generatorsKey"));
					if(generatorsElement != null)
						foreach (XmlElement i in generatorsElement.GetElementsByTagName("generator"))
						{
							var gk = new GeneratorKeyMapping(i);
							IGeneratorKey instance = null;
							try
							{
								instance = Activator.CreateInstance(gk.ClassType) as IGeneratorKey;
							}
							catch(Exception ex)
							{
								if(ex is System.Reflection.TargetInvocationException)
									ex = ex.InnerException;
								throw new GDAMappingException("Fail on create instance for \"{0}\".", gk.ClassType.FullName);
							}
							if(instance == null)
								throw new GDAMappingException("\"{0}\" not inherits of {1}.", gk.ClassType.FullName, typeof(IGeneratorKey).FullName);
							GDASettings.AddGeneratorKey(gk.Name, instance);
						}
				}
			}
			catch(Exception ex)
			{
				if(ex is GDAMappingException)
					throw ex;
				else
					throw new GDAMappingException("Fail on load mapping", ex);
			}
		}

		/// <summary>
		/// Realiza um refactor no mapeamento do sistema.
		/// </summary>
		/// <returns></returns>
		public static XmlDocument RefactorSystemMapping(System.Reflection.Assembly assembly)
		{
			var doc = new XmlDocument();
			var baseNamespace = assembly.FullName;
			baseNamespace = baseNamespace.Substring(0, baseNamespace.IndexOf(','));
			var root = doc.CreateElement("gda-mapping");
			root.SetAttribute("assembly", assembly.FullName);
			root.SetAttribute("namespace", baseNamespace);
			root.SetAttribute("xmlns", "urn:gda-mapping-1.0");
			var mnElement = doc.CreateElement("modelsNamespace");
			foreach (var i in GDASettings.ModelsNamespaces)
			{
				var nElement = doc.CreateElement("namespace");
				nElement.SetAttribute("name", i.Namespace);
				nElement.SetAttribute("assembly", i.AssemblyName);
				mnElement.AppendChild(nElement);
			}
			root.AppendChild(mnElement);
			#if !PocketPC
			if(GDAOperations.GlobalGenerateKey != null)
			{
				var mi = GDAOperations.GlobalGenerateKey.Method;
				var ggkElement = doc.CreateElement("generateKeyHandler");
				ggkElement.SetAttribute("methodName", mi.Name);
				ggkElement.SetAttribute("classType", mi.DeclaringType.FullName);
			}
			#endif
			var gkElement = doc.CreateElement("generatorsKey");
			foreach (var i in GDASettings.GetGeneratorsKey())
			{
				var gElement = doc.CreateElement("generator");
				gElement.SetAttribute("name", i.Key);
				gElement.SetAttribute("classType", i.Value.GetType().FullName);
				gkElement.AppendChild(gElement);
			}
			root.AppendChild(gkElement);
			foreach (var i in assembly.GetTypes())
			{
				var classElement = RefactorClass(i, baseNamespace, doc);
				if(classElement != null)
					root.AppendChild(classElement);
			}
			doc.AppendChild(root);
			return doc;
		}

		/// <summary>
		/// Processa a classe e recupera um elemento que representa seu mapeamento.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="baseNamespace">Namespace base.</param>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static XmlElement RefactorClass(Type type, string baseNamespace, XmlDocument doc)
		{
			var ppa = Caching.MappingManager.GetPersistenceClassAttribute(type);
			var element = doc.CreateElement("class");
			var name = type.FullName;
			if(name.StartsWith(baseNamespace + '.'))
				name = name.Substring(baseNamespace.Length + 1);
			element.SetAttribute("name", name);
			if(ppa != null)
			{
				element.SetAttribute("table", ppa.Name);
				if(!string.IsNullOrEmpty(ppa.Schema))
					element.SetAttribute("schema", ppa.Schema);
			}
			var bda = Caching.MappingManager.GetPersistenceBaseDAOAttribute(type);
			if(bda != null)
			{
				var bdaElement = doc.CreateElement("baseDAO");
				name = bda.BaseDAOType.FullName;
				if(name.StartsWith(baseNamespace + '.'))
					name = name.Substring(baseNamespace.Length + 1);
				bdaElement.SetAttribute("name", name);
				if(bda.BaseDAOGenericTypes != null)
					foreach (var i in bda.BaseDAOGenericTypes)
					{
						var gElement = doc.CreateElement("genericType");
						gElement.SetAttribute("name", i.FullName);
					}
				element.AppendChild(bdaElement);
			}
			var prov = Caching.MappingManager.GetPersistenceProviderAttribute(type);
			if(prov != null)
			{
				var provElement = doc.CreateElement("provider");
				provElement.SetAttribute("name", prov.ProviderName);
				if(!string.IsNullOrEmpty(prov.ProviderConfigurationName))
					provElement.SetAttribute("configurationName", prov.ProviderConfigurationName);
				if(!string.IsNullOrEmpty(prov.ConnectionString))
				{
					var csElement = doc.CreateElement("connectionString");
					csElement.InnerText = prov.ConnectionString;
					provElement.AppendChild(csElement);
				}
				element.AppendChild(provElement);
			}
			var fkMembers = Caching.MappingManager.GetForeignMemberMapper(type);
			foreach (var m in Caching.MappingManager.GetMappers(type))
			{
				if(m.PropertyMapper.DeclaringType != m.PropertyMapper.ReflectedType)
					continue;
				var pElement = doc.CreateElement("property");
				pElement.SetAttribute("name", m.PropertyMapperName);
				if(m.Name != m.PropertyMapperName)
					pElement.SetAttribute("column", m.Name);
				if(m.ParameterType != PersistenceParameterType.Field)
					pElement.SetAttribute("parameterType", m.ParameterType.ToString());
				if(m.Size > 0)
					pElement.SetAttribute("size", m.Size.ToString());
				if(m.Direction != DirectionParameter.InputOutput)
					pElement.SetAttribute("direction", m.Direction.ToString());
				if(m.IsNotNull)
					pElement.SetAttribute("not-null", m.IsNotNull.ToString());
				if(!string.IsNullOrEmpty(m.GeneratorKeyName))
				{
					var gElement = doc.CreateElement("generator");
					gElement.SetAttribute("name", m.GeneratorKeyName);
					pElement.AppendChild(gElement);
				}
				if(m.ForeignKeys != null)
					foreach (var fk in m.ForeignKeys)
					{
						var fkElement = doc.CreateElement("foreignKey");
						name = fk.TypeOfClassRelated.FullName;
						if(name.StartsWith(baseNamespace + '.'))
							name = name.Substring(baseNamespace.Length + 1);
						fkElement.SetAttribute("typeOfClassRelated", name);
						fkElement.SetAttribute("propertyName", fk.PropertyOfClassRelated.Name);
						if(!string.IsNullOrEmpty(fk.GroupOfRelationship))
							fkElement.SetAttribute("groupOfRelationship", fk.GroupOfRelationship);
						pElement.AppendChild(fkElement);
					}
				foreach (var fkm in fkMembers)
					if(fkm.PropertyModel.Name == m.PropertyMapperName)
					{
						var fkElement = doc.CreateElement("foreignKey");
						name = fkm.TypeOfClassRelated.FullName;
						if(name.StartsWith(baseNamespace + '.'))
							name = name.Substring(baseNamespace.Length + 1);
						fkElement.SetAttribute("typeOfClassRelated", name);
						fkElement.SetAttribute("propertyName", fkm.PropertyOfClassRelated.Name);
						if(!string.IsNullOrEmpty(fkm.GroupOfRelationship))
							fkElement.SetAttribute("groupOfRelationship", fkm.GroupOfRelationship);
						pElement.AppendChild(fkElement);
					}
				if(m.Validation != null && m.Validation.Validators != null)
					foreach (var v in m.Validation.Validators)
					{
						var vElement = doc.CreateElement("validator");
						vElement.SetAttribute("name", vElement.Name);
						foreach (var pi in v.GetType().GetProperties())
							try
							{
								var val = pi.GetValue(v, null);
								var eParam = doc.CreateElement("param");
								eParam.SetAttribute("name", pi.Name);
								eParam.InnerText = val.ToString();
							}
							catch
							{
							}
					}
				element.AppendChild(pElement);
			}
			if(element.IsEmpty)
				return null;
			return element;
		}
	}
}
