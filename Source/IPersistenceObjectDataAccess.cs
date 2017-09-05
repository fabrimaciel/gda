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

namespace GDA.Interfaces
{
	public interface IPersistenceObjectDataAccess<T>
	{
		int Delete(T objDelete);

		uint Insert(T objInsert);

		uint InsertForced(T objInsert);

		uint InsertOrUpdate(T objData);

		System.Collections.Generic.List<Mapper> Keys
		{
			get;
		}

		string TableName
		{
			get;
		}

		GDA.Sql.TableName TableNameInfo
		{
			get;
		}

		string SystemTableName
		{
			get;
		}

		int Update(T objUpdate);
	}
}
