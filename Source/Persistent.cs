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
using System.Collections;
using System.Text;
using System.Reflection;
using GDA.Interfaces;
using GDA.Collections;

namespace GDA
{
	/// <summary>
	/// Classe base para as models com métodos especializados.
	/// </summary>
	[Serializable]
	public abstract class Persistent
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public Persistent()
		{
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public uint Insert(string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return Insert(null, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public uint Insert(GDASession session, string propertiesNamesInsert, DirectionPropertiesName direction)
		{
			return GDA.GDAOperations.Insert(session, this, propertiesNamesInsert, direction);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public uint Insert(string propertiesNamesInsert)
		{
			return Insert(propertiesNamesInsert, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Inseri os dados contidos no objInsert no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
		/// <returns>Chave inserido.</returns>
		/// <exception cref="ArgumentNullException">ObjInsert it cannot be null.</exception>
		/// <exception cref="GDAException"></exception>
		public uint Insert(GDASession session, string propertiesNamesInsert)
		{
			return Insert(session, propertiesNamesInsert, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public uint Insert()
		{
			return Insert(null, null);
		}

		/// <summary>
		/// Inseri o registro no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Chave gerada no processo.</returns>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		public uint Insert(GDASession session)
		{
			return Insert(session, null);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(GDASession session, string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return GDAOperations.Update(session, this, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separados por virgula, que serão atualizadas no comando.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public virtual int Update(string propertiesNamesUpdate, DirectionPropertiesName direction)
		{
			return GDAOperations.Update(null, this, propertiesNamesUpdate, direction);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que serão atualizadas no comando.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public int Update(GDASession session, string propertiesNamesUpdate)
		{
			return GDAOperations.Update(session, this, propertiesNamesUpdate, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Atualiza os dados contidos no objUpdate no BD.
		/// </summary>
		/// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que serão atualizadas no comando.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		/// <returns>Número de linhas afetadas.</returns>
		public int Update(string propertiesNamesUpdate)
		{
			return GDAOperations.Update(this, propertiesNamesUpdate, DirectionPropertiesName.Inclusion);
		}

		/// <summary>
		/// Atualiza o registro na BD.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Update(GDASession session)
		{
			return GDAOperations.Update(session, this, null);
		}

		/// <summary>
		/// Atualiza o registro na BD.
		/// </summary>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Update()
		{
			return GDAOperations.Update(this);
		}

		/// <summary>
		/// Remove o registro da base de dados.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Delete(GDASession session)
		{
			return GDAOperations.Delete(session, this);
		}

		/// <summary>
		/// Remove o registro da base de dados.
		/// </summary>
		/// <returns>Número de linhas afetadas.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
		/// <exception cref="GDAException"></exception>
		public int Delete()
		{
			return GDAOperations.Delete(this);
		}

		/// <summary>
		/// Salva os dados na base. Primeiro verifica se o registro existe, se existir ele será atualizado
		/// senão ele será inserido.
		/// </summary>
		/// <returns>A chave do registro inserido ou 0 se ele for atualizado.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAException">Se o tipo de dados utilizado não possuir chaves.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		public uint Save()
		{
			return GDAOperations.Save(this);
		}

		/// <summary>
		/// Salva os dados na base. Primeiro verifica se o registro existe, se existir ele será atualizado
		/// senão ele será inserido.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <returns>A chave do registro inserido ou 0 se ele for atualizado.</returns>
		/// <exception cref="GDAReferenceDAONotFoundException"></exception>
		/// <exception cref="GDAException">Se o tipo de dados utilizado não possuir chaves.</exception>
		/// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
		public uint Save(GDASession session)
		{
			return GDAOperations.Save(session, this);
		}

		/// <summary>
		/// Recupera os valores da Model com base nos valores da chaves preenchidas.
		/// </summary>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ItemNotFoundException"></exception>
		public void RecoverData()
		{
			GDAOperations.RecoverData(this);
		}

		/// <summary>
		/// Recupera os valores da Model com base nos valores da chaves preenchidas.
		/// </summary>
		/// <param name="session">Sessão utilizada para a execução do comando.</param>
		/// <exception cref="GDAColumnNotFoundException"></exception>
		/// <exception cref="GDAException"></exception>
		/// <exception cref="ItemNotFoundException"></exception>
		public void RecoverData(GDASession session)
		{
			GDAOperations.RecoverData(session, this);
		}

		/// <summary>
		/// Captura a DAO relacionada com a Model.
		/// </summary>
		/// <returns>DAO.</returns>
		public ISimpleBaseDAO GetDAO()
		{
			return GDAOperations.GetDAO(this);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <param name="sortProperty">Informação sobre o propriedade a ser ordenada.</param>
		/// <param name="paging">Informações sobre a paginação do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(string group, InfoSortExpression sortProperty, InfoPaging paging) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, group, sortProperty, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="sortProperty">Informação sobre o propriedade a ser ordenada.</param>
		/// <param name="paging">Informações sobre a paginação do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(InfoSortExpression sortProperty, InfoPaging paging) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, null, sortProperty, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <param name="sortProperty">Informação sobre o propriedade a ser ordenada.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(string group, InfoSortExpression sortProperty) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, group, sortProperty, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="sortProperty">Informação sobre o propriedade a ser ordenada.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(InfoSortExpression sortProperty) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, null, sortProperty, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <param name="paging">Informações sobre a paginação do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(string group, InfoPaging paging) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, group, null, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="paging">Informações sobre a paginação do resultado.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(InfoPaging paging) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, null, null, paging);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>(string group) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, group, null, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public GDAList<ClassRelated> LoadRelationship1toN<ClassRelated>() where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1toN<ClassRelated>(this, null, null, null);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Será informado também o grupo
		/// no qual o relacionamento será carregado. Utiliza a estrura 1 para 1
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public ClassRelated LoadRelationship1to1<ClassRelated>(string group) where ClassRelated : new()
		{
			GDAList<ClassRelated> list = GDAOperations.LoadRelationship1toN<ClassRelated>(this, group);
			if(list.Count > 1)
				throw new GDAException("There is more one row found for this relationship.");
			else if(list.Count == 1)
				return list[0];
			else
				return default(ClassRelated);
		}

		/// <summary>
		/// Carrega as lista itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para 1
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <returns>Lista tipada do tipo da classe que representa a tabela do relacionamento.</returns>
		public ClassRelated LoadRelationship1to1<ClassRelated>(Type typeOfClassRelated) where ClassRelated : new()
		{
			return GDAOperations.LoadRelationship1to1<ClassRelated>(this, (string)null);
		}

		/// <summary>
		/// Carrega a quantidade de itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <param name="group">Nome do grupo de relacionamento.</param>
		/// <returns>Quantidade de itens tipo da classe que representa a tabela do relacionamento.</returns>
		public int CountRowRelationship1toN<ClassRelated>(string group) where ClassRelated : new()
		{
			return GDAOperations.CountRowRelationship1toN<ClassRelated>(this, group);
		}

		/// <summary>
		/// Carrega a quantidade de itens da tabela representada pelo tipo da classe
		/// submetida relacionados com a atual model. Utiliza a estrura 1 para N.
		/// </summary>
		/// <typeparam name="ClassRelated">Tipo da classe que representa a tabela do relacionamento.</typeparam>
		/// <returns>Quantidade de itens tipo da classe que representa a tabela do relacionamento.</returns>
		public int CountRowRelationship1toN<ClassRelated>() where ClassRelated : new()
		{
			return GDAOperations.CountRowRelationship1toN<ClassRelated>(this, null);
		}

		/// <summary>
		/// Captura o nome da tabela que a class T representa.
		/// </summary>
		public string GetTableName()
		{
			return GDAOperations.GetTableName(this.GetType());
		}
	}
}
