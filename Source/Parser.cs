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
using GDA.Sql.InterpreterExpression.Enums;
using GDA.Sql.InterpreterExpression.Nodes;

namespace GDA.Sql.InterpreterExpression
{
	class Parser
	{
		/// <summary>
		/// Examinador lexer relacionado.
		/// </summary>
		private Lexer _lex;

		private List<Expression> expressions;

		private int posCurrentExpression = 0;

		private List<Select> _selectParts = new List<Select>();

		public List<Select> SelectParts
		{
			get
			{
				return _selectParts;
			}
			set
			{
				_selectParts = value;
			}
		}

		private Expression CurrentExpression
		{
			get
			{
				if(posCurrentExpression < expressions.Count)
					return expressions[posCurrentExpression];
				else
				{
					Expression expr = new Expression();
					expr.Token = TokenID.End;
					return expr;
				}
			}
		}

		/// <summary>
		/// Examinador lexer relacionado.
		/// </summary>
		public Lexer Lex
		{
			get
			{
				return _lex;
			}
		}

		private void Error(string message)
		{
			int step = 13;
			if(posCurrentExpression <= step)
				step = posCurrentExpression;
			StringBuilder express = new StringBuilder();
			if(step > 0)
			{
				express.Append("\"");
				if(step == 13)
					express.Append("...");
				Expression e = GetPreviousExpression(step);
				int pos = e.BeginPoint + e.Length;
				express.Append(e.Text);
				for(int i = (step - 1); i > 0; i--)
				{
					e = GetPreviousExpression(i);
					express.Append(' ', (e.BeginPoint - pos));
					if(e.CurrentSpecialContainer != null)
						express.Append(e.CurrentSpecialContainer.BeginCharSpecialContainer).Append(e.Text).Append(e.CurrentSpecialContainer.EndCharSpecialContainer);
					else if(e is SpecialContainerExpression)
						express.Append(((SpecialContainerExpression)e).ContainerChar).Append(e.Text).Append(((SpecialContainerExpression)e).ContainerChar);
					else
						express.Append(e.Text);
					pos = e.BeginPoint + e.Length;
				}
				express.Append("...\"");
			}
			throw new SqlParserException(message + " Command: " + express.ToString());
		}

		private Expression GetPreviousExpression(int step)
		{
			return expressions[posCurrentExpression - step];
		}

		private Expression GetNextExpression(int step)
		{
			if(posCurrentExpression + step < expressions.Count)
			{
				return expressions[posCurrentExpression + step];
			}
			else
				return new Expression(TokenID.InvalidExpression);
		}

		/// <summary>
		/// Move para a proxima expressao.
		/// </summary>
		/// <param name="expected">Identifica se a proxima expressao e esperada.</param>
		/// <returns></returns>
		private bool NextExpression(bool expected)
		{
			if(posCurrentExpression < expressions.Count)
			{
				posCurrentExpression++;
				return true;
			}
			else if(expected)
			{
				Error("Expected a expression.");
				return false;
			}
			else
				return false;
		}

		public Parser(Lexer lex)
		{
			this._lex = lex;
			this.expressions = lex.Expressions;
		}

		/// <summary>
		/// Executa o parser.
		/// </summary>
		public void Execute()
		{
			while (true)
			{
				switch(CurrentExpression.Token)
				{
				case TokenID.kSelect:
				case TokenID.kFrom:
					_selectParts.Add(ParserSelect());
					break;
				default:
					break;
				}
				if(CurrentExpression.Token == TokenID.Semi)
				{
					NextExpression(false);
					continue;
				}
				break;
			}
			if(CurrentExpression.Token != TokenID.Semi && CurrentExpression.Token != TokenID.End)
				Error("Not found end command.");
		}

		/// <summary>
		/// Executa o parser somente encima da parte da clausula SELECT.
		/// </summary>
		/// <returns></returns>
		public SelectPart ExecuteSelectPart()
		{
			return ParserSelectPart();
		}

		/// <summary>
		/// Executa um parser somente em cima da parte da clausula WHERE.
		/// </summary>
		/// <returns></returns>
		public WherePart ExecuteWherePart()
		{
			WherePart wherePart = new WherePart();
			List<SqlExpression> expressions = new List<SqlExpression>();
			int i = 0;
			do
			{
				ContainerSqlExpression ce = ParserContainerConditional();
				if(ce.ContainerToken == TokenID.kAs)
					throw new SqlParserException("Not support AS in WHERE PART");
				if(ce.ContainerToken == TokenID.RParen)
				{
					this.GetNextExpression(0);
					break;
				}
				expressions.Add(ce);
				i++;
				if(i > 1000)
					throw new SqlParserException("Invalid instruction WHERE");
			}
			while (CurrentExpression.Token != TokenID.End);
			wherePart.Expressions = expressions;
			return wherePart;
		}

		/// <summary>
		/// Executa um parser somente em cima da parte da clausula ORDER BY
		/// </summary>
		/// <returns></returns>
		public OrderByPart ExecuteOrderByPart()
		{
			return ParserOrderByPart(true);
		}

		/// <summary>
		/// Executa um parser somente em cima da parte da clausula GROUP BY.
		/// </summary>
		/// <returns></returns>
		public GroupByPart ExecuteGroupByPart()
		{
			return ParserGroupByPart(true);
		}

		/// <summary>
		/// Executa um parser somente na parte do HAVING.
		/// </summary>
		/// <returns></returns>
		public HavingPart ExecuteHavingPart()
		{
			var havingPart = new HavingPart();
			List<SqlExpression> expressions = new List<SqlExpression>();
			int i = 0;
			do
			{
				ContainerSqlExpression ce = ParserContainerConditional();
				if(ce.ContainerToken == TokenID.kAs)
					throw new SqlParserException("Not support AS in HAVING PART");
				if(ce.ContainerToken == TokenID.RParen)
				{
					this.GetNextExpression(0);
					break;
				}
				expressions.Add(ce);
				i++;
				if(i > 1000)
					throw new SqlParserException("Invalid instruction HAVING");
			}
			while (CurrentExpression.Token != TokenID.End);
			havingPart.Expressions.AddRange(expressions);
			return havingPart;
		}

		/// <summary>
		/// Recupera os dados do comando SELECT
		/// </summary>
		/// <returns></returns>
		private Select ParserSelect()
		{
			Select select = new Select(CurrentExpression);
			if(CurrentExpression.Token == TokenID.kSelect)
			{
				NextExpression(true);
				select.SelectPart = ParserSelectPart();
			}
			if(CurrentExpression.Token == TokenID.kFrom)
			{
				select.FromPart = ParserFromPart();
				if(CurrentExpression.Token == TokenID.kWhere)
				{
					select.WherePart = ParserWherePart();
				}
			}
			if(CurrentExpression.Token == TokenID.kGroup)
			{
				NextExpression(true);
				if(CurrentExpression.Token == TokenID.kBy)
				{
					select.GroupByPart = ParserGroupByPart();
				}
				else
				{
					Error("Expected expression by.");
				}
			}
			if(CurrentExpression.Token == TokenID.kHaving)
			{
				select.HavingPart = ParserHavingPart();
			}
			if(CurrentExpression.Token == TokenID.kOrder)
			{
				NextExpression(true);
				if(CurrentExpression.Token == TokenID.kBy)
				{
					select.OrderByPart = ParserOrderByPart();
				}
				else
				{
					Error("Expected expression by.");
				}
			}
			if(select.SelectPart == null && CurrentExpression.Token == TokenID.kSelect)
			{
				NextExpression(true);
				select.SelectPart = ParserSelectPart();
			}
			else if(select.SelectPart == null)
				Error("Select not found.");
			return select;
		}

		/// <summary>
		/// Recupera os dados da parte do SELECT.
		/// </summary>
		/// <returns></returns>
		private SelectPart ParserSelectPart()
		{
			SelectPart selectPart = new SelectPart();
			if(CurrentExpression.Token == TokenID.kTop)
			{
				if(!NextExpression(true) || CurrentExpression.Token == TokenID.IntLiteral)
					Error("Expected a number integer.");
				selectPart.Top = uint.Parse(CurrentExpression.Text);
				NextExpression(false);
				if(CurrentExpression.Token == TokenID.kPercent)
				{
					selectPart.TopInPercent = true;
					NextExpression(false);
				}
			}
			if(CurrentExpression.Token == TokenID.kDistinct || CurrentExpression.Token == TokenID.kAll)
			{
				selectPart.ResultType = (CurrentExpression.Token == TokenID.kDistinct ? SelectClauseResultTypes.Distinct : SelectClauseResultTypes.All);
				NextExpression(false);
			}
			while (true)
			{
				SelectExpression se = ParserSelectExpression();
				if(se != null)
				{
					selectPart.SelectionExpressions.Add(se);
					if(CurrentExpression.Token == TokenID.Comma)
						NextExpression(true);
					else
						break;
				}
				else
					break;
			}
			switch(CurrentExpression.Token)
			{
			case TokenID.End:
			case TokenID.RParen:
			case TokenID.Semi:
			case TokenID.kFrom:
				break;
			default:
				switch(GetPreviousExpression(1).Token)
				{
				case TokenID.kAnd:
				case TokenID.kOr:
				case TokenID.End:
				case TokenID.RParen:
				case TokenID.Semi:
				case TokenID.kFrom:
					posCurrentExpression--;
					break;
				default:
					Error("\")\" or \";\" or \"from\" expected.");
					break;
				}
				break;
			}
			return selectPart;
		}

		private SelectExpression ParserSelectExpression()
		{
			SelectExpression se = new SelectExpression();
			if(CurrentExpression.Token == TokenID.Star)
			{
				se = new SelectExpression(new SqlExpression(CurrentExpression, SqlExpressionType.Column));
				NextExpression(false);
				return se;
			}
			else if(CurrentExpression.Token == TokenID.kCase)
			{
				throw new SqlParserException("Not implemented keyword Case.");
			}
			else if(CurrentExpression.Token == TokenID.LParen)
			{
				ContainerSqlExpression ce = new ContainerSqlExpression(CurrentExpression);
				NextExpression(true);
				ce.Expressions.Add(ParserContainerSqlExpression(TokenID.RParen));
				se.ColumnName = ce;
				if(CurrentExpression.Token == TokenID.RParen)
					NextExpression(false);
			}
			else
			{
				try
				{
					se.ColumnName = ParserContainerSqlExpression(TokenID.kAs, TokenID.Comma, TokenID.kFrom).Expressions[0];
				}
				catch(Exception ex)
				{
					Error("Invalid expression.");
				}
			}
			if(CurrentExpression.Token == TokenID.kAs)
			{
				se.AsExpression = CurrentExpression;
				NextExpression(true);
				if(CurrentExpression.Token != TokenID.Identifier)
					Error("Alias of column name expected.");
				se.ColumnAlias = new SqlExpression(CurrentExpression);
				NextExpression(false);
			}
			else if(CurrentExpression.Token == TokenID.Identifier)
			{
				se.ColumnAlias = new SqlExpression(CurrentExpression);
				NextExpression(false);
			}
			return se;
		}

		private SqlFunction ParserFunction()
		{
			SqlFunction func = new SqlFunction(GetPreviousExpression(1));
			NextExpression(true);
			while (true)
			{
				ContainerSqlExpression ce = ParserContainerSqlExpression(TokenID.Equal, TokenID.Less, TokenID.LessEqual, TokenID.Greater, TokenID.GreaterEqual, TokenID.NotEqual, TokenID.Comma, TokenID.RParen);
				if(ce != null)
				{
					List<SqlExpression> exprs = ce.Expressions;
					switch(CurrentExpression.Token)
					{
					case TokenID.Equal:
					case TokenID.Less:
					case TokenID.LessEqual:
					case TokenID.Greater:
					case TokenID.GreaterEqual:
					case TokenID.NotEqual:
						exprs.Add(new SqlExpression(CurrentExpression));
						NextExpression(true);
						ce = ParserContainerSqlExpression(TokenID.Comma, TokenID.RParen);
						if(ce != null)
							exprs.AddRange(ce.Expressions);
						break;
					}
					func.Parameters.Add(exprs);
					if(CurrentExpression.Token == TokenID.RParen)
					{
						NextExpression(false);
						break;
					}
					else if(CurrentExpression.Token == TokenID.Comma)
					{
						NextExpression(false);
					}
				}
				else
					Error("Invalid expression on context.");
			}
			return func;
		}

		private ContainerSqlExpression ParserContainerSqlExpression(params TokenID[] endToken)
		{
			ContainerSqlExpression container = new ContainerSqlExpression(CurrentExpression);
			Stack<ContainerSqlExpression> tokensOpeneds = new Stack<ContainerSqlExpression>();
			bool findOperator = false;
			while (true)
			{
				switch(CurrentExpression.Token)
				{
				case TokenID.Identifier:
					if(findOperator)
					{
						if(container.Expressions.Count > 0)
							return container;
						else
							return null;
					}
					findOperator = true;
					switch(GetNextExpression(1).Token)
					{
					case TokenID.LParen:
						NextExpression(true);
						container.Expressions.Add(ParserFunction());
						continue;
					case TokenID.Dot:
						container.Expressions.Add(ParserNamespace());
						continue;
					default:
						container.Expressions.Add(new SqlExpression(CurrentExpression, SqlExpressionType.Column));
						break;
					}
					break;
				case TokenID.LParen:
					tokensOpeneds.Push(container);
					container = new ContainerSqlExpression(CurrentExpression);
					tokensOpeneds.Peek().Expressions.Add(container);
					break;
				case TokenID.RParen:
					findOperator = true;
					if(tokensOpeneds.Count == 0)
					{
						if(ArrayHelper.Exists<TokenID>(endToken, delegate(TokenID t) {
							return t == CurrentExpression.Token;
						}))
						{
							if(container.Expressions.Count > 0)
								return container;
							else
								return null;
						}
					}
					else
					{
						container = tokensOpeneds.Pop();
					}
					break;
				case TokenID.StringLiteral:
				case TokenID.IntLiteral:
				case TokenID.DecimalLiteral:
				case TokenID.RealLiteral:
					if(findOperator)
					{
						if(container.Expressions.Count > 0)
							return container;
						else
							return null;
					}
					findOperator = true;
					container.Expressions.Add(new SqlExpression(CurrentExpression));
					break;
				case TokenID.Plus:
				case TokenID.Minus:
				case TokenID.Star:
				case TokenID.Slash:
				case TokenID.Equal:
				case TokenID.EqualEqual:
					if(findOperator)
						container.Expressions.Add(new SqlExpression(CurrentExpression));
					else if(!findOperator && CurrentExpression.Token == TokenID.Star)
						container.Expressions.Add(new SqlExpression(CurrentExpression, SqlExpressionType.Column));
					else if(container.Expressions.Count > 0)
						return container;
					else
						return null;
					findOperator = false;
					break;
				case TokenID.kSelect:
					if(GetPreviousExpression(1).Token == TokenID.LParen)
					{
						container.Expressions.Add(ParserSelect());
						findOperator = true;
						continue;
					}
					else if(container.Expressions.Count > 0)
						return container;
					else
						return null;
				case TokenID.Comma:
				case TokenID.End:
					if(container.Expressions.Count > 0)
						return container;
					else
						return null;
				default:
					if(CurrentExpression.Token != TokenID.kFrom && CurrentExpression.Token.ToString()[0] == 'k')
					{
						switch(CurrentExpression.Token)
						{
						case TokenID.kLike:
						case TokenID.kIs:
						case TokenID.kIn:
						case TokenID.kBetween:
						case TokenID.kAnd:
						case TokenID.kOr:
						case TokenID.kAny:
						case TokenID.kAll:
						case TokenID.kSome:
							break;
						default:
							if(GetNextExpression(1).Token == TokenID.LParen)
							{
								NextExpression(true);
								container.Expressions.Add(ParserFunction());
								findOperator = true;
								continue;
							}
							break;
						}
					}
					if(CurrentExpression.Token == TokenID.Semi || ArrayHelper.Exists<TokenID>(endToken, delegate(TokenID t) {
						return t == CurrentExpression.Token;
					}) && tokensOpeneds.Count == 0 || CurrentExpression.Token.ToString()[0] == 'k')
					{
						if(container.Expressions.Count > 0)
							return container;
						else
							return null;
					}
					else
					{
						if(tokensOpeneds.Count > 0)
							Error("Parent not closed.");
						else
							Error("Invalid expression.");
					}
					break;
				}
				NextExpression(false);
			}
		}

		private SqlExpression ParserNamespace()
		{
			List<Expression> expr = new List<Expression>();
			expr.Add(CurrentExpression);
			bool dot = true;
			while (true)
			{
				if(!dot && (GetNextExpression(1).Token == TokenID.Identifier || (expr.Count > 0 && GetNextExpression(1).Token == TokenID.Star)))
				{
					dot = true;
					NextExpression(true);
					expr.Add(CurrentExpression);
				}
				else if(dot && GetNextExpression(1).Token == TokenID.Dot)
				{
					dot = false;
					NextExpression(true);
					expr.Add(CurrentExpression);
				}
				else
				{
					NextExpression(false);
					int beginPoint = expr[0].BeginPoint;
					int length = expr[0].Length;
					if(expr.Count > 1)
					{
						length = (expr[expr.Count - 1].BeginPoint - beginPoint) + expr[expr.Count - 1].Length;
					}
					StringBuilder e = new StringBuilder();
					if(expr[0].CurrentSpecialContainer != null)
						e.Append(expr[0].CurrentSpecialContainer.BeginCharSpecialContainer).Append(expr[0].Text).Append(expr[0].CurrentSpecialContainer.EndCharSpecialContainer);
					else
						e.Append(expr[0].Text);
					for(int i = 1; i < expr.Count; i++)
					{
						e.Append(' ', expr[i].BeginPoint + (expr[i].CurrentSpecialContainer != null ? -1 : 0) - ((expr[i - 1].BeginPoint + expr[i - 1].Length) + (expr[i - 1].CurrentSpecialContainer != null ? 1 : 0)));
						if(expr[i].CurrentSpecialContainer != null)
						{
							e.Append(expr[i].CurrentSpecialContainer.BeginCharSpecialContainer).Append(expr[i].Text).Append(expr[i].CurrentSpecialContainer.EndCharSpecialContainer);
						}
						else
						{
							e.Append(expr[i].Text);
						}
					}
					Expression ee = new Expression(TokenID.Identifier);
					ee.BeginPoint = beginPoint;
					ee.Length = length;
					ee.Text = e.ToString();
					ee.Line = expr[0].Line;
					return new SqlExpression(ee, SqlExpressionType.Column);
				}
			}
		}

		private FromPart ParserFromPart()
		{
			FromPart fromPart = new FromPart();
			NextExpression(true);
			while (true)
			{
				switch(CurrentExpression.Token)
				{
				case TokenID.Identifier:
				case TokenID.LParen:
				case TokenID.kLeft:
				case TokenID.kRight:
				case TokenID.kFull:
				case TokenID.kInner:
				case TokenID.kCross:
				case TokenID.kNatural:
					fromPart.TableExpressions.Add(ParserTableExpression());
					break;
				default:
					return fromPart;
				}
			}
		}

		private TableExpression ParserTableExpression()
		{
			TableExpression te = new TableExpression();
			while (true)
			{
				switch(CurrentExpression.Token)
				{
				case TokenID.Identifier:
					if(te.TableName == null)
					{
						if(GetNextExpression(1).Token == TokenID.Dot)
						{
							te.TableName = new TableNameExpression(ParserNamespace().Value);
							continue;
						}
						else
							te.TableName = new TableNameExpression(CurrentExpression);
					}
					else
						te.TableAlias = CurrentExpression;
					break;
				case TokenID.kAs:
					te.AsExpression = CurrentExpression;
					NextExpression(true);
					te.TableAlias = CurrentExpression;
					break;
				case TokenID.LParen:
					NextExpression(true);
					if(CurrentExpression.Token == TokenID.kSelect)
					{
						te.SelectInfo = ParserSelect();
						if(CurrentExpression.Token == TokenID.RParen)
						{
							NextExpression(false);
						}
						else
							Error("Expected character )");
						continue;
					}
					break;
				case TokenID.kLeft:
				case TokenID.kRight:
				case TokenID.kFull:
				case TokenID.kInner:
				case TokenID.kCross:
				case TokenID.kNatural:
					if(te.TableName != null)
						return te;
					if(CurrentExpression.Token == TokenID.kLeft || CurrentExpression.Token == TokenID.kRight || CurrentExpression.Token == TokenID.kFull)
					{
						te.LeftOrRight = CurrentExpression;
						NextExpression(true);
					}
					switch(CurrentExpression.Token)
					{
					case TokenID.kOuter:
					case TokenID.kInner:
					case TokenID.kCross:
					case TokenID.kNatural:
						te.OuterOrInnerOrCrossOrNatural = CurrentExpression;
						NextExpression(true);
						break;
					default:
						break;
					}
					if(CurrentExpression.Token != TokenID.kJoin)
					{
						Error("Expected Join expression");
					}
					te.Join = CurrentExpression;
					NextExpression(true);
					if(CurrentExpression.Token == TokenID.LParen)
					{
						NextExpression(true);
						if(CurrentExpression.Token == TokenID.kSelect)
						{
							te.SelectInfo = ParserSelect();
						}
						else
							Error("Expected select command.");
					}
					else if(CurrentExpression.Token == TokenID.Identifier)
					{
						te.TableName = new TableNameExpression(CurrentExpression);
						NextExpression(false);
					}
					else
						Error("Expected tablename or viewname.");
					if(CurrentExpression.Token == TokenID.kAs)
					{
						te.AsExpression = CurrentExpression;
						if(!NextExpression(false) && CurrentExpression.Token != TokenID.Identifier)
							Error("Expected alias of table name.");
						te.TableAlias = CurrentExpression;
						NextExpression(false);
					}
					else if(CurrentExpression.Token == TokenID.Identifier)
					{
						te.TableAlias = CurrentExpression;
						NextExpression(false);
					}
					if(CurrentExpression.Token == TokenID.kOn)
					{
						NextExpression(true);
						if(CurrentExpression.Token == TokenID.LParen)
						{
							te.OnExpressions = ParserContainerConditional(true);
							if(CurrentExpression.Token == TokenID.RParen)
							{
								NextExpression(false);
							}
							else
								Error("Expected character ')'.");
						}
						else
							te.OnExpressions = ParserContainerConditional();
					}
					return te;
				default:
					return te;
				}
				NextExpression(true);
			}
		}

		/// <summary>
		/// Executa o parser de um container condicional.
		/// </summary>
		/// <returns></returns>
		private ContainerSqlExpression ParserContainerConditional()
		{
			return ParserContainerConditional(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jumpFirstExpression">Identifica se deve pular a primeira expressão.</param>
		/// <returns></returns>
		private ContainerSqlExpression ParserContainerConditional(bool jumpFirstExpression)
		{
			ContainerSqlExpression container = new ContainerSqlExpression(CurrentExpression);
			if(jumpFirstExpression || CurrentExpression.Token == TokenID.LParen)
				NextExpression(true);
			Stack<ContainerSqlExpression> tokensOpeneds = new Stack<ContainerSqlExpression>();
			bool findConditional = false;
			while (true)
			{
				switch(CurrentExpression.Token)
				{
				case TokenID.Identifier:
				case TokenID.StringLiteral:
				case TokenID.IntLiteral:
					if(findConditional)
					{
						if(container.Expressions.Count > 0)
							return container;
						else
							return null;
					}
					findConditional = true;
					container.Expressions.Add(ParserContainerSqlExpression(TokenID.Equal, TokenID.EqualEqual, TokenID.Less, TokenID.LessEqual, TokenID.Greater, TokenID.GreaterEqual, TokenID.NotEqual, TokenID.kLike, TokenID.kIs, TokenID.kNot, TokenID.kIn, TokenID.kBetween, TokenID.kAnd, TokenID.kOr, TokenID.RParen));
					VERIFYOPERATOR:
					switch(CurrentExpression.Token)
					{
					case TokenID.Equal:
					case TokenID.Less:
					case TokenID.LessEqual:
					case TokenID.Greater:
					case TokenID.GreaterEqual:
					case TokenID.NotEqual:
						container.Expressions.Add(new SqlExpression(CurrentExpression));
						NextExpression(true);
						if(CurrentExpression.Token == TokenID.kAny || CurrentExpression.Token == TokenID.kSome || CurrentExpression.Token == TokenID.kAll)
						{
							container.Expressions.Add(new SqlExpression(CurrentExpression));
							NextExpression(true);
							if(CurrentExpression.Token == TokenID.LParen)
							{
								ContainerSqlExpression ce = new ContainerSqlExpression(CurrentExpression);
								NextExpression(true);
								ce.Expressions.Add(ParserSelect());
								if(CurrentExpression.Token == TokenID.RParen)
								{
									NextExpression(false);
									container.Expressions.Add(ce);
									return container;
								}
								else
									Error("Expected character '('.");
							}
							else
								Error("Expected character ')'.");
						}
						else
						{
							container.Expressions.Add(ParserContainerConditional());
						}
						continue;
					case TokenID.kLike:
						container.Expressions.Add(new SqlExpression(CurrentExpression));
						NextExpression(true);
						container.Expressions.Add(ParserContainerSqlExpression(TokenID.RParen, TokenID.kAnd, TokenID.kOr));
						continue;
					case TokenID.kBetween:
						container.Expressions.Add(new SqlExpression(CurrentExpression));
						NextExpression(true);
						container.Expressions.Add(ParserContainerSqlExpression(TokenID.kAnd));
						if(CurrentExpression.Token == TokenID.kAnd)
						{
							container.Expressions.Add(new SqlExpression(CurrentExpression));
							NextExpression(true);
							container.Expressions.Add(ParserContainerSqlExpression(TokenID.RParen, TokenID.kAnd, TokenID.kOr));
						}
						else
							Error("Expected AND expression.");
						continue;
					case TokenID.kIs:
						container.Expressions.Add(new SqlExpression(CurrentExpression));
						NextExpression(true);
						if(CurrentExpression.Token == TokenID.kNot)
						{
							container.Expressions.Add(new SqlExpression(CurrentExpression));
							NextExpression(true);
						}
						if(CurrentExpression.Token == TokenID.kNull)
						{
							SqlExpression sqlEx = container.Expressions[container.Expressions.Count - 1];
							int beginPoint = sqlEx.Value.BeginPoint;
							int length = (CurrentExpression.BeginPoint + CurrentExpression.Length) - beginPoint;
							StringBuilder strBuilder = new StringBuilder();
							strBuilder.Append(sqlEx.Value.Text);
							strBuilder.Append(' ', CurrentExpression.BeginPoint - (sqlEx.Value.BeginPoint + sqlEx.Value.Length));
							strBuilder.Append(CurrentExpression.Text);
							Expression ee = new Expression(TokenID.kIsNull);
							ee.BeginPoint = beginPoint;
							ee.Length = length;
							ee.Text = strBuilder.ToString();
							ee.Line = sqlEx.Value.Line;
							container.Expressions.RemoveAt(container.Expressions.Count - 1);
							container.Expressions.Add(new SqlExpression(ee));
							NextExpression(false);
						}
						else
						{
							Error("Esperado NULL");
						}
						continue;
					case TokenID.kNot:
						container.Expressions.Add(new SqlExpression(CurrentExpression));
						NextExpression(false);
						if(CurrentExpression.Token == TokenID.kIn)
						{
							goto VERIFYOPERATOR;
						}
						continue;
					case TokenID.kIn:
					case TokenID.kAll:
					case TokenID.kAny:
					case TokenID.kSome:
						container.Expressions.Add(new SqlExpression(CurrentExpression));
						NextExpression(true);
						if(CurrentExpression.Token == TokenID.LParen)
						{
							ContainerSqlExpression ce = new ContainerSqlExpression(CurrentExpression);
							NextExpression(true);
							if(CurrentExpression.Token != TokenID.kSelect)
							{
								Select select = new Select(new Expression(0, 0, CurrentExpression.Line));
								select.Value.Text = "";
								select.SelectPart = ParserSelectPart();
								ce.Expressions.Add(select);
								if(CurrentExpression.Token == TokenID.RParen || GetPreviousExpression(1).Token == TokenID.RParen)
								{
									NextExpression(false);
									container.Expressions.Add(ce);
									return container;
								}
								else
									Error("Expected character ')'.");
							}
							else
							{
								ce.Expressions.Add(ParserSelect());
								if(CurrentExpression.Token == TokenID.RParen || (CurrentExpression.Token == TokenID.End && GetPreviousExpression(1).Token == TokenID.RParen))
								{
									NextExpression(false);
									container.Expressions.Add(ce);
									return container;
								}
								else if(CurrentExpression.Token != TokenID.RParen && GetPreviousExpression(1).Token == TokenID.RParen)
								{
									container.Expressions.Add(ce);
								}
								else
									Error("Expected character ')'.");
							}
						}
						else
							Error("Expected '('.");
						continue;
					case TokenID.RParen:
						return container;
					case TokenID.kAnd:
					case TokenID.kOr:
						continue;
					default:
						continue;
					}
					break;
				case TokenID.LParen:
					findConditional = true;
					container.Expressions.Add(ParserContainerConditional(true));
					if(GetPreviousExpression(1).Token == TokenID.RParen)
						continue;
					else if(CurrentExpression.Token == TokenID.RParen)
						continue;
					else
						Error("Expected character ')'.");
					return container;
				case TokenID.RParen:
					if(container.ContainerToken == TokenID.LParen)
						NextExpression(false);
					return container;
				case TokenID.kSelect:
					if(GetPreviousExpression(1).Token == TokenID.LParen)
					{
						container.Expressions.Add(ParserSelect());
						continue;
					}
					else
						return container;
				case TokenID.kExists:
					container.Expressions.Add(new SqlExpression(CurrentExpression) {
						Type = SqlExpressionType.Function
					});
					NextExpression(true);
					if(CurrentExpression.Token == TokenID.LParen)
					{
						ContainerSqlExpression ce = new ContainerSqlExpression(CurrentExpression);
						NextExpression(true);
						if(CurrentExpression.Token == TokenID.Identifier)
						{
							ce.Expressions.Add(new SelectVariable(CurrentExpression));
							NextExpression(true);
						}
						else
							ce.Expressions.Add(ParserSelect());
						if(CurrentExpression.Token == TokenID.RParen)
						{
							NextExpression(false);
							container.Expressions.Add(ce);
							return container;
						}
						else
							Error("Expected character '('.");
					}
					else
						Error("Expected character ')'.");
					continue;
				case TokenID.kAnd:
				case TokenID.kOr:
					findConditional = false;
					container.Expressions.Add(new SqlExpression(CurrentExpression));
					container.Expressions.Add(ParserContainerConditional(true));
					continue;
				case TokenID.End:
					if(container.Expressions.Count > 0)
						return container;
					else
						return null;
				default:
					if(CurrentExpression.Token.ToString()[0] == 'k')
					{
						switch(CurrentExpression.Token)
						{
						case TokenID.kLike:
						case TokenID.kIs:
						case TokenID.kIn:
						case TokenID.kBetween:
						case TokenID.kAnd:
						case TokenID.kOr:
						case TokenID.kAny:
						case TokenID.kAll:
						case TokenID.kSome:
							break;
						case TokenID.kValue:
						case TokenID.kValues:
						case TokenID.kZone:
						case TokenID.kMonth:
						case TokenID.kFound:
						case TokenID.kHour:
						case TokenID.kMinute:
						case TokenID.kSecond:
							Error("Invalid keyword " + CurrentExpression.Token.ToString().Substring(1));
							break;
						default:
							if(GetNextExpression(1).Token == TokenID.LParen)
							{
								NextExpression(true);
								container.Expressions.Add(ParserFunction());
								findConditional = true;
								goto VERIFYOPERATOR;
								continue;
							}
							break;
						}
					}
					return container;
				}
			}
		}

		private WherePart ParserWherePart()
		{
			WherePart wherePart = new WherePart();
			wherePart.Where = CurrentExpression;
			NextExpression(true);
			ContainerSqlExpression ce = ParserContainerConditional();
			wherePart.Expressions = ce.Expressions;
			if(ce.ContainerToken == TokenID.LParen)
			{
				NextExpression(false);
			}
			return wherePart;
		}

		private GroupByPart ParserGroupByPart()
		{
			return ParserGroupByPart(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreOrderByExpression">
		/// Ignora a existencia da expressão group by e não 
		/// pula para a próxima expressão para começar a fazer a analise.
		/// </param>
		/// <returns></returns>
		private GroupByPart ParserGroupByPart(bool ignoreGroupByExpression)
		{
			GroupByPart groupByPart = new GroupByPart();
			if(!ignoreGroupByExpression)
			{
				groupByPart.Group = GetPreviousExpression(1);
				groupByPart.By = CurrentExpression;
				NextExpression(true);
			}
			while (true)
			{
				ContainerSqlExpression ce = ParserContainerSqlExpression(TokenID.Comma);
				if(ce != null)
				{
					groupByPart.Expressions.Add(ce);
					if(CurrentExpression.Token == TokenID.Comma)
						NextExpression(true);
					else
						break;
				}
				else
					break;
			}
			return groupByPart;
		}

		public OrderByPart ParserOrderByPart()
		{
			return ParserOrderByPart(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ignoreOrderByExpression">
		/// Ignora a existencia da expressão order by e não 
		/// pula para a próxima expressão para começar a fazer a analise.
		/// </param>
		/// <returns></returns>
		public OrderByPart ParserOrderByPart(bool ignoreOrderByExpression)
		{
			OrderByPart orderByPart = new OrderByPart();
			if(!ignoreOrderByExpression)
				NextExpression(true);
			while (true)
			{
				ContainerSqlExpression ce = ParserContainerSqlExpression(TokenID.Comma, TokenID.kAsc, TokenID.kDesc, TokenID.kNulls, TokenID.RParen, TokenID.Semi);
				if(ce != null)
				{
					OrderByExpression obe = new OrderByExpression();
					obe.Expression = ce;
					if(CurrentExpression.Token == TokenID.kAsc || CurrentExpression.Token == TokenID.kDesc)
					{
						obe.AscOrDesc = CurrentExpression;
						obe.Asc = (CurrentExpression.Token == TokenID.kAsc);
						NextExpression(false);
					}
					if(CurrentExpression.Token == TokenID.kNulls)
					{
						obe.Nulls = true;
						NextExpression(false);
						if(CurrentExpression.Token == TokenID.kFirst || CurrentExpression.Token == TokenID.kLast)
						{
							obe.First = (CurrentExpression.Token == TokenID.kFirst);
							NextExpression(false);
						}
					}
					orderByPart.OrderByExpressions.Add(obe);
					if(CurrentExpression.Token == TokenID.Comma)
						NextExpression(true);
					else
						break;
				}
				else
					break;
			}
			return orderByPart;
		}

		private HavingPart ParserHavingPart()
		{
			HavingPart havingPart = new HavingPart();
			NextExpression(true);
			while (true)
			{
				ContainerSqlExpression ce = ParserContainerConditional();
				if(ce != null)
				{
					havingPart.Expressions.Add(ce);
					if(CurrentExpression.Token == TokenID.Comma)
						NextExpression(true);
					else
						break;
				}
				else
					break;
			}
			return havingPart;
		}
	}
}
