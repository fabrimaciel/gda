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
using System.Reflection;

namespace GDA.Provider
{
	/// <summary>
	/// Provider que ir� fezer o controle DAO referenciado para o projeto.
	/// </summary>
	public class Provider : GDA.Interfaces.IProvider
	{
		/// <summary>
		/// Identifica que n�o um tipo da BD v�lido.
		/// </summary>
		public const long No_DbType = -1;

		/// <summary>
		/// Type do objeto que representa o connection.
		/// </summary>
		private Type m_ConnectionType;

		private string m_ConnectionTypeName;

		private Type m_CommandType;

		private string m_CommandTypeName;

		private Type m_AdapterType;

		private string m_AdapterTypeName;

		private Type m_ParameterType;

		private string m_ParameterTypeName;

		private string m_ParameterPrefix;

		private string m_ParameterSuffix;

		private string m_Name;

		protected Assembly providerAssembly;

		private string m_AssemblyName;

		private string m_SqlQueryReturnIdentity;

		private bool m_GenerateIdentity = false;

		/// <summary>
		/// Identifica se os comando ser�o executados um de cada vez.
		/// </summary>
		private bool m_ExecuteCommandsOneAtATime = false;

		/// <summary>
		/// Lista as palavras reservadas do provider.
		/// </summary>
		private System.Collections.Generic.List<string> _reservedsWords;

		/// <summary>
		/// Identifica se os comando ser�o executados um de cada vez.
		/// </summary>
		public bool ExecuteCommandsOneAtATime
		{
			get
			{
				return m_ExecuteCommandsOneAtATime;
			}
			set
			{
				m_ExecuteCommandsOneAtATime = value;
			}
		}

		/// <summary>
		/// Query sql que retorna a identidade gerada no auto incremental.
		/// </summary>
		public virtual string SqlQueryReturnIdentity
		{
			get
			{
				return m_SqlQueryReturnIdentity;
			}
		}

		/// <summary>
		/// Esse m�todo com base no nome da tabela e na coluna identidade da tabela 
		/// recupera a consulta SQL que ir� recupera o valor da chave identidade gerado
		/// para o registro recentemente inserido.
		/// </summary>
		/// <param name="tableName">Nome da tabela onde o registro ser� inserido.</param>
		/// <param name="identityColumnName">Nome da coluna identidade da tabela.</param>
		/// <returns>The modified sql string which also retrieves the identity value</returns>
		public virtual string GetIdentitySelect(string tableName, string identityColumnName)
		{
			if(string.IsNullOrEmpty(SqlQueryReturnIdentity))
				throw new GDAException("SqlQueryReturnIdentuty not found in provider.");
			else
				return SqlQueryReturnIdentity;
		}

		/// <summary>
		/// Esse m�todo com base no nome da tabela e na coluna identidade da tabela 
		/// recupera a consulta SQL que ir� recupera o valor da chave identidade gerado
		/// para o registro recentemente inserido.
		/// </summary>
		/// <param name="tableName">Nome da tabela onde o registro ser� inserido.</param>
		/// <param name="identityColumnName">Nome da coluna identidade da tabela.</param>
		/// <returns>The modified sql string which also retrieves the identity value</returns>
		public virtual string GetIdentitySelect(GDA.Sql.TableName tableName, string identityColumnName)
		{
			return GetIdentitySelect(tableName != null ? tableName.Name : null, identityColumnName);
		}

		/// <summary>
		/// Identifica se a identidade da chaves da tabelas ser�o geradas pelo BD ou pela aplica��o.
		/// True quando a aplica��o gera a identidade.
		/// </summary>
		/// <value>
		/// <list type="bool">
		/// <item>true: quando a aplica��o fica encarregada de gerar a chave.</item>
		/// <item>false: quando o DB fica respons�vel por gerar a chave</item>
		/// </list>
		/// </value>
		public bool GenerateIdentity
		{
			get
			{
				return m_GenerateIdentity;
			}
			set
			{
				m_GenerateIdentity = value;
			}
		}

		/// <summary>
		/// Carrega o tipo de classe que cuida da conex�o.
		/// </summary>
		public Type ConnectionType
		{
			get
			{
				if(m_ConnectionType == null)
				{
					m_ConnectionType = ProviderAssembly.GetType(m_ConnectionTypeName, false);
					if(m_ConnectionType == null)
					{
						throw new GDAException(string.Format("N�o � poss�vel carrega a classe de conex�o: {0} do assmbly: {1}", m_ConnectionTypeName, m_AssemblyName));
					}
				}
				return m_ConnectionType;
			}
		}

		/// <summary>
		/// Carrega o tipo de classe que cuida do command sql.
		/// </summary>
		public Type CommandType
		{
			get
			{
				if(m_CommandType == null)
				{
					m_CommandType = ProviderAssembly.GetType(m_CommandTypeName, false);
					if(m_CommandType == null)
					{
						throw new GDAException(string.Format("N�o � poss�vel carrega a classe de commando: {0} do assmbly: {1}", m_CommandTypeName, m_AssemblyName));
					}
				}
				return m_CommandType;
			}
		}

		/// <summary>
		/// Carrega o tipo de classe que cuida do DataAdapter do provider.
		/// </summary>
		public Type DataAdapterType
		{
			get
			{
				if(m_AdapterType == null)
				{
					m_AdapterType = ProviderAssembly.GetType(m_AdapterTypeName, false);
					if(m_AdapterType == null)
					{
						throw new GDAException(string.Format("N�o � poss�vel carrega a classe de adapter: {0} do assmbly: {1}", m_AdapterTypeName, m_AssemblyName));
					}
				}
				return m_AdapterType;
			}
		}

		/// <summary>
		/// Carrega o tipo de classe que cuida do paramater do provider.
		/// </summary>
		public Type ParameterType
		{
			get
			{
				if(m_ParameterType == null)
				{
					m_ParameterType = ProviderAssembly.GetType(m_ParameterTypeName, false);
					if(m_ParameterType == null)
					{
						throw new GDAException(string.Format("N�o � poss�vel carrega a classe de paramater: {0} do assmbly: {1}", m_ParameterTypeName, m_AssemblyName));
					}
				}
				return m_ParameterType;
			}
		}

		/// <summary>
		/// Nome do do tipo de classe de acesso do provider.
		/// </summary>
		public string Name
		{
			get
			{
				return m_Name;
			}
		}

		/// <summary>
		/// Prefixo usado nos parametros.
		/// </summary>
		public virtual string ParameterPrefix
		{
			get
			{
				return m_ParameterPrefix;
			}
		}

		/// <summary>
		/// Sufixo usado nos parametros.
		/// </summary>
		public virtual string ParameterSuffix
		{
			get
			{
				return m_ParameterSuffix;
			}
		}

		/// <summary>
		/// Identifica se o provider tem suporte ao m�todo SQLCommandLimit.
		/// </summary>
		public virtual bool SupportSQLCommandLimit
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Retorna o valor minimo suportado por um valor DateTime.
		/// O padr�o � 1/1/1800.
		/// </summary>
		public virtual DateTime MinimumSupportedDateTime
		{
			get
			{
				return new DateTime(1800, 1, 1);
			}
		}

		/// <summary>
		/// Retorna o valor maximo suportado por um valor DateTime.
		/// O padr�o � 1/1/3000.
		/// </summary>
		public virtual DateTime MaximumSupportedDateTime
		{
			get
			{
				return new DateTime(3000, 1, 1);
			}
		}

		/// <summary>
		/// Quote inicial da express�o.
		/// </summary>
		public virtual string QuoteExpressionBegin
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Quote final da express�o.
		/// </summary>
		public virtual string QuoteExpressionEnd
		{
			get
			{
				return "";
			}
		}

		/// <summary>
		/// Construtor
		/// </summary>
		/// <param name="name">Nome da biblioteca ADO a ser tratada pelo provider.</param>
		/// <param name="connection">Type do connection.</param>
		/// <param name="dataAdapter">Type do dataAdapter.</param>
		/// <param name="command">Type do command.</param>
		public Provider(string name, Type connection, Type dataAdapter, Type command)
		{
			if(connection == null)
				throw new ArgumentNullException("connection");
			else if(dataAdapter == null)
				throw new ArgumentNullException("dataAdapter");
			else if(command == null)
				throw new ArgumentNullException("command");
			m_Name = name;
			m_ConnectionTypeName = connection.FullName;
			m_AdapterTypeName = dataAdapter.FullName;
			m_CommandTypeName = command.FullName;
			m_ConnectionType = connection;
			m_AdapterType = dataAdapter;
			m_CommandType = command;
		}

		public Provider(string name, Type connection, Type dataAdapter, Type command, string sqlQueryReturnIdentity) : this(name, connection, dataAdapter, command)
		{
			m_SqlQueryReturnIdentity = sqlQueryReturnIdentity;
		}

		public Provider(string name, Type connection, Type dataAdapter, Type command, bool generateIdentity) : this(name, connection, dataAdapter, command)
		{
			m_GenerateIdentity = generateIdentity;
		}

		public Provider(string name, Type connection, Type dataAdapter, Type command, Type parameter, string parameterPrefix)
		{
			if(connection == null)
				throw new ArgumentNullException("connection");
			else if(dataAdapter == null)
				throw new ArgumentNullException("dataAdapter");
			else if(command == null)
				throw new ArgumentNullException("command");
			else if(parameter == null)
				throw new ArgumentNullException("parameter");
			else if(parameterPrefix == "" || parameterPrefix == null)
				throw new ArgumentNullException("paramterPrefix");
			m_Name = name;
			m_ConnectionTypeName = connection.FullName;
			m_AdapterTypeName = dataAdapter.FullName;
			m_CommandTypeName = command.FullName;
			m_ParameterTypeName = parameter.FullName;
			m_ConnectionType = connection;
			m_AdapterType = dataAdapter;
			m_CommandType = command;
			m_ParameterType = parameter;
			m_ParameterPrefix = parameterPrefix;
		}

		public Provider(string name, Type connection, Type dataAdapter, Type command, Type parameter, string parameterPrefix, string sqlQueryReturnIdentity) : this(name, connection, dataAdapter, command, parameter, parameterPrefix)
		{
			m_SqlQueryReturnIdentity = sqlQueryReturnIdentity;
		}

		public Provider(string name, Type connection, Type dataAdapter, Type command, Type parameter, string parameterPrefix, string sqlQueryReturnIdentity, bool generateIdentity) : this(name, connection, dataAdapter, command, parameter, parameterPrefix, sqlQueryReturnIdentity)
		{
			m_GenerateIdentity = generateIdentity;
		}

		public Provider(string name, Type connection, Type dataAdapter, Type command, Type parameter, string parameterPrefix, bool generateIdentity) : this(name, connection, dataAdapter, command, parameter, parameterPrefix)
		{
			m_GenerateIdentity = generateIdentity;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command)
		{
			m_Name = name;
			m_AssemblyName = assemblyName;
			m_ConnectionTypeName = connection;
			m_AdapterTypeName = dataAdapter;
			m_CommandTypeName = command;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command, string sqlQueryReturnIdentity) : this(name, assemblyName, connection, dataAdapter, command)
		{
			m_SqlQueryReturnIdentity = sqlQueryReturnIdentity;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command, bool generateIdentity) : this(name, assemblyName, connection, dataAdapter, command)
		{
			m_GenerateIdentity = generateIdentity;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command, string parameter, string parameterPrefix) : this(name, assemblyName, connection, dataAdapter, command)
		{
			m_ParameterTypeName = parameter;
			m_ParameterPrefix = parameterPrefix;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command, string parameter, string parameterPrefix, string sqlQueryReturnIdentity) : this(name, assemblyName, connection, dataAdapter, command, parameter, parameterPrefix)
		{
			m_SqlQueryReturnIdentity = sqlQueryReturnIdentity;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command, string parameter, string parameterPrefix, bool generateIdentity) : this(name, assemblyName, connection, dataAdapter, command, parameter, parameterPrefix)
		{
			m_GenerateIdentity = generateIdentity;
		}

		public Provider(string name, string assemblyName, string connection, string dataAdapter, string command, string parameter, string parameterPrefix, bool generateIdentity, string sqlQueryReturnIdentity) : this(name, assemblyName, connection, dataAdapter, command, parameter, parameterPrefix)
		{
			m_SqlQueryReturnIdentity = sqlQueryReturnIdentity;
			m_GenerateIdentity = generateIdentity;
		}

		/// <summary>
		/// Carrega o assembly do do namespace que cont�m os objetos de acesso.
		/// </summary>
		public Assembly ProviderAssembly
		{
			get
			{
				if(providerAssembly == null)
				{
					#if PocketPC
					                    providerAssembly = Assembly.Load(m_AssemblyName);
#else
					if(m_AssemblyName.IndexOf(',') == -1)
					{
						providerAssembly = Assembly.LoadWithPartialName(m_AssemblyName);
					}
					else
					{
						providerAssembly = Assembly.Load(m_AssemblyName);
					}
					#endif
				}
				return providerAssembly;
			}
		}

		/// <summary>
		/// Cria uma inst�ncia do connection que o provider representa.
		/// </summary>
		/// <returns>Connection.</returns>
		public virtual IDbConnection CreateConnection()
		{
			object obj = null;
			obj = Activator.CreateInstance(ConnectionType);
			if(obj == null)
				throw new GDAException(string.Format("N�o � poss�vel criar a classe connection: {0} do assmbly: {1}", m_ConnectionTypeName, m_AssemblyName));
			return (IDbConnection)obj;
		}

		/// <summary>
		/// Cria uma inst�ncia do command que o provider representa.
		/// </summary>
		/// <returns>Command.</returns>
		public virtual IDbCommand CreateCommand()
		{
			object obj = null;
			obj = Activator.CreateInstance(CommandType);
			if(obj == null)
				throw new GDAException(string.Format("N�o � poss�vel criar a classe command: {0} do assmbly: {1}", m_CommandTypeName, m_AssemblyName));
			return (IDbCommand)obj;
		}

		/// <summary>
		/// Cria uma inst�ncia do DataAdapter que o provider representa.
		/// </summary>
		/// <returns>DataAdapter.</returns>
		public virtual IDbDataAdapter CreateDataAdapter()
		{
			object obj = Activator.CreateInstance(DataAdapterType);
			if(obj == null)
				throw new GDAException(string.Format("N�o � poss�vel criar a classe adapter: {0} do assmbly: {1}", m_AdapterTypeName, m_AssemblyName));
			return (IDbDataAdapter)obj;
		}

		/// <summary>
		/// Cria uma inst�ncia do Parameter que o provider representa.
		/// </summary>
		/// <returns>Parameter.</returns>
		public virtual System.Data.Common.DbParameter CreateParameter()
		{
			object obj = Activator.CreateInstance(ParameterType);
			if(obj == null)
				throw new GDAException(string.Format("N�o � poss�vel criar a classe parameter: {0} do assmbly: {1}", m_ParameterTypeName, m_AssemblyName));
			return (System.Data.Common.DbParameter)obj;
		}

		/// <summary>
		/// M�todos respons�vel por tratar o comando sql e adapta-lo para executar
		/// estruturas de pagina��o.
		/// </summary>
		/// <param name="mapping">Mapeamentos dos campos da model.</param>
		/// <param name="sqlQuery">Clausula SQL que ser� tratada.</param>
		/// <param name="startRecord">Apartir da qual deve come�ar o resultado.</param>
		/// <param name="size">Quantidade de registros por resultado.</param>
		/// <returns>Consulta sql tratada</returns>
		public virtual string SQLCommandLimit(List<Mapper> mapping, string sqlQuery, int startRecord, int size)
		{
			return sqlQuery;
		}

		/// <summary>
		/// Transforma o nome de tabela representado pela classe <see cref="GDA.Sql.TableName"/>
		/// em uma string que ser� usada na consulta do banco de dados.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual string BuildTableName(GDA.Sql.TableName tableName)
		{
			if(tableName == null)
				return null;
			if(!string.IsNullOrEmpty(tableName.Schema))
				return string.Format("{0}.{1}", QuoteExpression(tableName.Schema), QuoteExpression(tableName.Name));
			else
				return QuoteExpression(tableName.Name);
		}

		/// <summary>
		/// Obtem um n�mero inteiro que corresponde ao tipo da base de dados que representa o tipo
		/// informado. O valor de retorno pode ser convertido em um tipo v�lido (enum value) para 
		/// o atual provider. Esse method � chamado para traduzir os tipos do sistema para os tipos
		/// do banco de dados que n�o s�o convertidos explicitamento.
		/// </summary>
		/// <param name="type">Tipo do sistema.</param>
		/// <returns>Tipo correspondente da base de dados.</returns>
		public virtual long GetDbType(Type type)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Esse m�todo converte a string (extra�da da tabelas do banco de dados) para o tipo do system
		/// correspondente.
		/// </summary>
		/// <param name="dbType">Nome do tipo usado no banco de dados.</param>
		/// <param name="isUnsigned">Valor boolean que identifica se o tipo � unsigned.</param>
		/// <returns>Valor do enumerator do tipo correspondente do banco de dados. O retorno � um n�mero
		/// inteiro por causa que em alguns provider o enumerations n�o seguem o padr�o do DbType definido
		/// no System.Data.</returns>
		public virtual long GetDbType(string dbType, bool isUnsigned)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Obtem a caracter ou string usado para terminar o delimitar o statement.
		/// </summary>
		public virtual string StatementTerminator
		{
			get
			{
				return ";";
			}
		}

		/// <summary>
		/// Determina se uma palavra � reservada and necessita de um atribui��o entre quoting.
		/// </summary>
		/// <returns>True se a palavra for reservada.</returns>
		public virtual bool IsReservedWord(string word)
		{
			return false;
		}

		/// <summary>
		/// Obtem o caracter usado para delimitar os parametros de string.
		/// </summary>
		/// <returns>The quote character.</returns>
		public virtual char QuoteCharacter
		{
			get
			{
				return '\0';
			}
		}

		/// <summary>
		/// Obtem um enum descrevendo as capacidades suportadas pelo banco de dados. 
		/// Por padr�o todas s�o suportadas. Veja <see cref="Capability"/> para mais 
		/// detalhes sobre as capacidades dispon�veis.	
		/// </summary>
		public virtual Capability Capabilities
		{
			get
			{
				return Capability.BatchQuery | Capability.Paging | Capability.NamedParameters;
			}
		}

		/// <summary>
		/// Esse m�todo retorna o tipo do sistema correspodente ao tipo specifico indicado no long.
		/// A implementa��o padr�o n�o retorna exception, mas sim null.
		/// </summary>
		/// <param name="dbType">Tipo especifico do provider.</param>
		/// <returns>Tipo do sistema correspondente.</returns>
		public virtual Type GetSystemType(long dbType)
		{
			return null;
		}

		/// <summary>
		/// Lista da palavra reservadas do provider.
		/// </summary>
		public virtual System.Collections.Generic.List<string> ReservedsWords
		{
			get
			{
				if(_reservedsWords == null)
					_reservedsWords = new System.Collections.Generic.List<string>();
				return _reservedsWords;
			}
		}

		/// <summary>
		/// Obtem uma container (Quote) para permitir que colunas com nomes especiais
		/// sejam inseridas na consulta.
		/// </summary>
		/// <param name="word">Nome da coluna ou do paramenrto.</param>
		/// <returns>Express�o com a formata��o adequada.</returns>
		public virtual string QuoteExpression(string word)
		{
			return word;
		}

		/// <summary>
		/// Converte o valor do parametro.
		/// </summary>
		/// <param name="parameter">Instancia do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public virtual void SetParameterValue(IDbDataParameter parameter, object value)
		{
			parameter.Value = value;
		}
	}
}
