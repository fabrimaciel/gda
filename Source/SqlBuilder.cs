using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
using System.Reflection;
namespace GDA.Sql
{
	public static class SqlBuilder
	{
		internal static string SimbolOperator (Operator a)
		{
			switch (a) {
			case Operator.Equals:
				return "=";
			case Operator.GreaterThan:
				return ">";
			case Operator.GreaterThanOrEquals:
				return ">=";
			case Operator.In:
				return " IN ";
			case Operator.LessThan:
				return "<";
			case Operator.LessThanOrEquals:
				return "<=";
			case Operator.Like:
				return " LIKE ";
			case Operator.NotEquals:
				return "<>";
			case Operator.NotIn:
				return " NOT IN ";
			case Operator.NotLike:
				return " NOT LIKE ";
			default:
				return "";
			}
		}
		public static void ImportNamespace (Assembly a, string b)
		{
			GDASettings.AddModelsNamespace (a, b);
		}
		public static SelectStatement P (ISelectStatementReferences a, string b)
		{
			if (a == null)
				throw new ArgumentNullException ("references");
			Lexer c = new Lexer (b);
			Parser d = new Parser (c);
			d.Execute ();
			return new SelectStatement (a, d);
		}
		public static SelectStatement P (string a)
		{
			Lexer b = new Lexer (a);
			Parser c = new Parser (b);
			c.Execute ();
			return new SelectStatement (c);
		}
	}
}
