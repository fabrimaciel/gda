using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	public class Mapper
	{
		private string _name;
		private PersistenceParameterType _parameterType;
		private int _size;
		private DirectionParameter _direction;
		private string _propertyMapperName;
		private PropertyInfo _propertyMapper;
		private ValidationMapper _validation;
		private IList<ForeignKeyMapper> _foreignKeys;
		private Type _modelType;
		private string _generatorKeyName;
		private IGeneratorKey _generatorKey;
		private bool _isNotNull;
		public string Name {
			get {
				return _name;
			}
			set {
				_name = value;
			}
		}
		public PersistenceParameterType ParameterType {
			get {
				return _parameterType;
			}
			set {
				_parameterType = value;
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
		public DirectionParameter Direction {
			get {
				return _direction;
			}
			set {
				_direction = value;
			}
		}
		public PropertyInfo PropertyMapper {
			get {
				return _propertyMapper;
			}
		}
		public string PropertyMapperName {
			get {
				return _propertyMapperName;
			}
		}
		public IList<ForeignKeyMapper> ForeignKeys {
			get {
				return _foreignKeys;
			}
		}
		public ValidationMapper Validation {
			get {
				return _validation;
			}
			internal set {
				_validation = value;
			}
		}
		public string GeneratorKeyName {
			get {
				return _generatorKeyName;
			}
			internal set {
				_generatorKeyName = value;
			}
		}
		public IGeneratorKey GeneratorKey {
			get {
				if (_generatorKey == null && !string.IsNullOrEmpty (_generatorKeyName)) {
					_generatorKey = GDASettings.GetGeneratorKey (_generatorKeyName);
					if (_generatorKey == null)
						throw new GDAException ("Generator key \"{0}\" not found.", _generatorKeyName);
				}
				return _generatorKey;
			}
		}
		public bool IsNotNull {
			get {
				return _isNotNull;
			}
			set {
				_isNotNull = value;
			}
		}
		public bool IsNullableType {
			get {
				return Helper.TypeHelper.IsNullableType (PropertyMapper.PropertyType);
			}
		}
		public Type DeclaringType {
			get {
				return PropertyMapper.DeclaringType;
			}
		}
		public Type Type {
			get {
				return Helper.TypeHelper.GetMemberType (PropertyMapper);
			}
		}
		public bool IsAssociation {
			get {
				return (ForeignKeys != null && ForeignKeys.Count > 0);
			}
		}
		public bool IsDbGenerated {
			get {
				return ParameterType == PersistenceParameterType.IdentityKey;
			}
		}
		public bool IsPrimaryKey {
			get {
				return ParameterType == PersistenceParameterType.Key || ParameterType == PersistenceParameterType.IdentityKey;
			}
		}
		public Mapper (Type a, PersistencePropertyAttribute b, PropertyInfo c)
		{
			this._modelType = a;
			this._name = b.Name;
			this._direction = b.Direction;
			this._parameterType = b.ParameterType;
			this._size = b.Size;
			this._propertyMapper = c;
			this._propertyMapperName = c.Name;
			this._generatorKeyName = b.GeneratorKeyName;
			this._isNotNull = b.IsNotNull;
		}
		public Mapper (Type a, string b, DirectionParameter c, PersistenceParameterType d, int e, PropertyInfo f, string g)
		{
			this._modelType = a;
			this._name = b;
			this._direction = c;
			this._parameterType = d;
			this._size = e;
			this._propertyMapper = f;
			if (f != null)
				this._propertyMapperName = f.Name;
			this._generatorKeyName = g;
		}
		public void AddForeignKey (ForeignKeyMapper a)
		{
			if (_foreignKeys == null)
				_foreignKeys = new List<ForeignKeyMapper> ();
			_foreignKeys.Add (a);
		}
		public override string ToString ()
		{
			return _propertyMapperName + " : " + _name;
		}
	}
}
