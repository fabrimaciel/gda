using System;
using System.Collections.Generic;
using System.Text;
namespace GDA.Sql.InterpreterExpression.Nodes
{
	class SelectPart
	{
		private uint? _top;
		private bool _topInPercent = false;
		private Enums.SelectClauseResultTypes _resultType = GDA.Sql.InterpreterExpression.Enums.SelectClauseResultTypes.All;
		private List<SelectExpression> _selectionExpressions = new List<SelectExpression> ();
		public uint? Top {
			get {
				return _top;
			}
			set {
				_top = value;
			}
		}
		public bool TopInPercent {
			get {
				return _topInPercent;
			}
			set {
				_topInPercent = value;
			}
		}
		public Enums.SelectClauseResultTypes ResultType {
			get {
				return _resultType;
			}
			set {
				_resultType = value;
			}
		}
		public List<SelectExpression> SelectionExpressions {
			get {
				return _selectionExpressions;
			}
			set {
				_selectionExpressions = value;
			}
		}
	}
}
