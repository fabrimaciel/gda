using System;
using System.Collections.Generic;
using System.Text;
namespace GDA
{
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public class PersistenceClassAttribute : Attribute
	{
		private string _name;
		private string _schema;
		public string Name {
			get {
				return _name;
			}
		}
		public string Schema {
			get {
				return _schema;
			}
			set {
				_schema = value;
			}
		}
		public PersistenceClassAttribute (string a)
		{
			_name = a;
		}
		public PersistenceClassAttribute ()
		{
		}
		public Sql.TableName GetTableName ()
		{
			return new GDA.Sql.TableName (_name, _schema);
		}
	}
}
