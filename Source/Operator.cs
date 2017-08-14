using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql
{
	public enum Operator
	{
		Equals,
		NotEquals,
		Like,
		NotLike,
		LessThan,
		LessThanOrEquals,
		GreaterThan,
		GreaterThanOrEquals,
		In,
		NotIn
	}
	public enum LogicalOperator
	{
		And,
		Or
	}
	public enum Order
	{
		Ascending,
		Descending
	}
}
