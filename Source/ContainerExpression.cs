using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression
{
	class ContainerExpression
	{
		private char containerChar;
		private char endContainerChar;
		private ExpressionLine _line;
		private int _beginPos = 0;
		public char ContainerChar {
			get {
				return containerChar;
			}
			set {
				containerChar = value;
				switch (value) {
				case '(':
					endContainerChar = ')';
					break;
				case '[':
					endContainerChar = ']';
					break;
				case '{':
					endContainerChar = '}';
					break;
				default:
					throw new Exception ("AD");
				}
			}
		}
		public char EndContainerChar {
			get {
				return endContainerChar;
			}
		}
		public ExpressionLine Line {
			get {
				return _line;
			}
			set {
				_line = value;
			}
		}
		public int BeginPos {
			get {
				return _beginPos;
			}
			set {
				_beginPos = value;
			}
		}
		public ContainerExpression (int a, char b, ExpressionLine c)
		{
			_beginPos = a;
			ContainerChar = b;
			_line = c;
		}
	}
}
