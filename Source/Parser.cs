using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression.Enums;
using GDA.Sql.InterpreterExpression.Nodes;
namespace GDA.Sql.InterpreterExpression
{
	class Parser
	{
		private Lexer _lex;
		private List<Expression> expressions;
		private int posCurrentExpression = 0;
		private List<Select> _selectParts = new List<Select> ();
		public List<Select> SelectParts {
			get {
				return _selectParts;
			}
			set {
				_selectParts = value;
			}
		}
		private Expression CurrentExpression {
			get {
				if (posCurrentExpression < expressions.Count)
					return expressions [posCurrentExpression];
				else {
					Expression a = new Expression ();
					a.Token = TokenID.End;
					return a;
				}
			}
		}
		public Lexer Lex {
			get {
				return _lex;
			}
		}
		private void Error (string a)
		{
			int b = 13;
			if (posCurrentExpression <= b)
				b = posCurrentExpression;
			StringBuilder c = new StringBuilder ();
			if (b > 0) {
				c.Append ("\"");
				if (b == 13)
					c.Append ("...");
				Expression d = GetPreviousExpression (b);
				int e = d.BeginPoint + d.Length;
				c.Append (d.Text);
				for (int f = (b - 1); f > 0; f--) {
					d = GetPreviousExpression (f);
					c.Append (' ', (d.BeginPoint - e));
					if (d.CurrentSpecialContainer != null)
						c.Append (d.CurrentSpecialContainer.BeginCharSpecialContainer).Append (d.Text).Append (d.CurrentSpecialContainer.EndCharSpecialContainer);
					else if (d is SpecialContainerExpression)
						c.Append (((SpecialContainerExpression)d).ContainerChar).Append (d.Text).Append (((SpecialContainerExpression)d).ContainerChar);
					else
						c.Append (d.Text);
					e = d.BeginPoint + d.Length;
				}
				c.Append ("...\"");
			}
			throw new SqlParserException (a + " Command: " + c.ToString ());
		}
		private Expression GetPreviousExpression (int a)
		{
			return expressions [posCurrentExpression - a];
		}
		private Expression GetNextExpression (int a)
		{
			if (posCurrentExpression + a < expressions.Count) {
				return expressions [posCurrentExpression + a];
			}
			else
				return new Expression (TokenID.InvalidExpression);
		}
		private bool NextExpression (bool a)
		{
			if (posCurrentExpression < expressions.Count) {
				posCurrentExpression++;
				return true;
			}
			else if (a) {
				Error ("Expected a expression.");
				return false;
			}
			else
				return false;
		}
		public Parser (Lexer a)
		{
			this._lex = a;
			this.expressions = a.Expressions;
		}
		public void Execute ()
		{
			while (true) {
				switch (CurrentExpression.Token) {
				case TokenID.kSelect:
				case TokenID.kFrom:
					_selectParts.Add (ParserSelect ());
					break;
				default:
					break;
				}
				if (CurrentExpression.Token == TokenID.Semi) {
					NextExpression (false);
					continue;
				}
				break;
			}
			if (CurrentExpression.Token != TokenID.Semi && CurrentExpression.Token != TokenID.End)
				Error ("Not found end command.");
		}
		public SelectPart ExecuteSelectPart ()
		{
			return ParserSelectPart ();
		}
		public WherePart ExecuteWherePart ()
		{
			WherePart a = new WherePart ();
			List<SqlExpression> b = new List<SqlExpression> ();
			ContainerSqlExpression c = null;
			int d = 0;
			do {
				ContainerSqlExpression e = ParserContainerConditional ();
				if (e.ContainerToken == TokenID.kAs)
					throw new SqlParserException ("Not support AS in WHERE PART");
				if (e.ContainerToken == TokenID.RParen) {
					this.GetNextExpression (0);
					break;
				}
				b.Add (e);
				d++;
				if (d > 1000)
					throw new SqlParserException ("Invalid instruction WHERE");
			}
			while (CurrentExpression.Token != TokenID.End);
			a.Expressions = b;
			return a;
		}
		public OrderByPart ExecuteOrderByPart ()
		{
			return ParserOrderByPart (true);
		}
		public GroupByPart ExecuteGroupByPart ()
		{
			return ParserGroupByPart (true);
		}
		private Select ParserSelect ()
		{
			Select a = new Select (CurrentExpression);
			if (CurrentExpression.Token == TokenID.kSelect) {
				NextExpression (true);
				a.SelectPart = ParserSelectPart ();
			}
			if (CurrentExpression.Token == TokenID.kFrom) {
				a.FromPart = ParserFromPart ();
				if (CurrentExpression.Token == TokenID.kWhere) {
					a.WherePart = ParserWherePart ();
				}
			}
			if (CurrentExpression.Token == TokenID.kGroup) {
				NextExpression (true);
				if (CurrentExpression.Token == TokenID.kBy) {
					a.GroupByPart = ParserGroupByPart ();
				}
				else {
					Error ("Expected expression by.");
				}
			}
			if (CurrentExpression.Token == TokenID.kHaving) {
				a.HavingPart = ParserHavingPart ();
			}
			if (CurrentExpression.Token == TokenID.kOrder) {
				NextExpression (true);
				if (CurrentExpression.Token == TokenID.kBy) {
					a.OrderByPart = ParserOrderByPart ();
				}
				else {
					Error ("Expected expression by.");
				}
			}
			if (a.SelectPart == null && CurrentExpression.Token == TokenID.kSelect) {
				NextExpression (true);
				a.SelectPart = ParserSelectPart ();
			}
			else if (a.SelectPart == null)
				Error ("Select not found.");
			return a;
		}
		private SelectPart ParserSelectPart ()
		{
			SelectPart a = new SelectPart ();
			if (CurrentExpression.Token == TokenID.kTop) {
				if (!NextExpression (true) || CurrentExpression.Token == TokenID.IntLiteral)
					Error ("Expected a number integer.");
				a.Top = uint.Parse (CurrentExpression.Text);
				NextExpression (false);
				if (CurrentExpression.Token == TokenID.kPercent) {
					a.TopInPercent = true;
					NextExpression (false);
				}
			}
			if (CurrentExpression.Token == TokenID.kDistinct || CurrentExpression.Token == TokenID.kAll) {
				a.ResultType = (CurrentExpression.Token == TokenID.kDistinct ? SelectClauseResultTypes.Distinct : SelectClauseResultTypes.All);
				NextExpression (false);
			}
			while (true) {
				SelectExpression b = ParserSelectExpression ();
				if (b != null) {
					a.SelectionExpressions.Add (b);
					if (CurrentExpression.Token == TokenID.Comma)
						NextExpression (true);
					else
						break;
				}
				else
					break;
			}
			switch (CurrentExpression.Token) {
			case TokenID.End:
			case TokenID.RParen:
			case TokenID.Semi:
			case TokenID.kFrom:
				break;
			default:
				switch (GetPreviousExpression (1).Token) {
				case TokenID.kAnd:
				case TokenID.kOr:
				case TokenID.End:
				case TokenID.RParen:
				case TokenID.Semi:
				case TokenID.kFrom:
					posCurrentExpression--;
					break;
				default:
					Error ("\")\" or \";\" or \"from\" expected.");
					break;
				}
				break;
			}
			return a;
		}
		private SelectExpression ParserSelectExpression ()
		{
			SelectExpression a = new SelectExpression ();
			if (CurrentExpression.Token == TokenID.Star) {
				a = new SelectExpression (new SqlExpression (CurrentExpression, SqlExpressionType.Column));
				NextExpression (false);
				return a;
			}
			else if (CurrentExpression.Token == TokenID.kCase) {
				throw new SqlParserException ("Not implemented keyword Case.");
			}
			else if (CurrentExpression.Token == TokenID.LParen) {
				ContainerSqlExpression b = new ContainerSqlExpression (CurrentExpression);
				NextExpression (true);
				b.Expressions.Add (ParserContainerSqlExpression (TokenID.RParen));
				a.ColumnName = b;
				if (CurrentExpression.Token == TokenID.RParen)
					NextExpression (false);
			}
			else {
				try {
					a.ColumnName = ParserContainerSqlExpression (TokenID.kAs, TokenID.Comma, TokenID.kFrom).Expressions [0];
				}
				catch (Exception ex) {
					Error ("Invalid expression.");
				}
			}
			if (CurrentExpression.Token == TokenID.kAs) {
				a.AsExpression = CurrentExpression;
				NextExpression (true);
				if (CurrentExpression.Token != TokenID.Identifier)
					Error ("Alias of column name expected.");
				a.ColumnAlias = new SqlExpression (CurrentExpression);
				NextExpression (false);
			}
			else if (CurrentExpression.Token == TokenID.Identifier) {
				a.ColumnAlias = new SqlExpression (CurrentExpression);
				NextExpression (false);
			}
			return a;
		}
		private SqlFunction ParserFunction ()
		{
			SqlFunction a = new SqlFunction (GetPreviousExpression (1));
			NextExpression (true);
			while (true) {
				ContainerSqlExpression b = ParserContainerSqlExpression (TokenID.Equal, TokenID.Less, TokenID.LessEqual, TokenID.Greater, TokenID.GreaterEqual, TokenID.NotEqual, TokenID.Comma, TokenID.RParen);
				if (b != null) {
					List<SqlExpression> c = b.Expressions;
					switch (CurrentExpression.Token) {
					case TokenID.Equal:
					case TokenID.Less:
					case TokenID.LessEqual:
					case TokenID.Greater:
					case TokenID.GreaterEqual:
					case TokenID.NotEqual:
						c.Add (new SqlExpression (CurrentExpression));
						NextExpression (true);
						b = ParserContainerSqlExpression (TokenID.Comma, TokenID.RParen);
						if (b != null)
							c.AddRange (b.Expressions);
						break;
					}
					a.Parameters.Add (c);
					if (CurrentExpression.Token == TokenID.RParen) {
						NextExpression (false);
						break;
					}
					else if (CurrentExpression.Token == TokenID.Comma) {
						NextExpression (false);
					}
				}
				else
					Error ("Invalid expression on context.");
			}
			return a;
		}
		private ContainerSqlExpression ParserContainerSqlExpression (params TokenID[] a)
		{
			ContainerSqlExpression b = new ContainerSqlExpression (CurrentExpression);
			Stack<ContainerSqlExpression> c = new Stack<ContainerSqlExpression> ();
			bool d = false;
			while (true) {
				switch (CurrentExpression.Token) {
				case TokenID.Identifier:
					if (d) {
						if (b.Expressions.Count > 0)
							return b;
						else
							return null;
					}
					d = true;
					switch (GetNextExpression (1).Token) {
					case TokenID.LParen:
						NextExpression (true);
						b.Expressions.Add (ParserFunction ());
						continue;
					case TokenID.Dot:
						b.Expressions.Add (ParserNamespace ());
						continue;
					default:
						b.Expressions.Add (new SqlExpression (CurrentExpression, SqlExpressionType.Column));
						break;
					}
					break;
				case TokenID.LParen:
					c.Push (b);
					b = new ContainerSqlExpression (CurrentExpression);
					c.Peek ().Expressions.Add (b);
					break;
				case TokenID.RParen:
					d = true;
					if (c.Count == 0) {
						if (ArrayHelper.Exists<TokenID> (a, delegate (TokenID e) {
							return e == CurrentExpression.Token;
						})) {
							if (b.Expressions.Count > 0)
								return b;
							else
								return null;
						}
					}
					else {
						b = c.Pop ();
					}
					break;
				case TokenID.StringLiteral:
				case TokenID.IntLiteral:
				case TokenID.DecimalLiteral:
				case TokenID.RealLiteral:
					if (d) {
						if (b.Expressions.Count > 0)
							return b;
						else
							return null;
					}
					d = true;
					b.Expressions.Add (new SqlExpression (CurrentExpression));
					break;
				case TokenID.Plus:
				case TokenID.Minus:
				case TokenID.Star:
				case TokenID.Slash:
				case TokenID.Equal:
				case TokenID.EqualEqual:
					if (d)
						b.Expressions.Add (new SqlExpression (CurrentExpression));
					else if (!d && CurrentExpression.Token == TokenID.Star)
						b.Expressions.Add (new SqlExpression (CurrentExpression, SqlExpressionType.Column));
					else if (b.Expressions.Count > 0)
						return b;
					else
						return null;
					d = false;
					break;
				case TokenID.kSelect:
					if (GetPreviousExpression (1).Token == TokenID.LParen) {
						b.Expressions.Add (ParserSelect ());
						d = true;
						continue;
					}
					else if (b.Expressions.Count > 0)
						return b;
					else
						return null;
				case TokenID.End:
					if (b.Expressions.Count > 0)
						return b;
					else
						return null;
				default:
					if (CurrentExpression.Token != TokenID.kFrom && CurrentExpression.Token.ToString () [0] == 'k') {
						switch (CurrentExpression.Token) {
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
							if (GetNextExpression (1).Token == TokenID.LParen) {
								NextExpression (true);
								b.Expressions.Add (ParserFunction ());
								d = true;
								continue;
							}
							break;
						}
					}
					if (CurrentExpression.Token == TokenID.Semi || ArrayHelper.Exists<TokenID> (a, delegate (TokenID e) {
						return e == CurrentExpression.Token;
					}) && c.Count == 0 || CurrentExpression.Token.ToString () [0] == 'k') {
						if (b.Expressions.Count > 0)
							return b;
						else
							return null;
					}
					else {
						if (c.Count > 0)
							Error ("Parent not closed.");
						else
							Error ("Invalid expression.");
					}
					break;
				}
				NextExpression (false);
			}
		}
		private SqlExpression ParserNamespace ()
		{
			List<Expression> a = new List<Expression> ();
			a.Add (CurrentExpression);
			bool b = true;
			while (true) {
				if (!b && (GetNextExpression (1).Token == TokenID.Identifier || (a.Count > 0 && GetNextExpression (1).Token == TokenID.Star))) {
					b = true;
					NextExpression (true);
					a.Add (CurrentExpression);
				}
				else if (b && GetNextExpression (1).Token == TokenID.Dot) {
					b = false;
					NextExpression (true);
					a.Add (CurrentExpression);
				}
				else {
					NextExpression (false);
					int c = a [0].BeginPoint;
					int d = a [0].Length;
					if (a.Count > 1) {
						d = (a [a.Count - 1].BeginPoint - c) + a [a.Count - 1].Length;
					}
					StringBuilder e = new StringBuilder ();
					if (a [0].CurrentSpecialContainer != null)
						e.Append (a [0].CurrentSpecialContainer.BeginCharSpecialContainer).Append (a [0].Text).Append (a [0].CurrentSpecialContainer.EndCharSpecialContainer);
					else
						e.Append (a [0].Text);
					for (int f = 1; f < a.Count; f++) {
						e.Append (' ', a [f].BeginPoint + (a [f].CurrentSpecialContainer != null ? -1 : 0) - ((a [f - 1].BeginPoint + a [f - 1].Length) + (a [f - 1].CurrentSpecialContainer != null ? 1 : 0)));
						if (a [f].CurrentSpecialContainer != null) {
							e.Append (a [f].CurrentSpecialContainer.BeginCharSpecialContainer).Append (a [f].Text).Append (a [f].CurrentSpecialContainer.EndCharSpecialContainer);
						}
						else {
							e.Append (a [f].Text);
						}
					}
					Expression g = new Expression (TokenID.Identifier);
					g.BeginPoint = c;
					g.Length = d;
					g.Text = e.ToString ();
					g.Line = a [0].Line;
					return new SqlExpression (g, SqlExpressionType.Column);
				}
			}
		}
		private FromPart ParserFromPart ()
		{
			FromPart a = new FromPart ();
			NextExpression (true);
			while (true) {
				switch (CurrentExpression.Token) {
				case TokenID.Identifier:
				case TokenID.LParen:
				case TokenID.kLeft:
				case TokenID.kRight:
				case TokenID.kFull:
				case TokenID.kInner:
				case TokenID.kCross:
				case TokenID.kNatural:
					a.TableExpressions.Add (ParserTableExpression ());
					break;
				default:
					return a;
				}
			}
		}
		private TableExpression ParserTableExpression ()
		{
			TableExpression a = new TableExpression ();
			while (true) {
				switch (CurrentExpression.Token) {
				case TokenID.Identifier:
					if (a.TableName == null) {
						if (GetNextExpression (1).Token == TokenID.Dot) {
							a.TableName = new TableNameExpression (ParserNamespace ().Value);
							continue;
						}
						else
							a.TableName = new TableNameExpression (CurrentExpression);
					}
					else
						a.TableAlias = CurrentExpression;
					break;
				case TokenID.kAs:
					a.AsExpression = CurrentExpression;
					NextExpression (true);
					a.TableAlias = CurrentExpression;
					break;
				case TokenID.LParen:
					NextExpression (true);
					if (CurrentExpression.Token == TokenID.kSelect) {
						a.SelectInfo = ParserSelect ();
						if (CurrentExpression.Token == TokenID.RParen) {
							NextExpression (false);
						}
						else
							Error ("Expected character )");
						continue;
					}
					break;
				case TokenID.kLeft:
				case TokenID.kRight:
				case TokenID.kFull:
				case TokenID.kInner:
				case TokenID.kCross:
				case TokenID.kNatural:
					if (a.TableName != null)
						return a;
					if (CurrentExpression.Token == TokenID.kLeft || CurrentExpression.Token == TokenID.kRight || CurrentExpression.Token == TokenID.kFull) {
						a.LeftOrRight = CurrentExpression;
						NextExpression (true);
					}
					switch (CurrentExpression.Token) {
					case TokenID.kOuter:
					case TokenID.kInner:
					case TokenID.kCross:
					case TokenID.kNatural:
						a.OuterOrInnerOrCrossOrNatural = CurrentExpression;
						NextExpression (true);
						break;
					default:
						break;
					}
					if (CurrentExpression.Token != TokenID.kJoin) {
						Error ("Expected Join expression");
					}
					a.Join = CurrentExpression;
					NextExpression (true);
					if (CurrentExpression.Token == TokenID.LParen) {
						NextExpression (true);
						if (CurrentExpression.Token == TokenID.kSelect) {
							a.SelectInfo = ParserSelect ();
						}
						else
							Error ("Expected select command.");
					}
					else if (CurrentExpression.Token == TokenID.Identifier) {
						a.TableName = new TableNameExpression (CurrentExpression);
						NextExpression (false);
					}
					else
						Error ("Expected tablename or viewname.");
					if (CurrentExpression.Token == TokenID.kAs) {
						a.AsExpression = CurrentExpression;
						if (!NextExpression (false) && CurrentExpression.Token != TokenID.Identifier)
							Error ("Expected alias of table name.");
						a.TableAlias = CurrentExpression;
						NextExpression (false);
					}
					else if (CurrentExpression.Token == TokenID.Identifier) {
						a.TableAlias = CurrentExpression;
						NextExpression (false);
					}
					if (CurrentExpression.Token == TokenID.kOn) {
						NextExpression (true);
						if (CurrentExpression.Token == TokenID.LParen) {
							a.OnExpressions = ParserContainerConditional (true);
							if (CurrentExpression.Token == TokenID.RParen) {
								NextExpression (false);
							}
							else
								Error ("Expected character ')'.");
						}
						else
							a.OnExpressions = ParserContainerConditional ();
					}
					return a;
				default:
					return a;
				}
				NextExpression (true);
			}
		}
		private ContainerSqlExpression ParserContainerConditional ()
		{
			return ParserContainerConditional (false);
		}
		private ContainerSqlExpression ParserContainerConditional (bool a)
		{
			ContainerSqlExpression b = new ContainerSqlExpression (CurrentExpression);
			if (a || CurrentExpression.Token == TokenID.LParen)
				NextExpression (true);
			Stack<ContainerSqlExpression> c = new Stack<ContainerSqlExpression> ();
			bool d = false;
			while (true) {
				switch (CurrentExpression.Token) {
				case TokenID.Identifier:
				case TokenID.StringLiteral:
				case TokenID.IntLiteral:
					if (d) {
						if (b.Expressions.Count > 0)
							return b;
						else
							return null;
					}
					d = true;
					b.Expressions.Add (ParserContainerSqlExpression (TokenID.Equal, TokenID.EqualEqual, TokenID.Less, TokenID.LessEqual, TokenID.Greater, TokenID.GreaterEqual, TokenID.NotEqual, TokenID.kLike, TokenID.kIs, TokenID.kNot, TokenID.kIn, TokenID.kBetween, TokenID.kAnd, TokenID.kOr, TokenID.RParen));
					VERIFYOPERATOR:
					switch (CurrentExpression.Token) {
					case TokenID.Equal:
					case TokenID.Less:
					case TokenID.LessEqual:
					case TokenID.Greater:
					case TokenID.GreaterEqual:
					case TokenID.NotEqual:
						b.Expressions.Add (new SqlExpression (CurrentExpression));
						NextExpression (true);
						if (CurrentExpression.Token == TokenID.kAny || CurrentExpression.Token == TokenID.kSome || CurrentExpression.Token == TokenID.kAll) {
							b.Expressions.Add (new SqlExpression (CurrentExpression));
							NextExpression (true);
							if (CurrentExpression.Token == TokenID.LParen) {
								ContainerSqlExpression e = new ContainerSqlExpression (CurrentExpression);
								NextExpression (true);
								e.Expressions.Add (ParserSelect ());
								if (CurrentExpression.Token == TokenID.RParen) {
									NextExpression (false);
									b.Expressions.Add (e);
									return b;
								}
								else
									Error ("Expected character '('.");
							}
							else
								Error ("Expected character ')'.");
						}
						else {
							b.Expressions.Add (ParserContainerConditional ());
						}
						continue;
					case TokenID.kLike:
						b.Expressions.Add (new SqlExpression (CurrentExpression));
						NextExpression (true);
						b.Expressions.Add (ParserContainerSqlExpression (TokenID.RParen, TokenID.kAnd, TokenID.kOr));
						continue;
					case TokenID.kBetween:
						b.Expressions.Add (new SqlExpression (CurrentExpression));
						NextExpression (true);
						b.Expressions.Add (ParserContainerSqlExpression (TokenID.kAnd));
						if (CurrentExpression.Token == TokenID.kAnd) {
							b.Expressions.Add (new SqlExpression (CurrentExpression));
							NextExpression (true);
							b.Expressions.Add (ParserContainerSqlExpression (TokenID.RParen, TokenID.kAnd, TokenID.kOr));
						}
						else
							Error ("Expected AND expression.");
						continue;
					case TokenID.kIs:
						b.Expressions.Add (new SqlExpression (CurrentExpression));
						NextExpression (true);
						if (CurrentExpression.Token == TokenID.kNot) {
							b.Expressions.Add (new SqlExpression (CurrentExpression));
							NextExpression (true);
						}
						if (CurrentExpression.Token == TokenID.kNull) {
							SqlExpression f = b.Expressions [b.Expressions.Count - 1];
							int g = f.Value.BeginPoint;
							int h = (CurrentExpression.BeginPoint + CurrentExpression.Length) - g;
							StringBuilder i = new StringBuilder ();
							i.Append (f.Value.Text);
							i.Append (' ', CurrentExpression.BeginPoint - (f.Value.BeginPoint + f.Value.Length));
							i.Append (CurrentExpression.Text);
							Expression j = new Expression (TokenID.kIsNull);
							j.BeginPoint = g;
							j.Length = h;
							j.Text = i.ToString ();
							j.Line = f.Value.Line;
							b.Expressions.RemoveAt (b.Expressions.Count - 1);
							b.Expressions.Add (new SqlExpression (j));
							NextExpression (false);
						}
						else {
							Error ("Esperado NULL");
						}
						continue;
					case TokenID.kNot:
						b.Expressions.Add (new SqlExpression (CurrentExpression));
						NextExpression (false);
						if (CurrentExpression.Token == TokenID.kIn) {
							b.Expressions.Add (new SqlExpression (CurrentExpression));
							NextExpression (false);
						}
						continue;
					case TokenID.kIn:
					case TokenID.kAll:
					case TokenID.kAny:
					case TokenID.kSome:
						b.Expressions.Add (new SqlExpression (CurrentExpression));
						NextExpression (true);
						if (CurrentExpression.Token == TokenID.LParen) {
							ContainerSqlExpression e = new ContainerSqlExpression (CurrentExpression);
							NextExpression (true);
							if (CurrentExpression.Token != TokenID.kSelect) {
								Select k = new Select (new Expression (0, 0, CurrentExpression.Line));
								k.Value.Text = "";
								k.SelectPart = ParserSelectPart ();
								e.Expressions.Add (k);
								if (CurrentExpression.Token == TokenID.RParen || GetPreviousExpression (1).Token == TokenID.RParen) {
									NextExpression (false);
									b.Expressions.Add (e);
									return b;
								}
								else
									Error ("Expected character ')'.");
							}
							else {
								e.Expressions.Add (ParserSelect ());
								if (CurrentExpression.Token == TokenID.RParen || (CurrentExpression.Token == TokenID.End && GetPreviousExpression (1).Token == TokenID.RParen)) {
									NextExpression (false);
									b.Expressions.Add (e);
									return b;
								}
								else if (CurrentExpression.Token != TokenID.RParen && GetPreviousExpression (1).Token == TokenID.RParen) {
									b.Expressions.Add (e);
								}
								else
									Error ("Expected character ')'.");
							}
						}
						else
							Error ("Expected '('.");
						continue;
					case TokenID.RParen:
						return b;
					case TokenID.kAnd:
					case TokenID.kOr:
						continue;
					default:
						continue;
					}
					break;
				case TokenID.LParen:
					d = true;
					b.Expressions.Add (ParserContainerConditional (true));
					if (GetPreviousExpression (1).Token == TokenID.RParen)
						continue;
					else if (CurrentExpression.Token == TokenID.RParen)
						continue;
					else
						Error ("Expected character ')'.");
					return b;
				case TokenID.RParen:
					if (b.ContainerToken == TokenID.LParen)
						NextExpression (false);
					return b;
				case TokenID.kSelect:
					if (GetPreviousExpression (1).Token == TokenID.LParen) {
						b.Expressions.Add (ParserSelect ());
						continue;
					}
					else
						return b;
				case TokenID.kExists:
					b.Expressions.Add (new SqlExpression (CurrentExpression) {
						Type = SqlExpressionType.Function
					});
					NextExpression (true);
					if (CurrentExpression.Token == TokenID.LParen) {
						ContainerSqlExpression e = new ContainerSqlExpression (CurrentExpression);
						NextExpression (true);
						if (CurrentExpression.Token == TokenID.Identifier) {
							e.Expressions.Add (new SelectVariable (CurrentExpression));
							NextExpression (true);
						}
						else
							e.Expressions.Add (ParserSelect ());
						if (CurrentExpression.Token == TokenID.RParen) {
							NextExpression (false);
							b.Expressions.Add (e);
							return b;
						}
						else
							Error ("Expected character '('.");
					}
					else
						Error ("Expected character ')'.");
					continue;
				case TokenID.kAnd:
				case TokenID.kOr:
					d = false;
					b.Expressions.Add (new SqlExpression (CurrentExpression));
					b.Expressions.Add (ParserContainerConditional (true));
					continue;
				case TokenID.End:
					if (b.Expressions.Count > 0)
						return b;
					else
						return null;
				default:
					if (CurrentExpression.Token.ToString () [0] == 'k') {
						switch (CurrentExpression.Token) {
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
							Error ("Invalid keyword " + CurrentExpression.Token.ToString ().Substring (1));
							break;
						default:
							if (GetNextExpression (1).Token == TokenID.LParen) {
								NextExpression (true);
								b.Expressions.Add (ParserFunction ());
								d = true;
								goto VERIFYOPERATOR;
								continue;
							}
							break;
						}
					}
					return b;
				}
			}
		}
		private WherePart ParserWherePart ()
		{
			WherePart a = new WherePart ();
			a.Where = CurrentExpression;
			NextExpression (true);
			ContainerSqlExpression b = ParserContainerConditional ();
			a.Expressions = b.Expressions;
			if (b.ContainerToken == TokenID.LParen) {
				NextExpression (false);
			}
			return a;
		}
		private GroupByPart ParserGroupByPart ()
		{
			return ParserGroupByPart (false);
		}
		private GroupByPart ParserGroupByPart (bool a)
		{
			GroupByPart b = new GroupByPart ();
			if (!a) {
				b.Group = GetPreviousExpression (1);
				b.By = CurrentExpression;
				NextExpression (true);
			}
			while (true) {
				ContainerSqlExpression c = ParserContainerSqlExpression (TokenID.Comma);
				if (c != null) {
					b.Expressions.Add (c);
					if (CurrentExpression.Token == TokenID.Comma)
						NextExpression (true);
					else
						break;
				}
				else
					break;
			}
			return b;
		}
		public OrderByPart ParserOrderByPart ()
		{
			return ParserOrderByPart (false);
		}
		public OrderByPart ParserOrderByPart (bool a)
		{
			OrderByPart b = new OrderByPart ();
			if (!a)
				NextExpression (true);
			while (true) {
				ContainerSqlExpression c = ParserContainerSqlExpression (TokenID.Comma, TokenID.kAsc, TokenID.kDesc, TokenID.kNulls, TokenID.RParen, TokenID.Semi);
				if (c != null) {
					OrderByExpression d = new OrderByExpression ();
					d.Expression = c;
					if (CurrentExpression.Token == TokenID.kAsc || CurrentExpression.Token == TokenID.kDesc) {
						d.AscOrDesc = CurrentExpression;
						d.Asc = (CurrentExpression.Token == TokenID.kAsc);
						NextExpression (false);
					}
					if (CurrentExpression.Token == TokenID.kNulls) {
						d.Nulls = true;
						NextExpression (false);
						if (CurrentExpression.Token == TokenID.kFirst || CurrentExpression.Token == TokenID.kLast) {
							d.First = (CurrentExpression.Token == TokenID.kFirst);
							NextExpression (false);
						}
					}
					b.OrderByExpressions.Add (d);
					if (CurrentExpression.Token == TokenID.Comma)
						NextExpression (true);
					else
						break;
				}
				else
					break;
			}
			return b;
		}
		private HavingPart ParserHavingPart ()
		{
			HavingPart a = new HavingPart ();
			NextExpression (true);
			while (true) {
				ContainerSqlExpression b = ParserContainerConditional ();
				if (b != null) {
					a.Expressions.Add (b);
					if (CurrentExpression.Token == TokenID.Comma)
						NextExpression (true);
					else
						break;
				}
				else
					break;
			}
			return a;
		}
	}
}
