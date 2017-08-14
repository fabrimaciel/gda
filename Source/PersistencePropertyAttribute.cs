using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace GDA
{
	public enum PersistenceParameterType
	{
		Field,
		Key,
		IdentityKey,
		[Obsolete ("See PersistenceForeignMemberAttribute")]
		ForeignKey
	}
	public enum DirectionParameter
	{
		Output,
		Input,
		InputOutput,
		OutputOnlyInsert,
		OnlyInsert,
		InputOptionalOutput,
		InputOptional,
		InputOptionalOutputOnlyInsert
	}
	[AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class PersistencePropertyAttribute : Attribute
	{
		private string m_Name;
		private PersistenceParameterType m_ParameterType = PersistenceParameterType.Field;
		private int m_Size = 0;
		private DirectionParameter m_Direction = DirectionParameter.InputOutput;
		private string _propertyName;
		private string _generatorKeyName;
		private bool _isNotNull = false;
		public string Name {
			get {
				return m_Name;
			}
			set {
				m_Name = value;
			}
		}
		public PersistenceParameterType ParameterType {
			get {
				return m_ParameterType;
			}
			set {
				m_ParameterType = value;
			}
		}
		public int Size {
			get {
				return m_Size;
			}
			set {
				m_Size = value;
			}
		}
		public DirectionParameter Direction {
			get {
				return m_Direction;
			}
			set {
				m_Direction = value;
			}
		}
		public string PropertyName {
			get {
				return _propertyName;
			}
			set {
				_propertyName = value;
			}
		}
		public string GeneratorKeyName {
			get {
				return _generatorKeyName;
			}
			set {
				_generatorKeyName = value;
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
		public PersistencePropertyAttribute ()
		{
		}
		[Obsolete]
		public PersistencePropertyAttribute (string a, uint b) : this (a)
		{
			Size = (int)b;
		}
		public PersistencePropertyAttribute (PersistenceParameterType a)
		{
			this.m_ParameterType = a;
		}
		public PersistencePropertyAttribute (string a)
		{
			m_Name = a;
		}
		public PersistencePropertyAttribute (string a, DirectionParameter b) : this (a)
		{
			m_Direction = b;
		}
		public PersistencePropertyAttribute (string a, PersistenceParameterType b)
		{
			m_Name = a;
			m_ParameterType = b;
		}
		public PersistencePropertyAttribute (string a, PersistenceParameterType b, DirectionParameter c) : this (a, b)
		{
			m_Direction = c;
		}
		public PersistencePropertyAttribute (string a, PersistenceParameterType b, int c) : this (a, b)
		{
			m_Size = c;
		}
		public PersistencePropertyAttribute (string a, PersistenceParameterType b, int c, DirectionParameter d) : this (a, b, d)
		{
			m_Size = c;
		}
		public PersistencePropertyAttribute (string a, int b)
		{
			m_Name = a;
			m_Size = b;
		}
		public PersistencePropertyAttribute (string a, int b, DirectionParameter c)
		{
			m_Name = a;
			m_Size = b;
			m_Direction = c;
		}
		public override string ToString ()
		{
			return Name;
		}
	}
}
