using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GDA.Provider.MsSqlv45
{
	class SqlCommandFix : System.Data.IDbCommand
	{
		private System.Data.SqlClient.SqlCommand _command;
		protected System.Data.SqlClient.SqlCommand Command {
			get {
				return _command;
			}
		}
		public string CommandText {
			get {
				return Command.CommandText;
			}
			set {
				Command.CommandText = value;
			}
		}
		public int CommandTimeout {
			get {
				return Command.CommandTimeout;
			}
			set {
				Command.CommandTimeout = value;
			}
		}
		public System.Data.CommandType CommandType {
			get {
				return Command.CommandType;
			}
			set {
				Command.CommandType = value;
			}
		}
		public System.Data.IDbConnection Connection {
			get {
				return Command.Connection;
			}
			set {
				Command.Connection = (System.Data.SqlClient.SqlConnection)value;
			}
		}
		public System.Data.IDataParameterCollection Parameters {
			get {
				return Command.Parameters;
			}
		}
		public System.Data.IDbTransaction Transaction {
			get {
				return Command.Transaction;
			}
			set {
				Command.Transaction = (System.Data.SqlClient.SqlTransaction)value;
			}
		}
		public System.Data.UpdateRowSource UpdatedRowSource {
			get {
				return Command.UpdatedRowSource;
			}
			set {
				Command.UpdatedRowSource = value;
			}
		}
		public SqlCommandFix (System.Data.SqlClient.SqlCommand a)
		{
			if (a == null)
				throw new ArgumentNullException ("command");
			_command = a;
		}
		~SqlCommandFix ()
		{
			Dispose (false);
		}
		private static T GetTaskResult<T> (Task<T> a)
		{
			Exception b = null;
			try {
				a.Wait ();
			}
			catch (Exception ex) {
				b = ex;
			}
			if (b != null) {
				var c = b as AggregateException;
				if (c != null && c.InnerExceptions.Count == 1)
					b = c.InnerExceptions [0];
				throw b;
			}
			return a.Result;
		}
		public void Cancel ()
		{
			Command.Cancel ();
		}
		public System.Data.IDbDataParameter CreateParameter ()
		{
			return Command.CreateParameter ();
		}
		public int ExecuteNonQuery ()
		{
			return GetTaskResult (Command.ExecuteNonQueryAsync ());
		}
		public System.Data.IDataReader ExecuteReader (System.Data.CommandBehavior a)
		{
			return GetTaskResult (Command.ExecuteReaderAsync (a));
		}
		public System.Data.IDataReader ExecuteReader ()
		{
			return GetTaskResult (Command.ExecuteReaderAsync ());
		}
		public object ExecuteScalar ()
		{
			return GetTaskResult (Command.ExecuteScalarAsync ());
		}
		public void Prepare ()
		{
			Command.Prepare ();
		}
		protected virtual void Dispose (bool a)
		{
			Command.Dispose ();
		}
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}
	}
}
