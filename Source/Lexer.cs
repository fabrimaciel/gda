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

namespace GDA.Sql.InterpreterExpression
{
	public class Lexer
	{
		public enum State
		{
			S0,
			S1,
			S2,
			S3,
			S4,
			S5,
			S6,
			S7,
			S8,
			End
		}

		private static SortedList<string, TokenID> keywords = new SortedList<string, TokenID>();

		/// <summary>
		/// Caracteres separadores.
		/// </summary>
		private static char[] tabsChar =  {
			'+',
			'-',
			'*',
			'/',
			'<',
			'>',
			'=',
			'!',
			';',
			':',
			',',
			'&',
			'|',
			'%',
			'.'
		};

		/// <summary>
		/// Caracteres que podem se conectar com caracteres separadores.
		/// </summary>
		private static char[] joinTabsChar =  {
			'+',
			'-',
			'=',
			'>',
			'|',
			'&',
			'/',
			'*'
		};

		/// <summary>
		/// Containers iniciais.
		/// </summary>
		private static char[] beginContainers =  {
			'(',
			'{'
		};

		/// <summary>
		/// Containers finais.
		/// </summary>
		private static char[] endContainers =  {
			')',
			'}'
		};

		private static char[] spacesChar =  {
			' ',
			'\n',
			'\t',
			'\r'
		};

		/// <summary>
		/// Caracters que define containers de expressões string
		/// </summary>
		private static char[] beginStringContainers =  {
			'\"',
			'\'',
			'`',
			'#',
			'['
		};

		private static char[] endStringContainers =  {
			'\"',
			'\'',
			'`',
			'#',
			']'
		};

		/// <summary>
		/// Linhas do documento.
		/// </summary>
		private List<ExpressionLine> _lines;

		/// <summary>
		/// Comando a ser interpretado.
		/// </summary>
		private string _command;

		/// <summary>
		/// Lista das expressões interpretadas.
		/// </summary>
		private List<Expression> _expressions;

		/// <summary>
		/// Lista da expressões interpretadas.
		/// </summary>
		internal List<Expression> Expressions
		{
			get
			{
				if(_expressions == null)
					return Lex();
				else
					return _expressions;
			}
		}

		/// <summary>
		/// Recupera as posições do identificaro informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public int[] FindIdentifierPositions(string identifier)
		{
			List<int> positions = new List<int>();
			foreach (Expression e in Expressions)
				if(e.Text == identifier && e.Token != TokenID.StringLiteral)
					positions.Add(e.BeginPoint);
			return positions.ToArray();
		}

		/// <summary>
		/// Comando a ser interpretado.
		/// </summary>
		public string Command
		{
			get
			{
				return _command;
			}
			set
			{
				_command = value;
			}
		}

		static Lexer()
		{
			if(keywords.Count == 0)
			{
				keywords.Add("absolute", TokenID.kAbsolute);
				keywords.Add("action", TokenID.kAction);
				keywords.Add("add", TokenID.kAdd);
				keywords.Add("all", TokenID.kAll);
				keywords.Add("allocate", TokenID.kAllocate);
				keywords.Add("alter", TokenID.kAlter);
				keywords.Add("and", TokenID.kAnd);
				keywords.Add("any", TokenID.kAny);
				keywords.Add("are", TokenID.kAre);
				keywords.Add("as", TokenID.kAs);
				keywords.Add("asc", TokenID.kAsc);
				keywords.Add("assertion", TokenID.kAssertion);
				keywords.Add("at", TokenID.kAt);
				keywords.Add("authorization", TokenID.kAuthorization);
				keywords.Add("avg", TokenID.kAvg);
				keywords.Add("begin", TokenID.kBegin);
				keywords.Add("between", TokenID.kBetween);
				keywords.Add("bit", TokenID.kBit);
				keywords.Add("bit_length", TokenID.kBit_Length);
				keywords.Add("both", TokenID.kBoth);
				keywords.Add("by", TokenID.kBy);
				keywords.Add("cascade", TokenID.kCascade);
				keywords.Add("cascaded", TokenID.kCascaded);
				keywords.Add("case", TokenID.kCase);
				keywords.Add("cast", TokenID.kCast);
				keywords.Add("catalog", TokenID.kCatalog);
				keywords.Add("char", TokenID.kChar);
				keywords.Add("char_length", TokenID.kChar_Length);
				keywords.Add("character", TokenID.kCharacter);
				keywords.Add("character_length", TokenID.kCharacter_Length);
				keywords.Add("check", TokenID.kCheck);
				keywords.Add("close", TokenID.kClose);
				keywords.Add("coalesce", TokenID.kCoalesce);
				keywords.Add("collate", TokenID.kCollate);
				keywords.Add("collation", TokenID.kCollation);
				keywords.Add("column", TokenID.kColumn);
				keywords.Add("commit", TokenID.kCommit);
				keywords.Add("connect", TokenID.kConnect);
				keywords.Add("connection", TokenID.kConnection);
				keywords.Add("constraint", TokenID.kConstraint);
				keywords.Add("constraints", TokenID.kConstraints);
				keywords.Add("continue", TokenID.kContinue);
				keywords.Add("convert", TokenID.kConvert);
				keywords.Add("corresponding", TokenID.kCorresponding);
				keywords.Add("count", TokenID.kCount);
				keywords.Add("create", TokenID.kCreate);
				keywords.Add("cross", TokenID.kCross);
				keywords.Add("current", TokenID.kCurrent);
				keywords.Add("current_date", TokenID.kCurrent_Date);
				keywords.Add("current_time", TokenID.kCurrent_Time);
				keywords.Add("current_timestamp", TokenID.kCurrent_Timestamp);
				keywords.Add("current_user", TokenID.kCurrent_User);
				keywords.Add("cursor", TokenID.kCursor);
				keywords.Add("date", TokenID.kDate);
				keywords.Add("day", TokenID.kDay);
				keywords.Add("deallocate", TokenID.kDeallocate);
				keywords.Add("dec", TokenID.kDec);
				keywords.Add("decimal", TokenID.kDecimal);
				keywords.Add("declare", TokenID.kDeclare);
				keywords.Add("default", TokenID.kDefault);
				keywords.Add("deferrable", TokenID.kDeferrable);
				keywords.Add("deferred", TokenID.kDeferred);
				keywords.Add("delete", TokenID.kDelete);
				keywords.Add("desc", TokenID.kDesc);
				keywords.Add("describe", TokenID.kDescribe);
				keywords.Add("descriptor", TokenID.kDescriptor);
				keywords.Add("diagnostics", TokenID.kDiagnostics);
				keywords.Add("disconnect", TokenID.kDisconnect);
				keywords.Add("distinct", TokenID.kDistinct);
				keywords.Add("double", TokenID.kDouble);
				keywords.Add("drop", TokenID.kDrop);
				keywords.Add("else", TokenID.kElse);
				keywords.Add("end", TokenID.kEnd);
				keywords.Add("end_exec", TokenID.kEnd_Exec);
				keywords.Add("escape", TokenID.kEscape);
				keywords.Add("except", TokenID.kExcept);
				keywords.Add("exception", TokenID.kException);
				keywords.Add("exec", TokenID.kExec);
				keywords.Add("execute", TokenID.kExecute);
				keywords.Add("exists", TokenID.kExists);
				keywords.Add("external", TokenID.kExternal);
				keywords.Add("extract", TokenID.kExtract);
				keywords.Add("false", TokenID.kFalse);
				keywords.Add("fetch", TokenID.kFetch);
				keywords.Add("first", TokenID.kFirst);
				keywords.Add("float", TokenID.kFloat);
				keywords.Add("for", TokenID.kFor);
				keywords.Add("foreign", TokenID.kForeign);
				keywords.Add("fortran", TokenID.kFortran);
				keywords.Add("found", TokenID.kFound);
				keywords.Add("from", TokenID.kFrom);
				keywords.Add("full", TokenID.kFull);
				keywords.Add("get", TokenID.kGet);
				keywords.Add("global", TokenID.kGlobal);
				keywords.Add("go", TokenID.kGo);
				keywords.Add("goto", TokenID.kGoto);
				keywords.Add("grant", TokenID.kGrant);
				keywords.Add("group", TokenID.kGroup);
				keywords.Add("having", TokenID.kHaving);
				keywords.Add("hour", TokenID.kHour);
				keywords.Add("identity", TokenID.kIdentity);
				keywords.Add("immediate", TokenID.kImmediate);
				keywords.Add("in", TokenID.kIn);
				keywords.Add("include", TokenID.kInclude);
				keywords.Add("index", TokenID.kIndex);
				keywords.Add("indicator", TokenID.kIndicator);
				keywords.Add("initially", TokenID.kInitially);
				keywords.Add("inner", TokenID.kInner);
				keywords.Add("input", TokenID.kInput);
				keywords.Add("insensitive", TokenID.kInsensitive);
				keywords.Add("insert", TokenID.kInsert);
				keywords.Add("int", TokenID.kInt);
				keywords.Add("integer", TokenID.kInteger);
				keywords.Add("intersect", TokenID.kIntersect);
				keywords.Add("interval", TokenID.kInterval);
				keywords.Add("into", TokenID.kInto);
				keywords.Add("is", TokenID.kIs);
				keywords.Add("isolation", TokenID.kIsolation);
				keywords.Add("join", TokenID.kJoin);
				keywords.Add("key", TokenID.kKey);
				keywords.Add("language", TokenID.kLanguage);
				keywords.Add("last", TokenID.kLast);
				keywords.Add("leading", TokenID.kLeading);
				keywords.Add("left", TokenID.kLeft);
				keywords.Add("level", TokenID.kLevel);
				keywords.Add("like", TokenID.kLike);
				keywords.Add("local", TokenID.kLocal);
				keywords.Add("lower", TokenID.kLower);
				keywords.Add("match", TokenID.kMatch);
				keywords.Add("max", TokenID.kMax);
				keywords.Add("min", TokenID.kMin);
				keywords.Add("minute", TokenID.kMinute);
				keywords.Add("module", TokenID.kModule);
				keywords.Add("month", TokenID.kMonth);
				keywords.Add("names", TokenID.kNames);
				keywords.Add("national", TokenID.kNational);
				keywords.Add("natural", TokenID.kNatural);
				keywords.Add("nchar", TokenID.kNChar);
				keywords.Add("next", TokenID.kNext);
				keywords.Add("no", TokenID.kNo);
				keywords.Add("none", TokenID.kNone);
				keywords.Add("not", TokenID.kNot);
				keywords.Add("null", TokenID.kNull);
				keywords.Add("nullif", TokenID.kNullIf);
				keywords.Add("nulls", TokenID.kNulls);
				keywords.Add("numeric", TokenID.kNumeric);
				keywords.Add("octect_length", TokenID.kOctect_Length);
				keywords.Add("of", TokenID.kOf);
				keywords.Add("on", TokenID.kOn);
				keywords.Add("only", TokenID.kOnly);
				keywords.Add("open", TokenID.kOpen);
				keywords.Add("option", TokenID.kOption);
				keywords.Add("or", TokenID.kOr);
				keywords.Add("order", TokenID.kOrder);
				keywords.Add("outer", TokenID.kOuter);
				keywords.Add("output", TokenID.kOutput);
				keywords.Add("overlaps", TokenID.kOverlaps);
				keywords.Add("pad", TokenID.kPad);
				keywords.Add("partial", TokenID.kPartial);
				keywords.Add("pascal", TokenID.kPascal);
				keywords.Add("percent", TokenID.kPercent);
				keywords.Add("position", TokenID.kPosition);
				keywords.Add("precision", TokenID.kPrecision);
				keywords.Add("prepare", TokenID.kPrepare);
				keywords.Add("preserve", TokenID.kPreserve);
				keywords.Add("primary", TokenID.kPrimary);
				keywords.Add("prior", TokenID.kPrior);
				keywords.Add("privileges", TokenID.kPrivileges);
				keywords.Add("procedure", TokenID.kProcedure);
				keywords.Add("public", TokenID.kPublic);
				keywords.Add("read", TokenID.kRead);
				keywords.Add("real", TokenID.kReal);
				keywords.Add("references", TokenID.kReferences);
				keywords.Add("relative", TokenID.kRelative);
				keywords.Add("restrict", TokenID.kRestrict);
				keywords.Add("revoke", TokenID.kRevoke);
				keywords.Add("right", TokenID.kRight);
				keywords.Add("rollback", TokenID.kRollback);
				keywords.Add("rows", TokenID.kRows);
				keywords.Add("schema", TokenID.kSchema);
				keywords.Add("scroll", TokenID.kScroll);
				keywords.Add("second", TokenID.kSecond);
				keywords.Add("select", TokenID.kSelect);
				keywords.Add("set", TokenID.kSet);
				keywords.Add("smallint", TokenID.kSmallint);
				keywords.Add("some", TokenID.kSome);
				keywords.Add("space", TokenID.kSpace);
				keywords.Add("sql", TokenID.kSql);
				keywords.Add("sqlca", TokenID.kSqlCa);
				keywords.Add("sqlcode", TokenID.kSqlCode);
				keywords.Add("sqlerror", TokenID.kSqlError);
				keywords.Add("sqlstate", TokenID.kSqlState);
				keywords.Add("sqlwarning", TokenID.kSqlWarning);
				keywords.Add("substring", TokenID.kSubstring);
				keywords.Add("sum", TokenID.kSum);
				keywords.Add("system_user", TokenID.kSystem_User);
				keywords.Add("table", TokenID.kTable);
				keywords.Add("temporary", TokenID.kTemporary);
				keywords.Add("then", TokenID.kThen);
				keywords.Add("time", TokenID.kTime);
				keywords.Add("timestamp", TokenID.kTimestamp);
				keywords.Add("timezone_hour", TokenID.kTimezone_Hour);
				keywords.Add("timezone_minute", TokenID.kTimezone_Minute);
				keywords.Add("to", TokenID.kTo);
				keywords.Add("top", TokenID.kTop);
				keywords.Add("trailing", TokenID.kTrailing);
				keywords.Add("transaction", TokenID.kTransaction);
				keywords.Add("translate", TokenID.kTranslate);
				keywords.Add("translation", TokenID.kTranslation);
				keywords.Add("trim", TokenID.kTrim);
				keywords.Add("true", TokenID.kTrue);
				keywords.Add("union", TokenID.kUnion);
				keywords.Add("unique", TokenID.kUnique);
				keywords.Add("unknown", TokenID.kUnknown);
				keywords.Add("update", TokenID.kUpdate);
				keywords.Add("upper", TokenID.kUpper);
				keywords.Add("usage", TokenID.kUsage);
				keywords.Add("using", TokenID.kUsing);
				keywords.Add("value", TokenID.kValue);
				keywords.Add("values", TokenID.kValues);
				keywords.Add("varchar", TokenID.kVarchar);
				keywords.Add("varying", TokenID.kVarying);
				keywords.Add("view", TokenID.kView);
				keywords.Add("when", TokenID.kWhen);
				keywords.Add("whenever", TokenID.kWhenever);
				keywords.Add("where", TokenID.kWhere);
				keywords.Add("with", TokenID.kWith);
				keywords.Add("work", TokenID.kWork);
				keywords.Add("write", TokenID.kWrite);
				keywords.Add("year", TokenID.kYear);
				keywords.Add("zone", TokenID.kZone);
			}
		}

		public Lexer(string command)
		{
			_command = command;
		}

		/// <summary>
		/// Verifica se o caracter passado é alfanumerico.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private static bool isAlphanumeric(char c)
		{
			return ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_' || c == '@');
		}

		/// <summary>
		/// Verifica se o caracter passado está contido no alfabeto.
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private static bool isAlpha(char c)
		{
			return ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '@' || c == '?');
		}

		/// <summary>
		/// Verifica se o caracter pertence aos numeros decimais e hexadecimais
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		private static bool isNumeric(char c)
		{
			return ((c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'F') || (c >= '0' && c <= '9') || c == '.');
		}

		/// <summary>
		/// Executa a análise lexa no comando.
		/// </summary>
		/// <returns></returns>
		internal List<Expression> Lex()
		{
			_lines = new List<ExpressionLine>();
			_expressions = new List<Expression>();
			Stack<ContainerExpression> cExpressions = new Stack<ContainerExpression>();
			int beginPos = 0;
			int pos = 0;
			State state = State.S0;
			bool innerContainer = false;
			char usingSpecialContainerChar = '\"';
			ExpressionLine lastLine = new ExpressionLine(pos);
			_lines.Add(lastLine);
			bool end = false;
			while (!end || (pos == Command.Length && (state == State.S8 || state == State.S3 || state == State.S2)))
			{
				switch(state)
				{
				case State.S0:
					if(Array.IndexOf<char>(spacesChar, Command[pos]) != -1)
					{
						state = State.S0;
						if(Command[pos] == '\n')
						{
							lastLine.Length = pos - lastLine.BeginPoint;
							lastLine = new ExpressionLine(pos);
							_lines.Add(lastLine);
						}
						else
						{
						}
						beginPos = pos + 1;
					}
					else if(Array.IndexOf<char>(beginStringContainers, Command[pos]) != -1)
					{
						beginPos = pos + 1;
						usingSpecialContainerChar = endStringContainers[Array.IndexOf<char>(beginStringContainers, Command[pos])];
						state = State.S1;
					}
					else if(Array.IndexOf<char>(tabsChar, Command[pos]) != -1)
					{
						beginPos = pos;
						state = State.S2;
					}
					else if(isAlpha(Command[pos]))
						state = State.S3;
					else if(char.IsDigit(Command[pos]))
						state = State.S8;
					else if(Array.IndexOf<char>(beginContainers, Command[pos]) != -1)
					{
						Expression e = new Expression(pos, lastLine, Command[pos]);
						switch(Command[pos])
						{
						case '(':
							e.Token = TokenID.LParen;
							break;
						case '{':
							e.Token = TokenID.LCurly;
							break;
						}
						_expressions.Add(e);
						cExpressions.Push(new ContainerExpression(pos, e.Text[0], lastLine));
						beginPos = pos + 1;
						innerContainer = true;
						state = State.S0;
					}
					else if(Array.IndexOf<char>(endContainers, Command[pos]) != -1)
					{
						if(cExpressions.Count == 0)
						{
							throw new SqlLexerException(String.Format("Not open tag for caracter {0}. Line: {1} - Col: {2}.", Command[pos], _lines.Count, pos - lastLine.BeginPoint));
						}
						else if(cExpressions.Peek().EndContainerChar == Command[pos])
						{
							cExpressions.Pop();
							Expression e = new Expression(pos, lastLine, Command[pos]);
							switch(Command[pos])
							{
							case ')':
								e.Token = TokenID.RParen;
								break;
							case '}':
								e.Token = TokenID.RCurly;
								break;
							}
							_expressions.Add(e);
							if(cExpressions.Count == 0)
							{
								innerContainer = false;
							}
							beginPos = pos + 1;
							state = State.S0;
						}
						else
							throw new SqlLexerException(String.Format("Expected caracter {0}. Line: {1} - Col: {2}.", cExpressions.Peek().EndContainerChar, _lines.Count, pos - lastLine.BeginPoint));
					}
					else
					{
						throw new SqlLexerException(String.Format("Invalid caracter '{0}' in expression context.", Command[pos]));
					}
					break;
				case State.S1:
					if(Command[pos] == usingSpecialContainerChar)
					{
						if((pos > 0 && Command[pos - 1] != '\\') || (pos > 1 && Command[pos - 2] == '\\'))
						{
							if(usingSpecialContainerChar == ']' || usingSpecialContainerChar == '`')
							{
								Expression e = new Expression(beginPos, pos - beginPos, lastLine, Command, TokenID.Identifier);
								e.CurrentSpecialContainer = new SpecialContainer(Command[beginPos - 1], Command[pos]);
								_expressions.Add(e);
							}
							else
							{
								SpecialContainerExpression sce = new SpecialContainerExpression(beginPos, pos - beginPos, lastLine, Command, usingSpecialContainerChar);
								_expressions.Add(sce);
								switch(usingSpecialContainerChar)
								{
								case '"':
									sce.ContainerToken = TokenID.Quote;
									break;
								case '\'':
									sce.ContainerToken = TokenID.SQuote;
									break;
								}
							}
							beginPos = pos + 1;
							state = State.S0;
						}
					}
					break;
				case State.S2:
					if(Command[pos - 1] == '-' && char.IsDigit(Command[pos]))
					{
						if(Command.Length < 2 || (Command.Length > 2 && Array.IndexOf<char>(tabsChar, Command[pos - 2]) != -1) || (Command.Length > 2 && Array.IndexOf<char>(spacesChar, Command[pos - 2]) != -1))
						{
							state = State.S8;
							continue;
						}
					}
					string join;
					if(!end)
						join = Command[pos - 1].ToString() + Command[pos].ToString();
					else
						join = Command[pos - 1].ToString();
					switch(join)
					{
					case "&&":
					case "||":
					case "++":
					case "--":
					case "->":
					case "==":
					case "<>":
					case "!=":
					case "!<":
					case "!>":
					case ">=":
					case "<=":
					case "+=":
					case "/*":
					case "*/":
					case "//":
					case ":=":
						Expression e = new Expression(beginPos, 2, lastLine, Command);
						switch(e.Text)
						{
						case "&&":
							e.Token = TokenID.kAnd;
							break;
						case "||":
							e.Token = TokenID.kOr;
							break;
						case "++":
							e.Token = TokenID.PlusPlus;
							break;
						case "--":
							e.Token = TokenID.MinusMinus;
							break;
						case "->":
							e.Token = TokenID.MinusGreater;
							break;
						case ":=":
							e.Token = TokenID.Equal;
							break;
						case "==":
							e.Token = TokenID.EqualEqual;
							break;
						case "<>":
						case "!=":
							e.Token = TokenID.NotEqual;
							break;
						case ">=":
							e.Token = TokenID.GreaterEqual;
							break;
						case "<=":
							e.Token = TokenID.LessEqual;
							break;
						case "+=":
							e.Token = TokenID.PlusEqual;
							break;
						case "/*":
							e.Token = TokenID.BMultiComment;
							break;
						case "*/":
							e.Token = TokenID.EMultiComment;
							break;
						case "//":
							e.Token = TokenID.SingleComment;
							break;
						case "!>":
							e.Token = TokenID.Less;
							break;
						case "!<":
							e.Token = TokenID.Greater;
							break;
						default:
							e.Token = TokenID.InvalidTab;
							break;
						}
						_expressions.Add(e);
						beginPos = pos + 1;
						state = State.S0;
						break;
					default:
						TabExpression te = new TabExpression(beginPos, lastLine, Command);
						switch(Command[beginPos])
						{
						case '.':
							te.Token = TokenID.Dot;
							break;
						case '=':
							te.Token = TokenID.Equal;
							break;
						case '"':
							te.Token = TokenID.Hash;
							break;
						case '$':
							te.Token = TokenID.Dollar;
							break;
						case '%':
							te.Token = TokenID.Percent;
							break;
						case '&':
							te.Token = TokenID.BAnd;
							break;
						case '*':
							te.Token = TokenID.Star;
							break;
						case '+':
							te.Token = TokenID.Plus;
							break;
						case '-':
							te.Token = TokenID.Minus;
							break;
						case ',':
							te.Token = TokenID.Comma;
							break;
						case '/':
							te.Token = TokenID.Slash;
							break;
						case ':':
							te.Token = TokenID.Colon;
							break;
						case ';':
							te.Token = TokenID.Semi;
							break;
						case '<':
							te.Token = TokenID.Less;
							break;
						case '>':
							te.Token = TokenID.Greater;
							break;
						case '?':
							te.Token = TokenID.Question;
							break;
						case '!':
							te.Token = TokenID.kNot;
							break;
						default:
							te.Token = TokenID.InvalidTab;
							break;
						}
						_expressions.Add(te);
						beginPos = pos;
						pos--;
						state = State.S0;
						break;
					}
					break;
				case State.S3:
					if(pos == Command.Length || !isAlphanumeric(Command[pos]))
					{
						Expression e = new Expression(beginPos, pos - beginPos, lastLine, Command);
						if(char.IsDigit(e.Text[0]) || e.Text[0] == '-')
						{
							if(e.Text.Length > 2 && (e.Text[1] == 'x' || e.Text[1] == 'X') && System.Text.RegularExpressions.Regex.IsMatch(e.Text.Substring(2), "[0-9]"))
							{
								e.Token = TokenID.HexLiteral;
							}
							else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^([-]|[0-9])[0-9]*$"))
							{
								e.Token = TokenID.IntLiteral;
							}
							else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$"))
							{
								e.Token = TokenID.RealLiteral;
							}
						}
						else if(keywords.ContainsKey(e.Text.ToLower()))
						{
							e.Token = keywords[e.Text.ToLower()];
						}
						else
						{
							e.Token = TokenID.Identifier;
						}
						_expressions.Add(e);
						beginPos = pos;
						pos--;
						state = State.S0;
					}
					break;
				case State.S8:
					if(pos == Command.Length || !isNumeric(Command[pos]))
					{
						Expression e = new Expression(beginPos, pos - beginPos, lastLine, Command);
						if(e.Text.Length > 2 && (e.Text[1] == 'x' || e.Text[1] == 'X') && System.Text.RegularExpressions.Regex.IsMatch(e.Text.Substring(2), "[0-9]"))
						{
							e.Token = TokenID.HexLiteral;
						}
						else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]"))
						{
							e.Token = TokenID.IntLiteral;
						}
						else if(System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$"))
						{
							e.Token = TokenID.RealLiteral;
						}
						else
							throw new SqlLexerException("Expected number or hexadecimal.");
						_expressions.Add(e);
						beginPos = pos;
						pos--;
						state = State.S0;
					}
					break;
				}
				pos++;
				end = (pos == Command.Length);
			}
			lastLine.Length = pos - lastLine.BeginPoint;
			if(state != State.S0)
				throw new SqlLexerException("Invalid expression.");
			if(cExpressions.Count > 0)
				throw new SqlLexerException(String.Format("{0} expected. Line: {1} - Col: {2} -- opened in Line: {3} - Col: {4}", cExpressions.Peek().EndContainerChar, _lines.Count, lastLine.Length, cExpressions.Peek().Line.BeginPoint, cExpressions.Peek().BeginPos));
			return _expressions;
		}

		public static bool operator ==(Lexer l1, Lexer l2)
		{
			return Equals(l1, l2);
		}

		public static bool operator !=(Lexer l1, Lexer l2)
		{
			return !Equals(l1, l2);
		}

		private static bool Equals(Lexer l1, Lexer l2)
		{
			if(l1.Expressions.Count == l2.Expressions.Count)
			{
				for(int i = 0; i < l1.Expressions.Count; i++)
				{
					if(l1.Expressions[i].Token != l2.Expressions[i].Token)
						return false;
				}
				return true;
			}
			else
				return false;
		}
	}
}
