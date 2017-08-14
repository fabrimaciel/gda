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
		private static SortedList<string, TokenID> keywords = new SortedList<string, TokenID> ();
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
		private static char[] beginContainers =  {
			'(',
			'{'
		};
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
		private List<ExpressionLine> _lines;
		private string _command;
		private List<Expression> _expressions;
		internal List<Expression> Expressions {
			get {
				if (_expressions == null)
					return Lex ();
				else
					return _expressions;
			}
		}
		public int[] FindIdentifierPositions (string a)
		{
			List<int> b = new List<int> ();
			foreach (Expression e in Expressions)
				if (e.Text == a && e.Token != TokenID.StringLiteral)
					b.Add (e.BeginPoint);
			return b.ToArray ();
		}
		public string Command {
			get {
				return _command;
			}
			set {
				_command = value;
			}
		}
		static Lexer ()
		{
			if (keywords.Count == 0) {
				keywords.Add ("absolute", TokenID.kAbsolute);
				keywords.Add ("action", TokenID.kAction);
				keywords.Add ("add", TokenID.kAdd);
				keywords.Add ("all", TokenID.kAll);
				keywords.Add ("allocate", TokenID.kAllocate);
				keywords.Add ("alter", TokenID.kAlter);
				keywords.Add ("and", TokenID.kAnd);
				keywords.Add ("any", TokenID.kAny);
				keywords.Add ("are", TokenID.kAre);
				keywords.Add ("as", TokenID.kAs);
				keywords.Add ("asc", TokenID.kAsc);
				keywords.Add ("assertion", TokenID.kAssertion);
				keywords.Add ("at", TokenID.kAt);
				keywords.Add ("authorization", TokenID.kAuthorization);
				keywords.Add ("avg", TokenID.kAvg);
				keywords.Add ("begin", TokenID.kBegin);
				keywords.Add ("between", TokenID.kBetween);
				keywords.Add ("bit", TokenID.kBit);
				keywords.Add ("bit_length", TokenID.kBit_Length);
				keywords.Add ("both", TokenID.kBoth);
				keywords.Add ("by", TokenID.kBy);
				keywords.Add ("cascade", TokenID.kCascade);
				keywords.Add ("cascaded", TokenID.kCascaded);
				keywords.Add ("case", TokenID.kCase);
				keywords.Add ("cast", TokenID.kCast);
				keywords.Add ("catalog", TokenID.kCatalog);
				keywords.Add ("char", TokenID.kChar);
				keywords.Add ("char_length", TokenID.kChar_Length);
				keywords.Add ("character", TokenID.kCharacter);
				keywords.Add ("character_length", TokenID.kCharacter_Length);
				keywords.Add ("check", TokenID.kCheck);
				keywords.Add ("close", TokenID.kClose);
				keywords.Add ("coalesce", TokenID.kCoalesce);
				keywords.Add ("collate", TokenID.kCollate);
				keywords.Add ("collation", TokenID.kCollation);
				keywords.Add ("column", TokenID.kColumn);
				keywords.Add ("commit", TokenID.kCommit);
				keywords.Add ("connect", TokenID.kConnect);
				keywords.Add ("connection", TokenID.kConnection);
				keywords.Add ("constraint", TokenID.kConstraint);
				keywords.Add ("constraints", TokenID.kConstraints);
				keywords.Add ("continue", TokenID.kContinue);
				keywords.Add ("convert", TokenID.kConvert);
				keywords.Add ("corresponding", TokenID.kCorresponding);
				keywords.Add ("count", TokenID.kCount);
				keywords.Add ("create", TokenID.kCreate);
				keywords.Add ("cross", TokenID.kCross);
				keywords.Add ("current", TokenID.kCurrent);
				keywords.Add ("current_date", TokenID.kCurrent_Date);
				keywords.Add ("current_time", TokenID.kCurrent_Time);
				keywords.Add ("current_timestamp", TokenID.kCurrent_Timestamp);
				keywords.Add ("current_user", TokenID.kCurrent_User);
				keywords.Add ("cursor", TokenID.kCursor);
				keywords.Add ("date", TokenID.kDate);
				keywords.Add ("day", TokenID.kDay);
				keywords.Add ("deallocate", TokenID.kDeallocate);
				keywords.Add ("dec", TokenID.kDec);
				keywords.Add ("decimal", TokenID.kDecimal);
				keywords.Add ("declare", TokenID.kDeclare);
				keywords.Add ("default", TokenID.kDefault);
				keywords.Add ("deferrable", TokenID.kDeferrable);
				keywords.Add ("deferred", TokenID.kDeferred);
				keywords.Add ("delete", TokenID.kDelete);
				keywords.Add ("desc", TokenID.kDesc);
				keywords.Add ("describe", TokenID.kDescribe);
				keywords.Add ("descriptor", TokenID.kDescriptor);
				keywords.Add ("diagnostics", TokenID.kDiagnostics);
				keywords.Add ("disconnect", TokenID.kDisconnect);
				keywords.Add ("distinct", TokenID.kDistinct);
				keywords.Add ("double", TokenID.kDouble);
				keywords.Add ("drop", TokenID.kDrop);
				keywords.Add ("else", TokenID.kElse);
				keywords.Add ("end", TokenID.kEnd);
				keywords.Add ("end_exec", TokenID.kEnd_Exec);
				keywords.Add ("escape", TokenID.kEscape);
				keywords.Add ("except", TokenID.kExcept);
				keywords.Add ("exception", TokenID.kException);
				keywords.Add ("exec", TokenID.kExec);
				keywords.Add ("execute", TokenID.kExecute);
				keywords.Add ("exists", TokenID.kExists);
				keywords.Add ("external", TokenID.kExternal);
				keywords.Add ("extract", TokenID.kExtract);
				keywords.Add ("false", TokenID.kFalse);
				keywords.Add ("fetch", TokenID.kFetch);
				keywords.Add ("first", TokenID.kFirst);
				keywords.Add ("float", TokenID.kFloat);
				keywords.Add ("for", TokenID.kFor);
				keywords.Add ("foreign", TokenID.kForeign);
				keywords.Add ("fortran", TokenID.kFortran);
				keywords.Add ("found", TokenID.kFound);
				keywords.Add ("from", TokenID.kFrom);
				keywords.Add ("full", TokenID.kFull);
				keywords.Add ("get", TokenID.kGet);
				keywords.Add ("global", TokenID.kGlobal);
				keywords.Add ("go", TokenID.kGo);
				keywords.Add ("goto", TokenID.kGoto);
				keywords.Add ("grant", TokenID.kGrant);
				keywords.Add ("group", TokenID.kGroup);
				keywords.Add ("having", TokenID.kHaving);
				keywords.Add ("hour", TokenID.kHour);
				keywords.Add ("identity", TokenID.kIdentity);
				keywords.Add ("immediate", TokenID.kImmediate);
				keywords.Add ("in", TokenID.kIn);
				keywords.Add ("include", TokenID.kInclude);
				keywords.Add ("index", TokenID.kIndex);
				keywords.Add ("indicator", TokenID.kIndicator);
				keywords.Add ("initially", TokenID.kInitially);
				keywords.Add ("inner", TokenID.kInner);
				keywords.Add ("input", TokenID.kInput);
				keywords.Add ("insensitive", TokenID.kInsensitive);
				keywords.Add ("insert", TokenID.kInsert);
				keywords.Add ("int", TokenID.kInt);
				keywords.Add ("integer", TokenID.kInteger);
				keywords.Add ("intersect", TokenID.kIntersect);
				keywords.Add ("interval", TokenID.kInterval);
				keywords.Add ("into", TokenID.kInto);
				keywords.Add ("is", TokenID.kIs);
				keywords.Add ("isolation", TokenID.kIsolation);
				keywords.Add ("join", TokenID.kJoin);
				keywords.Add ("key", TokenID.kKey);
				keywords.Add ("language", TokenID.kLanguage);
				keywords.Add ("last", TokenID.kLast);
				keywords.Add ("leading", TokenID.kLeading);
				keywords.Add ("left", TokenID.kLeft);
				keywords.Add ("level", TokenID.kLevel);
				keywords.Add ("like", TokenID.kLike);
				keywords.Add ("local", TokenID.kLocal);
				keywords.Add ("lower", TokenID.kLower);
				keywords.Add ("match", TokenID.kMatch);
				keywords.Add ("max", TokenID.kMax);
				keywords.Add ("min", TokenID.kMin);
				keywords.Add ("minute", TokenID.kMinute);
				keywords.Add ("module", TokenID.kModule);
				keywords.Add ("month", TokenID.kMonth);
				keywords.Add ("names", TokenID.kNames);
				keywords.Add ("national", TokenID.kNational);
				keywords.Add ("natural", TokenID.kNatural);
				keywords.Add ("nchar", TokenID.kNChar);
				keywords.Add ("next", TokenID.kNext);
				keywords.Add ("no", TokenID.kNo);
				keywords.Add ("none", TokenID.kNone);
				keywords.Add ("not", TokenID.kNot);
				keywords.Add ("null", TokenID.kNull);
				keywords.Add ("nullif", TokenID.kNullIf);
				keywords.Add ("nulls", TokenID.kNulls);
				keywords.Add ("numeric", TokenID.kNumeric);
				keywords.Add ("octect_length", TokenID.kOctect_Length);
				keywords.Add ("of", TokenID.kOf);
				keywords.Add ("on", TokenID.kOn);
				keywords.Add ("only", TokenID.kOnly);
				keywords.Add ("open", TokenID.kOpen);
				keywords.Add ("option", TokenID.kOption);
				keywords.Add ("or", TokenID.kOr);
				keywords.Add ("order", TokenID.kOrder);
				keywords.Add ("outer", TokenID.kOuter);
				keywords.Add ("output", TokenID.kOutput);
				keywords.Add ("overlaps", TokenID.kOverlaps);
				keywords.Add ("pad", TokenID.kPad);
				keywords.Add ("partial", TokenID.kPartial);
				keywords.Add ("pascal", TokenID.kPascal);
				keywords.Add ("percent", TokenID.kPercent);
				keywords.Add ("position", TokenID.kPosition);
				keywords.Add ("precision", TokenID.kPrecision);
				keywords.Add ("prepare", TokenID.kPrepare);
				keywords.Add ("preserve", TokenID.kPreserve);
				keywords.Add ("primary", TokenID.kPrimary);
				keywords.Add ("prior", TokenID.kPrior);
				keywords.Add ("privileges", TokenID.kPrivileges);
				keywords.Add ("procedure", TokenID.kProcedure);
				keywords.Add ("public", TokenID.kPublic);
				keywords.Add ("read", TokenID.kRead);
				keywords.Add ("real", TokenID.kReal);
				keywords.Add ("references", TokenID.kReferences);
				keywords.Add ("relative", TokenID.kRelative);
				keywords.Add ("restrict", TokenID.kRestrict);
				keywords.Add ("revoke", TokenID.kRevoke);
				keywords.Add ("right", TokenID.kRight);
				keywords.Add ("rollback", TokenID.kRollback);
				keywords.Add ("rows", TokenID.kRows);
				keywords.Add ("schema", TokenID.kSchema);
				keywords.Add ("scroll", TokenID.kScroll);
				keywords.Add ("second", TokenID.kSecond);
				keywords.Add ("select", TokenID.kSelect);
				keywords.Add ("set", TokenID.kSet);
				keywords.Add ("smallint", TokenID.kSmallint);
				keywords.Add ("some", TokenID.kSome);
				keywords.Add ("space", TokenID.kSpace);
				keywords.Add ("sql", TokenID.kSql);
				keywords.Add ("sqlca", TokenID.kSqlCa);
				keywords.Add ("sqlcode", TokenID.kSqlCode);
				keywords.Add ("sqlerror", TokenID.kSqlError);
				keywords.Add ("sqlstate", TokenID.kSqlState);
				keywords.Add ("sqlwarning", TokenID.kSqlWarning);
				keywords.Add ("substring", TokenID.kSubstring);
				keywords.Add ("sum", TokenID.kSum);
				keywords.Add ("system_user", TokenID.kSystem_User);
				keywords.Add ("table", TokenID.kTable);
				keywords.Add ("temporary", TokenID.kTemporary);
				keywords.Add ("then", TokenID.kThen);
				keywords.Add ("time", TokenID.kTime);
				keywords.Add ("timestamp", TokenID.kTimestamp);
				keywords.Add ("timezone_hour", TokenID.kTimezone_Hour);
				keywords.Add ("timezone_minute", TokenID.kTimezone_Minute);
				keywords.Add ("to", TokenID.kTo);
				keywords.Add ("top", TokenID.kTop);
				keywords.Add ("trailing", TokenID.kTrailing);
				keywords.Add ("transaction", TokenID.kTransaction);
				keywords.Add ("translate", TokenID.kTranslate);
				keywords.Add ("translation", TokenID.kTranslation);
				keywords.Add ("trim", TokenID.kTrim);
				keywords.Add ("true", TokenID.kTrue);
				keywords.Add ("union", TokenID.kUnion);
				keywords.Add ("unique", TokenID.kUnique);
				keywords.Add ("unknown", TokenID.kUnknown);
				keywords.Add ("update", TokenID.kUpdate);
				keywords.Add ("upper", TokenID.kUpper);
				keywords.Add ("usage", TokenID.kUsage);
				keywords.Add ("using", TokenID.kUsing);
				keywords.Add ("value", TokenID.kValue);
				keywords.Add ("values", TokenID.kValues);
				keywords.Add ("varchar", TokenID.kVarchar);
				keywords.Add ("varying", TokenID.kVarying);
				keywords.Add ("view", TokenID.kView);
				keywords.Add ("when", TokenID.kWhen);
				keywords.Add ("whenever", TokenID.kWhenever);
				keywords.Add ("where", TokenID.kWhere);
				keywords.Add ("with", TokenID.kWith);
				keywords.Add ("work", TokenID.kWork);
				keywords.Add ("write", TokenID.kWrite);
				keywords.Add ("year", TokenID.kYear);
				keywords.Add ("zone", TokenID.kZone);
			}
		}
		public Lexer (string a)
		{
			_command = a;
		}
		private static bool isAlphanumeric (char a)
		{
			return ((a >= 'A' && a <= 'Z') || (a >= 'a' && a <= 'z') || (a >= '0' && a <= '9') || a == '_' || a == '@');
		}
		private static bool isAlpha (char a)
		{
			return ((a >= 'A' && a <= 'Z') || (a >= 'a' && a <= 'z') || a == '@' || a == '?');
		}
		private static bool isNumeric (char a)
		{
			return ((a >= 'A' && a <= 'F') || (a >= 'a' && a <= 'F') || (a >= '0' && a <= '9') || a == '.');
		}
		internal List<Expression> Lex ()
		{
			_lines = new List<ExpressionLine> ();
			_expressions = new List<Expression> ();
			Stack<ContainerExpression> a = new Stack<ContainerExpression> ();
			int b = 0;
			int c = 0;
			State d = State.S0;
			bool f = false;
			char g = '\"';
			ExpressionLine h = new ExpressionLine (c);
			_lines.Add (h);
			bool i = false;
			while (!i || (c == Command.Length && (d == State.S8 || d == State.S3 || d == State.S2))) {
				switch (d) {
				case State.S0:
					if (Array.IndexOf<char> (spacesChar, Command [c]) != -1) {
						d = State.S0;
						if (Command [c] == '\n') {
							h.Length = c - h.BeginPoint;
							h = new ExpressionLine (c);
							_lines.Add (h);
						}
						else {
						}
						b = c + 1;
					}
					else if (Array.IndexOf<char> (beginStringContainers, Command [c]) != -1) {
						b = c + 1;
						g = endStringContainers [Array.IndexOf<char> (beginStringContainers, Command [c])];
						d = State.S1;
					}
					else if (Array.IndexOf<char> (tabsChar, Command [c]) != -1) {
						b = c;
						d = State.S2;
					}
					else if (isAlpha (Command [c]))
						d = State.S3;
					else if (char.IsDigit (Command [c]))
						d = State.S8;
					else if (Array.IndexOf<char> (beginContainers, Command [c]) != -1) {
						Expression j = new Expression (c, h, Command [c]);
						switch (Command [c]) {
						case '(':
							j.Token = TokenID.LParen;
							break;
						case '{':
							j.Token = TokenID.LCurly;
							break;
						}
						_expressions.Add (j);
						a.Push (new ContainerExpression (c, j.Text [0], h));
						b = c + 1;
						f = true;
						d = State.S0;
					}
					else if (Array.IndexOf<char> (endContainers, Command [c]) != -1) {
						if (a.Count == 0) {
							throw new SqlLexerException (String.Format ("Not open tag for caracter {0}. Line: {1} - Col: {2}.", Command [c], _lines.Count, c - h.BeginPoint));
						}
						else if (a.Peek ().EndContainerChar == Command [c]) {
							a.Pop ();
							Expression j = new Expression (c, h, Command [c]);
							switch (Command [c]) {
							case ')':
								j.Token = TokenID.RParen;
								break;
							case '}':
								j.Token = TokenID.RCurly;
								break;
							}
							_expressions.Add (j);
							if (a.Count == 0) {
								f = false;
							}
							b = c + 1;
							d = State.S0;
						}
						else
							throw new SqlLexerException (String.Format ("Expected caracter {0}. Line: {1} - Col: {2}.", a.Peek ().EndContainerChar, _lines.Count, c - h.BeginPoint));
					}
					else {
						throw new SqlLexerException (String.Format ("Invalid caracter '{0}' in expression context.", Command [c]));
					}
					break;
				case State.S1:
					if (Command [c] == g) {
						if ((c > 0 && Command [c - 1] != '\\') || (c > 1 && Command [c - 2] == '\\')) {
							if (g == ']' || g == '`') {
								Expression j = new Expression (b, c - b, h, Command, TokenID.Identifier);
								j.CurrentSpecialContainer = new SpecialContainer (Command [b - 1], Command [c]);
								_expressions.Add (j);
							}
							else {
								SpecialContainerExpression k = new SpecialContainerExpression (b, c - b, h, Command, g);
								_expressions.Add (k);
								switch (g) {
								case '"':
									k.ContainerToken = TokenID.Quote;
									break;
								case '\'':
									k.ContainerToken = TokenID.SQuote;
									break;
								}
							}
							b = c + 1;
							d = State.S0;
						}
					}
					break;
				case State.S2:
					if (Command [c - 1] == '-' && char.IsDigit (Command [c])) {
						if (Command.Length < 2 || (Command.Length > 2 && Array.IndexOf<char> (tabsChar, Command [c - 2]) != -1) || (Command.Length > 2 && Array.IndexOf<char> (spacesChar, Command [c - 2]) != -1)) {
							d = State.S8;
							continue;
						}
					}
					string l;
					if (!i)
						l = Command [c - 1].ToString () + Command [c].ToString ();
					else
						l = Command [c - 1].ToString ();
					switch (l) {
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
						Expression j = new Expression (b, 2, h, Command);
						switch (j.Text) {
						case "&&":
							j.Token = TokenID.kAnd;
							break;
						case "||":
							j.Token = TokenID.kOr;
							break;
						case "++":
							j.Token = TokenID.PlusPlus;
							break;
						case "--":
							j.Token = TokenID.MinusMinus;
							break;
						case "->":
							j.Token = TokenID.MinusGreater;
							break;
						case ":=":
							j.Token = TokenID.Equal;
							break;
						case "==":
							j.Token = TokenID.EqualEqual;
							break;
						case "<>":
						case "!=":
							j.Token = TokenID.NotEqual;
							break;
						case ">=":
							j.Token = TokenID.GreaterEqual;
							break;
						case "<=":
							j.Token = TokenID.LessEqual;
							break;
						case "+=":
							j.Token = TokenID.PlusEqual;
							break;
						case "/*":
							j.Token = TokenID.BMultiComment;
							break;
						case "*/":
							j.Token = TokenID.EMultiComment;
							break;
						case "//":
							j.Token = TokenID.SingleComment;
							break;
						case "!>":
							j.Token = TokenID.Less;
							break;
						case "!<":
							j.Token = TokenID.Greater;
							break;
						default:
							j.Token = TokenID.InvalidTab;
							break;
						}
						_expressions.Add (j);
						b = c + 1;
						d = State.S0;
						break;
					default:
						TabExpression m = new TabExpression (b, h, Command);
						switch (Command [b]) {
						case '.':
							m.Token = TokenID.Dot;
							break;
						case '=':
							m.Token = TokenID.Equal;
							break;
						case '"':
							m.Token = TokenID.Hash;
							break;
						case '$':
							m.Token = TokenID.Dollar;
							break;
						case '%':
							m.Token = TokenID.Percent;
							break;
						case '&':
							m.Token = TokenID.BAnd;
							break;
						case '*':
							m.Token = TokenID.Star;
							break;
						case '+':
							m.Token = TokenID.Plus;
							break;
						case '-':
							m.Token = TokenID.Minus;
							break;
						case ',':
							m.Token = TokenID.Comma;
							break;
						case '/':
							m.Token = TokenID.Slash;
							break;
						case ':':
							m.Token = TokenID.Colon;
							break;
						case ';':
							m.Token = TokenID.Semi;
							break;
						case '<':
							m.Token = TokenID.Less;
							break;
						case '>':
							m.Token = TokenID.Greater;
							break;
						case '?':
							m.Token = TokenID.Question;
							break;
						case '!':
							m.Token = TokenID.kNot;
							break;
						default:
							m.Token = TokenID.InvalidTab;
							break;
						}
						_expressions.Add (m);
						b = c;
						c--;
						d = State.S0;
						break;
					}
					break;
				case State.S3:
					if (c == Command.Length || !isAlphanumeric (Command [c])) {
						Expression j = new Expression (b, c - b, h, Command);
						if (char.IsDigit (j.Text [0]) || j.Text [0] == '-') {
							if (j.Text.Length > 2 && (j.Text [1] == 'x' || j.Text [1] == 'X') && System.Text.RegularExpressions.Regex.IsMatch (j.Text.Substring (2), "[0-9]")) {
								j.Token = TokenID.HexLiteral;
							}
							else if (System.Text.RegularExpressions.Regex.IsMatch (j.Text, "^([-]|[0-9])[0-9]*$")) {
								j.Token = TokenID.IntLiteral;
							}
							else if (System.Text.RegularExpressions.Regex.IsMatch (j.Text, "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$")) {
								j.Token = TokenID.RealLiteral;
							}
						}
						else if (keywords.ContainsKey (j.Text.ToLower ())) {
							j.Token = keywords [j.Text.ToLower ()];
						}
						else {
							j.Token = TokenID.Identifier;
						}
						_expressions.Add (j);
						b = c;
						c--;
						d = State.S0;
					}
					break;
				case State.S8:
					if (c == Command.Length || !isNumeric (Command [c])) {
						Expression j = new Expression (b, c - b, h, Command);
						if (j.Text.Length > 2 && (j.Text [1] == 'x' || j.Text [1] == 'X') && System.Text.RegularExpressions.Regex.IsMatch (j.Text.Substring (2), "[0-9]")) {
							j.Token = TokenID.HexLiteral;
						}
						else if (System.Text.RegularExpressions.Regex.IsMatch (j.Text, "[0-9]")) {
							j.Token = TokenID.IntLiteral;
						}
						else if (System.Text.RegularExpressions.Regex.IsMatch (j.Text, "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$")) {
							j.Token = TokenID.RealLiteral;
						}
						else
							throw new SqlLexerException ("Expected number or hexadecimal.");
						_expressions.Add (j);
						b = c;
						c--;
						d = State.S0;
					}
					break;
				}
				c++;
				i = (c == Command.Length);
			}
			h.Length = c - h.BeginPoint;
			if (d != State.S0)
				throw new SqlLexerException ("Invalid expression.");
			if (a.Count > 0)
				throw new SqlLexerException (String.Format ("{0} expected. Line: {1} - Col: {2} -- opened in Line: {3} - Col: {4}", a.Peek ().EndContainerChar, _lines.Count, h.Length, a.Peek ().Line.BeginPoint, a.Peek ().BeginPos));
			return _expressions;
		}
		public static bool operator == (Lexer a, Lexer b) {
			return Equals (a, b);
		}
		public static bool operator != (Lexer a, Lexer b) {
			return !Equals (a, b);
		}
		private static bool Equals (Lexer a, Lexer b)
		{
			if (a.Expressions.Count == b.Expressions.Count) {
				for (int c = 0; c < a.Expressions.Count; c++) {
					if (a.Expressions [c].Token != b.Expressions [c].Token)
						return false;
				}
				return true;
			}
			else
				return false;
		}
	}
}
