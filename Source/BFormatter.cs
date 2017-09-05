/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections;

namespace GDA.Helper.Serialization
{
	/// <summary>
	/// Atributo para identifica que aquele campo ou método não será serializado.
	/// </summary>
	public class BNonSerializeAttribute : Attribute
	{
	}
	/// <summary>
	/// Atribute que identifica o tamanho máximo em byte que o campo pode conter.
	/// </summary>
	public class SerializableMaxLenghtAttribute : Attribute
	{
		private int _maxLenght;

		/// <summary>
		/// Gets a quantidade máxima de byte que o campo suporta.
		/// </summary>
		public int MaxLenght
		{
			get
			{
				return _maxLenght;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="maxLenght">Tamanho máximo para serializar.</param>
		public SerializableMaxLenghtAttribute(int maxLenght)
		{
			_maxLenght = maxLenght;
		}
	}
	internal class PropertyInfoComparer : IComparer<PropertyInfo>
	{
		public int Compare(PropertyInfo x, PropertyInfo y)
		{
			return Comparer.Default.Compare(x.Name, y.Name);
		}
	}
	internal class FieldInfoComparer : IComparer<FieldInfo>
	{
		public int Compare(FieldInfo x, FieldInfo y)
		{
			return Comparer.Default.Compare(x.Name, y.Name);
		}
	}
	/// <summary>
	/// Serializa e deserializa um objeto em um junto de dados binários.
	/// </summary>
	public class BFormatter
	{
		/// <summary>
		/// Tipos básico suportados para serialização
		/// </summary>
		private readonly static Type[] coreTypes =  {
			typeof(byte[]),
			typeof(byte),
			typeof(bool),
			typeof(char),
			typeof(double),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(float),
			typeof(ushort),
			typeof(uint),
			typeof(ulong),
			typeof(string),
			typeof(DateTime)
		};

		/// <summary>
		/// Tipos que terão uma serialização completa.
		/// </summary>
		private static List<Type> fullSerializeTypes = new List<Type>(new Type[] {
			typeof(Guid)
		});

		/// <summary>
		/// Armazena as informações sobre os tipos suportados.
		/// </summary>
		internal class InfoCoreSupport
		{
			/// <summary>
			/// Construtor padrão
			/// </summary>
			/// <param name="coreTypeSupported">Identifica se é um tipo básico suportado.</param>
			/// <param name="allowNullValue">Permite valores nulos.</param>
			public InfoCoreSupport(bool coreTypeSupported, bool allowNullValue)
			{
				this.coreTypeSupported = coreTypeSupported;
				this.allowNullValue = allowNullValue;
				fieldInfo = null;
				propertyInfo = null;
			}

			/// <summary>
			/// Identifica se o tipo do paramentro é suportado
			/// </summary>
			public bool coreTypeSupported;

			/// <summary>
			/// Identifica se o tipo do paramentro suporta valor nulos.
			/// </summary>
			public bool allowNullValue;

			/// <summary>
			/// Informações sobre o campo.
			/// </summary>
			public FieldInfo fieldInfo;

			/// <summary>
			/// Informações sobre a propriedade.
			/// </summary>
			public PropertyInfo propertyInfo;

			/// <summary>
			/// Tamanho máximo do membro.
			/// </summary>
			public int maxLenght;

			/// <summary>
			/// Recupera o valor do membro do objeto informado. 
			/// </summary>
			/// <param name="graph"></param>
			/// <returns></returns>
			public object GetValue(object graph)
			{
				if(fieldInfo != null)
					return fieldInfo.GetValue(graph);
				else if(propertyInfo != null)
					return propertyInfo.GetValue(graph, null);
				else
					throw new InvalidOperationException();
			}

			/// <summary>
			/// Recupera o tipo do membro.
			/// </summary>
			/// <returns></returns>
			public Type GetMemberType()
			{
				if(fieldInfo != null)
					return fieldInfo.FieldType;
				else if(propertyInfo != null)
					return propertyInfo.PropertyType;
				else
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Verifica se o tipo informado é uma estrutura.
		/// </summary>
		/// <param name="type">Tipo.</param>
		/// <returns></returns>
		public static bool IsStruct(Type type)
		{
			return type.IsValueType && !type.IsPrimitive && type.BaseType == typeof(ValueType);
		}

		/// <summary>
		/// Verifica se é um tipo básico aceitável para serialização.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsCoreType(Type type)
		{
			foreach (Type t in coreTypes)
				if(t == type)
					return true;
			return false;
		}

		/// <summary>
		/// Verifica se o tipo é suportado pela serialização.
		/// </summary>
		/// <param name="type">Tipo a ser verificado.</param>
		/// <returns>
		/// <list type="bool">
		/// <item>True: o tipo é suportado.</item>
		/// <item>False: o tipo não é suportado.</item>
		/// </list>
		/// </returns>
		private static InfoCoreSupport Support(Type type)
		{
			InfoCoreSupport icts = new InfoCoreSupport(false, false);
			if(type.Name == "Nullable`1")
			{
				icts.allowNullValue = true;
				type = Nullable.GetUnderlyingType(type);
			}
			if(type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			if(type.IsArray && type.GetElementType() != typeof(object))
			{
				icts.coreTypeSupported = true;
				icts.allowNullValue = true;
				return icts;
			}
			else if(IsStruct(type))
			{
				icts.coreTypeSupported = true;
				return icts;
			}
			foreach (Type supportedType in BFormatter.coreTypes)
				if(supportedType.IsAssignableFrom(type))
				{
					icts.coreTypeSupported = true;
					if(!icts.allowNullValue && (supportedType.IsAssignableFrom(typeof(string)) || supportedType.IsAssignableFrom(typeof(byte[]))))
						icts.allowNullValue = true;
					return icts;
				}
			icts.allowNullValue = true;
			icts.coreTypeSupported = true;
			return icts;
		}

		/// <summary>
		/// Escreve o valor do objeto na stream.
		/// </summary>
		/// <param name="stream">Stream aonde serão salvos os dados do objeto.</param>
		/// <param name="o">Objeto contendo os dados.</param>
		/// <param name="type">Tipo do valor a ser serializado.</param>
		/// <param name="maxLenght">Tamanho máximo a ser serializado.</param>
		private static void WriteData(Stream stream, object o, Type type, int maxLenght)
		{
			if(type.IsAssignableFrom(typeof(string)) && o == null)
				o = "";
			if(type.Name == "Nullable`1")
			{
				type = Nullable.GetUnderlyingType(type);
				if(o == null)
					o = Activator.CreateInstance(type);
			}
			if(type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
				switch(type.Name)
				{
				case "Int16":
					o = (short)o;
					break;
				case "UInt16":
					o = (ushort)o;
					break;
				case "Int32":
					o = (int)o;
					break;
				case "UInt32":
					o = (uint)o;
					break;
				case "Byte":
					o = (byte)o;
					break;
				default:
					o = (int)o;
					break;
				}
			}
			else if(IsStruct(type) && !type.IsAssignableFrom(typeof(DateTime)) && !type.IsAssignableFrom(typeof(decimal)))
			{
				SerializeBase(stream, null, 0, 0, o);
				return;
			}
			if(type.IsAssignableFrom(typeof(string)))
			{
				string s = (string)o;
				int size = 0;
				int lenght = s.Length;
				if(maxLenght > 0)
				{
					lenght = (lenght > maxLenght ? maxLenght : lenght);
					if(maxLenght < byte.MaxValue)
						size = 1;
					else if(maxLenght < ushort.MaxValue)
						size = sizeof(ushort);
					else if(maxLenght < int.MaxValue)
						size = sizeof(uint);
				}
				else
				{
					size = sizeof(ushort);
					lenght = s.Length;
				}
				stream.Write(BitConverter.GetBytes(lenght), 0, size);
				stream.Write(Encoding.Default.GetBytes(s), 0, lenght);
			}
			else if(type.IsAssignableFrom(typeof(DateTime)))
			{
				stream.Write(BitConverter.GetBytes(((DateTime)o).Ticks), 0, sizeof(long));
			}
			else if(type.IsAssignableFrom(typeof(decimal)))
			{
				stream.Write(DecimalToBytes(Convert.ToDecimal(o)), 0, sizeof(decimal));
			}
			else if(type.IsAssignableFrom(typeof(byte)))
			{
				stream.WriteByte((byte)o);
			}
			else if(type.IsAssignableFrom(typeof(byte[])))
			{
				byte[] buffer = (byte[])o;
				int size = 0;
				int lenght = buffer.Length;
				if(maxLenght > 0)
				{
					lenght = (lenght > maxLenght ? maxLenght : lenght);
					if(maxLenght < byte.MaxValue)
						size = 1;
					else if(maxLenght < ushort.MaxValue)
						size = sizeof(ushort);
					else if(maxLenght < int.MaxValue)
						size = sizeof(uint);
				}
				else
				{
					size = sizeof(ushort);
					lenght = buffer.Length;
				}
				stream.Write(BitConverter.GetBytes(lenght), 0, size);
				stream.Write(buffer, 0, lenght);
			}
			else if(type.IsArray)
			{
				SerializeBase(stream, null, 0, maxLenght, o);
			}
			else
			{
				int size = 0;
				if(type.IsAssignableFrom(typeof(bool)))
					size = sizeof(bool);
				else if(type.IsAssignableFrom(typeof(char)))
					size = sizeof(char);
				else if(!Array.Exists(coreTypes, delegate(Type tc1) {
					return tc1 == type;
				}))
				{
					SerializeBase(stream, null, 0, 0, o);
					return;
				}
				else
				{
					try
					{
						size = System.Runtime.InteropServices.Marshal.SizeOf(type);
					}
					catch(ArgumentException)
					{
						var buffer = BFormatter.Serialize(o);
						stream.Write(BitConverter.GetBytes(buffer.Length), 0, sizeof(int));
						stream.Write(buffer, 0, buffer.Length);
						return;
					}
				}
				MethodInfo m = typeof(BitConverter).GetMethod("GetBytes", BindingFlags.Public | BindingFlags.Static, null, new Type[] {
					type
				}, null);
				stream.Write((byte[])m.Invoke(null, BindingFlags.Default, null, new object[] {
					o
				}, CultureInfo.CurrentCulture), 0, size);
			}
		}

		/// <summary>
		/// Lê os dados da stream e salva no objeto
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="type">Tipo de dado a ser lido.</param>
		/// <return>Valor lido.</return>
		private static object ReadData(Stream stream, Type type, int maxLenght)
		{
			if(type.Name == "Nullable`1")
				type = Nullable.GetUnderlyingType(type);
			Type typeEnum = null;
			if(type.IsEnum)
			{
				typeEnum = type;
				type = Enum.GetUnderlyingType(type);
			}
			else if(IsStruct(type) && !type.IsAssignableFrom(typeof(DateTime)) && !type.IsAssignableFrom(typeof(decimal)))
			{
				object obj = Activator.CreateInstance(type);
				DeserializeBase(stream, type, null, 0, 0, obj);
				return obj;
			}
			int size = 0;
			if(type.IsAssignableFrom(typeof(string)))
			{
				if(maxLenght > 0)
				{
					if(maxLenght < byte.MaxValue)
						size = 1;
					else if(maxLenght < ushort.MaxValue)
						size = sizeof(ushort);
					else if(maxLenght < int.MaxValue)
						size = sizeof(uint);
				}
				else
				{
					size = sizeof(ushort);
				}
				byte[] bufferAux = new byte[size];
				stream.Read(bufferAux, 0, size);
				if(maxLenght > 0)
				{
					if(maxLenght < byte.MaxValue)
						size = (int)bufferAux[0];
					else if(maxLenght < ushort.MaxValue)
						size = BitConverter.ToInt16(bufferAux, 0);
					else if(maxLenght < int.MaxValue)
						size = BitConverter.ToInt32(bufferAux, 0);
				}
				else
					size = BitConverter.ToUInt16(bufferAux, 0);
			}
			else if(type.IsAssignableFrom(typeof(bool)))
				size = sizeof(bool);
			else if(type.IsAssignableFrom(typeof(DateTime)))
				size = sizeof(long);
			else if(type.IsAssignableFrom(typeof(decimal)))
				size = sizeof(decimal);
			else if(type.IsAssignableFrom(typeof(char)))
				size = sizeof(char);
			else if(type.IsAssignableFrom(typeof(byte)))
				size = sizeof(byte);
			else if(type.IsAssignableFrom(typeof(byte[])))
			{
				if(maxLenght > 0)
				{
					if(maxLenght < byte.MaxValue)
						size = 1;
					else if(maxLenght < ushort.MaxValue)
						size = sizeof(ushort);
					else if(maxLenght < int.MaxValue)
						size = sizeof(uint);
				}
				else
				{
					size = sizeof(ushort);
				}
				byte[] bufferAux = new byte[size];
				stream.Read(bufferAux, 0, size);
				if(maxLenght > 0)
				{
					if(maxLenght < byte.MaxValue)
						size = (int)bufferAux[0];
					else if(maxLenght < ushort.MaxValue)
						size = BitConverter.ToInt16(bufferAux, 0);
					else if(maxLenght < int.MaxValue)
						size = BitConverter.ToInt32(bufferAux, 0);
				}
				else
					size = BitConverter.ToUInt16(bufferAux, 0);
				bufferAux = new byte[size];
				stream.Read(bufferAux, 0, size);
				return bufferAux;
			}
			else if(!type.IsArray && !Array.Exists(coreTypes, delegate(Type tc1) {
				return tc1 == type;
			}))
			{
				return DeserializeBase(stream, type, null, 0, maxLenght, null);
			}
			else
			{
				try
				{
					size = System.Runtime.InteropServices.Marshal.SizeOf(type);
				}
				catch(ArgumentException)
				{
					byte[] buffer = new byte[sizeof(int)];
					stream.Read(buffer, 0, sizeof(int));
					size = BitConverter.ToInt16(buffer, 0);
					buffer = new byte[size];
					stream.Read(buffer, 0, size);
					return BFormatter.Deserialize(buffer, type);
				}
			}
			if(type.IsArray)
			{
				return DeserializeBase(stream, type, null, 0, maxLenght, null);
			}
			else
			{
				byte[] buffer = new byte[size];
				stream.Read(buffer, 0, size);
				if(type.IsAssignableFrom(typeof(string)))
					return Encoding.Default.GetString(buffer, 0, size);
				else if(type.IsAssignableFrom(typeof(byte[])))
					return buffer;
				else if(type.IsAssignableFrom(typeof(byte)))
					return buffer[0];
				else if(type.IsAssignableFrom(typeof(DateTime)))
					return new DateTime(BitConverter.ToInt64(buffer, 0));
				else if(type.IsAssignableFrom(typeof(decimal)))
					return BytesToDecimal(buffer);
				else
				{
					MethodInfo m = typeof(BitConverter).GetMethod("To" + type.Name);
					if(typeEnum != null)
					{
						return Enum.ToObject(typeEnum, m.Invoke(null, new object[] {
							buffer,
							0
						}));
					}
					else
						return m.Invoke(null, new object[] {
							buffer,
							0
						});
				}
			}
		}

		/// <summary>
		/// Extrai os dados do objeto.
		/// </summary>
		/// <param name="streamOut">Stream onde será armazenado os dados extraídos.</param>
		/// <param name="supports">Informações dos membros onde estão os dados.</param>
		/// <param name="membersAllowNullCount">Quantidade de membros que aceitam valores nulos.</param>
		/// <param name="graph">Objeto de onde será estraído os dados.</param>
		private static void Export(Stream streamOut, InfoCoreSupport[] supports, short membersAllowNullCount, object graph)
		{
			int beginStreamPos = (int)streamOut.Position;
			int nBytesAllowNull = Convert.ToInt32(Math.Ceiling(membersAllowNullCount / 8.0d));
			byte[] bAllowNull = new byte[nBytesAllowNull];
			streamOut.Write(bAllowNull, 0, bAllowNull.Length);
			bool[] memberNulls = new bool[membersAllowNullCount];
			int i = 0;
			foreach (InfoCoreSupport sp in supports)
			{
				object value = sp.GetValue(graph);
				if(sp.allowNullValue)
					memberNulls[i++] = (value == null);
				if(!sp.allowNullValue || value != null)
					WriteData(streamOut, value, sp.GetMemberType(), sp.maxLenght);
			}
			int j = 0, x = 0;
			for(i = 0; i < nBytesAllowNull; i++)
			{
				bAllowNull[i] = 0x00;
				x = 0;
				for(; (x < 8) && (j < (nBytesAllowNull * 8)) && j < membersAllowNullCount; j++)
				{
					if(memberNulls[j])
					{
						bAllowNull[i] = (byte)(bAllowNull[i] | (byte)Convert.ToInt32(Math.Pow(2.0d, (double)x)));
					}
					x++;
				}
			}
			int pos = (int)streamOut.Position;
			streamOut.Seek(beginStreamPos, SeekOrigin.Begin);
			streamOut.Write(bAllowNull, 0, bAllowNull.Length);
			streamOut.Seek(pos, SeekOrigin.Begin);
		}

		/// <summary>
		/// Importa os dados do objeto contidos na stream.
		/// </summary>
		/// <param name="streamIn">Stream onde estão os dados a serem importados.</param>
		/// <param name="supports">Informações dos membros que mapeam os dados.</param>
		/// <param name="memberAllowNullCount">Quantidade de membros que aceitam valores nulos.</param>
		/// <param name="graph">Objeto onde os dados importados serão salvos.</param>
		private static void Import(Stream streamIn, InfoCoreSupport[] supports, short memberAllowNullCount, object graph)
		{
			int n = Convert.ToInt32(Math.Ceiling(memberAllowNullCount / 8.0d));
			bool[] fieldsNulls = new bool[memberAllowNullCount];
			if(n > 0)
			{
				byte[] buffer = new byte[n];
				streamIn.Read(buffer, 0, n);
				int j, x = 0;
				for(int i = 0; i < n; i++)
				{
					j = 0;
					for(; j < 8 && j < memberAllowNullCount; j++)
					{
						fieldsNulls[x++] = (((buffer[i] >> j) % 2) != 0);
					}
					memberAllowNullCount -= 8;
				}
			}
			memberAllowNullCount = 0;
			object[] attributes;
			int maxLenght = 0;
			for(int i = 0; i < supports.Length; i++)
			{
				if(supports[i].fieldInfo != null)
				{
					if(supports[i].allowNullValue && fieldsNulls[memberAllowNullCount++])
					{
						supports[i].fieldInfo.SetValue(graph, null);
						continue;
					}
					attributes = supports[i].fieldInfo.GetCustomAttributes(typeof(SerializableMaxLenghtAttribute), true);
					if(attributes.Length > 0)
						maxLenght = (int)((SerializableMaxLenghtAttribute)attributes[0]).MaxLenght;
					else
						maxLenght = 0;
					supports[i].fieldInfo.SetValue(graph, ReadData(streamIn, supports[i].fieldInfo.FieldType, maxLenght));
				}
				else
				{
					if(supports[i].allowNullValue && fieldsNulls[memberAllowNullCount++])
					{
						supports[i].propertyInfo.SetValue(graph, null, null);
						continue;
					}
					attributes = supports[i].propertyInfo.GetCustomAttributes(typeof(SerializableMaxLenghtAttribute), true);
					if(attributes.Length > 0)
						maxLenght = (int)((SerializableMaxLenghtAttribute)attributes[0]).MaxLenght;
					else
						maxLenght = 0;
					supports[i].propertyInfo.SetValue(graph, ReadData(streamIn, supports[i].propertyInfo.PropertyType, maxLenght), null);
				}
			}
		}

		/// <summary>
		/// Carrega as informações sobre o tipo.
		/// </summary>
		/// <param name="type">Tipo a ser examinado.</param>
		/// <param name="membersAllowNullCount">Númeor de membros que permite valores nulos.</param>
		/// <returns>Informações de suporte.</returns>
		internal static InfoCoreSupport[] LoadTypeInformation(Type type, out short membersAllowNullCount)
		{
			short allowNullCount = 0;
			List<InfoCoreSupport> supports = new List<InfoCoreSupport>();
			object[] attributes;
			int maxLenght = 0;
			InfoCoreSupport coreSupport;
			List<FieldInfo> fsInfo = new List<FieldInfo>(fullSerializeTypes.Exists(delegate(Type tc1) {
				return tc1 == type;
			}) ? type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance) : type.GetFields());
			fsInfo.Sort(0, fsInfo.Count, new FieldInfoComparer());
			foreach (FieldInfo fi in fsInfo)
			{
				attributes = fi.GetCustomAttributes(typeof(BNonSerializeAttribute), true);
				if(attributes.Length > 0)
					continue;
				attributes = fi.GetCustomAttributes(typeof(SerializableMaxLenghtAttribute), true);
				if(attributes.Length > 0)
					maxLenght = (int)((SerializableMaxLenghtAttribute)attributes[0]).MaxLenght;
				else
					maxLenght = 0;
				coreSupport = Support(fi.FieldType);
				if(coreSupport.coreTypeSupported && !((fi.Attributes & FieldAttributes.Static) == FieldAttributes.Static))
				{
					if(coreSupport.allowNullValue)
						allowNullCount++;
					coreSupport.fieldInfo = fi;
					coreSupport.maxLenght = maxLenght;
					supports.Add(coreSupport);
				}
			}
			List<PropertyInfo> psInfo = new List<PropertyInfo>(fullSerializeTypes.Exists(delegate(Type tc1) {
				return tc1 == type;
			}) ? type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance) : type.GetProperties());
			psInfo.Sort(0, psInfo.Count, new PropertyInfoComparer());
			foreach (PropertyInfo pi in psInfo)
			{
				if(pi.GetSetMethod() == null)
					continue;
				attributes = pi.GetCustomAttributes(typeof(BNonSerializeAttribute), true);
				if(attributes.Length > 0)
					continue;
				attributes = pi.GetCustomAttributes(typeof(SerializableMaxLenghtAttribute), true);
				if(attributes.Length > 0)
					maxLenght = (int)((SerializableMaxLenghtAttribute)attributes[0]).MaxLenght;
				else
					maxLenght = 0;
				coreSupport = Support(pi.PropertyType);
				if(coreSupport.coreTypeSupported)
				{
					if(coreSupport.allowNullValue)
						allowNullCount++;
					coreSupport.propertyInfo = pi;
					coreSupport.maxLenght = maxLenght;
					supports.Add(coreSupport);
				}
			}
			membersAllowNullCount = allowNullCount;
			return supports.ToArray();
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="stream">Stream onde os dados estão armazenados.</param>
		/// <param name="type">Tipo que será recuperado.</param>
		/// <param name="supports">Membros mapeados.</param>
		/// <param name="memberAllowNull">Quantidade de membros que aceitam valores nulos.</param>
		/// <param name="maxLenght">Tamanho máximo aceito.</param>
		/// <param name="graph">Objeto onde a deserialização será salva.</param>
		/// <returns></returns>
		internal static object DeserializeBase(Stream stream, Type type, InfoCoreSupport[] supports, short memberAllowNullCount, int maxLenght, object graph)
		{
			bool isArray = type.IsArray;
			if(isArray)
				type = type.GetElementType();
			bool isCore = IsCoreType(type);
			if(isCore && !isArray)
			{
				if(stream.Length > 0)
					return ReadData(stream, type, 0);
				else
					return null;
			}
			if(!type.IsArray && graph == null)
			{
				ConstructorInfo ci = type.GetConstructor(new Type[] {

				});
				if(ci != null)
					graph = ci.Invoke(null);
				else
					graph = Activator.CreateInstance(type);
			}
			if(isArray)
			{
				int size = ReadArrayLenght(stream, maxLenght);
				byte[] buffer = new byte[sizeof(int)];
				Array array = Array.CreateInstance(type, size);
				for(int j = 0; j < size; j++)
				{
					if(isCore)
					{
						array.SetValue(ReadData(stream, type, 0), j);
					}
					else
					{
						stream.Read(buffer, 0, sizeof(int));
						int itemSize = BitConverter.ToInt32(buffer, 0);
						array.SetValue(DeserializeBase(stream, type, supports, memberAllowNullCount, 0, null), j);
					}
				}
				return array;
			}
			else
			{
				if(supports == null)
					supports = LoadTypeInformation(type, out memberAllowNullCount);
				Import(stream, supports, memberAllowNullCount, graph);
				return graph;
			}
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="stream">Stream onde será salvo os dados.</param>
		/// <param name="supports">Membros mapeados.</param>
		/// <param name="membersAllowNull">Quantidade de membros que aceitam valores nulos.</param>
		/// <param name="maxLenght">Tamanho máximo aceito.</param>
		/// <param name="graph">Objeto que será serializado.</param>
		internal static void SerializeBase(Stream stream, InfoCoreSupport[] supports, short membersAllowNullCount, int maxLenght, object graph)
		{
			if(graph == null)
				throw new ArgumentException("graph");
			Type t = graph.GetType();
			bool isArray = t.IsArray;
			if(isArray)
			{
				t = t.GetElementType();
			}
			bool isCore = IsCoreType(t);
			if(isCore && !isArray)
			{
				if(graph != null)
					WriteData(stream, graph, t, 0);
				return;
			}
			if(supports == null)
			{
				if(!isCore)
					supports = LoadTypeInformation(t, out membersAllowNullCount);
				else
					supports = new InfoCoreSupport[0];
			}
			if(isArray)
			{
				Array array = (Array)graph;
				int size = 0;
				int lenght = array.Length;
				if(maxLenght > 0)
				{
					lenght = (lenght > maxLenght ? maxLenght : lenght);
					if(maxLenght < byte.MaxValue)
						size = 1;
					else if(maxLenght < ushort.MaxValue)
						size = sizeof(ushort);
					else if(maxLenght < int.MaxValue)
						size = sizeof(uint);
				}
				else
					size = sizeof(ushort);
				stream.Write(BitConverter.GetBytes(lenght), 0, size);
				for(int i = 0; i < lenght; i++)
				{
					object obj = array.GetValue(i);
					if(isCore)
					{
						WriteData(stream, obj, t, 0);
					}
					else
					{
						int pos = (int)stream.Position;
						stream.Write(new byte[sizeof(int)], 0, sizeof(int));
						Export(stream, supports, membersAllowNullCount, obj);
						int endPos = (int)stream.Position;
						stream.Seek(pos, SeekOrigin.Begin);
						stream.Write(BitConverter.GetBytes((int)(endPos - (pos + sizeof(int)))), 0, sizeof(int));
						stream.Seek(endPos, SeekOrigin.Begin);
					}
				}
				return;
			}
			else
				Export(stream, supports, membersAllowNullCount, graph);
		}

		/// <summary>
		/// Registra o tipo que terá uma serialização completa.
		/// </summary>
		/// <param name="type"></param>
		public static void RegisterFullSerializeType(Type type)
		{
			if(!fullSerializeTypes.Exists(delegate(Type tc1) {
				return tc1 == type;
			}))
				fullSerializeTypes.Add(type);
		}

		/// <summary>
		/// Lê o tamanho do vetor serializado no stream.
		/// </summary>
		/// <param name="stream">Stream onde está armazenada os dados.</param>
		/// <param name="maxLenght">Tamanho máximo do vetor.</param>
		/// <returns>Tamanho do vetor salvo no arquivo.</returns>
		public static int ReadArrayLenght(Stream stream, int maxLenght)
		{
			int size = 0;
			if(maxLenght > 0)
			{
				if(maxLenght <= byte.MaxValue)
					size = 1;
				else if(maxLenght <= ushort.MaxValue)
					size = sizeof(ushort);
				else if(maxLenght <= int.MaxValue)
					size = sizeof(uint);
			}
			else
				size = sizeof(int);
			byte[] buffer = new byte[size];
			stream.Read(buffer, 0, size);
			if(maxLenght > 0)
			{
				if(maxLenght <= byte.MaxValue)
					size = (int)buffer[0];
				else if(maxLenght <= ushort.MaxValue)
					size = BitConverter.ToInt16(buffer, 0);
				else if(maxLenght <= int.MaxValue)
					size = BitConverter.ToInt32(buffer, 0);
			}
			else
				size = BitConverter.ToInt32(buffer, 0);
			return size;
		}

		/// <summary>
		/// Serializa o objeto passado, e armazena os dados na stream.
		/// </summary>
		/// <param name="graph">Objeto a serializado.</param>
		public static byte[] Serialize(object graph)
		{
			if(graph == null)
				throw new ArgumentException("graph");
			byte[] buffer = null;
			using (MemoryStream stream = new MemoryStream())
			{
				Serialize(stream, graph);
				stream.Seek(0, SeekOrigin.Begin);
				buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
			}
			return buffer;
		}

		/// <summary>
		/// Serializa o objeto passado, e armazena os dados na stream.
		/// </summary>
		/// <param name="serializationStream">Stream aonde serão armazenados os dados.</param>
		/// <param name="graph">Objeto a serializado.</param>
		/// movimentar o curso da stream para o inicio.</param>
		public static void Serialize(Stream serializationStream, object graph)
		{
			SerializeBase(serializationStream, null, 0, 0, graph);
		}

		/// <summary>
		/// Deseriliza os dados contidos na stream e retorna o objeto do tipo passado com os dados preenchidos.
		/// </summary>
		/// <param name="serializationStream">Stream onde estão os dados para serem deserializados.</param>
		/// <param name="typeReturn">Tipo de retorno do elemento.</param>
		/// <returns>Objeto com os dados dados preenchidos.</returns>
		public static object Deserialize(Stream serializationStream, Type typeReturn)
		{
			return DeserializeBase(serializationStream, typeReturn, null, 0, 0, null);
		}

		/// <summary>
		/// Deseriliza os dados contidos no buffer e retorna o objeto do tipo passado com os dados preenchidos.
		/// </summary>
		/// <param name="buffer">Buffer onde estão os dados para serem deserializados.</param>
		/// <param name="typeReturn">Tipo de retorno do elemento.</param>
		/// <returns>Objeto com os dados dados preenchidos.</returns>
		public static object Deserialize(byte[] buffer, Type typeReturn)
		{
			using (Stream stream = new MemoryStream(buffer, 0, buffer.Length))
			{
				return Deserialize(stream, typeReturn);
			}
		}

		/// <summary>
		/// Deseriliza os dados contidos no buffer e retorna o objeto do tipo passado com os dados preenchidos.
		/// </summary>
		/// <param name="buffer">Buffer onde estão os dados para serem deserializados.</param>
		/// <param name="typeReturn">Tipo de retorno do elemento.</param>
		/// <returns>Objeto com os dados dados preenchidos.</returns>
		public static object Deserialize(byte[] buffer, Type typeReturn, object destination)
		{
			using (Stream stream = new MemoryStream(buffer, 0, buffer.Length))
			{
				return DeserializeBase(stream, typeReturn, null, 0, 0, destination);
			}
		}

		/// <summary>
		/// Copia os dados de uma instância para a outra sem
		/// nenhum vinculo de ponteiro.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado para cópia.</typeparam>
		/// <param name="source">Objeto contendo a fonte dos dados.</param>
		/// <returns>Objeto para onde dados foram copiados.</returns>
		public static T CopyInstance<T>(T source) where T : new()
		{
			T destination = new T();
			CopyInstance<T>(source, destination);
			return destination;
		}

		/// <summary>
		/// Copia os dados de uma instância para a outra sem
		/// nenhum vinculo de ponteiro.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado para cópia.</typeparam>
		/// <param name="source">Objeto contendo a fonte dos dados.</param>
		/// <param name="destination">Objeto para onde será copiado os dados.</param>
		public static void CopyInstance<T>(T source, T destination) where T : new()
		{
			if(source == null)
				throw new ArgumentException("source");
			using (MemoryStream ms = new MemoryStream())
			{
				Serialize(ms, source);
				ms.Seek(0, SeekOrigin.Begin);
				DeserializeBase(ms, typeof(T), null, 0, 0, destination);
			}
		}

		/// <summary>
		/// Converte um array de bytes para decimal.
		/// </summary>
		/// <param name="bytes">O array de bytes que será convertido.</param>
		/// <returns></returns>
		private static decimal BytesToDecimal(byte[] bytes)
		{
			int[] bits = new int[4];
			bits[0] = ((bytes[0] | (bytes[1] << 8)) | (bytes[2] << 0x10)) | (bytes[3] << 0x18);
			bits[1] = ((bytes[4] | (bytes[5] << 8)) | (bytes[6] << 0x10)) | (bytes[7] << 0x18);
			bits[2] = ((bytes[8] | (bytes[9] << 8)) | (bytes[10] << 0x10)) | (bytes[11] << 0x18);
			bits[3] = ((bytes[12] | (bytes[13] << 8)) | (bytes[14] << 0x10)) | (bytes[15] << 0x18);
			return new decimal(bits);
		}

		/// <summary>
		/// Converte um decimal para um array de bytes.
		/// </summary>
		/// <param name="d">O decimal que será convertido.</param>
		/// <returns></returns>
		private static byte[] DecimalToBytes(decimal d)
		{
			byte[] bytes = new byte[16];
			int[] bits = decimal.GetBits(d);
			int lo = bits[0];
			int mid = bits[1];
			int hi = bits[2];
			int flags = bits[3];
			bytes[0] = (byte)lo;
			bytes[1] = (byte)(lo >> 8);
			bytes[2] = (byte)(lo >> 0x10);
			bytes[3] = (byte)(lo >> 0x18);
			bytes[4] = (byte)mid;
			bytes[5] = (byte)(mid >> 8);
			bytes[6] = (byte)(mid >> 0x10);
			bytes[7] = (byte)(mid >> 0x18);
			bytes[8] = (byte)hi;
			bytes[9] = (byte)(hi >> 8);
			bytes[10] = (byte)(hi >> 0x10);
			bytes[11] = (byte)(hi >> 0x18);
			bytes[12] = (byte)flags;
			bytes[13] = (byte)(flags >> 8);
			bytes[14] = (byte)(flags >> 0x10);
			bytes[15] = (byte)(flags >> 0x18);
			return bytes;
		}
	}
}
