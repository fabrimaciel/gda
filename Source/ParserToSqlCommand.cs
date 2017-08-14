using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Nodes;
namespace GDA.Sql
{
	class ParserToSqlCommand
	{
		private StringBuilder sqlCommand = new StringBuilder ();
		private string _quoteExpressionBegin;
		private string _quoteExpressionEnd;
		public string SqlCommand {
			get {
				return sqlCommand.ToString ().TrimEnd (' ');
			}
		}
		public string QuoteExpressionBegin {
			get {
				return _quoteExpressionBegin;
			}
			set {
				_quoteExpressionBegin = value;
			}
		}
		public string QuoteExpressionEnd {
			get {
				return _quoteExpressionEnd;
			}
			set {
				_quoteExpressionEnd = value;
			}
		}
		internal ParserToSqlCommand (Parser a) : this (a, "", "")
		{
		}
		internal ParserToSqlCommand (Parser a, string b, string c)
		{
			_quoteExpressionBegin = b;
			_quoteExpressionEnd = c;
			foreach (Select select in a.SelectParts) {
				GetSelectInfo (select);
			}
		}
		internal ParserToSqlCommand (WherePart a) : this (a, "", "")
		{
		}
		internal ParserToSqlCommand (WherePart a, string b, string c)
		{
			_quoteExpressionBegin = b;
			_quoteExpressionEnd = c;
			foreach (SqlExpression exp in a.Expressions)
				ColumnName (exp);
		}
		internal ParserToSqlCommand (OrderByPart a) : this (a, "", "")
		{
		}
		internal ParserToSqlCommand (OrderByPart a, string b, string c)
		{
			_quoteExpressionBegin = b;
			_quoteExpressionEnd = c;
			for (int d = 0; d < a.OrderByExpressions.Count; d++) {
				ColumnName (a.OrderByExpressions [d].Expression);
				if (a.OrderByExpressions [d].Asc)
					sqlCommand.Append (" ASC");
				else
					sqlCommand.Append (" DESC");
				if (d + 1 != a.OrderByExpressions.Count)
					sqlCommand.Append (", ");
			}
		}
		internal ParserToSqlCommand (GroupByPart a, string b, string c)
		{
			_quoteExpressionBegin = b;
			_quoteExpressionEnd = c;
			for (int d = 0; d < a.Expressions.Count; d++) {
				ColumnName (a.Expressions [d]);
				if (d + 1 != a.Expressions.Count)
					sqlCommand.Append (", ");
			}
		}
		private string ApplyQuoteExpression (string a)
		{
			if (a != null) {
				var b = a.TrimEnd (' ').TrimStart (' ');
				if (b.Length > 3 && b [0] == '[' && b [b.Length - 1] == ']')
					return a;
			}
			return this.QuoteExpressionBegin + a + this.QuoteExpressionEnd;
		}
		private void GetSelectInfo (Select a)
		{
			sqlCommand.Append (a.Value.Text.ToUpper ()).Append (" ");
			if (a.SelectPart.ResultType == GDA.Sql.InterpreterExpression.Enums.SelectClauseResultTypes.Distinct)
				sqlCommand.Append ("DISTINCT ");
			foreach (SelectExpression se in a.SelectPart.SelectionExpressions) {
				ColumnName (se.ColumnName);
				if (se.ColumnAlias != null && se.AsExpression != null)
					sqlCommand.Append (" AS ").Append (se.ColumnAlias.Value.Text);
				else if (se.ColumnAlias != null)
					sqlCommand.Append (" ").Append (se.ColumnAlias.Value.Text);
				sqlCommand.Append (", ");
			}
			if (a.SelectPart.SelectionExpressions.Count > 0)
				sqlCommand.Remove (sqlCommand.Length - 2, 1);
			if (a.FromPart != null) {
				sqlCommand.Append ("FROM ");
				foreach (TableExpression te in a.FromPart.TableExpressions) {
					if (te.LeftOrRight != null)
						sqlCommand.Append (te.LeftOrRight.Text).Append (" ");
					if (te.OuterOrInnerOrCrossOrNatural != null)
						sqlCommand.Append (te.OuterOrInnerOrCrossOrNatural.Text.ToUpper ()).Append (" ");
					if (te.Join != null)
						sqlCommand.Append ("JOIN ");
					if (te.SelectInfo != null) {
						sqlCommand.Append ("(");
						GetSelectInfo (te.SelectInfo);
						sqlCommand.Append (") ");
					}
					else {
						TableName (te);
					}
					if (te.TableAlias != null) {
						sqlCommand.Append ((te.AsExpression != null ? "AS " : " ")).Append (te.TableAlias).Append (" ");
					}
					if (te.OnExpressions != null) {
						sqlCommand.Append ("ON(");
						foreach (SqlExpression se in te.OnExpressions.Expressions)
							ColumnName (se);
						sqlCommand.Append (")");
					}
					sqlCommand.Append (" ");
				}
			}
			if (a.WherePart != null) {
				sqlCommand.Append ("WHERE ");
				foreach (SqlExpression se in a.WherePart.Expressions)
					ColumnName (se);
				sqlCommand.Append (" ");
			}
			if (a.GroupByPart != null) {
				sqlCommand.Append ("GROUP BY ");
				foreach (SqlExpression se in a.GroupByPart.Expressions) {
					ColumnName (se);
					sqlCommand.Append (", ");
				}
				if (a.GroupByPart.Expressions.Count > 0)
					sqlCommand.Remove (sqlCommand.Length - 2, 1);
				sqlCommand.Append (" ");
			}
			if (a.HavingPart != null) {
				sqlCommand.Append ("HAVING ");
				foreach (SqlExpression se in a.HavingPart.Expressions)
					ColumnName (se);
				sqlCommand.Append (" ");
			}
			if (a.OrderByPart != null) {
				sqlCommand.Append ("ORDER BY ");
				int b = 0;
				foreach (OrderByExpression oe in a.OrderByPart.OrderByExpressions) {
					if (b > 0)
						sqlCommand.Append (", ");
					ColumnName (oe.Expression);
					if (!oe.Asc)
						sqlCommand.Append (" DESC");
					b++;
				}
				sqlCommand.Append (" ");
			}
		}
		private void ColumnName (SqlExpression a)
		{
			if (a is ContainerSqlExpression) {
				if (((ContainerSqlExpression)a).ContainerToken == GDA.Sql.InterpreterExpression.Enums.TokenID.LParen)
					sqlCommand.Append ("(");
				foreach (SqlExpression se1 in ((ContainerSqlExpression)a).Expressions) {
					ColumnName (se1);
				}
				if (((ContainerSqlExpression)a).ContainerToken == GDA.Sql.InterpreterExpression.Enums.TokenID.LParen)
					sqlCommand.Append (")");
			}
			else if (a is Select) {
				GetSelectInfo ((Select)a);
			}
			else if (a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Column) {
				if (a.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.Star)
					sqlCommand.Append (a.Value.Text);
				else {
					int b = a.Value.Text.LastIndexOf ('.');
					if (b >= 0) {
						var c = RemoveDefaultQuote (a.Value.Text.Substring (0, b));
						if (c.Length > 2 && !c.StartsWith (QuoteExpressionBegin) && !c.EndsWith (QuoteExpressionEnd))
							sqlCommand.Append (QuoteExpressionBegin).Append (c).Append (QuoteExpressionEnd).Append (".");
						else
							sqlCommand.Append (c).Append (".");
						if (a.Value.Text.Substring (b + 1) == "*")
							sqlCommand.Append (a.Value.Text.Substring (b + 1));
						else {
							var d = RemoveDefaultQuote (a.Value.Text.Substring (b + 1));
							if (d.Length > 2 && !d.StartsWith (QuoteExpressionBegin) && !d.EndsWith (QuoteExpressionEnd))
								sqlCommand.Append (QuoteExpressionBegin).Append (d).Append (QuoteExpressionEnd);
							else
								sqlCommand.Append (d);
						}
					}
					else {
						var c = RemoveDefaultQuote (a.Value.Text);
						if (c.Length > 2 && !c.StartsWith (QuoteExpressionBegin) && !c.EndsWith (QuoteExpressionEnd))
							sqlCommand.Append (QuoteExpressionBegin).Append (c).Append (QuoteExpressionEnd);
						else
							sqlCommand.Append (c);
					}
				}
			}
			else if (a is SqlFunction) {
				sqlCommand.Append (a.Value.Text).Append ("(");
				foreach (List<SqlExpression> parameter in ((SqlFunction)a).Parameters) {
					foreach (SqlExpression pSe in parameter) {
						ColumnName (pSe);
					}
					sqlCommand.Append (", ");
				}
				if (((SqlFunction)a).Parameters.Count > 0)
					sqlCommand.Remove (sqlCommand.Length - 2, 2);
				sqlCommand.Append (")");
			}
			else if (a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.ComparerScalar || a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Boolean || a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Operation || a.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.kNot) {
				sqlCommand.Append (" ").Append (a.Value.Text.ToUpper ()).Append (" ");
			}
			else if (a.Value is SpecialContainerExpression) {
				SpecialContainerExpression e = (SpecialContainerExpression)a.Value;
				sqlCommand.Append (e.ContainerChar);
				sqlCommand.Append (a.Value.Text);
				sqlCommand.Append (e.ContainerChar);
			}
			else if (a.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.kIsNull || a.Value.Token == GDA.Sql.InterpreterExpression.Enums.TokenID.kIs)
				sqlCommand.Append (" ").Append (a.Value.Text);
			else if (a.Type == GDA.Sql.InterpreterExpression.Enums.SqlExpressionType.Boolean) {
				switch (a.Value.Token) {
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kAnd:
					if (a.Value.Text == "&&")
						a.Value.Text = "AND";
					break;
				case GDA.Sql.InterpreterExpression.Enums.TokenID.kOr:
					if (a.Value.Text == "||")
						a.Value.Text = "OR";
					break;
				}
			}
			else
				switch (a.Value.Token) {
				case GDA.Sql.InterpreterExpression.Enums.TokenID.EqualEqual:
					sqlCommand.Append ("=");
					break;
				default:
					sqlCommand.Append (a.Value.Text);
					break;
				}
		}
		private static string RemoveDefaultQuote (string a)
		{
			if (a != null) {
				var b = a.TrimEnd (' ').TrimStart (' ');
				if (b.Length > 3 && b [0] == '[' && b [b.Length - 1] == ']')
					a = a.Substring (1, a.Length - 2);
			}
			return a;
		}
		private void TableName (TableExpression a)
		{
			var b = a.TableName.InnerExpression.Text.TrimEnd (' ').TrimStart (' ');
			var c = (a.TableName.Schema ?? "").TrimEnd (' ').TrimStart (' ');
			if (a.TableName.InnerExpression.CurrentSpecialContainer != null && a.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer == QuoteExpressionBegin [0] && a.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer == QuoteExpressionEnd [0]) {
				if (c.Length > 0) {
					if (!(c.Length > 3 && c [0] == a.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer && c [c.Length - 1] == a.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer))
						sqlCommand.Append (a.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer).Append (a.TableName.Schema).Append (a.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer).Append ('.');
					else
						sqlCommand.Append (a.TableName.Schema).Append ('.');
				}
				if (!(b.Length > 3 && b [0] == a.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer && b [b.Length - 1] == a.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer))
					sqlCommand.Append (a.TableName.InnerExpression.CurrentSpecialContainer.BeginCharSpecialContainer).Append (a.TableName.Name).Append (a.TableName.InnerExpression.CurrentSpecialContainer.EndCharSpecialContainer).Append (" ");
			}
			else {
				if (c.Length > 0) {
					if (c.Length > 2 && !c.StartsWith (QuoteExpressionBegin) && !c.EndsWith (QuoteExpressionEnd))
						sqlCommand.Append (QuoteExpressionBegin).Append (a.TableName.Schema).Append (QuoteExpressionEnd).Append ('.');
					else
						sqlCommand.Append (a.TableName.Schema).Append ('.');
				}
				if (b.Length > 2 && !b.StartsWith (QuoteExpressionBegin) && !b.EndsWith (QuoteExpressionEnd))
					sqlCommand.Append (QuoteExpressionBegin).Append (a.TableName.Name).Append (QuoteExpressionEnd).Append (" ");
				else
					sqlCommand.Append (a.TableName.Name).Append (" ");
			}
		}
		public override string ToString ()
		{
			return sqlCommand.ToString ();
		}
	}
}
