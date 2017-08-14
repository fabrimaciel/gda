using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Grammar
{
	enum ExpressionComparerType
	{
		Simple,
		Regex
	}
	class ExpressionComparer
	{
		private ExpressionComparerType _type = ExpressionComparerType.Simple;
		private string _textComparer;
		private bool _caseSensitive = false;
		private uint _numberPreviousExpressions;
		private uint _numberNextExpressions;
		public ExpressionComparerType Type {
			get {
				return _type;
			}
			set {
				_type = value;
			}
		}
		public string TextComparer {
			get {
				return _textComparer;
			}
			set {
				_textComparer = value;
			}
		}
		public bool CaseSensitive {
			get {
				return _caseSensitive;
			}
			set {
				_caseSensitive = value;
			}
		}
		public uint NumberPreviousExpressions {
			get {
				return _numberPreviousExpressions;
			}
			set {
				_numberPreviousExpressions = value;
			}
		}
		public uint NumberNextExpressions {
			get {
				return _numberNextExpressions;
			}
			set {
				_numberNextExpressions = value;
			}
		}
		public uint JumpNextExpressions {
			get {
				return 1 + _numberNextExpressions;
			}
		}
		public ExpressionComparer (string a) : this (a, ExpressionComparerType.Simple, false)
		{
		}
		public ExpressionComparer (string a, ExpressionComparerType b, bool c)
		{
			_textComparer = a;
			_type = b;
			_caseSensitive = c;
		}
		public bool Comparer (Expression a)
		{
			return false;
		}
	}
}
