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
using System.Data;

namespace GDA.Interfaces
{
	/// <summary>
	/// Interface contendo os métodos base que são usados para gerênciar o provider
	/// com a base de dados.
	/// </summary>
	public interface IProvider
	{
		/// <summary>
		/// Identifica se os comando serão executados um de cada vez.
		/// </summary>
		bool ExecuteCommandsOneAtATime
		{
			get;
		}

		/// <summary>
		/// Query sql que retorna a identidade gerada no auto incremental.
		/// </summary>
		string SqlQueryReturnIdentity
		{
			get;
		}

		/// <summary>
		/// Identifica se a identidade da chaves da tabelas serão geradas pelo BD ou pela aplicação.
		/// True quando a aplicação gera a identidade.
		/// </summary>
		/// <value>
		/// <list type="bool">
		/// <item>true: quando a aplicação fica encarregada de gerar a chave.</item>
		/// <item>false: quando o DB fica responsável por gerar a chave</item>
		/// </list>
		/// </value>
		bool GenerateIdentity
		{
			get;
		}

		/// <summary>
		/// Esse método com base no nome da tabela e na coluna identidade da tabela 
		/// recupera a consulta SQL que irá recupera o valor da chave identidade gerado
		/// para o registro recentemente inserido.
		/// </summary>
		/// <param name="tableName">Nome da tabela onde o registro será inserido.</param>
		/// <param name="identityColumnName">Nome da coluna identidade da tabela.</param>
		/// <returns>The modified sql string which also retrieves the identity value</returns>
		[Obsolete("Use GetIdentitySelect with GDA.Sql.TableName")]
		string GetIdentitySelect(string tableName, string identityColumnName);

		/// <summary>
		/// Esse método com base no nome da tabela e na coluna identidade da tabela 
		/// recupera a consulta SQL que irá recupera o valor da chave identidade gerado
		/// para o registro recentemente inserido.
		/// </summary>
		/// <param name="tableName">Nome da tabela onde o registro será inserido.</param>
		/// <param name="identityColumnName">Nome da coluna identidade da tabela.</param>
		/// <returns>The modified sql string which also retrieves the identity value</returns>
		string GetIdentitySelect(GDA.Sql.TableName tableName, string identityColumnName);

		/// <summary>
		/// Prefixo usado nos parametros.
		/// </summary>
		string ParameterPrefix
		{
			get;
		}

		/// <summary>
		/// Sufixo usado nos parametros.
		/// </summary>
		string ParameterSuffix
		{
			get;
		}

		/// <summary>
		/// Nome do do tipo de classe de acesso do provider.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Identifica se o provider tem suporte ao método SQLCommandLimit.
		/// </summary>
		bool SupportSQLCommandLimit
		{
			get;
		}

		/// <summary>
		/// Obtem um número inteiro que corresponde ao tipo da base de dados que representa o tipo
		/// informado. O valor de retorno pode ser convertido em um tipo válido (enum value) para 
		/// o atual provider. Esse method é chamado para traduzir os tipos do sistema para os tipos
		/// do banco de dados que não são convertidos explicitamento.
		/// </summary>
		/// <param name="type">Tipo do sistema.</param>
		/// <returns>Tipo correspondente da base de dados.</returns>
		long GetDbType(Type type);

		/// <summary>
		/// Esse método converte a string (extraída da tabelas do banco de dados) para o tipo do system
		/// correspondente.
		/// </summary>
		/// <param name="dbType">Nome do tipo usado no banco de dados.</param>
		/// <param name="isUnsigned">Valor boolean que identifica se o tipo é unsigned.</param>
		/// <returns>Valor do enumerator do tipo correspondente do banco de dados. O retorno é um número
		/// inteiro por causa que em alguns provider o enumerations não seguem o padrão do DbType definido
		/// no System.Data.</returns>
		long GetDbType(string dbType, bool isUnsigned);

		/// <summary>
		/// Esse método retorna o tipo do sistema correspodente ao tipo specifico indicado no long.
		/// A implementação padrão não retorna exception, mas sim null.
		/// </summary>
		/// <param name="dbType">Tipo especifico do provider.</param>
		/// <returns>Tipo do sistema correspondente.</returns>
		Type GetSystemType(long dbType);

		/// <summary>
		/// Retorna o valor minimo suportado por um valor DateTime.
		/// </summary>
		DateTime MinimumSupportedDateTime
		{
			get;
		}

		/// <summary>
		/// Retorna o valor maximo suportado por um valor DateTime.
		/// </summary>
		DateTime MaximumSupportedDateTime
		{
			get;
		}

		/// <summary>
		/// Obtem a caracter ou string usado para terminar o delimitar o statement.
		/// </summary>
		string StatementTerminator
		{
			get;
		}

		/// <summary>
		/// Determina se uma palavra é reservada and necessita de um atribuição entre quoting.
		/// </summary>
		/// <returns>True se a palavra for reservada.</returns>
		bool IsReservedWord(string word);

		/// <summary>
		/// Obtem o caracter usado para delimitar os parametros de string.
		/// </summary>
		/// <returns>The quote character.</returns>
		char QuoteCharacter
		{
			get;
		}

		/// <summary>
		/// Obtem um enum descrevendo as capacidades suportadas pelo banco de dados. 
		/// Por padrão todas são suportadas. Veja <see cref="GDA.Provider.Capability"/> para mais 
		/// detalhes sobre as capacidades disponíveis.	
		/// </summary>
		GDA.Provider.Capability Capabilities
		{
			get;
		}

		/// <summary>
		/// Lista das palavras reservadas do provider.
		/// </summary>
		System.Collections.Generic.List<string> ReservedsWords
		{
			get;
		}

		/// <summary>
		/// Obtem uma container (Quote) para permitir que colunas com nomes especiais
		/// sejam inseridas na consulta.
		/// </summary>
		/// <param name="word">Nome da coluna ou do paramenrto.</param>
		/// <returns>Expressão com a formatação adequada.</returns>
		string QuoteExpression(string word);

		/// <summary>
		/// Quote inicial da expressão.
		/// </summary>
		string QuoteExpressionBegin
		{
			get;
		}

		/// <summary>
		/// Quote final da expressão.
		/// </summary>
		string QuoteExpressionEnd
		{
			get;
		}

		/// <summary>
		/// Transforma o nome de tabela representado pela classe <see cref="GDA.Sql.TableName"/>
		/// em uma string que será usada na consulta do banco de dados.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		string BuildTableName(GDA.Sql.TableName tableName);

		/// <summary>
		/// Cria uma instância do connection que o provider representa.
		/// </summary>
		/// <returns>Connection.</returns>
		IDbConnection CreateConnection();

		/// <summary>
		/// Cria uma instância do command que o provider representa.
		/// </summary>
		/// <returns>Command.</returns>
		IDbCommand CreateCommand();

		/// <summary>
		/// Cria uma instância do DataAdapter que o provider representa.
		/// </summary>
		/// <returns>DataAdapter.</returns>
		IDbDataAdapter CreateDataAdapter();

		/// <summary>
		/// Cria uma instância do Parameter que o provider representa.
		/// </summary>
		/// <returns>Parameter.</returns>
		System.Data.Common.DbParameter CreateParameter();

		/// <summary>
		/// Métodos responsável por tratar o comando sql e adapta-lo para executar
		/// estruturas de paginação.
		/// </summary>
		/// <param name="sqlQuery">Clausula SQL que será tratada.</param>
		/// <param name="startRecord">Apartir da qual deve começar o resultado.</param>
		/// <param name="size">Quantidade de registros por resultado.</param>
		/// <returns></returns>
		string SQLCommandLimit(List<Mapper> mapping, string sqlQuery, int startRecord, int size);

		/// <summary>
		/// Converte o valor do parametro.
		/// </summary>
		/// <param name="parameter">Instancia do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		void SetParameterValue(IDbDataParameter parameter, object value);
	}
}
