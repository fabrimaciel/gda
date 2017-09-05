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
using System.Reflection;

namespace GDA
{
	/// <summary>
	/// Representa o error ocorrido no GDA.
	/// </summary>
	[Serializable]
	#if !PocketPC
	public class GDAException : Exception, System.Runtime.Serialization.ISerializable
	#else
	    public class GDAException : Exception
#endif
	{
		/// <summary>
		/// Construtor padr�o.
		/// </summary>
		/// <param name="message">Mensagem do error.</param>
		public GDAException(string message) : base(message)
		{
		}

		public GDAException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		public GDAException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public GDAException(string message, params object[] args) : this(String.Format(message, args))
		{
		}

		#if !PocketPC
		protected GDAException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	#endif
	}
	[Serializable]
	public class GDAColumnNotFoundException : Exception
	{
		public GDAColumnNotFoundException(string columnName, string message) : base("Column " + columnName + " not found in result. " + message)
		{
		}
	}
	[Serializable]
	public class GDAReferenceDAONotFoundException : GDAException
	{
		public GDAReferenceDAONotFoundException(string message) : base(message)
		{
		}
	}
	[Serializable]
	public class ItemNotFoundException : GDAException
	{
		public ItemNotFoundException(string message) : base(message)
		{
		}
	}
}
