using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public interface IQuery
	{
		Type ReturnTypeQuery {
			get;
			set;
		}
		int SkipCount {
			get;
		}
		int TakeCount {
			get;
		}
		string AggregationFunctionProperty {
			get;
		}
		QueryReturnInfo BuildResultInfo<T> (GDA.Interfaces.IProviderConfiguration a);
		QueryReturnInfo BuildResultInfo (string a);
		QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b);
		QueryReturnInfo BuildResultInfo2 (GDA.Interfaces.IProvider a, string b, Dictionary<string, Type> c);
	}
}
