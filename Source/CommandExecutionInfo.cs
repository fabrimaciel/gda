using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Diagnostics
{
	public class CommandExecutionInfo : System.Runtime.Serialization.ISerializable, System.Xml.Serialization.IXmlSerializable
	{
		private string _commandText;
		private System.Data.CommandType _commandType;
		private IEnumerable<System.Data.IDataParameter> _parameters;
		private TimeSpan _elapsedTime;
		private int _timeout;
		private bool _success;
		private Exception _error;
		private int _rowsAffects;
		public string CommandText {
			get {
				return _commandText;
			}
		}
		public System.Data.CommandType CommandType {
			get {
				return _commandType;
			}
		}
		public IEnumerable<System.Data.IDataParameter> Parameters {
			get {
				return _parameters;
			}
		}
		public TimeSpan ElapsedTime {
			get {
				return _elapsedTime;
			}
		}
		public bool Success {
			get {
				return _success;
			}
		}
		public Exception Error {
			get {
				return _error;
			}
		}
		public int RowsAffects {
			get {
				return _rowsAffects;
			}
		}
		public int Timeout {
			get {
				return _timeout;
			}
		}
		public CommandExecutionInfo ()
		{
		}
		public CommandExecutionInfo (string a, System.Data.CommandType b, IEnumerable<System.Data.IDataParameter> c, int d, int e, TimeSpan f, Exception g)
		{
			_commandText = a;
			_commandType = b;
			_parameters = c;
			_timeout = d;
			_rowsAffects = e;
			_elapsedTime = f;
			_error = g;
			_success = g == null;
		}
		public CommandExecutionInfo (System.Data.IDbCommand a, TimeSpan b, int c)
		{
			if (a == null)
				throw new ArgumentNullException ("command");
			_commandText = a.CommandText;
			_commandType = a.CommandType;
			if (a.Parameters != null) {
				var d = new System.Data.IDataParameter[a.Parameters.Count];
				var e = a.Parameters.GetEnumerator ();
				for (var f = 0; e.MoveNext (); f++)
					d [f] = (System.Data.IDataParameter)e.Current;
				_parameters = d;
			}
			else
				_parameters = new System.Data.IDataParameter[0];
			_timeout = a.CommandTimeout;
			_rowsAffects = c;
			_elapsedTime = b;
			_success = true;
		}
		public CommandExecutionInfo (System.Data.IDbCommand a) : this (a, TimeSpan.Zero, 0)
		{
		}
		public CommandExecutionInfo (System.Data.IDbCommand a, TimeSpan b, Exception c) : this (a, b, 0)
		{
			_error = c;
			_success = false;
		}
		protected CommandExecutionInfo (System.Runtime.Serialization.SerializationInfo a, System.Runtime.Serialization.StreamingContext b)
		{
			_commandText = a.GetString ("CommandText");
			_commandType = (System.Data.CommandType)a.GetInt16 ("CommandType");
			_elapsedTime = new TimeSpan (a.GetInt64 ("ElapsedTime"));
			_timeout = a.GetInt32 ("Timeout");
			_success = a.GetBoolean ("Success");
			_error = (Exception)a.GetValue ("Error", typeof(Exception));
			_rowsAffects = a.GetInt32 ("RowsAffects");
		}
		public CommandExecutionInfo Finish (TimeSpan a, int b)
		{
			return new CommandExecutionInfo (CommandText, CommandType, Parameters, Timeout, b, a, null);
		}
		public CommandExecutionInfo Fail (TimeSpan a, Exception b)
		{
			return new CommandExecutionInfo (CommandText, CommandType, Parameters, Timeout, 0, a, b);
		}
		public override string ToString ()
		{
			var a = string.Empty;
			var b = Parameters;
			if (b != null)
				a = string.Join (",", b.Select (c => string.Format ("[Name: {0}, Value: {1}]", c.ParameterName, c.Value)).ToArray ());
			if (Success)
				return string.Format ("[CommandType: {0}, ExecutionTime: {1}, RowsAffects: {2},\r\nCommandText: {3},\r\nParameters: {4}]", CommandType, ElapsedTime, RowsAffects, CommandText, a);
			else
				return string.Format ("[CommandType: {0}, ExecutionTime: {1},\r\nCommandText: {3},\r\nParameters: {4},\r\nError: {5}]", CommandType, ElapsedTime, RowsAffects, CommandText, a, Error != null ? Error.Message : "");
		}
		public void GetObjectData (System.Runtime.Serialization.SerializationInfo a, System.Runtime.Serialization.StreamingContext b)
		{
			a.AddValue ("CommandText", CommandText);
			a.AddValue ("CommandType", (short)CommandType);
			a.AddValue ("ElapsedTime", ElapsedTime.Ticks);
			a.AddValue ("Timeout", Timeout);
			a.AddValue ("Success", Success);
			a.AddValue ("Error", Error, typeof(Exception));
			a.AddValue ("RowsAffects", RowsAffects);
		}
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema ()
		{
			throw new NotImplementedException ();
		}
		void System.Xml.Serialization.IXmlSerializable.ReadXml (System.Xml.XmlReader a)
		{
			_commandType = (System.Data.CommandType)Enum.Parse (typeof(System.Data.CommandType), a.GetAttribute ("CommandType"));
			var b = a.GetAttribute ("ElapsedTime").Split (':');
			_elapsedTime = new TimeSpan (0, int.Parse (b [0]), int.Parse (b [1]), int.Parse (b [2].Split ('.') [0]), int.Parse (b [2].Split ('.') [1]));
			_timeout = int.Parse (a.GetAttribute ("Timeout"));
			_success = bool.Parse (a.GetAttribute ("Success"));
			_rowsAffects = int.Parse (a.GetAttribute ("RowsAffects"));
			a.MoveToElement ();
			a.ReadStartElement ();
			_commandText = a.ReadString ();
			a.ReadEndElement ();
			if (!a.IsEmptyElement) {
				var c = new List<System.Data.IDataParameter> ();
				a.ReadStartElement ();
				while (a.NodeType != System.Xml.XmlNodeType.EndElement) {
					var d = new DataParameter ();
					((System.Xml.Serialization.IXmlSerializable)d).ReadXml (a);
					c.Add (d);
				}
				a.ReadEndElement ();
			}
			else
				a.Skip ();
		}
		void System.Xml.Serialization.IXmlSerializable.WriteXml (System.Xml.XmlWriter a)
		{
			a.WriteAttributeString ("CommandType", CommandType.ToString ());
			a.WriteAttributeString ("ElapsedTime", string.Format ("{0:00}:{1:00}:{2:00}.{3:000}", ElapsedTime.TotalHours, ElapsedTime.Minutes, ElapsedTime.Seconds, ElapsedTime.Milliseconds));
			a.WriteAttributeString ("Timeout", Timeout.ToString ());
			a.WriteAttributeString ("Success", Success.ToString ());
			a.WriteAttributeString ("RowsAffects", RowsAffects.ToString ());
			a.WriteStartElement ("CommandText");
			a.WriteString (CommandText);
			a.WriteEndElement ();
			a.WriteStartElement ("Parameters");
			foreach (System.Xml.Serialization.IXmlSerializable i in Parameters.Select (b => new DataParameter (b))) {
				a.WriteStartElement (i.GetType ().Name);
				i.WriteXml (a);
				a.WriteEndElement ();
			}
			a.WriteEndElement ();
		}
		class DataParameter : System.Data.IDataParameter, System.Xml.Serialization.IXmlSerializable
		{
			public System.Data.DbType DbType {
				get;
				set;
			}
			public System.Data.ParameterDirection Direction {
				get;
				set;
			}
			public bool IsNullable {
				get;
				set;
			}
			public string ParameterName {
				get;
				set;
			}
			public string SourceColumn {
				get;
				set;
			}
			public System.Data.DataRowVersion SourceVersion {
				get;
				set;
			}
			public object Value {
				get;
				set;
			}
			public DataParameter (System.Data.IDataParameter a)
			{
				if (a == null)
					throw new ArgumentNullException ("parameter");
				ParameterName = a.ParameterName;
				Value = a.Value;
			}
			public DataParameter (string a, object b)
			{
				this.ParameterName = a;
				this.Value = b;
			}
			public DataParameter ()
			{
			}
			System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema ()
			{
				throw new NotImplementedException ();
			}
			void System.Xml.Serialization.IXmlSerializable.ReadXml (System.Xml.XmlReader a)
			{
				ParameterName = a.GetAttribute ("Name");
				;
				a.MoveToElement ();
				a.ReadStartElement ();
				if (!a.IsEmptyElement)
					Value = a.ReadString ();
				else
					a.Skip ();
				a.ReadEndElement ();
			}
			void System.Xml.Serialization.IXmlSerializable.WriteXml (System.Xml.XmlWriter a)
			{
				a.WriteAttributeString ("Name", ParameterName);
				if (Value != null && Value != DBNull.Value)
					a.WriteString (Value.ToString ());
			}
		}
	}
}
