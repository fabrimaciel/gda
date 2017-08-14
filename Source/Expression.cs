using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression.Enums;
namespace GDA.Sql.InterpreterExpression
{
	class SpecialContainer
	{
		public readonly char BeginCharSpecialContainer;
		public readonly char EndCharSpecialContainer;
		public SpecialContainer (char a, char b)
		{
			BeginCharSpecialContainer = a;
			EndCharSpecialContainer = b;
		}
	}
	class Expression
	{
		private int beginPoint;
		private int length;
		private string text;
		private ExpressionLine line;
		private ContainerExpression container;
		private int containerPosition;
		private TokenID _token;
		private SpecialContainer _currentSpecialContainer = null;
		public int BeginPoint {
			get {
				return beginPoint;
			}
			set {
				beginPoint = value;
			}
		}
		public int Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		public ExpressionLine Line {
			get {
				return line;
			}
			set {
				line = value;
			}
		}
		public ContainerExpression Container {
			get {
				return container;
			}
			set {
				container = value;
			}
		}
		public int ContainerPosition {
			get {
				return containerPosition;
			}
			set {
				containerPosition = value;
			}
		}
		public TokenID Token {
			get {
				return _token;
			}
			set {
				_token = value;
			}
		}
		public SpecialContainer CurrentSpecialContainer {
			get {
				return _currentSpecialContainer;
			}
			set {
				_currentSpecialContainer = value;
			}
		}
		public Expression ()
		{
		}
		public Expression (TokenID a)
		{
			_token = a;
		}
		public Expression (int a, int b)
		{
			this.beginPoint = a;
			this.length = b;
		}
		public Expression (int a, int b, ExpressionLine c)
		{
			this.beginPoint = a;
			this.length = b;
			this.line = c;
			this.line.Expressions.Add (this);
		}
		public Expression (int a, int b, ExpressionLine c, string d) : this (a, b, c, d, TokenID.Identifier)
		{
			this.text = d.Substring (a, b);
		}
		public Expression (int a, int b, ExpressionLine c, string d, TokenID e) : this (a, b, c)
		{
			this.text = d.Substring (a, b);
			_token = e;
		}
		public Expression (int a, ExpressionLine b, char c) : this (a, 1, b)
		{
			this.text = new string (c, 1);
		}
		public override string ToString ()
		{
			return text;
		}
	}
}
