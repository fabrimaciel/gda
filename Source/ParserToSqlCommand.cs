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
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;

namespace GDA.Sql
{
	class ParserToSqlCommand
	{
		private StringBuilder sqlCommand = new StringBuilder();

		private string _quoteExpressionBegin;

		private string _quoteExpressionEnd;

		public string SqlCommand
		{
			get
			{
				return sqlCommand.ToString().TrimEnd(' ');
			}
		}

		/// <summary>
		/// Quote inicial para escrever a expressão.
		/// </summary>
		public string QuoteExpressionBegin
		{
			get
			{
				return _quoteExpressionBegin;
			}
			set
			{
				_quoteExpressionBegin = value;
			}
		}

		/// <summary>
		/// Quote final para escrever a expressão.
		/// </summary>
		public string QuoteExpressionEnd
		{
			get
			{
				return _quoteExpressionEnd;
			}
			set
			{
				_quoteExpressionEnd = value;
			}
		}

		/// <summary>
		/// Cria a instancia o com Parser que será usado na execução.
		/// </summary>
		/// <param name="parser"></param>
		internal ParserToSqlCommand(Parser parser) : this(parser, "", "")
		{
		}

		internal ParserToSqlCommand(Parser parser, string quoteExpressionBegin, string quoteExpressionEnd)
		{
			_quoteExpressionBegin = quoteExpressionBegin;
			_quoteExpressionEnd = quoteExpressionEnd;
			foreach (Select select in parser.SelectParts)
			{
				GetSelectInfo(select);
			}
		}

		internal ParserToSqlCommand(WherePart wp) : this(wp, "", "")
		{
		}

		/// <summary>
		/// Cria o parser da clausula WHERE.
		/// </summary>
		/// <param name="wp"></param>
		/// <param name="quoteExpressionBegin"></param>
		/// <param name="quoteExpressionEnd"></param>
		internal ParserToSqlCommand(WherePart wp, string quoteExpressionBegin, string quoteExpressionEnd)
		{
			_quoteExpressionBegin = quoteExpressionBegin;
			_quoteExpressionEnd = quoteExpressionEnd;
			foreach (SqlExpression exp in wp.Expressions)
				ColumnName(exp);
		}

		internal ParserToSqlCommand(OrderByPart op) : this(op, "", "")
		{
		}

		internal ParserToSqlCommand(OrderByPart op, string quoteExpressionBegin, string quoteExpressionEnd)
		{
			_quoteExpressionBegin = quoteExpressionBegin;
			_quoteExpressionEnd = quoteExpressionEnd;
			for(int i = 0; i < op.OrderByExpressions.Count; i++)
			{
				ColumnName(op.OrderByExpressions[i].Expression);
				if(op.OrderByExpressions[i].Asc)
					sqlCommand.Append(" ASC");
				else
					sqlCommand.Append(" DESC");
				if(i + 1 != op.OrderByExpressions.Count)
					sqlCommand.Append(", ");
			}
		}

		internal ParserToSqlCommand(GroupByPart gbp, string quoteExpressionBegin, string quoteExpressionEnd)
		{
			_quoteExpressionBegin = quoteExpressionBegin;
			_quoteExpressionEnd = quoteExpressionEnd;
			for(int i = 0; i < gbp.Expressions.Count; i++)
			{
				ColumnName(gbp.Expressions[i]);
				if(i + 1 != gbp.Expressions.Count)
					sqlCommand.Append(", ");
			}
		}

		/// <summary>
		/// Cria o parser para o clausula HAVING.
		/// </summary>
		/// <param name="havingPart"></param>
		/// <param name="quoteExpressionBegin"></param>
		/// <param name="quoteExpressionEnd"></param>
		internal ParserToSqlCommand(HavingPart havingPart, string quoteExpressionBegin, string quoteExpressionEnd)
		{
			_quoteExpressionBegin = quoteExpressionBegin;
			_quoteExpressionEnd = quoteExpressionEnd;
			foreach (SqlExpression exp in havingPart.Expressions)
				ColumnName(exp);
		}

		/// <summary>
		/// Aplica a marcação de citação na expressão.
		/// </summary>
		/// <param name="expression"></param>
		private string ApplyQuoteExpression(string expression)
		{
			if(expression != null)
			{
				var trimWord = expression.TrimEnd(' ').TrimStart(' ');
				if(trimWord.Length > 3 && trimWord[0] == '[' && trimWord[trimWord.Length - 1] == ']')
					return expression;
			}
			return this.QuoteExpressionBegin + expression + this.QuoteExpressionEnd;
		}

		/// <summary>
		/// Recupera as informações de Select
		/// </summary>
		/// <param name="select"></param>
		private void GetSelectInfo(Select select)
		{
			sqlCommand.Append(select.Value.Text.ToUpper()).Append(" ");
			if(select.SelectPart.ResultType == GDA.Sql.InterpreterExpression.Enums.SelectClauseResultTypes.Distinct)
				sqlCommand.Append("DISTINCT ");
			foreach (SelectExpression se in select.SelectPart.SelectionExpressions)
			{
				ColumnName(se.ColumnName);
				if(se.ColumnAlias != null && se.AsExpression != null)
					sqlCommand.Append(" AS ").Append(se.ColumnAlias.Value.Text);
				else if(se.ColumnAlias != null)
					sqlCommand.Append(" ").Append(se.ColumnAlias.Value.Text);
				sqlCommand.Append(", ");
			}
			if(select.SelectPart.SelectionExpressions.Count > 0)
				sqlCommand.Remove(sqlCommand.Length - 2, 1);
			if(select.FromPart != null)
			{
				sqlCommand.Append("FROM ");
				foreach (TableExpression te in select.FromPart.TableExpressions)
				{
					if(te.LeftOrRight != null)
						sqlCommand.Append(te.LeftOrRight.Text).Append(" ");
					if(te.OuterOrInnerOrCrossOrNatural != null)
						sqlCommand.Append(te.OuterOrInnerOrCrossOrNatural.Text.ToUpper()).Append(" ");
					if(te.Join != null)
						sqlCommand.Append("JOIN ");
					if(te.SelectInfo != null)
					{
						sqlCommand.Append("(");
						GetSelectInfo(te.SelectInfo);
						sqlCommand.Append(") ");
					}
					else
					{
						TableName(te);
					}
					if(te.TableAlias != null)
					{
						sqlCommand.Append((te.AsExpression != null ? "AS " : " ")).Append(te.TableAlias).Append(" ");
					}
					if(te.OnExpressions != null)
					{
						sqlCommand.Append("ON(");
						foreach (SqlExpression se in te.OnExpressions.Expressions)
							ColumnName(se);
						sqlCommand.Append(")");
					}
					sqlCommand.Append(" ");
				}
			}
			if(select.WherePart != null)
			{
				sqlCommand.Append("WHERE ");
				foreach (SqlExpression se in select.WherePart.Expressions)
					ColumnName(se);
				sqlCommand.Append(" ");
			}
			if(select.GroupByPart != null)
			{
				sqlCommand.Append("GROUP BY ");
				foreach (SqlExpression se in select.GroupByPart.Expressions)
				{
					ColumnName(se);
					sqlCommand.Append(", ");
				}
				if(select.GroupByPart.Expressions.Count > 0)
					sqlCommand.Remove(sqlCommand.Length - 2, 1);
				sqlCommand.Append(" ");
			}
			if(select.HavingPart != null)
			{
				sqlCommand.Append("HAVING ");
				foreach (SqlExpression se in select.HavingPart.Expressions)
					ColumnName(se);
				sqlCommand.Append(" ");
			}
			if(select.OrderByPart != null)
			{
				sqlCommand.Append("ORDER BY ");
				int currentPos = 0;
				foreach (OrderByExpression oe in select.OrderByPart.OrderByExpressions)
				{
					if(currentPos > 0)
						sqlCommand.Append(", ");
					ColumnName(oe.Expression);
					if(!oe.Asc)
						sqlCommand.Append(" DESC");
					currentPos++;
				}
				sqlCommand.Append(" ");
			}
		}

		private void ColumnName(SqlExpression se)
		{
			if(se is ContainerSqlExpression)
			{
				if(((ContainerSqlExpression)se).ContainerToken == GDA.Sql.InterpreterExpression.Enums.TokenID.LParen)
					sqlCommand.Append("(");
				foreach (SqlExpression se1 in ((ContainerSqlExpression)se).Expressions)
				{
					ColumnName(se1);
				}
				if(((ContainerSqlExpression)se).ContainerToken == GDA.Sql.InterpreterExpression.Enums.TokenID.LParen)
					sqlCommand.Append(")");
			}
			else if(se is Select)
			{
				GetSelectInfo((Select)se);
			}
			else if(se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column)
			{
				if(se.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.Star)
					sqlCommand.Append(se.Value.Text);
				else
				{
					int posEndInfo = se.Value.Text.LastIndexOf('.');
					if(posEndInfo >= 0)
					{
						var text1 = RemoveDefaultQuote(se.Value.Text.Substring(0, posEndInfo));
						if(text1.Length > 2 && !text1.StartsWith(QuoteExpressionBegin) && !text1.EndsWith(QuoteExpressionEnd))
							sqlCommand.Append(QuoteExpressionBegin).Append(text1).Append(QuoteExpressionEnd).Append(".");
						else
							sqlCommand.Append(text1).Append(".");
						if(se.Value.Text.Substring(posEndInfo + 1) == "*")
							sqlCommand.Append(se.Value.Text.Substring(posEndInfo + 1));
						else
						{
							var text = RemoveDefaultQuote(se.Value.Text.Substring(posEndInfo + 1));
							if(text.Length > 2 && !text.StartsWith(QuoteExpressionBegin) && !text.EndsWith(QuoteExpressionEnd))
								sqlCommand.Append(QuoteExpressionBegin).Append(text).Append(QuoteExpressionEnd);
							else
								sqlCommand.Append(text);
						}
					}
					else
					{
						var text1 = RemoveDefaultQuote(se.Value.Text);
						if(text1.Length > 2 && !text1.StartsWith(QuoteExpressionBegin) && !text1.EndsWith(QuoteExpressionEnd))
							sqlCommand.Append(QuoteExpressionBegin).Append(text1).Append(QuoteExpressionEnd);
						else
							sqlCommand.Append(text1);
					}
				}
			}
			else if(se is SqlFunction)
			{
				sqlCommand.Append(se.Value.Text).Append("(");
				foreach (List<SqlExpression> parameter in ((SqlFunction)se).Parameters)
				{
					foreach (SqlExpression pSe in parameter)
					{
						ColumnName(pSe);
					}
					sqlCommand.Append(", ");
				}
				if(((SqlFunction)se).Parameters.Count > 0)
					sqlCommand.Remove(sqlCommand.Length - 2, 2);
				sqlCommand.Append(")");
			}
			else if(se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.ComparerScalar || se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Boolean || se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Operation || se.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.kNot)
			{
				sqlCommand.Append(" ").Append(se.Value.Text.ToUpper()).Append(" ");
			}
			else if(se.Value is SpecialContainerExpression)
			{
				SpecialContainerExpression sce = (SpecialContainerExpression)se.Value;
				sqlCommand.Append(sce.ContainerChar);
				sqlCommand.Append(se.Value.Text);
				sqlCommand.Append(sce.ContainerChar);
			}
			else if(se.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.kIsNull || se.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.kIs)
				sqlCommand.Append(" ").Append(se.Value.Text);
			else if(se.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Boolean)
			{
				switch(se.Value.Token)
				{
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kAnd:
					if(se.Value.Text == "&&")
						se.Value.Text = "AND";
					break;
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kOr:
					if(se.Value.Text == "||")
						se.Value.Text = "OR";
					break;
				}
			}
			else
				switch(se.Value.Token)
				{
				case GDA.Sql.InterpreterExpression.Enums.TokenID.EqualEqual:
					sqlCommand.Append("=");
					break;
				default:
					sqlCommand.Append(se.Value.Text);
					break;
				}
		}

		/// <summary>
		/// Remove o quote padrao.
		/// </summary>
		/// <param name="text1"></param>
		/// <returns></returns>
		private static string RemoveDefaultQuote(string text1)
		{
			if(text1 != null)
			{
				var trimWord = text1.TrimEnd(' ').TrimStart(' ');
				if(trimWord.Length > 3 && trimWord[0] == '[' && trimWord[trimWord.Length - 1] == ']')
					text1 = text1.Substring(1, text1.Length - 2);
			}
			return text1;
		}

		private void TableName(TableExpression te)
		{
			var name = te.TableName.InnerExpression.Text.TrimEnd(' ').TrimStart(' ');
			var schema = (te.TableName.Schema ?? "").TrimEnd(' ').TrimStart(' ');
			if(te.TableName.InnerExpression.CurrentSpecialContainer != null && te.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer == QuoteExpressionBegin[0] && te.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer == QuoteExpressionEnd[0])
			{
				if(schema.Length > 0)
				{
					if(!(schema.Length > 3 && schema[0] == te.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer && schema[schema.Length - 1] == te.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer))
						sqlCommand.Append(te.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer).Append(te.TableName.Schema).Append(te.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer).Append('.');
					else
						sqlCommand.Append(te.TableName.Schema).Append('.');
				}
				if(!(name.Length > 3 && name[0] == te.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer && name[name.Length - 1] == te.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer))
					sqlCommand.Append(te.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer).Append(te.TableName.Name).Append(te.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer).Append(" ");
			}
			else
			{
				if(schema.Length > 0)
				{
					if(schema.Length > 2 && !schema.StartsWith(QuoteExpressionBegin) && !schema.EndsWith(QuoteExpressionEnd))
						sqlCommand.Append(QuoteExpressionBegin).Append(te.TableName.Schema).Append(QuoteExpressionEnd).Append('.');
					else
						sqlCommand.Append(te.TableName.Schema).Append('.');
				}
				if(name.Length > 2 && !name.StartsWith(QuoteExpressionBegin) && !name.EndsWith(QuoteExpressionEnd))
					sqlCommand.Append(QuoteExpressionBegin).Append(te.TableName.Name).Append(QuoteExpressionEnd).Append(" ");
				else
					sqlCommand.Append(te.TableName.Name).Append(" ");
			}
		}

		public override string ToString()
		{
			return sqlCommand.ToString();
		}
	}
}
