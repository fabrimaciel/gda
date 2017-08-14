using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Provider
{
    /// <summary>
    /// Representa o evento que é acionado quando uma conexão é criada no 
    /// provedor de configuração.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void CreateConnectionEvent(object sender, CreateConnectionEventArgs args);
    /// <summary>
    /// Classe que armazena os argumentos que são informados quando
    /// uma conexão do provedor de configuração é criada.
    /// </summary>
    public class CreateConnectionEventArgs : EventArgs
    {
        #region Local Variables
        private System.Data.IDbConnection _connection;
        #endregion
        #region Properties
        /// <summary>
        /// Instancia da conexão que foi criada.
        /// </summary>
        public System.Data.IDbConnection Connection
        {
            get { return _connection; }
        }
        #endregion
        #region Construtores
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="connection">Instancia da conexão criada.</param>
        public CreateConnectionEventArgs(System.Data.IDbConnection connection)
        {
            _connection = connection;
        }
        #endregion
    }
}
