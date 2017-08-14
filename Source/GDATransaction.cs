using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Interfaces;
namespace GDA
{
	public class GDATransaction : GDASession
	{
		private IDbTransaction _transaction;
		private IsolationLevel _isolationLevel;
		public event EventHandler Commited;
		public event EventHandler RolledBack;
		public GDATransaction () : this (IsolationLevel.Unspecified)
		{
		}
		public GDATransaction (IsolationLevel a)
		{
			_isolationLevel = a;
		}
		public GDATransaction (IProviderConfiguration a, IsolationLevel b) : base (a)
		{
			_isolationLevel = b;
		}
		public GDATransaction (IProviderConfiguration a) : this (a, IsolationLevel.Unspecified)
		{
		}
		internal override IDbTransaction CurrentTransaction {
			get {
				BeginTransaction ();
				return _transaction;
			}
			set {
				_transaction = null;
			}
		}
		public override IDbCommand CreateCommand ()
		{
			BeginTransaction ();
			var a = base.CreateCommand ();
			if (a.Transaction == null && _transaction != null)
				a.Transaction = _transaction;
			return a;
		}
		public void BeginTransaction ()
		{
			CheckDisposed ();
			if (_transaction == null) {
				CreateConnection ();
				if (_isolationLevel == IsolationLevel.Unspecified)
					_transaction = CurrentConnection.BeginTransaction ();
				else
					_transaction = CurrentConnection.BeginTransaction (_isolationLevel);
			}
		}
		public void Commit ()
		{
			CheckDisposed ();
			if (_transaction != null && _transaction.Connection != null && _transaction.Connection.State != ConnectionState.Closed) {
				_transaction.Commit ();
				if (Commited != null)
					Commited (this, EventArgs.Empty);
			}
		}
		public void Rollback ()
		{
			CheckDisposed ();
			if (_transaction != null && _transaction.Connection != null && _transaction.Connection.State != ConnectionState.Closed) {
				_transaction.Rollback ();
				if (RolledBack != null)
					RolledBack (this, EventArgs.Empty);
			}
		}
		protected override void DisposeCurrentTransaction (bool a)
		{
			if (_transaction != null)
				try {
					_transaction.Dispose ();
				}
				catch {
					if (a)
						throw;
				}
		}
	}
}
