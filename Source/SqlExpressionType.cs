using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Enums
{
	enum SqlExpressionType : short
	{
		Operation,
		StringLiteral,
		NumericLiteral,
		RealLiteral,
		Column,
		Variable,
		Function,
		Comparation,
		Container,
		Table,
		Select,
		Boolean,
		ComparerScalar,
		Constant
	}
}
