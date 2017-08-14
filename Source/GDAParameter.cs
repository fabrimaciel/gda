using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
namespace GDA
{
	public interface IGDAParameterContainer : IEnumerable<GDAParameter>
	{
		void Add (GDAParameter a);
		bool TryGet (string a, out GDAParameter b);
		bool ContainsKey (string a);
		bool Remove (string a);
	}
	public class GDAParameter
	{
		private System.Data.DbType _dbType;
		private Type _valueType;
		private string _parameterName;
		private object _value;
		private int _size;
		private string _sourceColumn;
		private ParameterDirection _direction = ParameterDirection.Input;
		private bool _isNullable;
		private bool _dbTypeIsDefined = false;
		private bool _nativeDbTypeIsDefined = false;
		private object _nativeDbType;
		public GDAParameter ()
		{
		}
		public GDAParameter (string a, object b)
		{
			_parameterName = a;
			if (b != null && b.GetType ().IsEnum) {
				switch (Enum.GetUnderlyingType (b.GetType ()).Name) {
				case "Int16":
					_value = (short)b;
					break;
				case "UInt16":
					_value = (ushort)b;
					break;
				case "Int32":
					_value = (int)b;
					break;
				case "UInt32":
					_value = (uint)b;
					break;
				case "Byte":
					_value = (byte)b;
					break;
				default:
					_value = b;
					break;
				}
			}
			else
				_value = b;
		}
		public GDAParameter (string a, object b, int c) : this (a, b)
		{
			_size = c;
		}
		public GDAParameter (string a, object b, string c) : this (a, b)
		{
			_sourceColumn = c;
		}
		public GDAParameter (string a, object b, string c, int d) : this (a, b, c)
		{
			_size = d;
		}
		public GDAParameter (string a, object b, ParameterDirection c) : this (a, b)
		{
			this._direction = c;
		}
		public DbType DbType {
			get {
				return _dbType;
			}
			set {
				_dbType = value;
				_dbTypeIsDefined = true;
			}
		}
		public string ParameterName {
			get {
				return _parameterName;
			}
			set {
				_parameterName = value;
			}
		}
		public object Value {
			get {
				return _value;
			}
			set {
				_value = value;
			}
		}
		public int Size {
			get {
				return _size;
			}
			set {
				_size = value;
			}
		}
		public System.Data.ParameterDirection Direction {
			get {
				return _direction;
			}
			set {
				_direction = value;
			}
		}
		public bool IsNullable {
			get {
				return _isNullable;
			}
			set {
				_isNullable = value;
			}
		}
		public string SourceColumn {
			get {
				return _sourceColumn;
			}
			set {
				_sourceColumn = value;
			}
		}
		public bool DbTypeIsDefined {
			get {
				return _dbTypeIsDefined;
			}
			set {
				_dbTypeIsDefined = value;
			}
		}
		public object NativeDbType {
			get {
				return _nativeDbType;
			}
			set {
				_nativeDbType = value;
				_nativeDbTypeIsDefined = true;
			}
		}
		public bool NativeDbTypeIsDefined {
			get {
				return _nativeDbTypeIsDefined;
			}
			set {
				_nativeDbTypeIsDefined = value;
			}
		}
	}
}
