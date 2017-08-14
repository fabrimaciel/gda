using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using GDA.Interfaces;
namespace GDA
{
	public class GDAStoredProcedure : IEnumerable<GDAParameter>
	{
		private string _name;
		private int _commandTimeOut = GDASession.DefaultCommandTimeout;
		private List<GDAParameter> _parameters = new List<GDAParameter> ();
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		public int CommandTimeout {
			get {
				return _commandTimeOut;
			}
			set {
				_commandTimeOut = value;
			}
		}
		public int Count {
			get {
				return _parameters.Count;
			}
		}
		public GDAStoredProcedure (string a)
		{
			_name = a;
		}
		private GDAParameter FindParameter (string a)
		{
			if (a == null)
				throw new ArgumentNullException ("name");
			GDAParameter b = null;
			if (a [0] == '?') {
				a = a.Substring (1);
				b = _parameters.Find (delegate (GDAParameter c) {
					return c.ParameterName.Substring (1) == a;
				});
			}
			else
				b = _parameters.Find (delegate (GDAParameter c) {
					return c.ParameterName == a;
				});
			if (b == null)
				throw new ItemNotFoundException (String.Format ("Parameter {0} not found in stored procedure.", a));
			return b;
		}
		public GDAStoredProcedure AddOutputParameter (string a, DbType b)
		{
			GDAParameter c = new GDAParameter (a, null);
			c.Direction = ParameterDirection.Output;
			c.DbType = b;
			_parameters.Add (c);
			return this;
		}
		public GDAStoredProcedure AddParameter (string a, object b)
		{
			return AddParameter (a, b, System.Data.ParameterDirection.Input);
		}
		public GDAStoredProcedure AddParameter (string a, object b, System.Data.ParameterDirection c)
		{
			_parameters.Add (new GDAParameter (a, b, c));
			return this;
		}
		public GDAStoredProcedure AddParameter (string a, object b, System.Data.ParameterDirection c, System.Data.DbType d)
		{
			var e = new GDAParameter (a, b, c);
			e.DbType = d;
			_parameters.Add (e);
			return this;
		}
		public GDAStoredProcedure AddParameter (GDAParameter a)
		{
			if (a == null)
				throw new ArgumentNullException ("parameter");
			_parameters.Add (a);
			return this;
		}
		public GDAStoredProcedure AddParameters (GDAParameter[] a)
		{
			if (a == null)
				throw new ArgumentNullException ("parameters");
			foreach (var i in a) {
				if (i != null)
					AddParameter (i);
			}
			return this;
		}
		public GDAStoredProcedure RemoveParameter (string a)
		{
			int b = _parameters.FindIndex (delegate (GDAParameter c) {
				return c.ParameterName == a;
			});
			if (b >= 0)
				_parameters.RemoveAt (b);
			else
				throw new ItemNotFoundException (String.Format ("Parameter {0} not found in stored procedure.", a));
			return this;
		}
		public GDAStoredProcedure SetParameterDirection (string a, System.Data.ParameterDirection b)
		{
			FindParameter (a).Direction = b;
			return this;
		}
		public GDAStoredProcedure SetParameterDbType (string a, System.Data.DbType b)
		{
			FindParameter (a).DbType = b;
			return this;
		}
		internal void Prepare (IDbCommand a, IProvider b)
		{
			a.CommandType = CommandType.StoredProcedure;
			a.CommandTimeout = this.CommandTimeout;
			a.CommandText = this.Name;
			foreach (GDAParameter param in this) {
				a.Parameters.Add (GDA.Helper.GDAHelper.ConvertGDAParameter (a, param, b));
			}
		}
		public object this [string a] {
			get {
				object b = FindParameter (a).Value;
				if (b is DBNull)
					return null;
				else
					return b;
			}
			set {
				FindParameter (a).Value = value;
			}
		}
		public object this [int a] {
			get {
				object b = _parameters [a].Value;
				if (b is DBNull)
					return null;
				else
					return b;
			}
			set {
				_parameters [a].Value = value;
			}
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return _parameters.GetEnumerator ();
		}
		IEnumerator<GDAParameter> IEnumerable<GDAParameter>.GetEnumerator ()
		{
			return _parameters.GetEnumerator ();
		}
	}
}
