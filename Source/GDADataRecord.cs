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
using System.Data;

#if CLS_3_5
using System.Linq.Expressions;
#endif
namespace GDA
{
	/// <summary>
	/// Representa um registro de dados do GDA.
	/// </summary>
	public class GDADataRecord : IDataRecord
	{
		/// <summary>
		/// Instancia base o item que está sendo trabalhado.
		/// </summary>
		internal protected IDataRecord _baseRecord;

		private IDictionary<string, int> _translator;

		/// <summary>
		/// Instancia do registro de dados base.
		/// </summary>
		public IDataRecord BaseDataRecord
		{
			get
			{
				return _baseRecord;
			}
		}

		/// <summary>
		/// Instancia do tradutor de acesso ao valor do campo.
		/// Armazena o nome do campo e a posição do campo no registro.
		/// </summary>
		protected internal IDictionary<string, int> Translator
		{
			get
			{
				return _translator;
			}
		}

		/// <summary>
		/// Cria uma instancia com dados já existentes.
		/// </summary>
		/// <param name="baseRecord"></param>
		/// <param name="translator">
		/// Instancia do tradutor de acesso ao valor do campo.
		/// Armazena o nome do campo e a posição do campo no registro.
		/// </param>
		public GDADataRecord(IDataRecord baseRecord, IDictionary<string, int> translator)
		{
			_baseRecord = baseRecord;
			_translator = translator;
		}

		/// <summary>
		/// Recupera a posição do campo com base no nome.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected int GetFieldPosition(string name)
		{
			var index = -1;
			if(Translator != null)
			{
				if(!Translator.TryGetValue(name, out index))
					index = -1;
			}
			else
				return _baseRecord.GetOrdinal(name);
			if(index < 0)
			{
				try
				{
					index = _baseRecord.GetOrdinal(name);
				}
				catch
				{
				}
				if(index < 0)
					throw new GDAException("Field {0} not found in result", name);
			}
			return index;
		}

		/// <summary>
		/// Recupera a posição roteada do campo.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected int GetFieldRoutePosition(int index)
		{
			if(Translator != null)
			{
				if(index >= 0)
				{
					int i = 0;
					foreach (int v in Translator.Values)
					{
						if(i == index)
							return v;
						i++;
					}
				}
				throw new IndexOutOfRangeException("The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.");
			}
			return index;
		}

		/// <summary>
		/// Quantidade de campos no resultado.
		/// </summary>
		public int FieldCount
		{
			get
			{
				if(Translator != null)
					return Translator.Count;
				else
					return _baseRecord.FieldCount;
			}
		}

		public bool GetBoolean(int i)
		{
			return _baseRecord.GetBoolean(GetFieldRoutePosition(i));
		}

		public bool GetBoolean(string name)
		{
			return _baseRecord.GetBoolean(GetFieldPosition(name));
		}

		public byte GetByte(int i)
		{
			return _baseRecord.GetByte(GetFieldRoutePosition(i));
		}

		public byte GetByte(string name)
		{
			return _baseRecord.GetByte(GetFieldPosition(name));
		}

		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return _baseRecord.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		public long GetBytes(string name, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return _baseRecord.GetBytes(GetFieldPosition(name), fieldOffset, buffer, bufferoffset, length);
		}

		public char GetChar(int i)
		{
			return _baseRecord.GetChar(GetFieldRoutePosition(i));
		}

		public char GetChar(string name)
		{
			return _baseRecord.GetChar(GetFieldPosition(name));
		}

		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return _baseRecord.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		public long GetChars(string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return _baseRecord.GetChars(GetFieldPosition(name), fieldoffset, buffer, bufferoffset, length);
		}

		public IDataReader GetData(int i)
		{
			return _baseRecord.GetData(GetFieldRoutePosition(i));
		}

		public IDataReader GetData(string name)
		{
			return _baseRecord.GetData(GetFieldPosition(name));
		}

		public string GetDataTypeName(int i)
		{
			return _baseRecord.GetDataTypeName(GetFieldRoutePosition(i));
		}

		public string GetDataTypeName(string name)
		{
			return _baseRecord.GetDataTypeName(GetFieldPosition(name));
		}

		public DateTime GetDateTime(int i)
		{
			return _baseRecord.GetDateTime(GetFieldRoutePosition(i));
		}

		public DateTime GetDateTime(string name)
		{
			return _baseRecord.GetDateTime(GetFieldPosition(name));
		}

		public decimal GetDecimal(int i)
		{
			return _baseRecord.GetDecimal(GetFieldRoutePosition(i));
		}

		public decimal GetDecimal(string name)
		{
			return _baseRecord.GetDecimal(GetFieldPosition(name));
		}

		public double GetDouble(int i)
		{
			return _baseRecord.GetDouble(GetFieldRoutePosition(i));
		}

		public double GetDouble(string name)
		{
			return _baseRecord.GetDouble(GetFieldPosition(name));
		}

		public Type GetFieldType(int i)
		{
			return _baseRecord.GetFieldType(GetFieldRoutePosition(i));
		}

		public Type GetFieldType(string name)
		{
			return _baseRecord.GetFieldType(GetFieldPosition(name));
		}

		public float GetFloat(int i)
		{
			return _baseRecord.GetFloat(GetFieldRoutePosition(i));
		}

		public float GetFloat(string name)
		{
			return _baseRecord.GetFloat(GetFieldPosition(name));
		}

		public Guid GetGuid(int i)
		{
			return _baseRecord.GetGuid(GetFieldRoutePosition(i));
		}

		public Guid GetGuid(string name)
		{
			return _baseRecord.GetGuid(GetFieldPosition(name));
		}

		public short GetInt16(int i)
		{
			return _baseRecord.GetInt16(GetFieldRoutePosition(i));
		}

		public short GetInt16(string name)
		{
			return _baseRecord.GetInt16(GetFieldPosition(name));
		}

		public ushort GetUInt16(int i)
		{
			return unchecked((ushort)GetInt16(i));
		}

		public ushort GetUInt16(string name)
		{
			return unchecked((ushort)GetInt16(name));
		}

		public int GetInt32(int i)
		{
			return _baseRecord.GetInt32(GetFieldRoutePosition(i));
		}

		public int GetInt32(string name)
		{
			return _baseRecord.GetInt32(GetFieldPosition(name));
		}

		public uint GetUInt32(int i)
		{
			return unchecked((uint)GetInt32(i));
		}

		public uint GetUInt32(string name)
		{
			return unchecked((uint)GetInt32(name));
		}

		public long GetInt64(int i)
		{
			return _baseRecord.GetInt64(GetFieldRoutePosition(i));
		}

		public long GetInt64(string name)
		{
			return _baseRecord.GetInt64(GetFieldPosition(name));
		}

		public ulong GetUInt64(int i)
		{
			return unchecked((ulong)GetInt64(i));
		}

		public ulong GetUInt64(string name)
		{
			return unchecked((ulong)GetInt64(name));
		}

		public string GetName(int i)
		{
			return _baseRecord.GetName(GetFieldRoutePosition(i));
		}

		public int GetOrdinal(string name)
		{
			return _baseRecord.GetOrdinal(name);
		}

		public string GetString(int i)
		{
			var pos = GetFieldRoutePosition(i);
			if(!_baseRecord.IsDBNull(pos))
				return _baseRecord.GetString(pos);
			return null;
		}

		public string GetString(string name)
		{
			var pos = GetFieldPosition(name);
			if(!_baseRecord.IsDBNull(pos))
				return _baseRecord.GetString(pos);
			return null;
		}

		public object GetValue(int i)
		{
			return _baseRecord.GetValue(GetFieldRoutePosition(i));
		}

		public object GetValue(string name)
		{
			return _baseRecord.GetValue(GetFieldPosition(name));
		}

		public int GetValues(object[] values)
		{
			int result = _baseRecord.GetValues(values);
			for(int i = 0; i < result; i++)
				if(values[i] is DBNull)
					values[i] = null;
			return result;
		}

		public bool IsDBNull(int i)
		{
			return _baseRecord.IsDBNull(GetFieldRoutePosition(i));
		}

		public bool IsDBNull(string name)
		{
			return _baseRecord.IsDBNull(GetFieldPosition(name));
		}

		public GDAPropertyValue this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				try
				{
					var obj = _baseRecord[GetFieldPosition(name)];
					return new GDAPropertyValue((obj is DBNull ? null : obj), !(obj is DBNull));
				}
				catch(IndexOutOfRangeException ex)
				{
					throw new GDAException(string.Format("No column with the name \"{0}\" was found.", name), ex);
				}
			}
		}

		public GDAPropertyValue this[int i]
		{
			get
			{
				var obj = _baseRecord[GetFieldRoutePosition(i)];
				return new GDAPropertyValue((obj is DBNull ? null : obj), !(obj is DBNull));
			}
		}

		object IDataRecord.this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				try
				{
					var obj = _baseRecord[GetFieldPosition(name)];
					return (obj is DBNull ? null : obj);
				}
				catch(IndexOutOfRangeException ex)
				{
					throw new GDAException(string.Format("No column with the name \"{0}\" was found.", name), ex);
				}
			}
		}

		object IDataRecord.this[int i]
		{
			get
			{
				var obj = _baseRecord[GetFieldRoutePosition(i)];
				return (obj is DBNull ? null : obj);
			}
		}
	}
	public class GDADataRecord<Model> : GDADataRecord
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="baseRecord"></param>
		/// <param name="translator">
		/// Instancia do tradutor de acesso ao valor do campo.
		/// Armazena o nome do campo e a posição do campo no registro.
		/// </param>
		internal GDADataRecord(IDataRecord baseRecord, TranslatorDataInfoCollection translator) : base(baseRecord, translator)
		{
		}

		#if CLS_3_5
		        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <typeparam name="T">Tipo da propriedade.</typeparam>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public T GetValue<T>(Expression<Func<Model, T>> propertySelector)
        {
            var property = propertySelector.GetMember();

            if (property == null)
                throw new ArgumentException("Not found property name.");

            return (T)this[property.Name].GetValue();
        }
#endif
		/// <summary>
		/// Recupera a instancia do objeto relacionado.
		/// </summary>
		/// <returns></returns>
		public virtual Model GetInstance()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Preenche a instancia com os dados do registro tem com base os nomes das propriedaes.
		/// </summary>
		/// <param name="instance">Instancia aonde os dados serão preenchidos.</param>
		public virtual T Fill<T>(T instance)
		{
			object obj = instance;
			DataAccess.RecoverValueOfResult(ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref obj, instance is Interfaces.IObjectDataRecord);
			return instance;
		}

		/// <summary>
		/// Cria uma instancia do tipo informado e preenche os dados com base nos dados do registro.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual T Fill<T>() where T : new()
		{
			T instance = new T();
			object obj = instance;
			DataAccess.RecoverValueOfResult(ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref obj, instance is Interfaces.IObjectDataRecord);
			return instance;
		}
	#if CLS_3_5
	
        /// <summary>
        /// Recupera o valor da propriedade.
        /// </summary>
        /// <param name="propertySelector"></param>
        /// <returns></returns>
        public GDAPropertyValue this[Expression<Func<Model, object>> propertySelector]
        {
            get
            {
                var property = propertySelector.GetMember();

                if (property == null)
                    throw new ArgumentException("Not found property name.");

                return this[property.Name];
            }
        }
#endif
	}
	public class GDADataRecordEx<Model> : GDADataRecord<Model> where Model : new()
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="baseRecord"></param>
		/// <param name="translator">
		/// Instancia do tradutor de acesso ao valor do campo.
		/// Armazena o nome do campo e a posição do campo no registro.
		/// </param>
		internal GDADataRecordEx(IDataRecord baseRecord, TranslatorDataInfoCollection translator) : base(baseRecord, translator)
		{
		}

		public override Model GetInstance()
		{
			var item = new Model();
			PersistenceObjectBase<Model>.RecoverValueOfResult(ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref item, true);
			return item;
		}

		/// <summary>
		/// Preenche a instancia com os dados do registro tem com base os nomes das propriedaes.
		/// </summary>
		/// <param name="instance">Instancia aonde os dados serão preenchidos.</param>
		public override T Fill<T>(T instance)
		{
			object obj = instance;
			DataAccess.RecoverValueOfResult(ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref obj, instance is Interfaces.IObjectDataRecord);
			return instance;
		}

		/// <summary>
		/// Cria uma instancia do tipo informado e preenche os dados com base nos dados do registro.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public override T Fill<T>()
		{
			T instance = new T();
			object obj = instance;
			DataAccess.RecoverValueOfResult(ref _baseRecord, (TranslatorDataInfoCollection)Translator, ref obj, instance is Interfaces.IObjectDataRecord);
			return instance;
		}
	}
}
