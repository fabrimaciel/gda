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
using System.Collections;
using GDA.Interfaces;

namespace GDA.Analysis
{
	/// <summary>
	/// Representa a interface para implementar o analisador do banco de dados.
	/// </summary>
	public abstract class DatabaseAnalyzer
	{
		/// <summary>
		/// Identifica se todos os metadados das tabelas já foram carregados.
		/// </summary>
		private bool done = false;

		/// <summary>
		/// Lista das chaves estrangeiras encontradas.
		/// </summary>
		private ForeignKeyList _foreignKeys = new ForeignKeyList();

		/// <summary>
		/// Hashtable dos mapeamento carregados das tabelas do banco de dados.
		/// </summary>
		protected Hashtable tablesMaps;

		/// <summary>
		/// Lista dos nomes das tabelas.
		/// </summary>
		protected List<string> tablesNames;

		/// <summary>
		/// Provider de configuração usado na analise.
		/// </summary>
		private IProviderConfiguration providerConfiguration;

		/// <summary>
		/// Provider relacionado.
		/// </summary>
		protected IProviderConfiguration ProviderConfiguration
		{
			get
			{
				return providerConfiguration;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="providerConfiguration">Provider de configuração usado na analise.</param>
		public DatabaseAnalyzer(IProviderConfiguration providerConfiguration)
		{
			this.providerConfiguration = providerConfiguration;
			tablesMaps = new Hashtable();
		}

		/// <summary>
		/// Método responsável por fazer uma análise dadas tabelas do banco de dados.
		/// </summary>
		/// <param name="tableName">Nome da tabela que será analisa ou null para analisar todas.</param>
		public abstract void Analyze(string tableName);

		/// <summary>
		/// Método responsável por fazer uma análise dadas tabelas do banco de dados.
		/// </summary>
		/// <param name="tableName">Nome da tabela que será analisa ou null para analisar todas.</param>
		/// <param name="tableSchema">Esquema da tabela.</param>
		public virtual void Analyze(string tableName, string tableSchema)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Recupera o dados do metadado da tabela especificada.
		/// </summary>
		/// <param name="tableName">Nome da tabela.</param>
		/// <returns>Dados ou null caso o metadado não tenha sido carregado ou não exista.</returns>
		[System.Obsolete("Use GetTableMap(string tableName, string schema)")]
		public virtual TableMap GetTableMap(string tableName)
		{
			if(tableName == null)
				return null;
			tableName = tableName.ToLower();
			return tablesMaps.ContainsKey(tableName) ? tablesMaps[tableName] as TableMap : null;
		}

		/// <summary>
		/// Recupera o dados do metadado da tabela especificada.
		/// </summary>
		/// <param name="tableName">Nome da tabela.</param>
		/// <param name="tableSchema">Esquema da tabela.</param>
		/// <returns>Dados ou null caso o metadado não tenha sido carregado ou não exista.</returns>
		public virtual TableMap GetTableMap(string tableName, string tableSchema)
		{
			if(tableName == null)
				return null;
			tableName = tableName.ToLower();
			TableMap tableMap = tablesMaps[tableName] as TableMap;
			if(tableMap != null && string.Compare(tableMap.TableSchema, tableSchema, true) == 0)
				return tableMap;
			return null;
		}

		/// <summary>
		/// Recupera os nomes das tabelas existentes no banco de dados.
		/// </summary>
		/// <returns></returns>
		public virtual IList<string> GetTablesName()
		{
			if(tablesNames == null)
				tablesNames = new List<string>();
			if(tablesMaps.Count != tablesNames.Count)
			{
				tablesNames.Clear();
				foreach (string tn in tablesMaps.Keys)
					tablesNames.Add(tn);
			}
			return tablesNames;
		}

		/// <summary>
		/// Hastable contendo as instancias dos metadados obtidos das tabelas
		/// do banco de dados.
		/// </summary>
		public Hashtable TablesMaps
		{
			get
			{
				return tablesMaps;
			}
		}

		/// <summary>
		/// Lista das chaves estrangeiras encontradas.
		/// </summary>
		public ForeignKeyList ForeignKeys
		{
			get
			{
				return _foreignKeys;
			}
		}

		/// <summary>
		/// Tabelas analisadas.
		/// </summary>
		public IEnumerable<TableMap> Tables
		{
			get
			{
				foreach (DictionaryEntry de in TablesMaps)
					yield return (TableMap)de.Value;
			}
		}

		#if !PocketPC
		/// <summary>
		/// Gera o código das classes das tabelas do analisador.
		/// </summary>
		/// <param name="language">Linguagem que o código será gerado.</param>
		/// <param name="namespaceName">Nome do namespace onde as classes serão inseridas.</param>
		/// <param name="baseTypeName">Nome do tipo base que as classes irão herdar.</param>
		/// <param name="directoryName">Diretório onde serão salvo os arquivos com os códigos gerados.</param>
		public void GenerateCode(string language, string namespaceName, string baseTypeName, string directoryName)
		{
			if(string.IsNullOrEmpty(language))
				throw new ArgumentNullException("language");
			else if(string.IsNullOrEmpty(namespaceName))
				throw new ArgumentNullException("namespaceName");
			else if(string.IsNullOrEmpty(directoryName))
				throw new ArgumentNullException("directoryName");
			if(!System.IO.Directory.Exists(directoryName))
				throw new System.IO.DirectoryNotFoundException(string.Format("Directory {0} not found.", directoryName));
			GDA.Analysis.Generator codeG = new Generator();
			codeG.CodeLanguage = language;
			codeG.NamespaceName = namespaceName;
			this.Analyze(null);
			string fileExtesion = System.CodeDom.Compiler.CodeDomProvider.CreateProvider(language).FileExtension;
			foreach (TableMap map in this.TablesMaps.Values)
				codeG.Generate(map, baseTypeName, directoryName + "\\" + Generator.StandartName(map.TableName, true) + "." + fileExtesion);
		}
	#endif
	}
}
