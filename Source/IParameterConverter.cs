using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GDA.Interfaces
{
	public interface IParameterConverter
	{
		System.Data.IDbDataParameter Convert (GDAParameter a);
	}
	public interface IParameterConverter2
	{
		System.Data.IDbDataParameter Converter (System.Data.IDbCommand a, GDAParameter b);
	}
}
