using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Interfaces
{
	public interface IObjectDataRecord
	{
		void InsertRecordField (string a, object b);
		bool LoadMappedsRecordFields {
			get;
		}
	}
}
