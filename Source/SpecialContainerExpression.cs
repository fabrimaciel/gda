using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression.Enums;
namespace GDA.Sql.InterpreterExpression
{
	class SpecialContainerExpression : Expression
	{
		private char containerChar;
		public char ContainerChar {
			get {
				return containerChar;
			}
		}
		private TokenID _containerToken;
		public TokenID ContainerToken {
			get {
				return _containerToken;
			}
			set {
				_containerToken = value;
			}
		}
		public SpecialContainerExpression (int a, int b, ExpressionLine c, string d, char e) : base (a, b, c, d)
		{
			this.Token = TokenID.StringLiteral;
			this.containerChar = e;
		}
		public override string ToString ()
		{
			return containerChar + base.Text + containerChar;
		}
	}
}
