using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
#if CLS_3_5_1
    public static class GDAOperationsEx
    {
        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem inseridos.</param>
        /// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
        /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public static uint Insert(this object model, string propertiesNamesInsert, DirectionPropertiesName direction)
        {
            return GDAOperations.Insert(null, model, propertiesNamesInsert, direction);
        }
        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem inseridos.</param>
        /// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
        /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public static uint Insert(this object model, GDASession session, string propertiesNamesInsert, DirectionPropertiesName direction)
        {
            return GDAOperations.Insert(session, model, propertiesNamesInsert, direction);
        }
        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem inseridos.</param>
        /// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public static uint Insert(this object model, GDASession session, string propertiesNamesInsert)
        {
            return GDAOperations.Insert(session, model, propertiesNamesInsert);
        }
        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem inseridos.</param>
        /// <param name="propertiesNamesInsert">Nome das propriedades separados por virgula, que serão inseridos no comando.</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public static uint Insert(this object model, string propertiesNamesInsert)
        {
            return GDAOperations.Insert(model, propertiesNamesInsert);
        }
        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem inseridos.</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public static uint Insert(this object model, GDASession session)
        {
            return GDAOperations.Insert(session, model);
        }
        /// <summary>
        /// Inseri o registro no BD.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem inseridos.</param>
        /// <returns>Chave gerada no processo.</returns>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        public static uint Insert(this object model)
        {
            return GDAOperations.Insert(model);
        }
        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem atualizados.</param>
        /// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que serão atualizadas no comando.</param>
        /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public static int Update(this object model, GDASession session, string propertiesNamesUpdate, DirectionPropertiesName direction)
        {
            return GDAOperations.Update(session, model, propertiesNamesUpdate, direction);
        }
        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem atualizados.</param>
        /// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que serão atualizadas no comando.</param>
        /// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public static int Update(this object model, string propertiesNamesUpdate, DirectionPropertiesName direction)
        {
            return GDAOperations.Update(model, propertiesNamesUpdate, direction);
        }
        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem atualizados.</param>
        /// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que serão atualizadas no comando.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public static int Update(this object model, GDASession session, string propertiesNamesUpdate)
        {
            return GDAOperations.Update(session, model, propertiesNamesUpdate);
        }
        /// <summary>
        /// Atualiza os dados contidos no objUpdate no BD.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem atualizados.</param>
        /// <param name="propertiesNamesUpdate">Nome das propriedades separadas por virgula, que serão atualizadas no comando.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        /// <returns>Número de linhas afetadas.</returns>
        public static int Update(this object model, string propertiesNamesUpdate)
        {
            return GDAOperations.Update(model, propertiesNamesUpdate);
        }
        /// <summary>
        /// Atualiza o registro na BD.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem atualizados.</param>
        /// <returns>Número de linhas afetadas.</returns>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        public static int Update(this object model, GDASession session)
        {
            return GDAOperations.Update(session, model);
        }
        /// <summary>
        /// Atualiza o registro na BD.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem atualizados.</param>
        /// <returns>Número de linhas afetadas.</returns>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        public static int Update(this object model)
        {
            return GDAOperations.Update(model);
        }
        /// <summary>
        /// Remove o registro da base de dados.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem removidos.</param>
        /// <returns>Número de linhas afetadas.</returns>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        public static int Delete(this object model, GDASession session)
        {
            // Captura a dao relacionada
            return GDAOperations.Delete(session, model);
        }
        /// <summary>
        /// Remove o registro da base de dados.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem removidos.</param>
        /// <returns>Número de linhas afetadas.</returns>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to contruir the conditional clause.</exception>
        /// <exception cref="GDAException"></exception>
        public static int Delete(this object model)
        {
            // Captura a dao relacionada
            return GDAOperations.Delete(model);
        }
        /// <summary>
        /// Salva os dados na base. Primeiro verifica se o registro existe, se existir ele será atualizado
        /// senão ele será inserido.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados a serem salvos.</param>
        /// <returns>A chave do registro inserido ou 0 se ele for atualizado.</returns>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        /// <exception cref="GDAException">Se o tipo de dados utilizado não possuir chaves.</exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        public static uint Save(this object model, GDASession session)
        {
            // Captura a dao relacionada
            return GDAOperations.Save(session, model);
        }
        /// <summary>
        /// Salva os dados na base. Primeiro verifica se o registro existe, se existir ele será atualizado
        /// senão ele será inserido.
        /// </summary>
        /// <param name="model">Model contendo os dados a serem salvos.</param>
        /// <returns>A chave do registro inserido ou 0 se ele for atualizado.</returns>
        /// <exception cref="GDAReferenceDAONotFoundException"></exception>
        /// <exception cref="GDAException">Se o tipo de dados utilizado não possuir chaves.</exception>
        /// <exception cref="GDAConditionalClauseException">Parameters do not exist to build the conditional clause.</exception>
        public static uint Save(this object model)
        {
            // Captura a dao relacionada
            return GDAOperations.Save(model);
        }
        /// <summary>
        /// Recupera os valores da Model com base nos valores da chaves preenchidas.
        /// </summary>
        /// <param name="session">Sessão utilizada para a execução do comando.</param>
        /// <param name="model">Model contendo os dados que seram usados com base para recuperar os restante dos dados.</param>
        /// <exception cref="GDAColumnNotFoundException"></exception>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        public static void RecoverData(this object model, GDASession session)
        {
           GDAOperations.RecoverData(session, model);
        }
        /// <summary>
        /// Recupera os valores da Model com base nos valores da chaves preenchidas.
        /// </summary>
        /// <param name="model">Model contendo os dados que seram usados com base para recuperar os restante dos dados.</param>
        /// <exception cref="GDAColumnNotFoundException"></exception>
        /// <exception cref="GDAException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        public static void RecoverData(this object model)
        {
            GDAOperations.RecoverData(null, model);
        }
    }
#endif
}
